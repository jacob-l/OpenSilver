using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Mono.Cecil;

namespace DotNetForHtml5.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector
{
    internal class MonoCecilAssembliesInspectorImpl : IDisposable
    {
        private const string IListFullName = "System.Collections.IList";
        private const string IDictionaryFullName = "System.Collections.IDictionary";
        private const string XmlnsDefinitionAttributeFullName = "System.Windows.Markup.XmlnsDefinitionAttribute";
        private const string GlobalPrefix = "global::";
        private const string SystemXamlNamespace = "System.Xaml";
        private const string GenericMarkupExtension = "IMarkupExtension`1";
        private const string ContentPropertyAttributeFullName = "System.Windows.Markup.ContentPropertyAttribute";
        private const string DependencyProperty = "DependencyProperty";
        private const string SetPrefix = "Set";
        private const string GetPrefix = "Get";
        private const string Name = "Name";
        private const string PropertySuffix = "Property";

        private readonly Dictionary<string, Dictionary<string, HashSet<string>>>
            _assemblyNameToXmlNamespaceToClrNamespaces = new Dictionary<string, Dictionary<string, HashSet<string>>>();

        private readonly Dictionary<string, AssemblyDefinition> _loadedAssemblySimpleNameToAssembly =
            new Dictionary<string, AssemblyDefinition>();

        private readonly Dictionary<string, TypeDefinition> _typeNameToType = new Dictionary<string, TypeDefinition>();

        public void Dispose()
        {
            foreach (var kvp in _loadedAssemblySimpleNameToAssembly) kvp.Value.Dispose();

            _loadedAssemblySimpleNameToAssembly.Clear();
        }

        private TypeDefinition FindType(string namespaceName, string localTypeName,
            string filterAssembliesAndRetainOnlyThoseThatHaveThisName = null,
            bool doNotRaiseExceptionIfNotFound = false)
        {
            // Fix the namespace:
            if (namespaceName.StartsWith("using:", StringComparison.CurrentCultureIgnoreCase))
            {
                namespaceName = namespaceName.Substring("using:".Length);
            }
            else if (namespaceName.StartsWith("clr-namespace:", StringComparison.CurrentCultureIgnoreCase))
            {
                GettingInformationAboutXamlTypes.ParseClrNamespaceDeclaration(namespaceName, out var ns,
                    out var assemblyNameIfAny);
                namespaceName = ns;
                GettingInformationAboutXamlTypes.FixNamespaceForCompatibility(ref assemblyNameIfAny, ref namespaceName);
            }

            if (namespaceName.StartsWith(GlobalPrefix,
                    StringComparison
                        .CurrentCultureIgnoreCase)) // Note: normally in XAML there is no "global::", but we may enter this method passing a C#-style namespace (cf. section that handles Binding in "GeneratingCSharpCode.cs")
                namespaceName = namespaceName.Substring(GlobalPrefix.Length);

            // Handle special cases:
            if (localTypeName == "StaticResource") localTypeName = "StaticResourceExtension";

            // Generate string representing the type:
            var fullTypeNameWithNamespaceInsideBraces = !string.IsNullOrEmpty(namespaceName)
                ? "{" + namespaceName + "}" + localTypeName
                : localTypeName;

            // Start by looking in the cache dictionary:
            if (_typeNameToType.TryGetValue(fullTypeNameWithNamespaceInsideBraces, out var type)) return type;

            // Look for the type in all loaded assemblies:
            foreach (var assemblyKeyValuePair in _loadedAssemblySimpleNameToAssembly)
            {
                var assemblySimpleName = assemblyKeyValuePair.Key;
                var assembly = assemblyKeyValuePair.Value;
                if (filterAssembliesAndRetainOnlyThoseThatHaveThisName == null
                    || assemblySimpleName == filterAssembliesAndRetainOnlyThoseThatHaveThisName)
                {
                    var namespacesToLookInto = new List<string>();

                    // If the namespace is a XML namespace (eg. "{http://schemas.microsoft.com/winfx/2006/xaml/presentation}"), we should iterate through all the corresponding CLR namespaces:
                    if (IsNamespaceAnXmlNamespace(namespaceName))
                        namespacesToLookInto.AddRange(
                            GetClrNamespacesFromXmlNamespace(assemblySimpleName, namespaceName));
                    else
                        namespacesToLookInto.Add(namespaceName);

                    // Search for the type:
                    foreach (var namespaceToLookInto in namespacesToLookInto)
                    {
                        var fullTypeNameToFind = namespaceToLookInto + "." + localTypeName;
                        var typeIfFound =
                            assembly.MainModule.Types.FirstOrDefault(x => x.FullName == fullTypeNameToFind);
                        if (typeIfFound == null)
                        {
                            //try to find a matching nested type.
                            fullTypeNameToFind = namespaceToLookInto + "+" + localTypeName;
                            typeIfFound =
                                assembly.MainModule.Types.FirstOrDefault(x => x.FullName == fullTypeNameToFind);
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
                return null;
            throw new Exception("Type not found: " + fullTypeNameWithNamespaceInsideBraces);
        }

        private static bool IsNamespaceAnXmlNamespace(string namespaceName)
        {
            return namespaceName.StartsWith("http://"); //todo: are there other conditions possible for XML namespaces declared with xmlnsDefinitionAttribute?
        }

        private IEnumerable<string> GetClrNamespacesFromXmlNamespace(string assemblySimpleName, string xmlNamespace)
        {
            // Note: This method returns an empty enumeration if no result was found.
            if (!_assemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
                return Enumerable.Empty<string>();

            var xmlNamespaceToClrNamespaces = _assemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];
            return xmlNamespaceToClrNamespaces.ContainsKey(xmlNamespace)
                ? xmlNamespaceToClrNamespaces[xmlNamespace]
                : Enumerable.Empty<string>();
        }

        private void ReadXmlnsDefinitionAttributes(AssemblyDefinition assembly, bool isBridgeBasedVersion)
        {
            var assemblySimpleName = assembly.Name.Name;

            // Extract the "XmlnsDefinition" attributes defined in the "AssemblyInfo.cs" files, for use with XAML namespace mappings:
            Dictionary<string, HashSet<string>> xmlNamespaceToClrNamespaces = null;
            if (_assemblyNameToXmlNamespaceToClrNamespaces.ContainsKey(assemblySimpleName))
                xmlNamespaceToClrNamespaces = _assemblyNameToXmlNamespaceToClrNamespaces[assemblySimpleName];

            var attributes = assembly.CustomAttributes.Where(x =>
                x.AttributeType.FullName == XmlnsDefinitionAttributeFullName).ToList();

            foreach (var attribute in attributes)
            {
                var xmlNamespace = (attribute.ConstructorArguments[0].Value ?? "").ToString();
                var clrNamespace = (attribute.ConstructorArguments[1].Value ?? "").ToString();

                if (string.IsNullOrEmpty(xmlNamespace) || string.IsNullOrEmpty(clrNamespace)) continue;

                if (xmlNamespaceToClrNamespaces == null)
                {
                    xmlNamespaceToClrNamespaces = new Dictionary<string, HashSet<string>>();
                    _assemblyNameToXmlNamespaceToClrNamespaces.Add(assemblySimpleName,
                        xmlNamespaceToClrNamespaces);
                }

                HashSet<string> clrNamespacesAssociatedToThisXmlNamespace;
                if (xmlNamespaceToClrNamespaces.ContainsKey(xmlNamespace))
                {
                    clrNamespacesAssociatedToThisXmlNamespace = xmlNamespaceToClrNamespaces[xmlNamespace];
                }
                else
                {
                    clrNamespacesAssociatedToThisXmlNamespace = new HashSet<string>();
                    xmlNamespaceToClrNamespaces.Add(xmlNamespace, clrNamespacesAssociatedToThisXmlNamespace);
                }

                if (!clrNamespacesAssociatedToThisXmlNamespace.Contains(clrNamespace))
                    clrNamespacesAssociatedToThisXmlNamespace.Add(clrNamespace);
            }
        }

        private IMemberDefinition GetMemberInfo(string memberName, string namespaceName, string localTypeName,
            string assemblyNameIfAny = null, bool returnNullIfNotFoundInsteadOfException = false)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            var typeIterator = elementType;
            while (typeIterator != null)
            {
                var prop = typeIterator.Properties.FirstOrDefault(x => x.Name == memberName);
                if (prop != null) return prop;

                var method = FindMethod(typeIterator, memberName);
                if (method != null) return method;

                var ev = typeIterator.Events.FirstOrDefault(x => x.Name == memberName);
                if (ev != null) return ev;

                var field = typeIterator.Fields.FirstOrDefault(x => x.Name == memberName);
                if (field != null) return field;

                typeIterator = typeIterator.BaseType?.Resolve();
            }

            if (returnNullIfNotFoundInsteadOfException)
                return null;

            throw new XamlParseException("Member \"" + memberName + "\" not found in type \"" + elementType);
        }

        private static PropertyDefinition FindPropertyDeep(TypeDefinition elementType, string propertyName)
        {
            while (elementType != null)
            {
                var propertyDefinition = elementType.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (propertyDefinition != null) return propertyDefinition;

                elementType = elementType.BaseType?.Resolve();
            }

            return null;
        }

        private static FieldDefinition FindFieldDeep(TypeDefinition elementType, string propertyName,
            bool ignoreCase = false,
            bool staticOnly = false, bool publicOnly = false)
        {
            while (elementType != null)
            {
                var fieldDefinition = elementType.Fields.FirstOrDefault(p =>
                    string.Compare(p.Name, propertyName, ignoreCase) == 0 && (!staticOnly || p.IsStatic) &&
                    (!publicOnly || p.IsPublic));
                if (fieldDefinition != null) return fieldDefinition;

                elementType = elementType.BaseType?.Resolve();
            }

            return null;
        }

        private TypeReference GetPropertyOrFieldType(string propertyName, string namespaceName, string localTypeName,
            string assemblyNameIfAny = null, bool isAttached = false)
        {
            if (isAttached)
                return GetMethodReturnValueType(GetPrefix + propertyName, namespaceName, localTypeName,
                    assemblyNameIfAny);

            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            var propertyInfo = FindPropertyDeep(elementType, propertyName);

            if (propertyInfo == null)
            {
                var fieldInfo = FindFieldDeep(elementType, propertyName);
                if (fieldInfo == null)
                    throw new XamlParseException("Property or field \"" + propertyName + "\" not found in type \"" +
                                                 elementType + "\".");

                var fieldType = fieldInfo.FieldType;
                return fieldType;
            }

            var propertyType = propertyInfo.PropertyType;
            return propertyType;
        }

        private static string BuildPropertyPathRecursively(TypeReference type)
        {
            var fullPath = string.Empty;
            var parentType = type;
            while ((parentType = parentType.DeclaringType) != null)
            {
                if (!string.IsNullOrEmpty(fullPath)) fullPath = "." + fullPath;

                fullPath = parentType.Name + fullPath;
            }

            fullPath = type.Namespace +
                       (!string.IsNullOrEmpty(type.Namespace) && !string.IsNullOrEmpty(fullPath) ? "." : string.Empty) +
                       fullPath;
            return fullPath;
        }

        private static string GetTypeNameIncludingGenericArguments(TypeReference type, bool appendNamespace)
        {
            var result = new StringBuilder();
            if (appendNamespace)
            {
                result.Append(GlobalPrefix);
                if (!string.IsNullOrEmpty(type.Namespace)) result.Append(type.Namespace + ".");
            }

            result.Append(type.Name);

            if (type is GenericInstanceType genericInstanceType)
            {
                result = new StringBuilder(result.ToString().Split('`')[0]);
                result.Append(
                    $"<{string.Join(", ", genericInstanceType.GenericArguments.Select(x => GetTypeNameIncludingGenericArguments(x, true)))}>");
            }

            return result.ToString();
        }

        private TypeDefinition GetMethodReturnValueType(string methodName, string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            var currentType = elementType;
            MethodDefinition methodInfo = null;

            while (methodInfo == null && currentType != null)
            {
                methodInfo = FindMethod(currentType, methodName);
                currentType = currentType.BaseType.Resolve();
            }

            if (methodInfo == null)
                throw new XamlParseException("Method \"" + methodName + "\" not found in type \"" +
                                             elementType + "\".");
            return methodInfo.ReturnType.Resolve();
        }

        private static IEnumerable<TypeDefinition> GetAllInterfaces(TypeDefinition elementType)
        {
            var res = new HashSet<TypeDefinition>();

            var queue = new Queue<TypeDefinition>();
            queue.Enqueue(elementType);
            while (queue.Any())
            {
                var curElementType = queue.Dequeue();
                if (curElementType.IsInterface) res.Add(curElementType);

                var interfaces =
                    curElementType.Interfaces.Select(i => i.InterfaceType.Resolve()).ToList();
                foreach (var i in interfaces.Where(i => !res.Contains(i)))
                {
                    res.Add(i);
                    queue.Enqueue(i);
                }

                if (curElementType.BaseType != null) queue.Enqueue(curElementType.BaseType.Resolve());
            }

            return res.ToList();
        }

        private static bool IsCollection(TypeDefinition elementType)
        {
            return GetAllInterfaces(elementType).Any(ii => ii.FullName == IListFullName);
        }

        private static bool IsDictionary(TypeDefinition elementType)
        {
            return GetAllInterfaces(elementType).Any(ii => ii.FullName == IDictionaryFullName);
        }

        private bool IsElementACollection(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
            return IsCollection(elementType);
        }

        private bool IsDictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
            return IsDictionary(elementType);
        }

        private static CustomAttribute GetCustomAttributeDeep(TypeDefinition type, string fullName)
        {
            while (type != null)
            {
                var customAttr = type.CustomAttributes.FirstOrDefault(ca =>
                    ca.AttributeType.FullName == fullName);

                if (customAttr != null) return customAttr;

                type = type.BaseType?.Resolve();
            }

            return null;
        }

        public string LoadAssembly(string assemblyPath, bool loadReferencedAssembliesToo, bool isBridgeBasedVersion,
            bool isCoreAssembly, string nameOfAssembliesThatDoNotContainUserCode,
            bool skipReadingAttributesFromAssemblies)
        {
            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters
            {
                AssemblyResolver =
                    new CustomAssemblyDefinitionResolver(assemblyName =>
                        _loadedAssemblySimpleNameToAssembly[assemblyName])
            });
            _loadedAssemblySimpleNameToAssembly[assembly.Name.Name] = assembly;
            ReadXmlnsDefinitionAttributes(assembly, true);
            return assembly.Name.Name;
        }

        public string GetCSharpEquivalentOfXamlTypeAsString(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null, bool ifTypeNotFoundTryGuessing = false)
        {
            // Distinguish between system types (String, Double...) and other types
            if (SystemTypesHelper.IsSupportedSystemType($"{namespaceName}.{localTypeName}", assemblyNameIfAny))
                return SystemTypesHelper.GetFullTypeName(namespaceName, localTypeName, assemblyNameIfAny);

            // Find the type:
            var type = FindType(
                namespaceName, localTypeName, assemblyNameIfAny, ifTypeNotFoundTryGuessing
            );

            if (type != null)
                // Use information from the type
                return $"global::{type}";

            if (ifTypeNotFoundTryGuessing)
            {
                // Try guessing
                if (IsNamespaceAnXmlNamespace(namespaceName))
                    // Attempt to find the type in the current namespace
                    return localTypeName;

                return
                    $"global::{namespaceName}{(string.IsNullOrEmpty(namespaceName) ? string.Empty : ".")}{localTypeName}";
            }

            throw new XamlParseException(
                $"Type '{localTypeName}' not found in namespace '{namespaceName}'."
            );
        }

        public bool IsAssignableFrom(string namespaceName, string typeName, string fromNamespaceName,
            string fromTypeName)
        {
            var type = FindType(namespaceName, typeName);
            var fromType = FindType(fromNamespaceName, fromTypeName);

            return type.IsAssignableFrom(fromType);
        }

        public MemberTypes GetMemberType(string memberName, string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            var memberInfo = GetMemberInfo(memberName, namespaceName, localTypeName, assemblyNameIfAny);
            switch (memberInfo)
            {
                case PropertyDefinition _:
                    return MemberTypes.Property;
                case MethodDefinition _:
                    return MemberTypes.Method;
                case FieldDefinition _:
                    return MemberTypes.Field;
                case EventDefinition _:
                    return MemberTypes.Event;
                default:
                    return MemberTypes.Custom;
            }
        }

        public void GetPropertyOrFieldTypeInfo(string propertyOrFieldName, string namespaceName, string localTypeName,
            out string propertyNamespaceName, out string propertyLocalTypeName, out string propertyAssemblyName,
            out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            var typeRef = GetPropertyOrFieldType(propertyOrFieldName, namespaceName, localTypeName,
                assemblyNameIfAny, isAttached);
            propertyNamespaceName = BuildPropertyPathRecursively(typeRef);
            propertyLocalTypeName = GetTypeNameIncludingGenericArguments(typeRef, false);
            propertyAssemblyName = typeRef.Resolve().Module.Assembly.Name.Name;
            isTypeString = typeRef.IsString();
            isTypeEnum = typeRef.Resolve().IsEnum;
        }

        public void GetMethodReturnValueTypeInfo(string methodName, string namespaceName, string localTypeName,
            out string returnValueNamespaceName, out string returnValueLocalTypeName,
            out string returnValueAssemblyName, out bool isTypeString, out bool isTypeEnum,
            string assemblyNameIfAny = null)
        {
            var typeDef = GetMethodReturnValueType(methodName, namespaceName, localTypeName, assemblyNameIfAny);
            returnValueNamespaceName = BuildPropertyPathRecursively(typeDef);
            returnValueLocalTypeName = GetTypeNameIncludingGenericArguments(typeDef, false);
            returnValueAssemblyName = typeDef.Module.Assembly.Name.Name;
            isTypeString = typeDef.IsString();
            isTypeEnum = typeDef.IsEnum;
        }

        public string GetEventHandlerType(string eventName, string namespaceName, string typeName, string assemblyName)
        {
            var type = FindType(namespaceName, typeName, assemblyName);
            var typeIterator = type;
            while (typeIterator != null)
            {
                var eventInfo = typeIterator.Events.FirstOrDefault(n => n.Name == eventName);
                if (eventInfo != null) return GetTypeNameIncludingGenericArguments(eventInfo.EventType, true);

                typeIterator = typeIterator.BaseType?.Resolve();
            }

            throw new XamlParseException($"'{type}' does not contain an event named '{eventName}'.");
        }

        public string GetFieldName(string fieldNameIgnoreCase, string namespaceName, string localTypeName,
            string assemblyIfAny = null)
        {
            var type = FindType(namespaceName, localTypeName, assemblyIfAny);

            if (type == null)
                throw new XamlParseException($"Type '{localTypeName}' not found in namespace '{namespaceName}'.");

            FieldDefinition field;
            if (type.IsEnum)
            {
                field = FindFieldDeep(type, fieldNameIgnoreCase, true, true, true);
                if (field == null)
                    // If the field isn't found "as is", we try to interpret it as the int corresponding to a field
                    if (int.TryParse(fieldNameIgnoreCase, out var value))
                    {
                        var fd = type.Fields.SingleOrDefault(f => (int)f.Constant == value);
                        var trueFieldName = fd?.Name;
                        field = FindFieldDeep(type, trueFieldName, true, true, true);
                    }
            }
            else
            {
                field = FindFieldDeep(type, fieldNameIgnoreCase, true, true, true);
            }

            return field?.Name ??
                   throw new XamlParseException($"Field '{fieldNameIgnoreCase}' not found in type: '{type.FullName}'.");
        }

        public bool IsElementAMarkupExtension(string elementNameSpace, string elementLocalName,
            string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);

            var markupExtensionGeneric = FindType(SystemXamlNamespace, GenericMarkupExtension);

            var isAssignableFrom = markupExtensionGeneric.IsAssignableFrom(elementType);
            var typeIsAMarkupExtension = isAssignableFrom && !elementType.IsString();
            return typeIsAMarkupExtension;
        }

        public bool IsTypeAssignableFrom(string nameSpaceOfTypeToAssignFrom, string nameOfTypeToAssignFrom,
            string assemblyNameOfTypeToAssignFrom, string nameSpaceOfTypeToAssignTo, string nameOfTypeToAssignTo,
            string assemblyNameOfTypeToAssignTo, bool isAttached = false)
        {
            TypeDefinition typeOfElementToAssignFrom;
            TypeDefinition typeOfElementToAssignTo;

            var indexOfLastDot = nameOfTypeToAssignFrom.LastIndexOf('.');

            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignFrom = FindType(nameSpaceOfTypeToAssignFrom, nameOfTypeToAssignFrom,
                    assemblyNameOfTypeToAssignFrom);
            }
            else
            {
                var localTypeName = nameOfTypeToAssignFrom.Substring(0, indexOfLastDot);
                var propertyName = nameOfTypeToAssignFrom.Substring(indexOfLastDot + 1);
                typeOfElementToAssignFrom = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignFrom,
                    localTypeName, assemblyNameOfTypeToAssignFrom).Resolve();
            }

            indexOfLastDot = nameOfTypeToAssignTo.LastIndexOf('.');
            if (indexOfLastDot == -1)
            {
                typeOfElementToAssignTo = FindType(nameSpaceOfTypeToAssignTo, nameOfTypeToAssignTo,
                    assemblyNameOfTypeToAssignTo);
            }
            else
            {
                var localTypeName = nameOfTypeToAssignTo.Substring(0, indexOfLastDot);
                var propertyName = nameOfTypeToAssignTo.Substring(indexOfLastDot + 1);
                typeOfElementToAssignTo = GetPropertyOrFieldType(propertyName, nameSpaceOfTypeToAssignTo, localTypeName,
                    assemblyNameOfTypeToAssignTo, isAttached).Resolve();
            }

            return typeOfElementToAssignTo.IsAssignableFrom(typeOfElementToAssignFrom);
        }

        public string GetContentPropertyName(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

            // Get instance of the attribute:
            var contentPropertyAttr = GetCustomAttributeDeep(type, ContentPropertyAttributeFullName);

            if (contentPropertyAttr == null &&
                !IsElementACollection(namespaceName, localTypeName, assemblyNameIfAny) &&
                !IsDictionary(namespaceName, localTypeName, assemblyNameIfAny))
                //if the element is a collection, it is possible to add the children directly to this element.
                throw new XamlParseException("No default content property exists for element: " + localTypeName);

            if (contentPropertyAttr == null)
                return null;

            var value = contentPropertyAttr.ConstructorArguments[0].Value.ToString();

            if (string.IsNullOrEmpty(value))
                throw new Exception("The ContentPropertyAttribute must have a non-empty Name.");

            return value;
        }

        public bool IsTypeAnEnum(string namespaceName, string localTypeName, string assemblyNameIfAny = null)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            return elementType.IsEnum;
        }

        public MethodDefinition FindMethod(TypeDefinition td, string methodName, bool onlyPublic = false,
            bool onlyStatic = false)
        {
            return td.Methods.FirstOrDefault(m =>
                m.Name == methodName && (!onlyPublic || m.IsPublic) && (!onlyStatic || m.IsStatic));
        }

        public bool IsPropertyAttached(string propertyOrFieldName, string declaringTypeNamespaceName,
            string declaringTypeLocalName, string parentNamespaceName, string parentLocalTypeName,
            string parentAssemblyNameIfAny = null)
        {
            var elementType = FindType(declaringTypeNamespaceName, declaringTypeLocalName, parentAssemblyNameIfAny);

            var field = FindFieldDeep(elementType, propertyOrFieldName + PropertySuffix) ??
                        FindFieldDeep(elementType, propertyOrFieldName + PropertySuffix.ToLower());

            if (field == null) return false;

            if (field.FieldType.Name != DependencyProperty) return false;

            var nbOfParameters = 2;
            var method = FindMethod(field.DeclaringType, SetPrefix + propertyOrFieldName, true, true);
            if (method == null)
            {
                method = FindMethod(field.DeclaringType, GetPrefix + propertyOrFieldName, true, true);
                nbOfParameters = 1;
            }

            if (method == null) return false;

            return method.Parameters.Count == nbOfParameters;
        }

        public bool IsPropertyOrFieldACollection(string propertyOrFieldName, string parentNamespaceName,
            string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            var propertyOrFieldType = GetPropertyOrFieldType(propertyOrFieldName, parentNamespaceName,
                parentLocalTypeName, parentAssemblyNameIfAny);
            return IsCollection(propertyOrFieldType.Resolve())
                   || IsDictionary(propertyOrFieldType.Resolve());
        }

        public bool IsPropertyOrFieldADictionary(string propertyName, string parentNamespaceName,
            string parentLocalTypeName, string parentAssemblyNameIfAny = null)
        {
            var propertyOrFieldType = GetPropertyOrFieldType(propertyName, parentNamespaceName, parentLocalTypeName,
                parentAssemblyNameIfAny);
            return IsDictionary(propertyOrFieldType.Resolve());
        }

        public XName GetCSharpEquivalentOfXamlTypeAsXName(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            //todo: in this method, we assume that the alias will be global, which will be false if the user chose something else --> find the right alias.
            // Find the type:
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            if (type == null)
                throw new XamlParseException(
                    $"Type \"{localTypeName}\" not found in namespace \"{namespaceName}\".");

            // Use information from the type:
            return XName.Get(type.Name, namespaceName);
        }

        public bool DoesTypeContainNameMemberOfTypeString(string namespaceName, string localTypeName,
            string assemblyNameIfAny = null)
        {
            var memberInfo = GetMemberInfo(Name, namespaceName, localTypeName, assemblyNameIfAny,
                true);
            if (memberInfo == null) return false;

            //ToDo: IsSecurityCritical?
            if (memberInfo is FieldDefinition fd && fd.IsString() && fd.IsPublic && !fd.IsStatic) return true;

            if (memberInfo is PropertyDefinition pd && pd.IsString()) return true;

            return false;
        }

        public bool DoesMethodReturnACollection(string methodName, string typeNamespaceName, string localTypeName,
            string typeAssemblyNameIfAny = null)
        {
            var propertyType =
                GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
            return IsCollection(propertyType.Resolve())
                   || IsDictionary(propertyType.Resolve());
        }

        public bool DoesMethodReturnADictionary(string methodName, string typeNamespaceName, string localTypeName,
            string typeAssemblyNameIfAny = null)
        {
            var propertyType =
                GetMethodReturnValueType(methodName, typeNamespaceName, localTypeName, typeAssemblyNameIfAny);
            return IsDictionary(propertyType.Resolve());
        }

        public string GetField(string fieldName, string namespaceName, string typeName, string assemblyName)
        {
            var type = FindType(namespaceName, typeName, null, true);

            var field = FindFieldDeep(type, fieldName, false, false, assemblyName != type.Module.Name);
            if (field != null &&
                (field.IsPublic || field.IsAssembly || field.IsFamilyOrAssembly))
                return $"{GetTypeNameIncludingGenericArguments(field.DeclaringType, true)}.{field.Name}";

            return null;
        }

        public string GetKeyNameOfProperty(string namespaceName, string localTypeName, string assemblyNameIfAny,
            string propertyName)
        {
            var type = FindType(namespaceName, localTypeName, assemblyNameIfAny);

            var property = FindPropertyDeep(type, propertyName);
            if (property == null) return null;

            // Look for the static dependency property field in the type and its ancestors:
            var fieldName = propertyName + PropertySuffix;
            var field = FindFieldDeep(type, fieldName, true, true, true);
            if (field != null) return GlobalPrefix + type + "." + fieldName;

            return null;
        }

        public void GetPropertyOrFieldInfo(string propertyOrFieldName, string namespaceName, string localTypeName,
            out string memberDeclaringTypeName, out string memberTypeNamespace, out string memberTypeName,
            out bool isTypeString, out bool isTypeEnum, string assemblyNameIfAny = null, bool isAttached = false)
        {
            var elementType = FindType(namespaceName, localTypeName, assemblyNameIfAny);
            var propertyInfo = FindPropertyDeep(elementType, propertyOrFieldName);
            TypeReference propertyOrFieldType;
            TypeDefinition propertyOrFieldDeclaringType;

            if (propertyInfo == null)
            {
                var fieldInfo = FindFieldDeep(elementType, propertyOrFieldName);
                if (fieldInfo == null)
                    throw new XamlParseException("Property or field \"" + propertyOrFieldName +
                                                 "\" not found in type \"" + elementType + "\".");

                propertyOrFieldType = fieldInfo.FieldType;
                propertyOrFieldDeclaringType = fieldInfo.DeclaringType;
            }
            else
            {
                propertyOrFieldType = propertyInfo.PropertyType;
                propertyOrFieldDeclaringType = propertyInfo.DeclaringType;
            }

            memberDeclaringTypeName = GetTypeNameIncludingGenericArguments(propertyOrFieldDeclaringType, true);
            memberTypeNamespace = BuildPropertyPathRecursively(propertyOrFieldType);
            memberTypeName = GetTypeNameIncludingGenericArguments(propertyOrFieldType, false);
            isTypeString = propertyOrFieldType.IsString();
            isTypeEnum = propertyOrFieldType.Resolve().IsEnum;
        }

        public bool IsElementADictionary(string elementNameSpace, string elementLocalName, string assemblyNameIfAny)
        {
            var elementType = FindType(elementNameSpace, elementLocalName, assemblyNameIfAny);
            return IsDictionary(elementType);
        }
    }
}