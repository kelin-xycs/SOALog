<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageDB.aspx.cs" Inherits="Demo.ManageDB" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <div>
            左边 文本框 是 要执行的 Sql ，  右边 文本框 是 一些常用的 Sql ， 可以 复制 到 左边 执行
        </div>
        <br />
        <asp:TextBox ID="txtSql" runat="server" TextMode="MultiLine" Height="328px" Width="581px"></asp:TextBox>
        <asp:TextBox ID="txtSqlBack" runat="server" TextMode="MultiLine" Height="328px" Width="581px"></asp:TextBox>
        <br />
         <asp:Button ID="btnExecQuery" runat="server" Text="ExecQuery" OnClick="btnExecQuery_Click" Width="107px" />
        <asp:Button ID="btnExecNonQuery" runat="server" Text="ExecNonQuery" OnClick="btnExecNonQuery_Click" Width="107px" />
        <asp:GridView ID="gdvData" runat="server"></asp:GridView>
    </form>
</body>
</html>
