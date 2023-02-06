
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


using DotNetForHtml5.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Compiler.Tests
{
    [TestClass]
    public class MonoCecilAssembliesInspectorTest
    {
        private const string ExperimentalSubjectName = "Experimental";
        private const string ExperimentalSubjectDll = ExperimentalSubjectName + ".dll";

        private static readonly MonoCecilAssembliesInspectorImpl MonoCecilVersion = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            MonoCecilVersion.LoadAssembly(ExperimentalSubjectDll, true);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            MonoCecilVersion.Dispose();
        }

        [TestMethod]
        public void GetAssemblyQualifiedNameOfXamlType_Should_Return_Name()
        {
            var res = MonoCecilVersion.GetAssemblyQualifiedNameOfXamlType("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Validation", null);

            res.Should().Be("System.Windows.Controls.Validation, OpenSilver");
        }

        [TestMethod]
        public void GetManifestResources_Should_Include_File_Js()
        {
            var res = MonoCecilVersion.GetManifestResources(ExperimentalSubjectName,
                new HashSet<string>
                {
                    ".js", ".css", ".png", ".jpg", ".gif"
                });
            Assert.IsTrue(res.ContainsKey("Experimental.file.js"));
        }
    }
}
