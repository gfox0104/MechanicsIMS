using MJC.common;
using MJC.common.components;
using MJC.forms.sku.subAssembly;
using MJC.model;
using System.Data;
using System.Security.Cryptography;


namespace MJC.forms.sku
{
    public partial class SubAssemblies : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkChangeStatus = new HotkeyButton("F2", "Change Status", Keys.F2);
        private HotkeyButton hkInsertComment = new HotkeyButton("F4", "Insert comment", Keys.F4);
        private HotkeyButton hkCalculateCot = new HotkeyButton("F5", "Calculate cost", Keys.F5);
        private HotkeyButton hkEsc = new HotkeyButton("ESC", "Previous Screen", Keys.Escape);

        private GridViewOrigin SubAssembliesGrid = new GridViewOrigin();
        private DataGridView SubAssembliesGridRefer;
        
        private FlabelConstant Status = new FlabelConstant("Status:");
        private FlabelConstant PrintInvoice = new FlabelConstant("Print on Invoice:");

        private int skuId = 0;
        private string targetSKU = "";
        private bool readOnly = false;
        public SubAssemblies(int skuId, bool readOnly) : base("Sub-assemblies for SKU#", "Describe the necessary inventory to construct the selected SKU#")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons;
            if (readOnly)
            {
                hkButtons = new HotkeyButton[1] { hkEsc };
            } else
            {
                hkButtons = new HotkeyButton[6] { hkAdds, hkDeletes, hkEdits, hkChangeStatus, hkInsertComment, hkCalculateCot };
            }
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();
            this.skuId = skuId;
            this.readOnly = readOnly;

            InitHeaderForm();
            InitSubAssembly();
            if (skuId != 0)
            {
                this.targetSKU = Session.SKUModelObj.GetSkuNameById(skuId);
                this._changeFormText("Sub-assemblies for SKU# " + this.targetSKU);
            }

        }

        private void InitHeaderForm()
        {
            Panel _panel = new System.Windows.Forms.Panel();
            _panel.BackColor = System.Drawing.Color.Transparent;
            _panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _panel.Location = new System.Drawing.Point(0, 95);
            _panel.Size = new Size(this.Width - 15, 75);
            _panel.AutoScroll = true;
            this.Controls.Add(_panel);

            List<dynamic> FormComponents = new List<dynamic>();
            List<dynamic> LineComponents = new List<dynamic>();

            Status.SetContext("Information Only");
            Status.GetLabel().Width = 80;
            PrintInvoice.SetContext("Yes");
            PrintInvoice.GetLabel().Width = 180;
            LineComponents.Add(Status);
            LineComponents.Add(PrintInvoice);

            FormComponents.Add(LineComponents);
            _addFormInputs(LineComponents, 30, 10, 350, 40, 20, _panel.Controls);
        }

        private void InitSubAssembly()
        {
            SubAssembliesGridRefer = SubAssembliesGrid.GetGrid();
            SubAssembliesGridRefer.Location = new Point(0, 145);
            SubAssembliesGridRefer.Width = this.Width;
            SubAssembliesGridRefer.Height = this.Height - 345;
            SubAssembliesGridRefer.VirtualMode = true;
            SubAssembliesGridRefer.AllowUserToAddRows = false;

            this.Controls.Add(SubAssembliesGridRefer);
            this.SubAssembliesGridRefer.CellDoubleClick += (sender, e) =>
            {
                updateSubAssembly();
            };
            this.LoadSubAssemblies();
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                SubAssemblyDetail detailModal = new SubAssemblyDetail(this.skuId);
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadSubAssemblies();
                }
            };

            hkEdits.GetButton().Click += (sender, e) =>
            {
                updateSubAssembly();
            };

            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int rowIndex = SubAssembliesGridRefer.CurrentCell.RowIndex;
                    DataGridViewRow row = SubAssembliesGridRefer.Rows[rowIndex];
                    int id = (int)row.Cells[0].Value;

                    var refreshData = Session.SubAssemblyModelObj.DeleteSubAssembly(id);
                    if (refreshData)
                    {
                        LoadSubAssemblies();
                    }
                }
            };

            hkInsertComment.GetButton().Click += (sender, e) =>
            {
                int rowIndex = SubAssembliesGridRefer.CurrentCell.RowIndex;
                DataGridViewRow row = SubAssembliesGridRefer.Rows[rowIndex];
                int id = (int)row.Cells[0].Value;

                SubAssemblyMessage detailModal = new SubAssemblyMessage();
                detailModal.setDetails(id);
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadSubAssemblies();
                }
            };

            hkChangeStatus.GetButton().Click += (sender, e) =>
            {
                string currentInvoicePrint = PrintInvoice.GetConstant().Text;
                int printInvoiceState = 0;
                if (currentInvoicePrint.Equals("Yes"))
                {
                    printInvoiceState = 0;
                } else
                {
                    printInvoiceState = 1;
                }
                var refreshData = Session.SubAssemblyModelObj.UpdatePrintInvoice(printInvoiceState, this.skuId);
                if (refreshData)
                {
                    LoadSubAssemblies();
                }
            };

            hkCalculateCot.GetButton().Click += (sender, e) =>
            {
                var refreshData = Session.SubAssemblyModelObj.UpdateTargetSKUCost(this.skuId);
                if (refreshData)
                {
                    LoadSubAssemblies();
                }
            };
        }

        private void LoadSubAssemblies()
        {
            List<SubAssembly> subAssemblyList = Session.SubAssemblyModelObj.LoadSubAssemblies();

            SubAssembliesGridRefer.DataSource = subAssemblyList;
            SubAssembliesGridRefer.Columns[0].HeaderText = "SubAssemblyId";
            SubAssembliesGridRefer.Columns[0].Visible = false;
            SubAssembliesGridRefer.Columns[1].HeaderText = "TargetSKU";
            SubAssembliesGridRefer.Columns[1].Visible = false;
            SubAssembliesGridRefer.Columns[2].HeaderText = "SKU #";
            SubAssembliesGridRefer.Columns[2].Width = 300;
            SubAssembliesGridRefer.Columns[3].HeaderText = "Category";
            SubAssembliesGridRefer.Columns[3].Width = 300;
            SubAssembliesGridRefer.Columns[4].HeaderText = "Description";
            SubAssembliesGridRefer.Columns[4].Width = 500;
            SubAssembliesGridRefer.Columns[5].HeaderText = "Qty";
            SubAssembliesGridRefer.Columns[5].Width = 300;
            SubAssembliesGridRefer.Columns[6].HeaderText = "TargetSkuId";
            SubAssembliesGridRefer.Columns[6].Visible = false;
            SubAssembliesGridRefer.Columns[7].HeaderText = "SubAssemblySkuId";
            SubAssembliesGridRefer.Columns[7].Visible = false;
            SubAssembliesGridRefer.Columns[8].HeaderText = "CategoryId";
            SubAssembliesGridRefer.Columns[8].Visible = false;
            SubAssembliesGridRefer.Columns[9].HeaderText = "Status";
            SubAssembliesGridRefer.Columns[9].Visible = false;
            SubAssembliesGridRefer.Columns[10].HeaderText = "InvoicePrint";
            SubAssembliesGridRefer.Columns[10].Visible = false;

            if(subAssemblyList.Count > 0)
            {
                SubAssembly subAssembly = subAssemblyList[0];
                Status.SetContext("Information Only");
                if (subAssembly.invoicePrint)
                {
                    PrintInvoice.SetContext("Yes");
                } else
                {
                    PrintInvoice.SetContext("No");
                }
            }
        }

        private void updateSubAssembly()
        {
            SubAssemblyDetail detailModal = new SubAssemblyDetail(0, this.readOnly);

            int rowIndex = SubAssembliesGridRefer.CurrentCell.RowIndex;
            DataGridViewRow row = SubAssembliesGridRefer.Rows[rowIndex];
            int id = (int)row.Cells[0].Value;

            SubAssembly subAssembly = Session.SubAssemblyModelObj.GetSubAssemblyId(id);                

            detailModal.setDetails(subAssembly);

            if (detailModal.ShowDialog() == DialogResult.OK)
            {
                LoadSubAssemblies();
            }
        }

    }
}
