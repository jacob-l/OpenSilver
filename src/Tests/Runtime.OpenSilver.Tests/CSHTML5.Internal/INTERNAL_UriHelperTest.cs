using CSHTML5.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSHTML5.Internal;

namespace Runtime.OpenSilver.Tests.CSHTML5.Internal
{
    [TestClass]
    public class INTERNAL_UriHelperTest
    {
        [TestMethod]
        public void ConvertToHtml5Path_Is_CaseSensitive()
        {
            //Assert.AreEqual("resources/OpenSilver/Logo.png", INTERNAL_UriHelper.ConvertToHtml5Path("ms-appx:///Logo.png"));
            Assert.AreEqual("resources/Runtime.OpenSilver.Tests/Logo.png", INTERNAL_UriHelper.ConvertToHtml5Path("ms-appx:///Runtime.OpenSilver.Tests/Logo.png"));
            Assert.AreEqual("resources/OpenSilver/Logo.png", INTERNAL_UriHelper.ConvertToHtml5Path("pack://application:,,,/Logo.png"));
            Assert.AreEqual("resources/Runtime.OpenSilver.Tests/Logo.png", INTERNAL_UriHelper.ConvertToHtml5Path("pack://application:,,,/Runtime.OpenSilver.Tests/Logo.png"));
        }
    }
}
