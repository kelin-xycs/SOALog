using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent;
using System.Threading;

namespace SOALog
{
    class ConsoleAppender : ICommonAppender
    {


        private int flushInterval = 200;


        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();


        public ConcurrentQueue<string> Queue
        {
            get { return queue; }
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

                    if (!this.queue.TryDequeue(out msg))
                        continue;

                    Console.WriteLine(msg);
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
