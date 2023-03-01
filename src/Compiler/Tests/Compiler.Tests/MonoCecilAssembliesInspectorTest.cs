
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
        private const string ExperimentalNamespace = "Experimental";

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
            res.ContainsKey("Experimental.file.js").Should().BeTrue();
        }

        [TestMethod]
        public void GetAttachedPropertyGetMethodInfo_Should_Find_AttachedPropertyGetMethod()
        {
            MonoCecilVersion.GetAttachedPropertyGetMethodInfo("GetPlacementTarget", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "ToolTipService",
                out var declaringTypeName, out var returnValueNamespaceName, out var returnValueLocalTypeName, out var isTypeString, out var isTypeEnum);

            declaringTypeName.Should().Be("global::System.Windows.Controls.ToolTipService");
            returnValueNamespaceName.Should().Be("System.Windows");
            returnValueLocalTypeName.Should().Be("UIElement");
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void IsTypeAnEnum_Should_Return_True_For_Enum()
        {
            var res = MonoCecilVersion.IsTypeAnEnum(ExperimentalSubjectName, "PlanetStructure");
            res.Should().BeTrue();
        }

        [TestMethod]
        public void GetPropertyOrFieldTypeInfo_Should_Handle_Generic_Parameters()
        {
            MonoCecilVersion.GetPropertyOrFieldTypeInfo("MyProperty", ExperimentalNamespace, "TypeWithGenericParameter",
                out var propertyNamespaceName, out var propertyLocalTypeName, out var propertyAssemblyName,
                out var isTypeString,
                out var isTypeEnum);
            propertyNamespaceName.Should().Be("System");
            propertyLocalTypeName.Should().Be("String");
            propertyAssemblyName.Should().Be("mscorlib");
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetPropertyOrFieldTypeInfo_Should_Handle_Nested_Enum_Field()
        {
            MonoCecilVersion.GetPropertyOrFieldTypeInfo("Behavior", ExperimentalNamespace, "ClassWithField",
                out var propertyNamespaceName, out var propertyLocalTypeName, out var propertyAssemblyName,
                out var isTypeString,
                out var isTypeEnum);
            propertyNamespaceName.Should().Be("Experimental.ClassWithNestedEnum");
            propertyLocalTypeName.Should().Be("InputBehavior");
            propertyAssemblyName.Should().Be(ExperimentalSubjectName);
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeTrue();
        }

        [TestMethod]
        public void GetPropertyOrFieldInfo_Should_Handle_Generic_Parameter()
        {
            MonoCecilVersion.GetPropertyOrFieldInfo("MyProperty", ExperimentalNamespace, "TypeWithGenericParameter",
                out var memberDeclaringTypeName, out var memberTypeNamespace, out var memberTypeName,
                out var isTypeString, out var isTypeEnum);
            memberDeclaringTypeName.Should().Be("global::Experimental.GenericType<global::System.String>");
            memberTypeNamespace.Should().Be("System");
            memberTypeName.Should().Be("String");
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetPropertyOrFieldInfo_Should_Handle_NonGeneric_Parameter()
        {
            MonoCecilVersion.GetPropertyOrFieldInfo("MyNonGenericProperty", ExperimentalNamespace, "TypeWithGenericParameter",
                out var memberDeclaringTypeName, out var memberTypeNamespace, out var memberTypeName,
                out var isTypeString, out var isTypeEnum);
            memberDeclaringTypeName.Should().Be("global::Experimental.GenericType<global::System.String>");
            memberTypeNamespace.Should().Be("System");
            memberTypeName.Should().Be("Int32");
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetFiled_Should_Return_Full_TypeName()
        {
            var res = MonoCecilVersion.GetField("MyField", ExperimentalNamespace, "TypeWithGenericParameter", null);
            res.Should().Be("global::Experimental.TypeWithGenericParameter.MyField");
        }

        [TestMethod]
        public void GetAttachedPropertyGetMethodInfo_Should_Handle_Generic()
        {
            MonoCecilVersion.GetAttachedPropertyGetMethodInfo("GetHasSomething", ExperimentalNamespace, "TypeWithGenericParameter",
                out var declaringTypeName, out var returnValueNamespaceName, out var returnValueLocalTypeName,
                out var isTypeString, out var isTypeEnum);
            declaringTypeName.Should().Be("global::Experimental.GenericType<global::System.String>");
            returnValueNamespaceName.Should().Be("System");
            returnValueLocalTypeName.Should().Be("Boolean");
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetFieldName_Should_Handle_Nested_Enum_Type()
        {
            var res = MonoCecilVersion.GetFieldName("Selectfromlist", "ClassWithNestedEnum", "InputBehavior");

            res.Should().Be("SelectFromList");
        }

        [TestMethod]
        public void GetContentPropertyName_Should_Return_Value()
        {
            var res = MonoCecilVersion.GetContentPropertyName("System.Windows.Controls", "ContentControl");
            res.Should().Be("Content");
        }

        [TestMethod]
        public void GetContentPropertyName_Should_Handle_3rd_Party()
        {
            var res = MonoCecilVersion.GetContentPropertyName(ExperimentalNamespace, "DerivedContentControl", ExperimentalSubjectName);
            res.Should().Be("Content");
        }
    }
}
