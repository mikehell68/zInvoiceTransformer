using System;
using System.Drawing;
using System.Windows.Forms;

namespace zInvoiceTransformer
{
    internal class TemplateFieldDefinition : UserControl
    {
        public event EventHandler<FieldLocationChangedEventArgs> FieldLocationChanged;
        
        readonly TableLayoutPanel _fieldPanel;
        readonly Label _fieldLabel;
        readonly NumericUpDown _fieldPosition;
        readonly int _fieldNameId;
        readonly int? _directiveId;
        readonly RadioButton _masterRB;
        readonly RadioButton _detailRB;
        readonly RadioButton _footerRB;
        readonly ToolTip _toolTip = new ToolTip();

        public TemplateFieldDefinition(string fieldName, int id, int fieldPosition, int recordLocation, int? hasDirective)
        {
            InitialiseControl();

            _directiveId = hasDirective;
            _fieldNameId = id;
            _fieldPanel = new TableLayoutPanel();
            _fieldLabel = new Label();
            _fieldPosition = new NumericUpDown();
            _masterRB = new RadioButton();
            _detailRB = new RadioButton();
            _footerRB = new RadioButton();

            _masterRB.CheckedChanged += new EventHandler(_fieldLocation_CheckedChanged);

            _fieldLabel.Text = fieldName+":";
            _fieldLabel.Dock = DockStyle.Left;
            _fieldLabel.TextAlign = ContentAlignment.MiddleLeft;

            _fieldPosition.DecimalPlaces = 0;
            _fieldPosition.Minimum = 0;
            _fieldPosition.Maximum = 3000;
            _fieldPosition.Dock = DockStyle.Fill;
            _fieldPosition.Value = fieldPosition;
            _toolTip.SetToolTip(_fieldPosition, "This is the position of the field in the master/detail/footer record with 0 being the first field in the record");

            _masterRB.Checked = recordLocation == 1;
            _masterRB.Text = "M";
            _masterRB.CheckAlign = ContentAlignment.MiddleRight;
            _masterRB.AutoSize = true;
            _toolTip.SetToolTip(_masterRB, "This field is located in the master record");

            _detailRB.Checked = recordLocation == 2;
            _detailRB.Text = "D";
            _detailRB.CheckAlign = ContentAlignment.MiddleRight;
            _detailRB.AutoSize = true;
            _toolTip.SetToolTip(_detailRB, "This field is located in the detail record");

            _footerRB.Checked = recordLocation == 3;
            _footerRB.Text = "F";
            _footerRB.CheckAlign = ContentAlignment.MiddleRight;
            _footerRB.AutoSize = true;
            _toolTip.SetToolTip(_footerRB, "This field is located in the footer record");

            _fieldPanel.ColumnCount = 5;
            _fieldPanel.Controls.Add(_fieldLabel);
            _fieldPanel.Controls.Add(_fieldPosition);
            _fieldPanel.Controls.Add(_masterRB);
            _fieldPanel.Controls.Add(_detailRB);
            _fieldPanel.Controls.Add(_footerRB);
            _fieldPanel.AutoSize = true;
            _fieldPanel.Height = _masterRB.Height + 1;

            Controls.Add(_fieldPanel);
        }

        public int FieldNameId
        {
            get { return _fieldNameId; }
        }

        public FieldRecordLocation FieldLocation
        {
            get 
            {
                if (_masterRB.Checked)
                    return FieldRecordLocation.MasterRow;
                else if (_footerRB.Checked)
                    return FieldRecordLocation.SummaryRow;
                else
                    return FieldRecordLocation.DetailFields;
            }
        }

        public int FieldPosition
        {
            get { return (int)_fieldPosition.Value; }
        }

        public int? DirectiveId
        {
            get { return _directiveId; }
        } 

        private void InitialiseControl()
        {
            AutoSize = true;
        }

        void _fieldLocation_CheckedChanged(object sender, EventArgs e)
        {
            var fl = new FieldLocationChangedEventArgs();
            fl.FieldLocation = FieldLocation;
            OnFieldLocationChanged(fl);
        }

        protected virtual void OnFieldLocationChanged(FieldLocationChangedEventArgs e)
        {
            EventHandler<FieldLocationChangedEventArgs> handler = FieldLocationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class FieldLocationChangedEventArgs : EventArgs
    {
        public FieldRecordLocation FieldLocation { get; set; }
    }

}