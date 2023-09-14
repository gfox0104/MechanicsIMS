using Microsoft.Web.WebView2.Core;
using MJC.common;
using MJC.common.components;
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
    public partial class VendorCosts : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);
            
        private GridViewOrigin VendorCostsGrid = new GridViewOrigin();
        private DataGridView VGridRefer;


        private int skuId = 0;

        public VendorCosts(int skuId) : base("Vendor Costs for SKU#", "Record of different vendors to order SKU from and individual costs")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();
            this.skuId = skuId;

            InitVendorCosts();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadVendorCosts();
            };
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                VendorCostsDetail detailModal = new VendorCostsDetail(this.skuId);
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadVendorCosts();
                }
            };

            hkEdits.GetButton().Click += (sender, e) =>
            {
                VendorCostsDetail detailModal = new VendorCostsDetail(this.skuId);
                
                int rowIndex = VGridRefer.CurrentCell.RowIndex;
                DataGridViewRow row = VGridRefer.Rows[rowIndex];
                int venderCostId = (int)row.Cells[0].Value;

                VendorCost vendorCost = new VendorCost();
                foreach (VendorCost vendor in Session.VendorCostModelObj.VendorCostList)
                {
                    if(vendor.id == venderCostId)
                    {
                        vendorCost = vendor;
                        break;
                    }
                }

                detailModal.setDetails(vendorCost);

                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadVendorCosts();
                }
            };

            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {

                    VendorCostsDetail detailModal = new VendorCostsDetail(this.skuId);

                    int rowIndex = VGridRefer.CurrentCell.RowIndex;
                    DataGridViewRow row = VGridRefer.Rows[rowIndex];
                    int venderCostId = (int)row.Cells[0].Value;

                    var refreshData = Session.VendorCostModelObj.DeleteVendorCost(venderCostId);
                    if (refreshData)
                    {
                        LoadVendorCosts();
                    }
                }

            };
        }

        private void InitVendorCosts()
        {
            VGridRefer = VendorCostsGrid.GetGrid();
            VGridRefer.Location = new Point(0, 95);
            VGridRefer.Width = this.Width;
            VGridRefer.Height = this.Height - 295;
            VGridRefer.VirtualMode = true;
            this.Controls.Add(VGridRefer);
            this.VGridRefer.CellDoubleClick += (sender, e) =>
            {
                //updateVendor();
            };
        }


        private void LoadVendorCosts()
        {
            var refreshData = Session.VendorCostModelObj.LoadVendorCosts();
            if (refreshData)
            {
                VGridRefer.DataSource = Session.VendorCostModelObj.VendorCostList;
                VGridRefer.Columns[0].HeaderText = "id";
                VGridRefer.Columns[0].Visible = false;
                VGridRefer.Columns[1].HeaderText = "skuId";
                VGridRefer.Columns[1].Visible = false;
                VGridRefer.Columns[2].HeaderText = "vendorId";
                VGridRefer.Columns[2].Visible = false;
                VGridRefer.Columns[3].HeaderText = "Vendor";
                VGridRefer.Columns[3].Width = 300;
                VGridRefer.Columns[4].HeaderText = "Manuf";
                VGridRefer.Columns[4].Width = 300;
                VGridRefer.Columns[5].HeaderText = "Memo";
                VGridRefer.Columns[5].Width = 300;
                VGridRefer.Columns[6].HeaderText = "packageQty";
                VGridRefer.Columns[6].Width = 200;
                VGridRefer.Columns[7].HeaderText = "Cost";
                VGridRefer.Columns[7].Width = 200;
                VGridRefer.Columns[8].HeaderText = "Core";
                VGridRefer.Columns[8].Width = 200;
            }
        }
    }
}
