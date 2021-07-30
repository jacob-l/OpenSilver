using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.JSInterop;
using OpenSilverLogger;

namespace Runtime.OpenSilver
{
    public class LoggerHelper
    {
        [JSInvokable("errorhappened")]
        public static void OnError()
        {
            Console.WriteLine("ERROR HANDLER");
            LoggerAdapter.List.Print();
        }

        [JSInvokable("enableLogger")]
        public static void EnableLogger()
        {
            Console.WriteLine("Enabled Logger helper");
            LoggerAdapter.Enabled = true;
        }

        [JSInvokable("disableLogger")]
        public static void DisableLogger()
        {
            Console.WriteLine("Disable Logger helper");
            LoggerAdapter.Enabled = false;
        }

        [JSInvokable("printLogs")]
        public static void PrintLogs()
        {
            LoggerAdapter.List.Print();
        }
    }
}
