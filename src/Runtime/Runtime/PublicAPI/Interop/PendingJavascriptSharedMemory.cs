using DotNetForHtml5;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runtime.OpenSilver.PublicAPI.Interop
{
    public static class PendingJavascriptSharedMemory
    {
        private static readonly char[] CharArr = new char[512 * 1024 * 1]; //1mb

        private static IJavaScriptExecutionHandler2 _executionHandler;

        private static int currentLength = 0;

        private static void Initialize(IJavaScriptExecutionHandler2 executionHandler)
        {
            //Console.WriteLine("INITIALIZE 1");
            try
            {
                _executionHandler = executionHandler;
                executionHandler.InvokeUnmarshalled<char[], object>("register", CharArr);
                //Console.WriteLine("INITIALIZE 2");
            }
            catch(Exception ex)
            {
                Console.WriteLine("INSIDE INITIALIZE - " + ex.Message);
                throw;
            }
        }

        public static void AddJavascript(string javascript)
        {
            //Console.WriteLine("AddJavascript 1");
            try
            {
                CharArr[currentLength++] = '\n';
                CharArr[currentLength++] = ';';
                for (var i = 0; i < javascript.Length; i++)
                {
                    CharArr[currentLength + i] = javascript[i];
                }

                currentLength += javascript.Length;
                //Console.WriteLine("AddJavascript 2");
            }
            catch(Exception ex)
            {
                Console.WriteLine("INSIDE AddJavascript - " + ex.Message);
                throw;
            }
        }

        public static object ExecutePending<T>(IJavaScriptExecutionHandler2 executionHandler)
        {
            //Console.WriteLine("ExecutePending 1 ");
            try
            {
                if (_executionHandler == null)
                {
                    Initialize(executionHandler);
                }

                var res = _executionHandler.InvokeUnmarshalled<int, T>("callJSUnmarshalledSharedMemory",
                    currentLength);
                currentLength = 0;
                //Console.WriteLine("ExecutePending 2");
                return res;
            }
            catch(Exception ex)
            {
                Console.WriteLine("INSIDE ExecutePending - " + ex.Message);
                throw;
            }
        }
        
        public static bool IsEmpty => currentLength == 0;
    }
}
