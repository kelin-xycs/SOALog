using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent;
using System.Windows.Forms;
using System.Threading;

namespace SOALog
{
    class WinFormAppender : ICommonAppender
    {

        private int flushInterval = 200;


        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

        public ConcurrentQueue<string> Queue
        {
            get { return queue; }
        }


        private WinForm form;
        
        public void Start()
        {
            form = new WinForm();

            Thread thread2 = new Thread(OpenForm);
            thread2.Start();

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
                    if (this.queue.Count == 0 || !this.form.isLoad)
                    {
                        Thread.Sleep(this.flushInterval);

                        continue;
                    }

                    if (!this.queue.TryDequeue(out msg))
                        continue;

                    form.BeginInvoke(new Action<string>(form.WriteMsg), new object[] { msg });
                }
            }
            finally
            {
                this.isValid = false;
            }
        }

        private void OpenForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(this.form);
        }

        private bool isValid = true;
        public bool IsValid
        {
            get { return isValid; }
        }
    }
}
