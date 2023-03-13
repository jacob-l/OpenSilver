
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

using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;

namespace OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector
{
    internal static class TypeReferenceExtensions
    {
        public static TypeReference ResolveGenericParameter(this TypeReference typeRef, TypeReference elementType)
        {
            var genericParameter = typeRef as GenericParameter;
            if (genericParameter == null)
            {
                throw new ArgumentException("Type Reference must be a GenericParameter");
            }

            var genericType = genericParameter.DeclaringType.GetGenericInstanceType(elementType);

            return genericType.GenericArguments[genericParameter.Position];
        }

        public static GenericInstanceType GetGenericInstanceType(this TypeReference typeRef, TypeReference elementType)
        {
            elementType = elementType.Resolve()?.BaseType;
            var typeDef = typeRef.Resolve();
            while (elementType != null && typeDef != elementType.Resolve())
            {
                elementType = elementType.Resolve()?.BaseType;
            }

            return elementType as GenericInstanceType;
        }

        public static TypeReference PopulateGeneric(this TypeReference typeRef, TypeReference rootElementType, TypeReference ownerElementType)
        {
            if (typeRef.IsGenericParameter)
            {
                return typeRef.ResolveGenericParameter(rootElementType);
            }

            if (typeRef is GenericInstanceType instance)
            {
                var cc = ownerElementType.GetGenericInstanceType(rootElementType);
                var arg = instance.GenericArguments.Select(ga =>
                {
                    if (ga is GenericParameter genericParameter)
                    {
                        return cc.GenericArguments[genericParameter.Position];
                    }

                    return ga.PopulateGeneric(rootElementType, ownerElementType);
                }).ToArray();
                return instance.ElementType.MakeGenericInstanceType(arg);
            }

            return typeRef;
        }
    }
}
