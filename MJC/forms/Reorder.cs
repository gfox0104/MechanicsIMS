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

namespace MJC.forms
{
    public partial class Reorder : GlobalLayout
    {

        private HotkeyButton hkClosesReport = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);
        private HotkeyButton hkPrint = new HotkeyButton("F9", "Print", Keys.F9);

        private GridViewOrigin recorderGrid = new GridViewOrigin();
        private DataGridView RecorderRefer;

        public Reorder() : base("Reorder - SKU List", "SKUs that are below critical quantity and need replenished")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkClosesReport, hkPrint };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitRecorderList();
        }

        private void AddHotKeyEvents()
        {
            hkClosesReport.GetButton().Click += (sender, e) =>
            {
            };
        }
        private void InitRecorderList()
        {
            RecorderRefer = recorderGrid.GetGrid();
            RecorderRefer.Location = new Point(0, 95);
            RecorderRefer.Width = this.Width;
            RecorderRefer.Height = this.Height - 295;
            this.Controls.Add(RecorderRefer);
            //this.RecorderRefer.CellDoubleClick += (sender, e) =>
            //{
            //    updateSKU(sender, e);
            //};

            LoadRecorderList();
        }

        private void LoadRecorderList()
        {
            List<SKURecorder> RecorderList = Session.SKUModelObj.LoadSKURecorderList();

            RecorderRefer.DataSource = RecorderList;
            RecorderRefer.Columns[0].Visible = false;
            RecorderRefer.Columns[1].HeaderText = "SKU#";
            RecorderRefer.Columns[1].Width = 200;
            RecorderRefer.Columns[2].HeaderText = "Description";
            RecorderRefer.Columns[2].Width = 400;
            RecorderRefer.Columns[3].HeaderText = "Qty On Hand";
            RecorderRefer.Columns[3].Width = 300;
            RecorderRefer.Columns[4].HeaderText = "Qty Available";
            RecorderRefer.Columns[4].Width = 300;
            RecorderRefer.Columns[5].HeaderText = "Critical Qty";
            RecorderRefer.Columns[5].Width = 300;
            RecorderRefer.Columns[6].HeaderText = "Reorder Qty";
            RecorderRefer.Columns[6].Width = 300;
            RecorderRefer.Columns[7].Visible = false;
        }
    }
}
