using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace OpenSilverLogger
{
    public class LoggerAdapter
    {
        public static CircularList List = new CircularList(100);
        private readonly Type type;
        private static int level = 0;

        public LoggerAdapter(Type type)
        {
            this.type = type;
        }

        public void TraceEnter(System.String methodInfo, System.Tuple<System.String, System.String>[] pn, System.String[] paramNames, System.Object[] paramValues)
        {
            var parameters = new StringBuilder();
            if (paramNames != null)
            {
                /*
                for (int i = 0; i < paramNames.Length; i++)
                {
                    parameters.AppendFormat("{0}={1}", paramNames[i], paramValues[i] ?? "null");
                    if (i < paramNames.Length - 1) parameters.Append(", ");
                }*/
            }

            var sb = new StringBuilder();
            sb.Append(new string(' ', level));
            sb.Append("Enter2. Type: " + type + " Method: " + methodInfo + " AOT: " + IsAot());

            Console.WriteLine(sb.ToString());
            //List.Add(sb.ToString());

            level++;
        }

        public void TraceLeave(System.String methodInfo, System.Tuple<System.String, System.String>[] on, Int64 start, Int64 end, System.String[] arr1, System.Object[] arr2)
        {
            level--;
            var sb = new StringBuilder();
            sb.Append(new string(' ', level));
            sb.Append("Leave2. Type: " + type + " Method: " + methodInfo);

            Console.WriteLine(sb.ToString());
            //List.Add(sb.ToString());
        }

        public static long FindPrimeNumber(uint n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;// to check if found a prime
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }

        private string IsAot()
        {
            var sw = Stopwatch.StartNew();
            var n = FindPrimeNumber(500);
            sw.Stop();
            var isAot = sw.Elapsed.TotalMilliseconds < 2;
            var message = isAot ? "aot" : "NOT AOT. ATTENTION!!!!";
            return message + " - " + sw.Elapsed.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        }
    }
}
