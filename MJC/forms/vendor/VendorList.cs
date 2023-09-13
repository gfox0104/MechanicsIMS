using MJC.common.components;
using MJC.common;
using MJC.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MJC.forms.vendor
{
    public partial class VendorList : GlobalLayout
    {

        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);
        private HotkeyButton hkArchivedVendors = new HotkeyButton("F8", "Archived Vendors", Keys.F8);

        private GridViewOrigin VendorListGrid = new GridViewOrigin();
        private DataGridView VGridRefer;

        private bool archievedView = false;

        public VendorList() : base("Vendor List", "List of vendors")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[5] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen, hkArchivedVendors };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitVendorList();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadVendorList();
            };
        }
        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                VendorDetail detailModal = new VendorDetail();
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadVendorList(false);
                }
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int selectedVendorId = 0;
                    if (VGridRefer.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in VGridRefer.SelectedRows)
                        {
                            selectedVendorId = (int)row.Cells[0].Value;
                        }
                    }
                    bool refreshData = Session.VendorsModelObj.DeleteVendor(selectedVendorId);
                    if (refreshData)
                    {
                        LoadVendorList();
                    }
                }
            };
            hkEdits.GetButton().Click += (sender, e) =>
            {
                updateVendor();
            };
            hkArchivedVendors.GetButton().Click += (sender, e) =>
            {
                if (!archievedView)
                {
                    hkArchivedVendors.GetLabel().Text = "Active Vendors";

                    archievedView = true;
                    this._changeFormText("Vendor List");
                }
                else
                {
                    hkArchivedVendors.GetLabel().Text = "Archived Vendors";

                    archievedView = false;
                    this._changeFormText("ARCHIVED - Vendor List");
                }
                LoadVendorList(archievedView);
            };
        }

        private void InitVendorList()
        {
            VGridRefer = VendorListGrid.GetGrid();
            VGridRefer.Location = new Point(0, 95);
            VGridRefer.Width = this.Width;
            VGridRefer.Height = this.Height - 295;
            this.Controls.Add(VGridRefer);
            this.VGridRefer.CellDoubleClick += (sender, e) =>
            {
                updateVendor();
            };
        }

        private void LoadVendorList(bool archivedView = false)
        {
            string filter = "";
            var refreshData = Session.VendorsModelObj.LoadVendorData(filter, archievedView);
            if (refreshData)
            {
                VGridRefer.DataSource = Session.VendorsModelObj.VendorDataList;
                VGridRefer.Columns[0].HeaderText = "ID";
                VGridRefer.Columns[0].Visible = false;

                VGridRefer.Columns[1].HeaderText = "Vendor #";
                VGridRefer.Columns[1].Width = 200;
                VGridRefer.Columns[1].DataPropertyName = "number";

                VGridRefer.Columns[2].HeaderText = "Name";
                VGridRefer.Columns[2].Width = 300;
                VGridRefer.Columns[2].DataPropertyName = "name";

                VGridRefer.Columns[3].HeaderText = "City";
                VGridRefer.Columns[3].Width = 500;
                VGridRefer.Columns[4].HeaderText = "State";
                VGridRefer.Columns[4].Width = 200;
                VGridRefer.Columns[5].HeaderText = "Phone";
                VGridRefer.Columns[5].Width = 200;
            }
        }

        private void updateVendor()
        {
            VendorDetail detailModal = new VendorDetail();

            int rowIndex = VGridRefer.CurrentCell.RowIndex;

            DataGridViewRow row = VGridRefer.Rows[rowIndex];

            int vendorId = (int)row.Cells[0].Value;
            List<dynamic> vendorData = new List<dynamic>();
            vendorData = Session.VendorsModelObj.GetVendorData(vendorId);
            detailModal.setDetails(vendorData, vendorData[0].id);

            if (detailModal.ShowDialog() == DialogResult.OK)
            {
                LoadVendorList();
            }
        }
    }
}
