using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace SOALog
{
    class FileAppender : ICommonAppender
    {


        private static Stream _stream;


        private int batchSize = 500;
        private int flushInterval = 200;


        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

        public ConcurrentQueue<string> Queue
        {
            get { return queue; }
        }


        static FileAppender()
        {

            string file = ConfigurationManager.AppSettings["SOALog.File.File"];

            if (string.IsNullOrEmpty(file))
            {
                throw new LogException("缺少 SOALog.File.File 配置。 需要通过 SOALog.File.File 指定 Log 文件的 文件名 或者 相对路径 。");
            }

            file = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, file);

            
            if (!File.Exists(file))
            {
                using(Stream s = File.Create(file))
                {

                }
            }

            _stream = File.Open(file, FileMode.Append, FileAccess.Write, FileShare.Read);

        }

        public void Start()
        {
            Thread thread = new Thread(Work);
            thread.Start();
        }
        
        private void Work()
        {

            try
            {
                string msg;

                while (true)
                {
                    if (this.queue.Count == 0)
                    {
                        Thread.Sleep(this.flushInterval);

                        continue;
                    }

                    IList<string> list = new List<string>();

                    for (int i = 0; i < this.batchSize; i++)
                    {
                        if (this.queue.Count == 0)
                            break;

                        if (this.queue.TryDequeue(out msg))
                        {
                            list.Add(msg);
                        }

                    }

                    byte[] bytes;
                    foreach(string s in list)
                    {
                        bytes = Encoding.UTF8.GetBytes(s + "\r\n");

                        _stream.Write(bytes, 0, bytes.Length);
                    }

                    _stream.Flush();

                }
            }
            finally
            {
                this.isValid = false;
            }
         
        }


        private bool isValid = true;

        public bool IsValid
        {
            get { return isValid; }
        }
    }
}
