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

CREATE TABLE Person
(
no nvarchar(24),
name nvarchar(24),
salary int,
create_date datetime,
)

insert into Person (no, name, salary, create_date)
values ('001', N'小明', '2000', '2018-07-09')

select * from Person
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