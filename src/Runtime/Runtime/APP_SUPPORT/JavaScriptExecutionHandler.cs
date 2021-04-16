

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


using System;
using System.Text.Json;
using Microsoft.JSInterop;
using TypeScriptDefinitionsSupport;
using Uno.Foundation;

namespace DotNetForHtml5
{
    public class JavaScriptExecutionHandler : IJavaScriptExecutionHandler
    {
        public IJSRuntime JSRuntime { get; set; }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public void ExecuteJavaScript(string javaScriptToExecute)
        {
            //Console.WriteLine("EXECUTE NO RESULT JS BEFORE");
            WebAssemblyRuntime.InvokeJS(javaScriptToExecute);
            //Console.WriteLine("EXECUTE NO RESULT JS AFTER");
        }

        // Called via reflection by the "INTERNAL_HtmlDomManager" class of the "Core" project.
        public object ExecuteJavaScriptWithResult(string javaScriptToExecute)
        {
            //Console.WriteLine("EXECUTE WITH RESULT JS BEFORE");
            var res = WebAssemblyRuntime.InvokeJS(javaScriptToExecute);
            //Console.WriteLine("EXECUTE WITH RESULT JS AFTER: " + res);

            if (bool.TryParse(res, out var resBool))
            {
                //Console.WriteLine("RETURN BOOL");
                return resBool;
            } else if (int.TryParse(res, out var resInt))
            {
                //Console.WriteLine("RETURN INT");
                return resInt;
            } else if (double.TryParse(res, out var resDouble))
            {
                //Console.WriteLine("RETURN DOUBLE");
                return resDouble;
            }
            return res;

            object result = ((JSInProcessRuntime)JSRuntime).Invoke<object>("callJS", javaScriptToExecute);
            return result;
        }
    }
}
