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
        // 这个项目是演示  Console , WinForm , File  3 种 输出方式
        // DB 的 方式 在 Demo 项目中
        static void Main(string[] args)
        {

            //  App.config 中有相应的配置

            Log.Info("Hello 1");
            Log.Info("Hello 2");
            Log.Info("Hello 3");
            Log.Info("Hello 4");
            Log.Info("Hello 5");
            Log.Info("Hello 6");
           
            Console.ReadLine();

        }
    }
}
