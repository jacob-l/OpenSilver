using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetForHtml5.Compiler;
using DotNetForHtml5.Compiler.OtherHelpersAndHandlers;
using Microsoft.Build.Framework;

namespace Compiler.ExperimentsRunner
{
    class Logger : DotNetForHtml5.Compiler.ILogger
    {
        public void WriteError(string message, string file = "", int lineNumber = 0, int columnNumber = 0)
        {
            
        }

        public void WriteWarning(string message, string file = "", int lineNumber = 0, int columnNumber = 0)
        {
        }

        public bool HasErrors { get; }

        public void WriteMessage(string message, MessageImportance messageImportance = MessageImportance.Normal)
        {
        }
    }
    internal class Program
    {
        static void MainA(string[] args)
        {
            var sourceFiles =
                "C:\\Users\\user\\Documents\\GitHub\\OpenSilver.Samples.Showcase\\src\\Other\\MaterialDesign_Styles_Kit\\MaterialDesign_CommonResources.xaml";
            var outputFile =
                "C:\\Users\\user\\Documents\\GitHub\\OpenSilver.Samples.Showcase\\src\\obj\\SL.Release\\net7.0\\Other\\MaterialDesign_Styles_Kit\\MaterialDesign_CommonResources.xaml.True.g.cs";
            var fileNameWithPathRelativeToProjectRoot = "Other\\MaterialDesign_Styles_Kit\\MaterialDesign_CommonResources.xaml";
            var assemblyNameWithoutExtension = "OpenSilver.Samples.Showcase";
            var coreAssemblyFiles = "C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\build\\..\\lib\\netstandard2.0\\OpenSilver.dll";
            var isSecondPass = true;
            var isSLMigration = true;
            var activationAppPath = "C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\build\\..\\Activation\\CSharpXamlForHtml5.Activation.exe";
            string cSharpXamlForHtml5OutputType = null;
            var overrideOutputOnlyIfSourceHasChanged = false;
            var outputRootPath = "wwwroot\\";
            var outputAppFilesPath = "app\\";
            var outputLibrariesPath = "libs\\";
            var outputResourcesPath = "resources\\";
            var flagsString = "noflags";
            var isBridgeBasedVersion = true;
            var nameOfAssembliesThatDoNotContainUserCode = "DotNetBrowser.Chromium";

            XamlPreprocessor.Execute(sourceFiles, outputFile, fileNameWithPathRelativeToProjectRoot,
                assemblyNameWithoutExtension,
                coreAssemblyFiles, isSecondPass, isSLMigration, new Logger(), activationAppPath,
                null, overrideOutputOnlyIfSourceHasChanged, outputRootPath,
                outputAppFilesPath, outputLibrariesPath, outputResourcesPath, flagsString, isBridgeBasedVersion,
                nameOfAssembliesThatDoNotContainUserCode);
        }

        static void Main(string[] args)
        {
            /*
            ResourcesExtractorAndCopier.Execute(
                "C:\\Users\\user\\Documents\\GitHub\\OpenSilver.Samples.Showcase\\src\\bin\\SL.Release\\net7.0\\OpenSilver.Samples.Showcase.dll",
                "wwwroot\\", "resources\\", "mscorlib|System.Core|Microsoft.CSharp|JSIL.Meta|Bridge",
                ".js|.css|.png|.jpg|.gif|.ico|.mp4|.ogv|.webm|.3gp|.mp3|.ogg|.txt|.xml|.ttf|.woff|.woff2|.cur|.config|.ClientConfig|.htm|.html|.svg",
                new Logger(), true,
                "C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\build\\..\\tools\\CSharpXamlForHtml5.Bridge.TypeForwarding.dll",
                "DotNetBrowser.Chromium",
                "C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\build\\..\\lib\\netstandard2.0\\OpenSilver.dll",
                out var list);
            */
            
            var sourceFile =
                "C:\\Users\\user\\Documents\\GitHub\\OpenSilver.Samples.Showcase\\src\\Other\\MaterialDesign_Styles_Kit\\MaterialDesign_CommonResources.xaml";
            var fileNameWithPathRelativeToProjectRoot =
                "Other\\MaterialDesign_Styles_Kit\\MaterialDesign_CommonResources.xaml";
            var assemblyNameWithoutExtension = "OpenSilver.Samples.Showcase";
            var reflectionOnSeparateAppDomain = new AssembliesInspector();
            var isSecondPass = true;
            var isSLMigration = true;
            var outputRootPath = "wwwroot\\";
            var outputAppFilesPath = "app\\";
            var outputLibrariesPath = "libs\\";
            var outputResourcesPath = "resources\\";

            LoadAssemblies(reflectionOnSeparateAppDomain);

            /*
            var resDict = reflectionOnSeparateAppDomain.GetManifestResources("OpenSilver.Samples.Showcase",
                new HashSet<string>
                {
                    ".js", ".css", ".png", ".jpg", ".gif", ".ico", ".mp4", ".ogv", ".webm", ".3gp", ".mp3", ".ogg",
                    ".txt", ".xml", ".ttf", ".woff", ".woff2", ".cur", ".config", ".clientconfig", ".htm", ".html",
                    ".svg"
                });
            foreach(var kvp in resDict)
            {
                Console.WriteLine($"{kvp.Key} - {kvp.Value}");
            }
            */
            reflectionOnSeparateAppDomain.GetAttachedPropertyGetMethodInfo("GetPlacementTarget", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "ToolTipService",
                out string declaringTypeName, out string returnValueNamespaceName, out string returnValueLocalTypeName, out bool isTypeString, out bool isTypeEnum, null);
            //var res = reflectionOnSeparateAppDomain.GetAssemblyQualifiedNameOfXamlType("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Validation", null);
            //Console.WriteLine(res);
            //var res = reflectionOnSeparateAppDomain.IsElementADictionary("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "ResourceDictionary");
            //reflectionOnSeparateAppDomain.GetPropertyOrFieldInfo("Value", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "DiscreteObjectKeyFrame", out var memberDeclaringTypeName,
            //    out var memberTypeNamespace, out var memberTypeName, out var isTypeString, out var isTypeEnum);
            //var res = reflectionOnSeparateAppDomain.GetKeyNameOfProperty("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Expander", null, "Style");
            //var res = reflectionOnSeparateAppDomain.GetField("BorderThicknessProperty", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Border", "OpenSilver.Samples.Showcase");
            //var res = reflectionOnSeparateAppDomain.DoesMethodReturnADictionary("GetVisualStateGroups", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "VisualStateManager");
            //var res = reflectionOnSeparateAppDomain.DoesMethodReturnACollection("GetVisualStateGroups", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "VisualStateManager");
            //var res = reflectionOnSeparateAppDomain.DoesTypeContainNameMemberOfTypeString("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "VisualStateGroup");
            //var res = reflectionOnSeparateAppDomain.GetCSharpEquivalentOfXamlTypeAsXName("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Button");
            //var res = reflectionOnSeparateAppDomain.IsPropertyOrFieldADictionary("RowDefinitions", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Grid");
            //var res = reflectionOnSeparateAppDomain.IsPropertyOrFieldACollection("RowDefinitions", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Grid");
            //var res = reflectionOnSeparateAppDomain.IsPropertyAttached("RowDefinitions", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Grid", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Grid");

            //var res = reflectionOnSeparateAppDomain.IsTypeAnEnum("OpenSilver.Samples.Showcase", "PlanetStructure");
            //Console.WriteLine(res);
            /*
            var res = reflectionOnSeparateAppDomain.GetContentPropertyName("http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "Page", null); ;
            Console.WriteLine(res);

            reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
                "Width",
                "http://schemas.microsoft.com/winfx/2006/xaml/presentation",
                "ChildWindow",
                out var valueNamespaceName,
                out var valueLocalTypeName,
                out var valueAssemblyName,
                out var isValueString,
                out var isValueEnum,
                null);
            Console.WriteLine(valueAssemblyName);
            
            
            var res = reflectionOnSeparateAppDomain.IsTypeAssignableFrom("http://schemas.microsoft.com/expression/2010/interactions",
                "ChangePropertyAction", null,
                "http://schemas.microsoft.com/expression/2010/interactions", "PropertyChangedTrigger.Actions",
                null, false);
            Console.WriteLine(res);
            
reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo(
    "To",
    "http://schemas.microsoft.com/winfx/2006/xaml/presentation",
    "DoubleAnimation",
    out var valueNamespaceName,
    out var valueLocalTypeName,
    out var valueAssemblyName,
    out var isValueString,
    out var isValueEnum,
    null);
Console.WriteLine(valueLocalTypeName);

var res = reflectionOnSeparateAppDomain.IsElementAMarkupExtension("System.Windows.Markup",
"StaticResourceExtension", null);
Console.WriteLine("Res - " + res);

reflectionOnSeparateAppDomain.GetPropertyOrFieldTypeInfo("Background",
"System.Windows.Controls", "Page",
out var propertyNamespaceName, out var propertyLocalTypeName,
out var propertyAssemblyName, out var isTypeString,
out var isTypeEnum);

var res2 = reflectionOnSeparateAppDomain.GetMemberType("Click", "http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Button");
var res = reflectionOnSeparateAppDomain.GetMemberType("HorizontalAlignment", "OpenSilver.Samples.Showcase", "WCF_SOAP_Demo");

reflectionOnSeparateAppDomain.IsAssignableFrom("System.Windows", "ResourceDictionary",
"http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk", "Page");


using (StreamReader sr = new StreamReader(sourceFile))
{
String xaml = sr.ReadToEnd();
ConvertingXamlToCSharp.Convert(xaml, sourceFile, fileNameWithPathRelativeToProjectRoot,
assemblyNameWithoutExtension, reflectionOnSeparateAppDomain, isFirstPass: !isSecondPass,
isSLMigration: isSLMigration, outputRootPath: outputRootPath,
outputAppFilesPath: outputAppFilesPath, outputLibrariesPath: outputLibrariesPath,
outputResourcesPath: outputResourcesPath, logger: new Logger());
}
*/
            Console.WriteLine("Success");
            Console.ReadKey();
        }

        private static void LoadAssemblies(AssembliesInspector reflectionOnSeparateAppDomain)
        {
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\microsoft.bcl.asyncinterfaces\\5.0.0\\lib\\netstandard2.1\\Microsoft.Bcl.AsyncInterfaces.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\Microsoft.CSharp.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\microsoft.extensions.objectpool\\5.0.10\\lib\\net5.0\\Microsoft.Extensions.ObjectPool.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\microsoft.jsinterop\\3.1.28\\lib\\netcoreapp3.1\\Microsoft.JSInterop.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\Microsoft.VisualBasic.Core.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\Microsoft.VisualBasic.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\Microsoft.Win32.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\Microsoft.Win32.Registry.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\mscorlib.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\netstandard.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Controls.Data.DataForm.Toolkit.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Controls.Data.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Controls.Data.Input.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Controls.DataVisualization.Toolkit.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Controls.Input.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Controls.Layout.Toolkit.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Controls.Navigation.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Expression.Effects.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Expression.Interactions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\opensilver\\1.2.0-preview-2023-01-17-113548-386ed8bc\\lib\\netstandard2.0\\OpenSilver.Interactivity.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.AppContext.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Buffers.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Collections.Concurrent.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Collections.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Collections.Immutable.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Collections.NonGeneric.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Collections.Specialized.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ComponentModel.Annotations.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.componentmodel.composition\\6.0.0\\lib\\net6.0\\System.ComponentModel.Composition.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ComponentModel.DataAnnotations.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ComponentModel.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ComponentModel.EventBasedAsync.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ComponentModel.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ComponentModel.TypeConverter.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Configuration.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Console.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Core.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Data.Common.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Data.DataSetExtensions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Data.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.Contracts.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.Debug.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.DiagnosticSource.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.FileVersionInfo.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.Process.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.StackTrace.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.TextWriterTraceListener.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.Tools.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.TraceSource.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Diagnostics.Tracing.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.drawing.common\\5.0.0\\ref\\netcoreapp3.0\\System.Drawing.Common.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Drawing.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Drawing.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Dynamic.Runtime.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Formats.Asn1.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Formats.Tar.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Globalization.Calendars.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Globalization.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Globalization.Extensions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.Compression.Brotli.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.Compression.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.Compression.FileSystem.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.Compression.ZipFile.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.FileSystem.AccessControl.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.FileSystem.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.FileSystem.DriveInfo.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.FileSystem.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.FileSystem.Watcher.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.IsolatedStorage.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.MemoryMappedFiles.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.Pipes.AccessControl.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.Pipes.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.IO.UnmanagedMemoryStream.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Linq.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Linq.Expressions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Linq.Parallel.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Linq.Queryable.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Memory.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Http.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Http.Json.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.HttpListener.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Mail.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.NameResolution.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.NetworkInformation.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Ping.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Quic.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Requests.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Security.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.ServicePoint.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.Sockets.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.WebClient.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.WebHeaderCollection.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.WebProxy.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.WebSockets.Client.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Net.WebSockets.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Numerics.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Numerics.Vectors.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ObjectModel.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.DispatchProxy.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.Emit.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.Emit.ILGeneration.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.Emit.Lightweight.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.Extensions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.Metadata.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Reflection.TypeExtensions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Resources.Reader.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Resources.ResourceManager.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Resources.Writer.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.CompilerServices.Unsafe.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.CompilerServices.VisualC.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Extensions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Handles.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.InteropServices.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.InteropServices.JavaScript.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.InteropServices.RuntimeInformation.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Intrinsics.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Loader.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Numerics.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Serialization.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Serialization.Formatters.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Serialization.Json.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Serialization.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Runtime.Serialization.Xml.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.AccessControl.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Claims.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Cryptography.Algorithms.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Cryptography.Cng.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Cryptography.Csp.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Cryptography.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Cryptography.Encoding.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Cryptography.OpenSsl.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Cryptography.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Cryptography.X509Certificates.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.security.cryptography.xml\\5.0.0\\ref\\netstandard2.0\\System.Security.Cryptography.Xml.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.security.permissions\\5.0.0\\ref\\net5.0\\System.Security.Permissions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Principal.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.Principal.Windows.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Security.SecureString.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.servicemodel.primitives\\4.10.0\\ref\\net6.0\\System.ServiceModel.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.servicemodel.duplex\\4.10.0\\ref\\net6.0\\System.ServiceModel.Duplex.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.servicemodel.http\\4.10.0\\ref\\net6.0\\System.ServiceModel.Http.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.servicemodel.nettcp\\4.10.0\\ref\\net6.0\\System.ServiceModel.NetTcp.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.servicemodel.primitives\\4.10.0\\ref\\net6.0\\System.ServiceModel.Primitives.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.servicemodel.security\\4.10.0\\ref\\net6.0\\System.ServiceModel.Security.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ServiceModel.Web.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ServiceProcess.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Text.Encoding.CodePages.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Text.Encoding.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Text.Encoding.Extensions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Text.Encodings.Web.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Text.Json.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Text.RegularExpressions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.Channels.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.Overlapped.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.Tasks.Dataflow.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.Tasks.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.Tasks.Extensions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.Tasks.Parallel.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.Thread.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.ThreadPool.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Threading.Timer.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Transactions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Transactions.Local.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.ValueTuple.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Web.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Web.HttpUtility.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Windows.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\.nuget\\packages\\system.windows.extensions\\5.0.0\\ref\\netcoreapp3.0\\System.Windows.Extensions.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.Linq.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.ReaderWriter.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.Serialization.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.XDocument.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.XmlDocument.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.XmlSerializer.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.XPath.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\System.Xml.XPath.XDocument.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\7.0.2\\ref\\net7.0\\WindowsBase.dll", false, true, false, "DotNetBrowser.Chromium", false);
            reflectionOnSeparateAppDomain.LoadAssemblyMscorlib(true, false, "DotNetBrowser.Chromium");
            reflectionOnSeparateAppDomain.LoadAssembly("C:\\Users\\user\\Documents\\GitHub\\OpenSilver.Samples.Showcase\\src\\bin\\SL.Release\\net7.0\\\\OpenSilver.Samples.Showcase.dll", true, true, false, "DotNetBrowser.Chromium", false);
        }
    }
}
