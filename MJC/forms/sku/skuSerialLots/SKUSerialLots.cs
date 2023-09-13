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
using MJC.forms.sku.skuSerialLots;

namespace MJC.forms.sku
{
    public partial class SKUSerialLots : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private GridViewOrigin SerialLotGrid = new GridViewOrigin();
        private DataGridView SerialLotGridRefer;

        private int skuId = 0;

        public SKUSerialLots(int skuId) : base("Serial/Lot for SKU#", "Record of cost per quantity of a SKU, used for inventory valuation")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkEdits, hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();
            this.skuId = skuId;

            InitSerialLots();
            if (skuId != 0)
            {
                string skuName = Session.SKUModelObj.GetSkuNameById(skuId);
                this._changeFormText("Serial/Lot for SKU# " + skuName);
            }

            this.VisibleChanged += (s, e) =>
            {
                this.LoadSKUSerialLosts();
            };
        }

        private void InitSerialLots()
        {
            SerialLotGridRefer = SerialLotGrid.GetGrid();
            SerialLotGridRefer.Location = new Point(0, 95);
            SerialLotGridRefer.Width = this.Width;
            SerialLotGridRefer.Height = this.Height - 295;
            SerialLotGridRefer.VirtualMode = true;
            this.Controls.Add(SerialLotGridRefer);
            this.SerialLotGridRefer.CellDoubleClick += (sender, e) =>
            {
                updateSKUSerialLots();
            };
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                SKUSerialLotsDetail detailModal = new SKUSerialLotsDetail(this.skuId);
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadSKUSerialLosts();
                }
            };

            hkEdits.GetButton().Click += (sender, e) =>
            {
                updateSKUSerialLots();
            };

            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int rowIndex = SerialLotGridRefer.CurrentCell.RowIndex;
                    DataGridViewRow row = SerialLotGridRefer.Rows[rowIndex];
                    int skuSerialLotId = (int)row.Cells[0].Value;

                    var refreshData = Session.SKUSerialLotsModelObj.DeleteSKUSerialLots(skuSerialLotId);
                    if (refreshData)
                    {
                        LoadSKUSerialLosts();
                    }
                }
            };
        }

        private void LoadSKUSerialLosts()
        {
            var refreshData = Session.SKUSerialLotsModelObj.LoadSKUSerialLot();

            if (refreshData)
            {
                SerialLotGridRefer.DataSource = Session.SKUSerialLotsModelObj.SKUSerialLotsList;
                SerialLotGridRefer.Columns[0].HeaderText = "id";
                SerialLotGridRefer.Columns[0].Visible = false;
                SerialLotGridRefer.Columns[1].HeaderText = "skuId";
                SerialLotGridRefer.Columns[1].Visible = false;
                SerialLotGridRefer.Columns[2].HeaderText = "Serial Number";
                SerialLotGridRefer.Columns[2].Width = 300;
                SerialLotGridRefer.Columns[3].HeaderText = "Lot Number";
                SerialLotGridRefer.Columns[3].Width = 300;
                SerialLotGridRefer.Columns[4].HeaderText = "Date Received";
                SerialLotGridRefer.Columns[4].Width = 300;
                SerialLotGridRefer.Columns[5].HeaderText = "Cost";
                SerialLotGridRefer.Columns[5].Width = 200;
                SerialLotGridRefer.Columns[6].HeaderText = "Invoice #";
                SerialLotGridRefer.Columns[6].Width = 200;
                SerialLotGridRefer.Columns[7].HeaderText = "Purchased from";
                SerialLotGridRefer.Columns[7].Width = 300;
            }
        }


        private void updateSKUSerialLots()
        {
            SKUSerialLotsDetail detailModal = new SKUSerialLotsDetail(this.skuId);

            int rowIndex = SerialLotGridRefer.CurrentCell.RowIndex;
            DataGridViewRow row = SerialLotGridRefer.Rows[rowIndex];
            int skuSerialLotId = (int)row.Cells[0].Value;

            SKUSerialLot tempSkuSerialLot = new SKUSerialLot();
            foreach (SKUSerialLot skuSerialLot in Session.SKUSerialLotsModelObj.SKUSerialLotsList)
            {
                if (skuSerialLot.id == skuSerialLotId)
                {
                    tempSkuSerialLot = skuSerialLot;
                    break;
                }
            }

            detailModal.setDetails(tempSkuSerialLot);

            if (detailModal.ShowDialog() == DialogResult.OK)
            {
                LoadSKUSerialLosts();
            }
        }

    }
}
