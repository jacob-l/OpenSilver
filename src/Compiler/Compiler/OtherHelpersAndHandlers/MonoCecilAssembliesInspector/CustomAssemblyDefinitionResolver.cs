
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
using Mono.Cecil;

namespace DotNetForHtml5.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector
{
    internal class CustomAssemblyDefinitionResolver : BaseAssemblyResolver
    {
        private readonly Func<string, AssemblyDefinition> _fallbackAssemblyResolver;
        private readonly DefaultAssemblyResolver _defaultResolver = new DefaultAssemblyResolver();

        public CustomAssemblyDefinitionResolver(Func<string, AssemblyDefinition> fallbackAssemblyResolver)
        {
            _fallbackAssemblyResolver = fallbackAssemblyResolver;
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            try
            {
                return _defaultResolver.Resolve(name);
            }
            catch (AssemblyResolutionException)
            {
                return _fallbackAssemblyResolver(name.Name);
            }
        }
    }
}
