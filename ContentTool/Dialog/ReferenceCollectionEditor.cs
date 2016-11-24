using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ContentTool.Dialog
{
    internal class ReferenceCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context == null || context.Instance == null)
                return base.GetEditStyle(context);

            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService;

            if (context == null || context.Instance == null || !(context.Instance is ContentProject) || provider == null)
                return value;

            editorService = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

            var collectionEditor = new FrmEditReferences();

            collectionEditor.RootDir = System.IO.Path.GetDirectoryName((context.Instance as ContentProject).File);
            collectionEditor.References = value as List<string>;
            return editorService.ShowDialog(collectionEditor) == System.Windows.Forms.DialogResult.OK
                ? collectionEditor.References
                : value;

            //return base.EditValue(context, provider, value);
        }
    }
}