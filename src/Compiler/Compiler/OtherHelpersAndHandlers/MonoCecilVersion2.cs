using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Cecil;

namespace DotNetForHtml5.Compiler.OtherHelpersAndHandlers
{
    class CustomResolver : BaseAssemblyResolver
    {
        private Dictionary<string, AssemblyDefinition> _loadedAssemblySimpleNameToAssembly;
        private DefaultAssemblyResolver _defaultResolver;

        public CustomResolver(Dictionary<string, AssemblyDefinition> loadedAssemblySimpleNameToAssembly)
        {
            _defaultResolver = new DefaultAssemblyResolver();
            _loadedAssemblySimpleNameToAssembly = loadedAssemblySimpleNameToAssembly;
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            AssemblyDefinition assembly;
            try
            {
                assembly = _defaultResolver.Resolve(name);
            }
            catch (AssemblyResolutionException ex)
            {
                assembly = _loadedAssemblySimpleNameToAssembly[name.Name];
            }
            return assembly;
        }
    }

    public static class TypeDefinitionExtensions
    {
        /*
        public static TypeDefinition From(Type t)
        {
            return AssemblyDefinition.ReadAssembly(t.Assembly.FullName).MainModule.GetType(t.FullName)
                .Resolve();
        }

        public static EventDefinition GetEvent(this TypeDefinition td, string name)
        {
            return td.Events.FirstOrDefault(e => e.FullName == name);
        }

        public static PropertyDefinition GetProperty(this TypeDefinition td, string name)
        {
            return td.Properties.FirstOrDefault(p => p.Name == name);
        }

        public static FieldDefinition GetField(this TypeDefinition td, string name, BindingFlags bf)
        {
            return td.Fields.FirstOrDefault(f => f.Name == name);
        }

        public static FieldDefinition GetField(this TypeDefinition td, string name)
        {
            return GetField(td, name, BindingFlags.Default);
        }*/

        /// <summary>
        /// Is childTypeDef a subclass of parentTypeDef. Does not test interface inheritance
        /// </summary>
        /// <param name="childTypeDef"></param>
        /// <param name="parentTypeDef"></param>
        /// <returns></returns>
        public static bool IsSubclassOf(this TypeDefinition childTypeDef, TypeDefinition parentTypeDef) =>
           childTypeDef.MetadataToken
               != parentTypeDef.MetadataToken
               && childTypeDef
              .EnumerateBaseClasses()
              .Any(b => b.MetadataToken == parentTypeDef.MetadataToken);

        /// <summary>
        /// Does childType inherit from parentInterface
        /// </summary>
        /// <param name="childType"></param>
        /// <param name="parentInterfaceDef"></param>
        /// <returns></returns>
        public static bool DoesAnySubTypeImplementInterface(this TypeDefinition childType, TypeDefinition parentInterfaceDef)
        {
            Debug.Assert(parentInterfaceDef.IsInterface);
            return childType
           .EnumerateBaseClasses()
           .Any(typeDefinition => typeDefinition.DoesSpecificTypeImplementInterface(parentInterfaceDef));
        }

        /// <summary>
        /// Does the childType directly inherit from parentInterface. Base
        /// classes of childType are not tested
        /// </summary>
        /// <param name="childTypeDef"></param>
        /// <param name="parentInterfaceDef"></param>
        /// <returns></returns>
        public static bool DoesSpecificTypeImplementInterface(this TypeDefinition childTypeDef, TypeDefinition parentInterfaceDef)
        {
            Debug.Assert(parentInterfaceDef.IsInterface);
            return childTypeDef
           .Interfaces
           .Any(ifaceDef => DoesSpecificInterfaceImplementInterface(ifaceDef.InterfaceType.Resolve(), parentInterfaceDef));
        }

        /// <summary>
        /// Does interface iface0 equal or implement interface iface1
        /// </summary>
        /// <param name="iface0"></param>
        /// <param name="iface1"></param>
        /// <returns></returns>
        public static bool DoesSpecificInterfaceImplementInterface(TypeDefinition iface0, TypeDefinition iface1)
        {
            Debug.Assert(iface1.IsInterface);
            Debug.Assert(iface0.IsInterface);
            return iface0.MetadataToken == iface1.MetadataToken || iface0.DoesAnySubTypeImplementInterface(iface1);
        }

        /// <summary>
        /// Is source type assignable to target type
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsAssignableFrom(this TypeDefinition target, TypeDefinition source)
       => target == source
          || Equals(target, source)
          || source.IsSubclassOf(target)
          || target.IsInterface && source.DoesAnySubTypeImplementInterface(target);

        /// <summary>
        /// Enumerate the current type, it's parent and all the way to the top type
        /// </summary>
        /// <param name="klassType"></param>
        /// <returns></returns>
        public static IEnumerable<TypeDefinition> EnumerateBaseClasses(this TypeDefinition klassType)
        {
            for (var typeDefinition = klassType; typeDefinition != null; typeDefinition = typeDefinition.BaseType?.Resolve())
            {
                yield return typeDefinition;
            }
        }
    }
    
    public class MonoCecilVersion2
    {
        private Dictionary<string, TypeDefinition> _typeNameToType = new Dictionary<string, TypeDefinition>();
        private Dictionary<string, AssemblyDefinition> _loadedAssemblySimpleNameToAssembly = new Dictionary<string, AssemblyDefinition>();
        private Dictionary<string, Dictionary<string, HashSet<string>>> _assemblyNameToXmlNamespaceToClrNamespaces = new Dictionary<string, Dictionary<string, HashSet<string>>>();

        private TypeDefinition FindType(string namespaceName, string localTypeName, string filterAssembliesAndRetainOnlyThoseThatHaveThisName = null, bool doNotRaiseExceptionIfNotFound = false)
        {
            // Fix the namespace:
            if (namespaceName.StartsWith("using:", StringComparison.CurrentCultureIgnoreCase))
            {
                namespaceName = namespaceName.Substring("using:".Length);
            }
            else if (namespaceName.StartsWith("clr-namespace:", StringComparison.CurrentCultureIgnoreCase))
            {
                GettingInformationAboutXamlTypes.ParseClrNamespaceDeclaration(namespaceName, out var ns, out var assemblyNameIfAny);
                namespaceName = ns;
                GettingInformationAboutXamlTypes.FixNamespaceForCompatibility(ref assemblyNameIfAny, ref namespaceName);
            }

            if (namespaceName.StartsWith("global::", StringComparison.CurrentCultureIgnoreCase)) // Note: normally in XAML there is no "global::", but we may enter this method passing a C#-style namespace (cf. section that handles Binding in "GeneratingCSharpCode.cs")
            {
                namespaceName = namespaceName.Substring("global::".Length);
            }

            // Handle special cases:
            if (localTypeName == "StaticResource")
            {
                localTypeName = "StaticResourceExtension";
            }

            // Generate string representing the type:
            string fullTypeNameWithNamespaceInsideBraces = !string.IsNullOrEmpty(namespaceName) ? "{" + namespaceName + "}" + localTypeName : localTypeName;

            // Start by looking in the cache dictionary:
            if (_typeNameToType.TryGetValue(fullTypeNameWithNamespaceInsideBraces, out var type))
            {
                return type;
            }

            // Look for the type in all loaded assemblies:
            foreach (var assemblyKeyValuePair in _loadedAssemblySimpleNameToAssembly)
            {
                string assemblySimpleName = assemblyKeyValuePair.Key;
                if (assemblySimpleName.Contains("OpenSilver"))
                {
                    Console.WriteLine("test");
                }
                AssemblyDefinition assembly = assemblyKeyValuePair.Value;
                if (filterAssembliesAndRetainOnlyThoseThatHaveThisName == null
                    || assemblySimpleName == filterAssembliesAndRetainOnlyThoseThatHaveThisName)
                {
                    List<string> namespacesToLookInto = new List<string>();

                    // If the namespace is a XML namespace (eg. "{http://schemas.microsoft.com/winfx/2006/xaml/presentation}"), we should iterate through all the corresponding CLR namespaces:
                    if (isNamespaceAnXmlNamespace(namespaceName))
                    {
                        namespacesToLookInto.AddRange(GetClrNamespacesFromXmlNamespace(assemblySimpleName, namespaceName));
                    }
                    else
                    {
                        namespacesToLookInto.Add(namespaceName);
                    }

                    // Search for the type:
                    foreach (var namespaceToLookInto in namespacesToLookInto)
                    {
                        string fullTypeNameToFind = namespaceToLookInto + "." + localTypeName;
                        var typeIfFound = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == fullTypeNameToFind);
                        if (typeIfFound == null)
                        {
                            //try to find a matching nested type.
                            fullTypeNameToFind = namespaceToLookInto + "+" + localTypeName;
                            typeIfFound = assembly.MainModule.Types.FirstOrDefault(x => x.FullName == fullTypeNameToFind);
                        }

                        if (typeIfFound != null)
                        {
                            _typeNameToType[fullTypeNameWithNamespaceInsideBraces] = typeIfFound;
                            return typeIfFound;
                        }
                    }
                }
            }

            if (doNotRaiseExceptionIfNotFound)
            {
                return null;
            }
            else
            {
                throw new Exception("Type not found: " + fullTypeNameWithNamespaceInsideBraces);
            }
        }

        bool isNamespaceAnXmlNamespace(string namespaceName)
        {
            return namespaceName.StartsWith("http://"); //todo: are there other conditions possible for XML namespaces declared with xmlnsDefinitionAttribute?
        }

        IEnumerable<string> GetClrNamespacesFromXmlNamespace(string assemblySimpleName, string xmlNamespace)
        {
            // Note: This method returns an empty enumeration if no result was found.
            if (_assemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
            {
                var xmlNamespaceToClrNamespaces = _assemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];
                if (xmlNamespaceToClrNamespaces.ContainsKey(xmlNamespace))
                {
                    return xmlNamespaceToClrNamespaces[xmlNamespace];
                }
            }
            return Enumerable.Empty<string>();
        }

        public string LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo, bool isBridgeBasedVersion,
            bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode, bool skipReadingAttributesFromAssemblies)
        {
            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters
            {
                AssemblyResolver = new CustomResolver(_loadedAssemblySimpleNameToAssembly)
            });
            _loadedAssemblySimpleNameToAssembly[assembly.Name.Name] = assembly;
            ReadXmlnsDefinitionAttributes(assembly, true);
            return "";
        }

        public void Unload()
        {
            foreach(var kvp in _loadedAssemblySimpleNameToAssembly)
            {
                kvp.Value.Dispose();
            }

            _loadedAssemblySimpleNameToAssembly.Clear();
        }

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            // Distinguish between system types (String, Double...) and other types
            if (SystemTypesHelper.IsSupportedSystemType($"{namespaceName}.{localTypeName}", assemblyNameIfAny))
            {
                return SystemTypesHelper.GetFullTypeName(namespaceName, localTypeName, assemblyNameIfAny);
            }
            else
            {
                // Find the type:
                var type = FindType(
                    namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing
                );

                if (type == null)
                {
                    if (ifTypeNotFoundTryGuessing)
                    {
                        // Try guessing

                        if (isNamespaceAnXmlNamespace(namespaceName))
                        {
                            // Attempt to find the type in the current namespace
                            return localTypeName;
                        }
                        else
                        {
                            return string.Format(
                                "global::{0}{1}{2}",
                                namespaceName,
                                string.IsNullOrEmpty(namespaceName) ? string.Empty : ".",
                                localTypeName
                            );
                        }
                    }
                    else
                    {
                        throw new XamlParseException(
                            $"Type '{localTypeName}' not found in namespace '{namespaceName}'."
                        );
                    }
                }
                else
                {
                    // Use information from the type
                    return $"global::{type}";
                }
            }
        }

        void ReadXmlnsDefinitionAttributes(AssemblyDefinition assembly, bool isBridgeBasedVersion)
        {
            string assemblySimpleName = assembly.Name.Name;

            if (assemblySimpleName.Contains("OpenSilver"))
            {
                Console.WriteLine("test");
            }

            try
            {
                // Extract the "XmlnsDefinition" attributes defined in the "AssemblyInfo.cs" files, for use with XAML namespace mappings:
                Dictionary<string, HashSet<string>> xmlNamespaceToClrNamespaces = null;
                if (_assemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
                    xmlNamespaceToClrNamespaces = _assemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];

#if BRIDGE || CSHTML5BLAZOR
                var xmlnsDefinitionAttributeType = this.FindType("System.Windows.Markup", "XmlnsDefinitionAttribute");
#else
            var xmlnsDefinitionAttributeType = typeof(XmlnsDefinitionAttribute);
#endif

#if CSHTML5BLAZOR
                var attributesData = new List<CustomAttribute>();
#endif
                var attributes = new List<CustomAttribute>();

#if CSHTML5BLAZOR
                /*
                // if assembly is loaded with reflection only we have to use GetCustomAttributesData instead of GetCustomAttributes
                if (_onlyReflectionLoaded.ContainsKey(assembly) && _onlyReflectionLoaded[assembly])
                    attributesData = assembly.GetCustomAttributesData();
                else
                    attributes = assembly.GetCustomAttributes(xmlnsDefinitionAttributeType);
                */

                // Instead of the commented code above, we now try both "GetCustomAttributes" and "GetCustomAttributesData"
                // to fix the compilation issue experienced with Client_REP (with the delivery dated Dec 22, 2020)
                try
                {
                    attributes = assembly.CustomAttributes.Where(x =>
                        x.AttributeType.FullName == "System.Windows.Markup.XmlnsDefinitionAttribute").ToList();
                }
                catch
                {
                    /*
                    try
                    {
                        attributesData = assembly.GetCustomAttributesData();
                    }
                    catch
                    {
                        // Fails silently
                    }
                    */
                }
#else
                attributes = assembly.GetCustomAttributes(xmlnsDefinitionAttributeType);
#endif
                foreach (var attribute in attributes)
                {
                    var xmlNamespace = (attribute.ConstructorArguments[0].Value ?? "").ToString();
                    var clrNamespace = (attribute.ConstructorArguments[1].Value ?? "").ToString();

                    //string xmlNamespace = (xmlnsDefinitionAttributeType.GetProperty("XmlNamespace").GetValue(attribute) ?? "").ToString();
                    //string clrNamespace = (xmlnsDefinitionAttributeType.GetProperty("ClrNamespace").GetValue(attribute) ?? "").ToString();
                    if (!string.IsNullOrEmpty(xmlNamespace) && !string.IsNullOrEmpty(clrNamespace))
                    {
                        if (xmlNamespaceToClrNamespaces == null)
                        {
                            xmlNamespaceToClrNamespaces = new Dictionary<string, HashSet<string>>();
                            _assemblyNameToXmlNamespaceToClrNamespaces.Add(assemblySimpleName,
                                xmlNamespaceToClrNamespaces);
                        }

                        HashSet<string> clrNamespacesAssociatedToThisXmlNamespace;
                        if (xmlNamespaceToClrNamespaces.ContainsKey(xmlNamespace))
                            clrNamespacesAssociatedToThisXmlNamespace = xmlNamespaceToClrNamespaces[xmlNamespace];
                        else
                        {
                            clrNamespacesAssociatedToThisXmlNamespace = new HashSet<string>();
                            xmlNamespaceToClrNamespaces.Add(xmlNamespace, clrNamespacesAssociatedToThisXmlNamespace);
                        }

                        if (!clrNamespacesAssociatedToThisXmlNamespace.Contains(clrNamespace))
                            clrNamespacesAssociatedToThisXmlNamespace.Add(clrNamespace);
                    }
                }

#if CSHTML5BLAZOR
                // we have to go through attributesData from only reflection loaded assemblies
                foreach (var attributeData in attributesData)
                {
                    if (attributeData.AttributeType ==
                        xmlnsDefinitionAttributeType) //note: should we use IsAssignableFrom instead? (I'd say no because I wouldn't see the point of inheriting from thit type.)
                    {
                        string xmlNamespace = attributeData.ConstructorArguments[0].ToString();
                        string clrNamespace = attributeData.ConstructorArguments[1].ToString();
                        if (!string.IsNullOrEmpty(xmlNamespace) && !string.IsNullOrEmpty(clrNamespace))
                        {
                            if (xmlNamespaceToClrNamespaces == null)
                            {
                                xmlNamespaceToClrNamespaces = new Dictionary<string, HashSet<string>>();
                                _assemblyNameToXmlNamespaceToClrNamespaces.Add(assemblySimpleName,
                                    xmlNamespaceToClrNamespaces);
                            }

                            HashSet<string> clrNamespacesAssociatedToThisXmlNamespace;
                            if (xmlNamespaceToClrNamespaces.ContainsKey(xmlNamespace))
                                clrNamespacesAssociatedToThisXmlNamespace = xmlNamespaceToClrNamespaces[xmlNamespace];
                            else
                            {
                                clrNamespacesAssociatedToThisXmlNamespace = new HashSet<string>();
                                xmlNamespaceToClrNamespaces.Add(xmlNamespace,
                                    clrNamespacesAssociatedToThisXmlNamespace);
                            }

                            if (!clrNamespacesAssociatedToThisXmlNamespace.Contains(clrNamespace))
                                clrNamespacesAssociatedToThisXmlNamespace.Add(clrNamespace);
                        }
                    }
                }
#endif
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName, string fromTypeName)
        {
            try
            {
                TypeDefinition type = this.FindType(namespaceName, typeName);
                TypeDefinition fromType = this.FindType(fromNamespaceName, fromTypeName);

                return type.IsAssignableFrom(fromType);
            }
            catch (Exception ex)
            {
                Debugger.Launch();
                throw;
            }
        }

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            var memberInfo = GetMemberInfo(memberName, namespaceName, localTypeName, assemblyNameIfAny);
            if (memberInfo is PropertyDefinition)
            {
                return MemberTypes.Property;
            }
            if (memberInfo is MethodDefinition)
            {
                return MemberTypes.Method;
            }
            if (memberInfo is FieldDefinition)
            {
                return MemberTypes.Field;
            }
            if (memberInfo is EventDefinition)
            {
                return MemberTypes.Event;
            }
            return MemberTypes.Custom;
        }

        IMemberDefinition GetMemberInfo(string memberName, string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool returnNullIfNotFoundInsteadOfException = false)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            while (elementType != null)
            {
                var prop = elementType.Properties.FirstOrDefault(x => x.Name == memberName);
                if (prop != null)
                {
                    return prop;
                }

                var method = elementType.Methods.FirstOrDefault(x => x.Name == memberName);
                if (method != null)
                {
                    return method;
                }

                var ev = elementType.Events.FirstOrDefault(x => x.Name == memberName);
                if (ev != null)
                {
                    return ev;
                }

                var field = elementType.Fields.FirstOrDefault(x => x.Name == memberName);
                if (field != null)
                {
                    return field;
                }

                elementType = elementType.BaseType?.Resolve();
            }

            if (returnNullIfNotFoundInsteadOfException)
                return null;

            throw new XamlParseException("Member \"" + memberName + "\" not found in type \"" + elementType.ToString() + "\". NamespaceName - " + namespaceName + ". LocalTypeName - " + localTypeName);
        }

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName, out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            var type = GetPropertyOrFieldType(propertyOrFieldName, namespaceName, localTypeName,
                assemblyNameIfAny, isAttached: isAttached);
            propertyNamespaceName = BuildPropertyPathRecursively(type);
            propertyLocalTypeName = GetTypeNameIncludingGenericArguments(type, false);
            propertyAssemblyName = type.Resolve().Module.Assembly.Name.Name;
            isTypeString = type.FullName == "System.String";
            isTypeEnum = (type.Resolve().IsEnum);
        }

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out string returnValueAssemblyName, out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null)
        {
            var type = GetMethodReturnValueType(methodName, namespaceName, localTypeName, assemblyNameIfAny);
            returnValueNamespaceName = this.BuildPropertyPathRecursively(type);
            returnValueLocalTypeName = GetTypeNameIncludingGenericArguments(type, false);
            returnValueAssemblyName = type.Module.Assembly.Name.Name;
            isTypeString = type.FullName == "System.String";
            isTypeEnum = (type.IsEnum);
        }
        
        private PropertyDefinition FindPropertyDeep(TypeDefinition elementType, string propertyName)
        {
            while (elementType != null)
            {
                var propertyDefinition = elementType.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (propertyDefinition != null)
                {
                    return propertyDefinition;
                }

                elementType = elementType.BaseType?.Resolve();
            }

            return null;
        }

        private FieldDefinition FindFieldDeep(TypeDefinition elementType, string propertyName, bool ignoreCase = false, bool staticOnly = false, bool publicOnly = false)
        {
            while (elementType != null)
            {
                var fieldDefinition = elementType.Fields.FirstOrDefault(p =>
                    string.Compare(p.Name, propertyName, ignoreCase) == 0 && (!staticOnly || p.IsStatic) && (!publicOnly || p.IsPublic));
                if (fieldDefinition != null)
                {
                    return fieldDefinition;
                }

                elementType = elementType.BaseType?.Resolve();
            }

            return null;
        }

        TypeReference GetPropertyOrFieldType(string propertyName, string namespaceName, string localTypeName, string assemblyNameIfAny = null, bool isAttached = false)
        {
            if (isAttached)
            {
                return GetMethodReturnValueType("Get" + propertyName, namespaceName, localTypeName, assemblyNameIfAny);
            }
            else
            {
                var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
                var propertyInfo = FindPropertyDeep(elementType, propertyName);
                /*
                try
                {
                    propertyInfo = elementType.Properties.FirstOrDefault(p => p.Name == propertyName);
                }
                catch (AmbiguousMatchException)
                {
                    propertyInfo = GetPropertyLastImplementationIfMultipleMatches(propertyName, elementType);
                }
                */
                if (propertyInfo == null)
                {
                    var fieldInfo = FindFieldDeep(elementType, propertyName);
                    //var fieldInfo = elementType.Fields.FirstOrDefault(f => f.Name == propertyName);
                    if (fieldInfo == null)
                    {
                        throw new XamlParseException("Property or field \"" + propertyName + "\" not found in type \"" + elementType.ToString() + "\".");
                    }
                    else
                    {
                        var fieldType = fieldInfo.FieldType;
                        return fieldType;
                        //return fieldType.Resolve();
                    }
                }
                else
                {
                    var propertyType = propertyInfo.PropertyType;
                    return propertyType;
                    //return propertyType.Resolve();
                }
            }
        }

        private string BuildPropertyPathRecursively(TypeReference type)
        {
            var fullPath = string.Empty;
            var parentType = type;
            while ((parentType = parentType.DeclaringType) != null)
            {
                if (!string.IsNullOrEmpty(fullPath))
                {
                    fullPath = "." + fullPath;
                }
                fullPath = parentType.Name + fullPath;
            }
            fullPath = type.Namespace + (!string.IsNullOrEmpty(type.Namespace) && !string.IsNullOrEmpty(fullPath) ? "." : string.Empty) + fullPath;
            return fullPath;
        }

        private static string GetTypeNameIncludingGenericArguments(TypeReference type, bool appendNamespace)
        {
            var result = new StringBuilder();
            if (appendNamespace)
            {
                result.Append("global::");
                if (!string.IsNullOrEmpty(type.Namespace))
                {
                    result.Append(type.Namespace + ".");
                }
            }

            result.Append(type.Name);

            if (type is GenericInstanceType genericInstanceType)
            {
                result = new StringBuilder(result.ToString().Split('`')[0]);
                result.Append($"<{string.Join(", ", genericInstanceType.GenericArguments.Select(x => GetTypeNameIncludingGenericArguments(x, true)))}>");
            }

            return result.ToString();
        }

        TypeDefinition GetMethodReturnValueType(string methodName, string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            var currentType = elementType;
            MethodDefinition methodInfo = null;

            while (methodInfo == null && currentType != null)
            {
                methodInfo = currentType.Methods.FirstOrDefault(m => m.Name == methodName);
                currentType = currentType.BaseType.Resolve();
            }

            if (methodInfo == null)
                throw new XamlParseException("Method \"" + methodName + "\" not found in type \"" + elementType.ToString() + "\".");
            var methodType = methodInfo.ReturnType.Resolve();
            return methodType;
        }

        PropertyDefinition GetPropertyLastImplementationIfMultipleMatches(string propertyName, TypeDefinition type)
        {
            var currentType = type;
            while (currentType != null)
            {
                foreach (var property in currentType.Properties)
                {
                    if (property.Name == propertyName)
                    {
                        return property;
                    }
                }
                currentType = currentType.BaseType.Resolve();
            }
            return null;
        }

        public string GetEventHandlerType(string eventName, string namespaceName, string typeName, string assemblyName)
        {
            var type = FindType(namespaceName, typeName, assemblyName);

            while (type != null)
            {
                var eventInfo = type.Events.FirstOrDefault(n => n.Name == eventName);
                if (eventInfo != null)
                {
                    return GetTypeNameIncludingGenericArguments(eventInfo.EventType, true);
                }
                type = type.BaseType.Resolve();
            }

            throw new XamlParseException($"'{type}' does not contain an event named '{eventName}'.");
        }

        public string GetFieldName(string fieldNameIgnoreCase, string namespaceName, string localTypeName, string assemblyIfAny = null)
        {
            var type = FindType(namespaceName, localTypeName, assemblyIfAny);

            if (type == null) throw new XamlParseException($"Type '{localTypeName}' not found in namepsace '{namespaceName}'.");

            FieldDefinition field;
            if (type.IsEnum)
            {
                field = FindFieldDeep(type, fieldNameIgnoreCase, true, true, true);
                if (field == null)
                {
                    // If the field isn't found "as is", we try to interpret it as the int corresponding to a field
                    if (int.TryParse(fieldNameIgnoreCase, out int value))
                    {
                        var fd = type.Fields.SingleOrDefault(f => (int)f.Constant == value);
                        var trueFieldName = fd.Name;
                        field = FindFieldDeep(type, trueFieldName, true, true, true);
                    }
                }
            }
            else
            {
                field = FindFieldDeep(type, fieldNameIgnoreCase, true, true, true);
            }

            return field?.Name ?? throw new XamlParseException($"Field '{fieldNameIgnoreCase}' not found in type: '{type.FullName}'.");
        }

        public bool IsElementAMarkupExtension(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);

            var markupExtensionGeneric = this.FindType("System.Xaml", "IMarkupExtension`1");

            var isAssignableFrom = markupExtensionGeneric.IsAssignableFrom(elementType);
            var typeIsAMarkupExtension = isAssignableFrom && elementType.FullName != "System.String";
            return typeIsAMarkupExtension;
        }

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom, string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo, string assemblyNameOfTypeToAssignTo, bool isAttached = false)
        {
            TypeDefinition typeOfElementToAssignFrom;
            TypeDefinition typeOfElementToAssignTo;

            var indexOfLastDot = nameOfTypeToAssignFrom.LastIndexOf('.');

            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignFrom = FindType(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom, assemblyNameOfTypeToAssignFrom);
            }
            else
            {
                string localTypeName = nameOfTypeToAssignFrom.Substring(0, indexOfLastDot);
                string propertyName = nameOfTypeToAssignFrom.Substring(indexOfLastDot + 1);
                typeOfElementToAssignFrom = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignFrom, localTypeName, assemblyNameOfTypeToAssignFrom).Resolve();
            }

            indexOfLastDot = nameOfTypeToAssignTo.LastIndexOf('.');
            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignTo = FindType(nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo, assemblyNameOfTypeToAssignTo);
            }
            else
            {
                string localTypeName = nameOfTypeToAssignTo.Substring(0, indexOfLastDot);
                string propertyName = nameOfTypeToAssignTo.Substring(indexOfLastDot + 1);
                typeOfElementToAssignTo = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignTo, localTypeName, assemblyNameOfTypeToAssignTo, isAttached).Resolve();
            }

            return typeOfElementToAssignTo.IsAssignableFrom(typeOfElementToAssignFrom);
        }
    }
}
