using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSilverLogger
{
    public static class LogManagerAdapter
    {
        public static LoggerAdapter GetLogger(Type type)
        {
            return new LoggerAdapter(type);
        }
    }
}
