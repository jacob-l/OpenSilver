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
