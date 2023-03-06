
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
using DotNetForHtml5.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Windows;
using Experimental;
using System.Windows.Controls;

namespace Compiler.Tests
{
    [TestClass]
    public class MonoCecilAssembliesInspectorTest
    {
        private const string ExperimentalSubjectName = "Experimental";
        private const string ExperimentalSubjectDll = ExperimentalSubjectName + ".dll";
        private const string ExperimentalNamespace = "Experimental";
        private const string Content = "Content";

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
            var res = MonoCecilVersion.GetAssemblyQualifiedNameOfXamlType("http://schemas.microsoft.com/winfx/2006/xaml/presentation", nameof(Validation), null);

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
            MonoCecilVersion.GetAttachedPropertyGetMethodInfo(nameof(ToolTipService.GetPlacementTarget), "http://schemas.microsoft.com/winfx/2006/xaml/presentation", nameof(ToolTipService),
                out var declaringTypeName, out var returnValueNamespaceName, out var returnValueLocalTypeName, out var isTypeString, out var isTypeEnum);

            declaringTypeName.Should().Be("global::System.Windows.Controls.ToolTipService");
            returnValueNamespaceName.Should().Be(typeof(UIElement).Namespace);
            returnValueLocalTypeName.Should().Be(nameof(UIElement));
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void IsTypeAnEnum_Should_Return_True_For_Enum()
        {
            var res = MonoCecilVersion.IsTypeAnEnum(ExperimentalSubjectName, nameof(PlanetStructure));
            res.Should().BeTrue();
        }

        [TestMethod]
        public void GetPropertyOrFieldTypeInfo_Should_Handle_Generic_Parameters()
        {
            MonoCecilVersion.GetPropertyOrFieldTypeInfo(nameof(TypeWithGenericParameter.MyProperty), ExperimentalNamespace, nameof(TypeWithGenericParameter),
                out var propertyNamespaceName, out var propertyLocalTypeName, out var propertyAssemblyName,
                out var isTypeString,
                out var isTypeEnum);
            propertyNamespaceName.Should().Be(typeof(string).Namespace);
            propertyLocalTypeName.Should().Be(nameof(String));
            propertyAssemblyName.Should().Be(typeof(string).Assembly.GetName().Name);
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetPropertyOrFieldTypeInfo_Should_Handle_Nested_Enum_Field()
        {
            MonoCecilVersion.GetPropertyOrFieldTypeInfo(nameof(ClassWithField.Behavior), ExperimentalNamespace, nameof(ClassWithField),
                out var propertyNamespaceName, out var propertyLocalTypeName, out var propertyAssemblyName,
                out var isTypeString,
                out var isTypeEnum);
            propertyNamespaceName.Should().Be(typeof(ClassWithNestedEnum).FullName);
            propertyLocalTypeName.Should().Be(nameof(ClassWithNestedEnum.InputBehavior));
            propertyAssemblyName.Should().Be(ExperimentalSubjectName);
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeTrue();
        }

        [TestMethod]
        public void GetPropertyOrFieldInfo_Should_Handle_Generic_Parameter()
        {
            MonoCecilVersion.GetPropertyOrFieldInfo(nameof(TypeWithGenericParameter.MyProperty), ExperimentalNamespace, nameof(TypeWithGenericParameter),
                out var memberDeclaringTypeName, out var memberTypeNamespace, out var memberTypeName,
                out var isTypeString, out var isTypeEnum);
            memberDeclaringTypeName.Should().Be("global::Experimental.GenericType<global::System.String>");
            memberTypeNamespace.Should().Be(typeof(string).Namespace);
            memberTypeName.Should().Be(nameof(String));
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetPropertyOrFieldInfo_Should_Handle_NonGeneric_Parameter()
        {
            MonoCecilVersion.GetPropertyOrFieldInfo(nameof(TypeWithGenericParameter.MyNonGenericProperty), ExperimentalNamespace, nameof(TypeWithGenericParameter),
                out var memberDeclaringTypeName, out var memberTypeNamespace, out var memberTypeName,
                out var isTypeString, out var isTypeEnum);
            memberDeclaringTypeName.Should().Be("global::Experimental.GenericType<global::System.String>");
            memberTypeNamespace.Should().Be(typeof(int).Namespace);
            memberTypeName.Should().Be(nameof(Int32));
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetFiled_Should_Return_Full_TypeName()
        {
            var res = MonoCecilVersion.GetField(nameof(TypeWithGenericParameter.MyField), ExperimentalNamespace, nameof(TypeWithGenericParameter), null);
            res.Should().Be("global::Experimental.TypeWithGenericParameter.MyField");
        }

        [TestMethod]
        public void GetAttachedPropertyGetMethodInfo_Should_Handle_Generic()
        {
            MonoCecilVersion.GetAttachedPropertyGetMethodInfo(nameof(TypeWithGenericParameter.GetHasSomething), ExperimentalNamespace, nameof(TypeWithGenericParameter),
                out var declaringTypeName, out var returnValueNamespaceName, out var returnValueLocalTypeName,
                out var isTypeString, out var isTypeEnum);
            declaringTypeName.Should().Be("global::Experimental.GenericType<global::System.String>");
            returnValueNamespaceName.Should().Be(typeof(bool).Namespace);
            returnValueLocalTypeName.Should().Be(nameof(Boolean));
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetFieldName_Should_Handle_Nested_Enum_Type()
        {
            var res = MonoCecilVersion.GetFieldName(nameof(ClassWithNestedEnum.InputBehavior.SelectFromList).ToLower(),typeof(ClassWithNestedEnum).FullName, nameof(ClassWithNestedEnum.InputBehavior));

            res.Should().Be(nameof(ClassWithNestedEnum.InputBehavior.SelectFromList));
        }

        [TestMethod]
        public void GetContentPropertyName_Should_Return_Value()
        {
            var res = MonoCecilVersion.GetContentPropertyName(typeof(ContentControl).Namespace, nameof(ContentControl));
            res.Should().Be(Content);
        }

        [TestMethod]
        public void GetContentPropertyName_Should_Handle_3rd_Party()
        {
            var res = MonoCecilVersion.GetContentPropertyName(ExperimentalNamespace, nameof(DerivedContentControl), ExperimentalSubjectName);
            res.Should().Be(Content);
        }

        [TestMethod]
        public void GetMethodReturnValueTypeInfo_Should_Handle_Generic_Return_Type()
        {
            MonoCecilVersion.GetMethodReturnValueTypeInfo(
                nameof(TypeWithGenericParameter.MethodWithGenericReturnType),
                ExperimentalNamespace, nameof(TypeWithGenericParameter), out var returnValueNamespace,
                out var returnValueTypeName, out var returnValueAssemblyName, out var isTypeString,
                out var isTypeEnum);
            returnValueNamespace.Should().Be(typeof(string).Namespace);
            returnValueTypeName.Should().Be(nameof(String));
            returnValueAssemblyName.Should().Be(typeof(string).Assembly.GetName().Name);
            isTypeString.Should().BeTrue();
            isTypeEnum.Should().BeFalse();
        }

        [TestMethod]
        public void GetMethodReturnValueTypeInfo_Returns_Method_With_GenericType()
        {
            MonoCecilVersion.GetMethodReturnValueTypeInfo(
                nameof(TypeWithGenericParameter.MethodReturnsAnotherGeneric),
                ExperimentalNamespace, nameof(TypeWithGenericParameter), out var returnValueNamespace,
                out var returnValueTypeName, out var returnValueAssemblyName, out var isTypeString,
                out var isTypeEnum);
            returnValueNamespace.Should().Be(ExperimentalNamespace);
            returnValueTypeName.Should().Be("AnotherGenericType<global::System.String>");
            returnValueAssemblyName.Should().Be(ExperimentalSubjectName);
            isTypeString.Should().BeFalse();
            isTypeEnum.Should().BeFalse();
        }
    }
}
