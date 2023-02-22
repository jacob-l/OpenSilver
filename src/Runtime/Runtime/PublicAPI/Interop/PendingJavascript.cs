
/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/


using DotNetForHtml5;
using System;
using System.Text;

namespace Runtime.OpenSilver.PublicAPI.Interop
{
    internal class PendingJavascript
    {
        private static readonly byte[] Delimiter = Encoding.UTF8.GetBytes(";\n");
        private byte[] _buffer;
        private bool _initialized;
        private int _currentLength;

        public PendingJavascript(int bufferSize)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentException("Buffer size can not be less or equal to 0");
            }
            _buffer = new byte[bufferSize];
        }

        private void Initialize(IJavaScriptExecutionHandler2 executionHandler)
        {
            executionHandler.InvokeUnmarshalled<byte[], object>("register", _buffer);
        }

        private void IncreaseBuffer()
        {
            Console.WriteLine($"WARNING! Increasing Pending Javascript Buffer. Please consider changing the initial size by passing '{_buffer.Length * 2}' to Initialize method");
            var currentBuffer = _buffer;
            _buffer = new byte[currentBuffer.Length * 2];
            currentBuffer.CopyTo(_buffer, 0);
            _initialized = false;
        }

        public void AddJavascript(string javascript)
        {
            lock (_buffer)
            {
                var maxByteCount = Encoding.UTF8.GetMaxByteCount(javascript.Length);
                while (maxByteCount + _currentLength > _buffer.Length)
                {
                    IncreaseBuffer();
                }

                _currentLength += Encoding.UTF8.GetBytes(javascript, 0, javascript.Length, _buffer, _currentLength);

                System.Buffer.BlockCopy(Delimiter, 0, _buffer, _currentLength, Delimiter.Length);
                _currentLength += Delimiter.Length;
            }
        }

        public object ExecutePending(IJavaScriptExecutionHandler2 executionHandler)
        {
            if (_currentLength == 0)
            {
                return null;
            }

            if (!_initialized)
            {
                Initialize(executionHandler);
            }

            var res = executionHandler.InvokeUnmarshalled<int, object>("callJSUnmarshalledSharedMemory2",
                _currentLength);
            _currentLength = 0;
            return res;
        }

        public string TakeJsOut()
        {
            lock (_buffer)
            {
                var res = Encoding.UTF8.GetString(_buffer, 0, _currentLength);
                _currentLength = 0;
                return res;
            }
        }
    }
}
