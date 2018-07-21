using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Concurrent;

namespace SOALog
{
    //***  下面说一下编写 SOALog 的 缘由 和 理念
    //
    //***  其实我不太赞同 微服务。但 SOA 倒是可能成为 趋势。因为 平台 与 平台 之间的 交互 应该会是一个 趋势
    //***  微服务 的 第一个 课题 就是 实现 数据一致性。SOA 也需要 解决 这个问题。
    //***  但实际上，数据不一致 并不可怕，关键在于 清晰的 记录Log，及 提示出来，
    //***  让 用户 和 各方 能够清楚的看到 这一笔交易 是 失败 的，问题出在了哪里，接下来要怎么处理。
    //***  所以，SOALog 的 作用 就是可以在 调用服务失败的时候，记录 Log 到 数据库 里。 
    //***  Log 记录到 数据库 的 优点 是 便于 查询 分析，还可以用 报表 呈现出来。
    //***  报表 也可以呈现给 用户和相关各方 看，作为问题处理追踪 的 一个 报表。
    //
    //***  总的来说，这是一种 松耦合 乐观 的 数据一致性 解决方案。
    public class DBAppender
    {

        private static string _tableName;
        private static IList<string> _columnList;
        private static string _connectionString;


        private int batchSize = 500;
        private int flushInterval = 200;


        private ConcurrentQueue<string[]> queue = new ConcurrentQueue<string[]>();



        static DBAppender()
        {

            _tableName = ConfigurationManager.AppSettings["SOALog.DB.Table"];

            if (string.IsNullOrEmpty(_tableName))
                throw new LogException("缺少 SOALog.DB.Table 配置 。");

            string connectionName = ConfigurationManager.AppSettings["SOALog.DB.ConnectionName"];

            if (string.IsNullOrEmpty(connectionName))
                throw new LogException("缺少 SOALog.DB.ConnectionName 配置 。");

            ConnectionStringSettings connSettings = ConfigurationManager.ConnectionStrings[connectionName];

            if (connSettings == null)
                throw new LogException("缺少 ConnectionString 配置 。");

            _connectionString = connSettings.ConnectionString;

            if (string.IsNullOrEmpty(_connectionString))
                throw new LogException("ConnectionString 不能为空 。");

            string columnStr = ConfigurationManager.AppSettings["SOALog.DB.Columns"];

            if (string.IsNullOrWhiteSpace(columnStr))
                throw new LogException("缺少 SOALog.DB.Columns 配置 。");

            string[] tokens = columnStr.Split(new char[] { ',' });

            _columnList = new List<string>();

            foreach(string token in tokens)
            {
                _columnList.Add(token.Trim());
            }

        }

        public void Start()
        {
            Thread thread = new Thread(new ThreadStart(Work));
            thread.Start();
        }

        private void Work()
        {

            try
            {
                string[] values;

                while (true)
                {
                    if (this.queue.Count == 0)
                    {
                        Thread.Sleep(this.flushInterval);

                        continue;
                    }

                    IList<string[]> list = new List<string[]>();

                    for (int i = 0; i < this.batchSize; i++)
                    {
                        if (this.queue.Count == 0)
                            break;

                        if (this.queue.TryDequeue(out values))
                        {
                            list.Add(values);
                        }

                    }

                    try
                    {
                        SaveToDB(list);
                    }
                    catch
                    {

                    }

                }
            }
            finally
            {
                this.isValid = false;
            }
         
        }

        private static void SaveToDB(IList<string[]> list)
        {

            if (list.Count == 0)
                return;


            StringBuilder sb = new StringBuilder();


            for(int i=0; i<list.Count; i++)
            {
                sb.Append("insert into " + _tableName + " (");
                
                foreach(string columnName in _columnList)
                {
                    sb.Append(columnName + ", ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append(") values (");

                foreach (string columnName in _columnList)
                {
                    sb.Append("@" + columnName + i.ToString() + ", ");
                }
                sb.Remove(sb.Length - 2, 2);

                sb.Append(");");
            }


            using (SqlCommand cmd = new SqlCommand(sb.ToString()))
            {
                string key;
                string value;
                string[] values;
                
                for (int i = 0; i < list.Count; i++)
                {
                    for(int j=0; j<_columnList.Count; j++)
                    {
                        key = _columnList[j];
                        values = list[i];
                        value = values[j];

                        cmd.Parameters.AddWithValue("@" + key + i.ToString(), value);
                    }
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    cmd.Connection = conn;

                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        public ConcurrentQueue<string[]> Queue
        {
            get { return this.queue; }
        }


        private bool isValid = true;
        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }
    }

}
