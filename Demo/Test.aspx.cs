using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SOALog;

namespace Demo
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            //  Web.config 中有相应的配置

            //  将 Log 写到 DB
            Log.Info(new string[] {"微信", "支付", "超时", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
                System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()});

            //  将 Log 写到 File
            Log.Info("Hello ");

        }
    }
}