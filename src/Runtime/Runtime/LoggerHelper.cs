using System;
using System.Collections.Generic;
using System.Text;
using OpenSilverLogger;

namespace Runtime.OpenSilver
{
    public class LoggerHelper
    {
        public static void Print()
        {
            LoggerAdapter.List.Print();
        }
    }
}
