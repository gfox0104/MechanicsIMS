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
using MJC.forms.sales;
using System.Data.SqlClient;

namespace MJC.forms.sku
{
    public partial class SKUList : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkSelects = new HotkeyButton("Enter", "Selects", Keys.Enter);
        private HotkeyButton hkCrossRefLookup = new HotkeyButton("F1", "Cross Ref Lookup", Keys.F1);
        private HotkeyButton hkViewAllocations = new HotkeyButton("F2", "View Allocations", Keys.F2);
        private HotkeyButton hkAdjustQty = new HotkeyButton("F4", "Adjust Qty", Keys.F4);
        private HotkeyButton hkSKUHistory = new HotkeyButton("F5", "SKU History", Keys.F5);
        private HotkeyButton hkProfileHistory = new HotkeyButton("F6", "Profile History", Keys.F6);
        private HotkeyButton hkArchivedSKUs = new HotkeyButton("F8", "Archived SKUs", Keys.F8);

        private GridViewOrigin SKUListGrid = new GridViewOrigin();
        private DataGridView SKUGridRefer;
        private int SKUGridSelectedIndex = 0;

        private string searchKey = "";
        private bool archievedView = false;


        public SKUList(bool ArchivedView = false) : base("SKU List", "List of SKUs")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[9] { hkAdds, hkDeletes, hkSelects, hkCrossRefLookup, hkViewAllocations, hkAdjustQty, hkSKUHistory, hkProfileHistory, hkArchivedSKUs };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitSKUGrid();

            this.VisibleChanged += (ss, sargs) => {
                this.LoadSKUList();
            };

            //this.KeyDown += (s, e) =>
            //{
            //    if (e.KeyCode == Keys.F && e.Control)
            //    {
            //        this.Enabled = false;
            //        SearchInput searchInputModal = new SearchInput();
            //        searchInputModal.SetSearchKey(this.searchKey);
            //        searchInputModal.Show();

            //        searchInputModal.FormClosed += (ss, ee) =>
            //        {
            //            if (this.searchKey != searchInputModal.GetSearchKey())
            //            {
            //                this.searchKey = searchInputModal.GetSearchKey();
            //                this.LoadSKUList(false, false);
            //            }
            //            this.Enabled = true;
            //        };
            //    }
            //};
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                this.Hide();
                SKUInformation detailModal = new SKUInformation();
                _navigateToForm(sender, e, detailModal);
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int selectedSKUId = 0;
                    if (SKUGridRefer.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in SKUGridRefer.SelectedRows)
                        {
                            selectedSKUId = (int)row.Cells[0].Value;
                        }
                    }

                    try
                    {
                        bool refreshData = Session.SKUModelObj.DeleteSKU(selectedSKUId);
                        if (refreshData)
                        {
                            LoadSKUList();
                        }
                    }
                    catch(Exception exc)
                    {
                        Sentry.SentrySdk.CaptureException(exc);
                        if (exc.Message.Contains("REFERENCE"))
                        {
                            ShowError("The SKU cannot be deleted because it is attached to an existing order.");
                        }
                        else
                        {
                            ShowError("There was a problem deleting the SKU.");
                        }
                    }
                }
            };
            hkSelects.GetButton().Click += (sender, e) =>
            {
                try
                {
                    updateSKU(sender, e);
                }
                catch (Exception exc)
                {
                    Sentry.SentrySdk.CaptureException(exc);
                    if (exc.Message.Contains("KEY"))
                    {
                        ShowError("There was a problem updating the SKU.");
                    }
                }
            };
            hkCrossRefLookup.GetButton().Click += (sender, e) =>
            {

                int rowIndex = SKUGridRefer.SelectedRows[0].Index;
                this.SKUGridSelectedIndex = SKUGridRefer.SelectedRows[0].Index;

                DataGridViewRow row = SKUGridRefer.Rows[rowIndex];
                int skuId = (int)row.Cells[0].Value;
                string skuName = row.Cells[1].Value.ToString();

                CrossReference CrossRefModal = new CrossReference(skuId);
                _navigateToForm(sender, e, CrossRefModal);
                this.Hide();
            };
            hkViewAllocations.GetButton().Click += (sender, e) =>
            {
                int rowIndex = SKUGridRefer.SelectedRows[0].Index;
                this.SKUGridSelectedIndex = rowIndex;

                DataGridViewRow row = SKUGridRefer.Rows[rowIndex];
                int skuId = (int)row.Cells[0].Value;
                string skuName = row.Cells[1].Value.ToString();

                Allocations AllocationsModal = new Allocations(skuId, skuName);
                _navigateToForm(sender, e, AllocationsModal);
                this.Hide();
            };

            hkAdjustQty.GetButton().Click += (sender, e) =>
            {
                int rowIndex = SKUGridRefer.SelectedRows[0].Index;
                this.SKUGridSelectedIndex = rowIndex;

                DataGridViewRow row = SKUGridRefer.Rows[rowIndex];
                int skuId = (int)row.Cells[0].Value;
                string skuName = row.Cells[1].Value.ToString();

                this.Enabled = false;
                AdjustQty AdjustQtyModal = new AdjustQty(skuId, skuName);
                AdjustQtyModal.Show();

                AdjustQtyModal.FormClosed += (ss, sargs) =>
                {
                    this.Enabled = true;
                    this.LoadSKUList();
                };
            };

            hkSKUHistory.GetButton().Click += (sender, e) =>
            {
                this.SKUGridSelectedIndex = SKUGridRefer.SelectedRows[0].Index;
                DataGridViewRow row = SKUGridRefer.Rows[this.SKUGridSelectedIndex];
                int skuId = (int)row.Cells[0].Value;
                string skuName = row.Cells[1].Value.ToString();

                SalesHisotry SalesHistoryModal = new SalesHisotry(skuId);
                _navigateToForm(sender, e, SalesHistoryModal);
                this.Hide();
            };
            hkProfileHistory.GetButton().Click += (sender, e) =>
            {
                this.SKUGridSelectedIndex = SKUGridRefer.SelectedRows[0].Index;
                DataGridViewRow row = SKUGridRefer.Rows[this.SKUGridSelectedIndex];
                int skuId = (int)row.Cells[0].Value;
                string skuNumber = row.Cells[1].Value.ToString();

                SKUProfile SKUProfileModal = new SKUProfile(skuId, skuNumber);
                _navigateToForm(sender, e, SKUProfileModal);
                this.Hide();
            };
            hkArchivedSKUs.GetButton().Click += (sender, e) => {

                if (!archievedView)
                {
                    hkCrossRefLookup.GetButton().Hide();
                    hkCrossRefLookup.GetLabel().Hide();

                    hkViewAllocations.GetButton().Hide();
                    hkViewAllocations.GetLabel().Hide();

                    hkAdjustQty.GetButton().Hide();
                    hkAdjustQty.GetLabel().Hide();

                    hkArchivedSKUs.GetLabel().Text = "Active SKUs";

                    archievedView = true;
                    this._changeFormText("SKU List");
                }
                else
                {
                    hkCrossRefLookup.GetButton().Show();
                    hkCrossRefLookup.GetLabel().Show();

                    hkViewAllocations.GetButton().Show();
                    hkViewAllocations.GetLabel().Show();

                    hkAdjustQty.GetButton().Show();
                    hkAdjustQty.GetLabel().Show();

                    hkArchivedSKUs.GetLabel().Text = "Archived SKUs";

                    archievedView = false;
                    this._changeFormText("ARCHIVED - SKU List");
                }

                LoadSKUList(archievedView, false);
            };
        }

        private void InitSKUGrid()
        {
            SKUGridRefer = SKUListGrid.GetGrid();
            SKUGridRefer.Location = new Point(0, 95);
            SKUGridRefer.Width = this.Width - 20;
            SKUGridRefer.Height = this.Height - 330;
            SKUGridRefer.VirtualMode = true;
            this.Controls.Add(SKUGridRefer);
            this.SKUGridRefer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SKUGridView_CellDoubleClick);
            this.SKUGridRefer.SelectionChanged += (s, e) => SKUGridRefer_SelectionChanged(s, e);
            SKUGridRefer.KeyPress += (s, e) =>
            {
                if (char.IsLetter(e.KeyChar) || char.IsNumber(e.KeyChar))
                {
                    e.Handled = true;
                    this.Enabled = false;
                    SearchInput searchInputModal = new SearchInput();
                    searchInputModal.SetSearchKey(e.KeyChar.ToString());
                    searchInputModal.SearchKeyInput.GetTextBox().Select(1, 0);
                    searchInputModal.Show();

                    searchInputModal.FormClosed += (ss, ee) =>
                    {
                        if (this.searchKey != searchInputModal.GetSearchKey())
                        {
                            this.searchKey = searchInputModal.GetSearchKey();
                            this.LoadSKUList(false, false);

                            int skuCount = Session.SKUModelObj.SKUDataList.Count;
                            SKUDetail exactSkuDetail = new SKUDetail();
                            int exactSkuDetailIndex = 0;
                            foreach(SKUDetail skuDetail in Session.SKUModelObj.SKUDataList)
                            {
                                bool isExactMatch = false;
                                if (skuDetail.Name.Equals(this.searchKey, StringComparison.OrdinalIgnoreCase))
                                {
                                    isExactMatch = true;
                                    exactSkuDetail = skuDetail;
                                }
                                if (skuDetail.Category.Equals(this.searchKey, StringComparison.OrdinalIgnoreCase))
                                {
                                    isExactMatch = true;
                                    exactSkuDetail = skuDetail;
                                }
                                if (skuDetail.Description.Equals(this.searchKey, StringComparison.OrdinalIgnoreCase))
                                {
                                    isExactMatch = true;
                                    exactSkuDetail = skuDetail;
                                }
                                if (isExactMatch)
                                {
                                    exactSkuDetailIndex += 1;
                                }
                            }
                            
                            if (exactSkuDetailIndex == 1)
                            {
                                SKUInformation detailModal = new SKUInformation();

                                List<dynamic> skuData = new List<dynamic>();
                                skuData = Session.SKUModelObj.GetSKUData(exactSkuDetail.Id);
                                detailModal.setDetails(skuData, skuData[0].id);

                                this.Hide();
                                _navigateToForm(s, e, detailModal);
                            }
                        }
                        this.Enabled = true;
                    };
                }
            };
        }

        public void LoadSKUList(bool archivedView = false, bool keepSelection = true)
        {
            if (this.searchKey == "")
            {
                this._changeFormText("SKU List");
            }
            else
            {
                this._changeFormText("SKU List searched by " + this.searchKey);
            }
            var refreshData = Session.SKUModelObj.LoadSKUData(this.searchKey, archivedView);
            if (refreshData)
            {
                SKUGridRefer.DataSource = Session.SKUModelObj.SKUDataList;
                SKUGridRefer.Columns[0].Visible = false;
                SKUGridRefer.Columns[1].HeaderText = "SKU#";
                SKUGridRefer.Columns[1].Width = 300;
                SKUGridRefer.Columns[2].HeaderText = "Category";
                SKUGridRefer.Columns[2].Width = 300;
                SKUGridRefer.Columns[3].HeaderText = "Description";
                SKUGridRefer.Columns[3].Width = 500;
                SKUGridRefer.Columns[4].HeaderText = "Quantity";
                SKUGridRefer.Columns[4].Width = 300;
                SKUGridRefer.Columns[5].HeaderText = "Available";
                SKUGridRefer.Columns[5].Width = 300;
                SKUGridRefer.Columns[6].HeaderText = "On Hand";
                SKUGridRefer.Columns[6].Visible = false;

            }

            if (keepSelection)
            {
                SKUGridRefer.ClearSelection();
                if (SKUGridSelectedIndex >= 0 && SKUGridSelectedIndex < SKUGridRefer.Rows.Count)
                {
                    SKUGridRefer.Rows[SKUGridSelectedIndex].Selected = true;
                    SKUGridRefer.CurrentCell = SKUGridRefer[1, SKUGridSelectedIndex];
                }
            }
        }

        private void updateSKU(object sender, EventArgs e)
        {
            SKUInformation detailModal = new SKUInformation();

            int rowIndex = SKUGridRefer.SelectedRows[0].Index;

            this.SKUGridSelectedIndex = rowIndex;

            DataGridViewRow row = SKUGridRefer.Rows[rowIndex];
            int skuId = (int)row.Cells[0].Value;

            List<dynamic> skuData = new List<dynamic>();
            skuData = Session.SKUModelObj.GetSKUData(skuId);
            detailModal.setDetails(skuData, skuData[0].id);

            this.Hide();
            _navigateToForm(sender, e, detailModal);
        }

        private void SKUGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            updateSKU(sender, e);
        }

        private void SKUGridRefer_SelectionChanged(object sender, EventArgs e)
        {

            if (SKUGridRefer.SelectedRows.Count > 0)
            {

            }
        }
    }
}
