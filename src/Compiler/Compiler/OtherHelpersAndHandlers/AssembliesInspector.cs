

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DotNetForHtml5.Compiler.Common;
using DotNetForHtml5.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector;

namespace DotNetForHtml5.Compiler
{
    public class AssembliesInspector : IDisposable
    {

        //----------------------------------------------------------------------
        // We create a static instance in the "BeforeXamlPreprocessor" task.
        // The static instance avoids reloading the assemblies for each XAML file.
        // We dispose it in the "AfterXamlPreprocessor" task.
        //----------------------------------------------------------------------
        public static AssembliesInspector Current;

        private readonly MonoCecilAssembliesInspectorImpl _monoCecilVersion = new MonoCecilAssembliesInspectorImpl();

        public void Dispose()
        {
            // Unload everything:
            _monoCecilVersion.Dispose();
            GC.Collect(); // Collects all unused memory
            GC.WaitForPendingFinalizers(); // Waits until GC has finished its work
            GC.Collect();
        }

        public void LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo, bool isBridgeBasedVersion, bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode, bool skipReadingAttributesFromAssemblies)
        {
            _monoCecilVersion.LoadAssembly(assemblyPath, loadReferencedAssembliesToo, isBridgeBasedVersion,
                isCoreAssembly, nameOfAssembliesThatDoNotContainUserCode, skipReadingAttributesFromAssemblies);
        }

        public void LoadAssemblyAndAllReferencedAssembliesRecursively(string assemblyPath, bool isBridgeBasedVersion, bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode, bool skipReadingAttributesFromAssemblies, out List<string> assemblySimpleNames)
        {
            assemblySimpleNames = new List<string>(_monoCecilVersion.LoadAssembly(assemblyPath, true, isBridgeBasedVersion: isBridgeBasedVersion, isCoreAssembly: isCoreAssembly, nameOfAssembliesThatDoNotContainUserCode: nameOfAssembliesThatDoNotContainUserCode));
        }

        public string GetContentPropertyName(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _monoCecilVersion.GetContentPropertyName(namespaceName, localTypeName, assemblyNameIfAny);
        }

        public bool IsPropertyAttached(string propertyName, string declaringTypeNamespaceName, string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName, string declaringTypeAssemblyIfAny = null)
        {
            return _monoCecilVersion.IsPropertyAttached(propertyName, declaringTypeNamespaceName, declaringTypeLocalName, parentNamespaceName, parentLocalTypeName, declaringTypeAssemblyIfAny);
        }

        public bool IsPropertyOrFieldACollection(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _monoCecilVersion.IsPropertyOrFieldACollection(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        public bool IsPropertyOrFieldADictionary(string propertyName, string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _monoCecilVersion.IsPropertyOrFieldADictionary(propertyName, parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
        {
            return _monoCecilVersion.DoesMethodReturnACollection(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
        }

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName, string typeAssemblyNameIfAny = null)
        {
            return _monoCecilVersion.DoesMethodReturnADictionary(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
        }

        public bool IsElementADictionary(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _monoCecilVersion.IsElementADictionary(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        public bool IsElementAMarkupExtension(string parentNamespaceName, string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            return _monoCecilVersion.IsElementAMarkupExtension(parentNamespaceName, parentLocalTypeName, parentAssemblyNameIfAny);
        }

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, bool isAttached = false)
        {
            return _monoCecilVersion.IsTypeAssignableFrom(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom, nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo, isAttached);
        }

        public string GetKeyNameOfProperty(string elementNameSpace, string elementLocalName, string assemblyNameIfAny, string propertyName)
        {
            return _monoCecilVersion.GetKeyNameOfProperty(elementNameSpace, elementLocalName, assemblyNameIfAny, propertyName);
        }

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _monoCecilVersion.DoesTypeContainNameMemberOfTypeString(namespaceName, localTypeName, assemblyNameIfAny);
        }

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _monoCecilVersion.GetCSharpEquivalentOfXamlTypeAsXName(namespaceName, localTypeName, assemblyNameIfAny);
        }

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            return _monoCecilVersion.GetCSharpEquivalentOfXamlTypeAsString(namespaceName, localTypeName,
                    assemblyNameIfAny, ifTypeNotFoundTryGuessing);
        }

        public string GetAssemblyQualifiedNameOfXamlType(string namespaceName, string localTypeName, string assemblyName)
        {
            return _monoCecilVersion.GetAssemblyQualifiedNameOfXamlType(namespaceName, localTypeName, assemblyName);
        }

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _monoCecilVersion.GetMemberType(memberName, namespaceName, localTypeName, assemblyNameIfAny);
        }

        public bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            return _monoCecilVersion.IsTypeAnEnum(namespaceName, localTypeName, assemblyNameIfAny);
        }

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out string returnValueAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
        {
            _monoCecilVersion.GetMethodReturnValueTypeInfo(methodName, namespaceName, localTypeName, out returnValueNamespaceName, out returnValueLocalTypeName, out returnValueAssemblyName, out isTypeString, out isTypeEnum, assemblyNameIfAny);
        }

        public void GetAttachedPropertyGetMethodInfo(string methodName, string namespaceName, string localTypeName, out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
        {
            _monoCecilVersion.GetAttachedPropertyGetMethodInfo(methodName, namespaceName, localTypeName,
                out declaringTypeName, out returnValueNamespaceName, out returnValueLocalTypeName, out isTypeString,
                out isTypeEnum, assemblyNameIfAny);
        }

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            _monoCecilVersion.GetPropertyOrFieldTypeInfo(propertyOrFieldName, namespaceName, localTypeName,
                out propertyNamespaceName, out propertyLocalTypeName, out propertyAssemblyName, out isTypeString,
                out isTypeEnum, assemblyNameIfAny, isAttached: isAttached);
        }

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            _monoCecilVersion.GetPropertyOrFieldInfo(propertyOrFieldName, namespaceName, localTypeName, out memberDeclaringTypeName, out memberTypeNamespace, out memberTypeName, out isTypeString, out isTypeEnum, assemblyNameIfAny, isAttached);
        }

        public string GetFieldName(string fieldNameIgnoreCase, string namespaceName, string localTypeName, string assemblyIfAny = null)
        {
            return _monoCecilVersion.GetFieldName(fieldNameIgnoreCase, namespaceName, localTypeName, assemblyIfAny);
        }

        public Dictionary<string, byte[]> GetManifestResources(string assemblySimpleName, HashSet<string> supportedExtensionsLowerCase)
        {
            return _monoCecilVersion.GetManifestResources(assemblySimpleName, supportedExtensionsLowerCase);
        }

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName)
        {
            return _monoCecilVersion.IsAssignableFrom(namespaceName, typeName, fromNamespaceName, fromTypeName);
        }

        public string GetField(string fieldName, string namespaceName, string typeName, string assemblyName)
        {
            return _monoCecilVersion.GetField(fieldName, namespaceName, typeName, assemblyName);
        }

        public string GetEventHandlerType(string eventName, string namespaceName, string typeName, string assemblyName)
        {
            return _monoCecilVersion.GetEventHandlerType(eventName, namespaceName, typeName, assemblyName);
        }
    }
}
