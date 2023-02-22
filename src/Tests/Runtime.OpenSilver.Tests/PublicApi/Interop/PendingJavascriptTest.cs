
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


using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runtime.OpenSilver.PublicAPI.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.OpenSilver.Tests.PublicApi.Interop
{
    [TestClass]
    public class PendingJavascriptTest
    {
        [TestMethod]
        public void Should_Aggregate_Javascript()
        {
            var pj = new PendingJavascript(1024);

            pj.AddJavascript("console.log(1)");
            pj.AddJavascript("console.log(2)");

            pj.TakeJsOut().Should().Be("console.log(1);\nconsole.log(2);\n");
        }

        [TestMethod]
        public void Should_Handle_Small_Buffer_Size()
        {
            var pj = new PendingJavascript(4);

            pj.AddJavascript("console.log(1)");

            pj.TakeJsOut().Should().Be("console.log(1);\n");
        }
    }
}
