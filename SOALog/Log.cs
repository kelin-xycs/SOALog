using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Threading;
using System.Data.SqlClient;

namespace SOALog
{
    //***  因为我本机没有 Sql Server 环境，所以这个程序实际上并没有在 Sql Server 上运行测试过
    //***  只测试过 生成的 Sql 和 参数
    //
    //***  下面说一下编写 SOALog 的 缘由 和 理念
    //
    //***  其实我不太赞同 微服务。但 SOA 倒是可能成为 趋势。因为 平台 与 平台 之间的 交互 应该会是一个 趋势
    //***  微服务 的 第一个 课题 就是 实现 数据一致性。SOA 也需要 解决 这个问题。
    //***  但实际上，数据不一致 并不可怕，关键在于 清晰的 记录Log，及 提示出来，
    //***  让 用户 和 各方 能够清楚的看到 这一笔交易 是 失败 的，问题出在了哪里，接下来要怎么处理。
    //***  所以，SOALog 的 作用 就是可以在 调用服务失败的时候，记录 Log 到 数据库 里。 
    //***  Log 记录到 数据库 的 优点 是 便于 查询 分析，还可以用 报表 呈现出来。
    //***  报表 也可以呈现给 用户和相关各方 看，作为问题处理追踪 的 一个 报表。
    public class Log
    {
        private static Queue<string[]> _queue = new Queue<string[]>();

        private static string _tableName;
        private static IList<string> _columnList;
        private static string _connectionString;
        
        //  可根据实际需要 修改 这些参数
        private static int _interval = 2000;  // 每次向数据库写入 Log 的 时间间隔
        private static int _batchInsertToDBCount = 10;  // 每次向数据库写入多少笔记录
        private static int _queueCount = 100;  // 队列中最多缓存多少笔记录

        static Log()
        {
            _tableName = ConfigurationManager.AppSettings["SOALog.Table"];
            _connectionString = ConfigurationManager.AppSettings["SOALog.ConnectionString"];

            string columnStr = ConfigurationManager.AppSettings["SOALog.Columns"];

            string[] tokens = columnStr.Split(new char[] { ';' });

            _columnList = new List<string>();

            foreach(string token in tokens)
            {
                _columnList.Add(token.Trim());
            }

            Thread thread = new Thread(new ThreadStart(LogWorker));
            thread.Start();
        }

        private static void LogWorker()
        {
            
            while (true)
            {
                Thread.Sleep(_interval);

                IList<string[]> list = new List<string[]>();

                for (int i = 0; i < _batchInsertToDBCount; i++)
                {
                    if (_queue.Count == 0)
                        break;

                    list.Add(_queue.Dequeue());
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

        private static void SaveToDB(IList<string[]> list)
        {
            

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

                Console.WriteLine(cmd.CommandText);
                foreach(SqlParameter para in cmd.Parameters)
                {
                    Console.WriteLine(para.ParameterName + " " + para.Value);
                }
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
            }

            
        }
        
        public static void Error(string[] values)
        {
            if (_queue.Count >= _queueCount)
                return;

            _queue.Enqueue(values);

        }
    }

}
