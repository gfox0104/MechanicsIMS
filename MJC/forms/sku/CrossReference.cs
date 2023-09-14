﻿using MJC.common.components;
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

namespace MJC.forms.sku
{
    public partial class CrossReference : GlobalLayout
    {

        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkSelects = new HotkeyButton("Enter", "Selects", Keys.Enter);
        private HotkeyButton hkPrevScreen = new HotkeyButton("ESC", "Previous screen", Keys.Escape);

        private GridViewOrigin crossRefGrid = new GridViewOrigin();
        private DataGridView CRGridRefer;
        private SKUCrossRefModel SUKCrossRefMModalObj = new SKUCrossRefModel();
        private SKUModel SKUModelObj = new SKUModel();
        private int SKUId;
        private bool readOnly = false;

        public CrossReference(int _skuId = 0, bool readOnly = false) : base("Cross References", "List all serials that SKUs may also be searched by based on")
        {
            InitializeComponent();
            _initBasicSize();

            this.readOnly = readOnly;
            this.SKUId = _skuId;
            if(_skuId != 0)
            {
                string skuName = SKUModelObj.GetSkuNameById(_skuId);
                this._changeFormText("Cross References for SKU# " + skuName);
            }

            HotkeyButton[] hkButtons;
            if (readOnly)
            {
                hkButtons = new HotkeyButton[2] { hkSelects, hkPrevScreen };
            } else
            {
                hkButtons = new HotkeyButton[4] { hkAdds, hkDeletes, hkSelects, hkPrevScreen };
            }
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitCrossRefGrid();
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                CrossRefDetail CrossRefDetailModal = new CrossRefDetail(this.SKUId);

                this.Enabled = false;
                CrossRefDetailModal.Show();
                CrossRefDetailModal.FormClosed += (ss, sargs) =>
                {
                    this.Enabled = true;
                    this.LoadCategoryList();
                };

            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No) return;

                int selectedId = 0;
                if (CRGridRefer.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in CRGridRefer.SelectedRows)
                    {
                        selectedId = (int)row.Cells[0].Value;
                    }
                }

                bool refreshData = SUKCrossRefMModalObj.DeleteSKUCrossRef(selectedId);
                if (refreshData)
                {
                    MessageBox.Show("The CrossRef was deleted.");
                    InitCrossRefGrid();
                }
            };
            hkSelects.GetButton().Click += (sender, e) =>
            {
                UpdateCrossRef();
            };
        }

        private void InitCrossRefGrid()
        {
            CRGridRefer = crossRefGrid.GetGrid();
            CRGridRefer.Location = new Point(0, 95);
            CRGridRefer.Width = this.Width;
            CRGridRefer.Height = this.Height - 295;
            CRGridRefer.VirtualMode = true;
            this.Controls.Add(CRGridRefer);
            this.CRGridRefer.CellDoubleClick += (s, e) =>
            {
                UpdateCrossRef();
            };

            LoadCategoryList();
        }

        private void LoadCategoryList()
        {
            var refreshData = SUKCrossRefMModalObj.LoadSKUCrossRefData(SKUId);
            if (refreshData)
            {
                CRGridRefer.DataSource = SUKCrossRefMModalObj.SKUCrossRefList;
                CRGridRefer.Columns[0].Visible = false;
                CRGridRefer.Columns[1].HeaderText = "Cross-Reference";
                CRGridRefer.Columns[1].Width = 300;
                CRGridRefer.Columns[2].HeaderText = "Manufacturer";
                CRGridRefer.Columns[2].Width = 300;
                CRGridRefer.Columns[3].HeaderText = "SKU#";
                CRGridRefer.Columns[3].Width = 300;
                CRGridRefer.Columns[4].HeaderText = "Description";
                CRGridRefer.Columns[4].Width = 500;
            }
            return;
        }

        private void UpdateCrossRef()
        {

            int CRId = (int)CRGridRefer.SelectedRows[0].Cells[0].Value;
            CrossRefDetail CrossRefDetailModal = new CrossRefDetail(this.SKUId, CRId, this.readOnly);

            if (CrossRefDetailModal.ShowDialog() == DialogResult.OK)
            {
                LoadCategoryList();
            }
        }
    }
}
