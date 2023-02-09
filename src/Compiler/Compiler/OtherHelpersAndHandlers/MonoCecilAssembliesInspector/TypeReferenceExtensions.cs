
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

namespace DotNetForHtml5.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector
{
    internal static class TypeReferenceExtensions
    {
        public static TypeReference ResolveGenericParameter(this TypeReference typeRef, TypeReference elementType)
        {
            while (elementType != null && !(elementType is GenericInstanceType))
            {
                elementType = elementType.Resolve()?.BaseType?.Resolve();
            }

            if (!(elementType is GenericInstanceType genericType))
            {
                return typeRef;
            }

            if (typeRef is GenericParameter genericParameter)
            {
                return genericType.GenericArguments[genericParameter.Position];
            }

            return typeRef;
        }
    }
}
