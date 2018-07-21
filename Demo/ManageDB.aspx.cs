using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;

using System.Data;
using System.Data.SqlClient;

namespace Demo
{
    public partial class ManageDB : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtSqlBack.Text = @"
一些常用 Sql         

CREATE TABLE SOALogTable
(
platform nvarchar(24),
action nvarchar(24),
exception nvarchar(200),
create_date datetime,
thread_id int,
)

insert into SOALogTable (platform, action, exception, create_date, thread_id)
values (N'微信', N'支付', N'超时', '2018-07-09', 1)

select * from SOALogTable
";

        }

        protected void btnExecNonQuery_Click(object sender, EventArgs e)
        {
            string sql = txtSql.Text; ;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnExecQuery_Click(object sender, EventArgs e)
        {
            string sql = txtSql.Text;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    gdvData.DataSource = cmd.ExecuteReader();
                    gdvData.DataBind();
                }
            }
        }
    }
}