using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOALog
{
    public partial class WinForm : Form
    {
        public WinForm()
        {
            InitializeComponent();
        }

        public bool isLoad;

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            this.isLoad = true;
        }
        
        public void WriteMsg(string msg)
        {
            if (this.isStop)
                return;

            txtMsg.AppendText(msg + "\r\n");
        }

        private bool isStop;

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.isStop)
            {
                this.isStop = false;
                btnStop.Text = "停止";
            }
            else
            {
                this.isStop = true;
                btnStop.Text = "开始";
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMsg.Clear();
        }

        
    }
}
