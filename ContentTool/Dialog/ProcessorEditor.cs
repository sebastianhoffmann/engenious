using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ContentTool.Dialog
{
    internal class ProcessorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context == null || context.Instance == null)
                return base.GetEditStyle(context);

            return UITypeEditorEditStyle.DropDown;
        }

        private IWindowsFormsEditorService _editorService;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.Instance == null || !(context.Instance is ContentFile) || provider == null)
                return value;

            _editorService = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));

            var lb = new ListBox();
            lb.SelectionMode = SelectionMode.One;
            lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

            var file = context.Instance as ContentFile;

            string ext = System.IO.Path.GetExtension(file.Name);
            var baseType = PipelineHelper.GetImporterOutputType(ext);
            lb.Items.AddRange(PipelineHelper.GetProcessors(baseType).ToArray());


            _editorService.DropDownControl(lb);
            if (lb.SelectedItem == null)
                return value;

            return lb.SelectedItem;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}