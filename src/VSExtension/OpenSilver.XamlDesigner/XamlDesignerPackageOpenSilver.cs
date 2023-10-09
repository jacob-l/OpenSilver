using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Userware.XamlDesigner.SplitXamlEditor;

namespace OpenSilver.XamlDesigner
{
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is a package:
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // This attribute is used to register the information needed to show this package in the Help/About dialog of Visual Studio:
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus:
    [ProvideMenuResource("Menus.ctmenu", 1)]

    // Register the editor:
#if OPENSILVER
    [ProvideEditorExtension(typeof(EditorFactory), EditorFactory.Extension, 2300, NameResourceID = 113)]
#elif XRSHARP
    [ProvideEditorExtension(typeof(EditorFactory), EditorFactory.Extension, 2200, NameResourceID = 114)]
#else
    [ProvideEditorExtension(typeof(EditorFactory), EditorFactory.Extension, 2100, NameResourceID = 106)]
#endif
    // Key binding table:
    [ProvideKeyBindingTable(GuidList.guidEditorFactoryString, 102)]

    // We register that our editor supports LOGVIEWID_Designer logical view:
    [ProvideEditorLogicalView(typeof(EditorFactory), LogicalViewID.Designer)]

    // Our Editor supports Find and Replace therefore we need to declare support for LOGVIEWID_TextView.
    // This attribute declares that your EditorPane class implements IVsCodeWindow interface
    // used to navigate to find results from a "Find in Files" type of operation.
    [ProvideEditorLogicalView(typeof(EditorFactory), LogicalViewID.TextView)]

    [Guid(GuidList.guidEditorPkgString)]
    public sealed class XamlDesignerPackageOpenSilver : Package
    {
        public XamlDesignerPackageOpenSilver() // Default constructor of the package. Inside this method you can place any initialization code that does not require any Visual Studio service because at this point the package object is created but not sited yet inside Visual Studio environment. The place to do all the other initialization is the Initialize method.
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        #region Package Members

        protected override void Initialize() // Initialization of the package; this method is called right after the package is sited, so this is the place where you can put all the initialization code that rely on services provided by VisualStudio.
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            //Create Editor Factory. Note that the base Package class will call Dispose on it.
            base.RegisterEditorFactory(new EditorFactory(this));
        }
        #endregion

    }
}
