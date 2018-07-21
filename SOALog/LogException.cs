using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOALog
{
    public class LogException : Exception 
    {
        internal LogException(string message) : base(message)
        {

        }
    }
}
