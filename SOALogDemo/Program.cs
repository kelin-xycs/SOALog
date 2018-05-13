using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SOALog;

namespace SOALogDemo
{
    class Program
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
        static void Main(string[] args)
        {
            Log.Error(new string[] { "微信", "支付", "0", "超时" });
            Log.Error(new string[] { "微信", "支付", "1", "超时" });
            
            Console.ReadLine();
        }
    }
}
