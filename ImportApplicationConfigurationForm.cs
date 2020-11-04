using System;
using System.Windows.Forms;
using System.IO;

namespace ZinvoiceTransformer
{
    public partial class ImportApplicationConfigurationForm : Form
    {
        InvoiceTemplateModel _invoiceTemplateModel;

        public ImportApplicationConfigurationForm(InvoiceTemplateModel invoiceTemplateModel)
        {
            InitializeComponent();
            _invoiceTemplateModel = invoiceTemplateModel;
            _setAppLocationButton.Click += OnSetAppLocationClick;
            _setInvoiceFileLocationButton.Click += OnSetInvoiceFileLocationClick;
            _saveButton.Click += OnSaveClick;

            _appLocationTextBox.DataBindings.Add(new Binding("Text", _invoiceTemplateModel, "ImportAppLocation", false, DataSourceUpdateMode.OnPropertyChanged));
            _invoiceFileLocationTextBox.DataBindings.Add(new Binding("Text", _invoiceTemplateModel, "ImportAppInvoiceFileLocation", false,DataSourceUpdateMode.OnPropertyChanged));
        }

        private void OnSetAppLocationClick(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(_appLocationTextBox.Text);
            }
            catch (Exception)
            {
                openFileDialog1.InitialDirectory = @"c:\";
            }

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                _appLocationTextBox.Text = openFileDialog1.FileName;
        }

        private void OnSetInvoiceFileLocationClick(object sender, EventArgs e)
        {
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
                _invoiceFileLocationTextBox.Text = folderBrowserDialog1.SelectedPath;
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            _invoiceTemplateModel.Save();
        }

        private void ImportApplicationConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_invoiceTemplateModel.IsDirty)
                switch (MessageBox.Show(this, "Do you want to save your changes before closing?", "Close", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        _invoiceTemplateModel.Save();
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                    default:
                        e.Cancel = true;
                        break;
                }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
