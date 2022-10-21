using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    internal class ConsoleLogger
    {
        internal static void Info(string message)
        {
            Console.WriteLine(string.Format("[Info] {0}:{1}", DateTime.Now, message));
        }

        internal static void Warning(string message)
        {
            Console.WriteLine(string.Format("[Warning] {0}:{1}", DateTime.Now, message));
        }
    }
}
