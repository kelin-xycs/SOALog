using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Collections.Concurrent;
using System.Threading;

namespace SOALog
{
    public class Log
    {


        private static ConcurrentQueue<string[]> _dbQueue = new ConcurrentQueue<string[]>();
        private static ConcurrentQueue<string> _commonQueue = new ConcurrentQueue<string>();


        private const int _dispatchInterval = 200;
        private const int _batchSize = 500;


        private static DBAppender _dbAppender;
        private static List<ICommonAppender> _commonAppenderList = new List<ICommonAppender>();


        static Log()
        {

            string str = ConfigurationManager.AppSettings["SOALog.Appenders"];

            if (str == null)
            {
                throw new LogException("缺少 SOALog.Appenders 配置 。");
            }
            

            string[] tokens = str.Split(',');


            string t;

            ICommonAppender appender;

            for (int i = 0; i < tokens.Length; i++)
            {
                t = tokens[i].Trim();

                if (t == "DB")
                {
                    _dbAppender = new DBAppender();
                    continue;
                }

                appender = GetAppender(t);

                _commonAppenderList.Add(appender);
                
            }


            for (int i = 0; i < _commonAppenderList.Count; i++)
            {
                appender = _commonAppenderList[i];

                appender.Start();
            }


            if (_dbAppender != null)
            {
                _dbAppender.Start();
            }
            

            Thread threadCommon = new Thread(DispatchCommon);
            threadCommon.Start();

            Thread threadDB = new Thread(DispatchDB);
            threadDB.Start();
        }

        private static ICommonAppender GetAppender(string name)
        {
            switch(name)
            {
                case "Console" :
                    return new ConsoleAppender();

                case "WinForm" :
                    return new WinFormAppender();

                case "File":
                    return new FileAppender();

                default:
                    throw new LogException("不存在的 Appender ： \"" + name + "\" 。 注意大小写区分 。");
            }
        }

        private static void DispatchCommon()
        {

            while (true)
            {

                if (_commonQueue.Count == 0)
                {
                    Thread.Sleep(_dispatchInterval);
                }

                DispatchCommonAppender();

            }

        }

        private static void DispatchDB()
        {

            while(true)
            {

                if (_dbQueue.Count == 0)
                {
                    Thread.Sleep(_dispatchInterval);
                }

                DispatchDBAppender();

            }

        }

        private static void DispatchCommonAppender()
        {
            string msg;

            ICommonAppender appender;

            for (int i = 0; i < _batchSize; i++)
            {
                if (!_commonQueue.TryDequeue(out msg))
                    break;

                for (int j = 0; j < _commonAppenderList.Count; j++)
                {
                    appender = _commonAppenderList[j];

                    if (!appender.IsValid)
                    {
                        _commonAppenderList.Remove(appender);

                        continue;
                    }

                    appender.Queue.Enqueue(msg);
                }
            }
        }

        private static void DispatchDBAppender()
        {
            string[] values;

            for (int i = 0; i < _batchSize; i++)
            {

                if (!_dbQueue.TryDequeue(out values))
                    break;

                if (!_dbAppender.IsValid)
                    continue;

                _dbAppender.Queue.Enqueue(values);

            }
        }

        public static void Info(string msg)
        {
            if (_commonAppenderList.Count == 0)
                return;

            _commonQueue.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") 
                + " [" + Thread.CurrentThread.ManagedThreadId + "] "
                + msg);
        }

        public static void Info(string[] values)
        {
            if (!_dbAppender.IsValid)
                return;

            _dbQueue.Enqueue(values);
        }
    }
}
