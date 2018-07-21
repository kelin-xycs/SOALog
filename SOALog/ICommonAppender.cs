using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent;

namespace SOALog
{
    interface ICommonAppender
    {
        ConcurrentQueue<string> Queue { get; }

        void Start();

        bool IsValid { get; }
    }
}
