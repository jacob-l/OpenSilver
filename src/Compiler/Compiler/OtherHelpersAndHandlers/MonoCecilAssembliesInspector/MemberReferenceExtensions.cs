using Mono.Cecil;

namespace DotNetForHtml5.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector
{
    internal static class MemberReferenceExtensions
    {
        private const string SystemString = "System.String";

        public static bool IsString(this MemberReference type)
        {
            return type.FullName == SystemString;
        }
    }
}
