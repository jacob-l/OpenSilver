
/*===================================================================================
*
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/


using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace OpenSilver.Compiler.EntryPoints
{
    public class BrowserFilesGenerator : Task
    {
        private const string IndexFileName = "Index.cs";

        [Required]
        public string Namespace { get; set; }

        [Required]
        public string Location { get; set; }

        [Output]
        public string[] GeneratedCsFiles { get; set; }

        public override bool Execute()
        {
            var generatedCsFiles = new List<string>
                {
                    GenerateIndexFile(Location, Namespace)
                };
            GeneratedCsFiles = generatedCsFiles.ToArray();
            return true;
        }

        private static string GenerateIndexFile(string location, string ns)
        {
            var content = "using DotNetForHtml5;\r\n" +
                          "using Microsoft.AspNetCore.Components;\r\n" +
                          "using Microsoft.AspNetCore.Components.Rendering;\r\n" +
                          "using Microsoft.JSInterop;\r\n" +
                          "using Microsoft.JSInterop.WebAssembly;\r\n\r\n" +
                          "//version: 2\r\n" +
                          $"namespace {ns}\r\n" +
                          "{\r\n" +
                          "    [Route(\"/\")]\r\n" +
                          "    public class Index : ComponentBase\r\n" +
                          "    {\r\n" +
                          "        private class ExecutionHandler : IWebAssemblyExecutionHandler\r\n" +
                          "        {\r\n" +
                          "            private const string MethodName = \"callJSUnmarshalled\";\r\n" +
                          "            private readonly WebAssemblyJSRuntime _runtime;\r\n\r\n" +
                          "            public ExecutionHandler(IJSRuntime runtime)\r\n" +
                          "            {\r\n" +
                          "                _runtime = runtime as WebAssemblyJSRuntime;\r\n" +
                          "            }\r\n\r\n" +
                          "            public void ExecuteJavaScript(string javaScriptToExecute)\r\n" +
                          "            {\r\n" +
                          "                _runtime.InvokeUnmarshalled<string, object>(MethodName, javaScriptToExecute);\r\n" +
                          "            }\r\n\r\n" +
                          "            public object ExecuteJavaScriptWithResult(string javaScriptToExecute)\r\n" +
                          "            {\r\n" +
                          "                return _runtime.InvokeUnmarshalled<string, object>(MethodName, javaScriptToExecute);\r\n" +
                          "            }\r\n\r\n" +
                          "            public TResult InvokeUnmarshalled<T0, TResult>(string identifier, T0 arg0)\r\n" +
                          "            {\r\n" +
                          "                return _runtime.InvokeUnmarshalled<T0, TResult>(identifier, arg0);\r\n" +
                          "            }\r\n\r\n" +
                          "            public TResult InvokeUnmarshalled<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1)\r\n" +
                          "            {\r\n" +
                          "                return _runtime.InvokeUnmarshalled<T0, T1, TResult>(identifier, arg0, arg1);\r\n" +
                          "            }\r\n\r\n" +
                          "            public TResult InvokeUnmarshalled<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2)\r\n" +
                          "            {\r\n" +
                          "                return _runtime.InvokeUnmarshalled<T0, T1, T2, TResult>(identifier, arg0, arg1, arg2);\r\n" +
                          "            }\r\n" +
                          "        }\r\n\r\n" +
                          "        protected override void BuildRenderTree(RenderTreeBuilder __builder)\r\n" +
                          "        {\r\n" +
                          "        }\r\n\r\n" +
                          "        protected override void OnInitialized()\r\n" +
                          "        {\r\n" +
                          "            base.OnInitialized();\r\n" +
                          "            Cshtml5Initializer.Initialize(new ExecutionHandler(JSRuntime));\r\n" +
                          "            Program.RunApplication();\r\n" +
                          "        }\r\n\r\n" +
                          "        [Inject]\r\n" +
                          "        private IJSRuntime JSRuntime { get; set; }\r\n" +
                          "    }\r\n" +
                          "}";

            var fullPath = Path.Combine(location, IndexFileName);
            File.WriteAllText(fullPath, content);

            return fullPath;
        }
    }
}
