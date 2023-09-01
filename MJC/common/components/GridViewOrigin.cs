﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJC.common.components
{
    public class GridViewOrigin
    {
        private DataGridView dataGridView = new DataGridView();

        public GridViewOrigin()
        {
            dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(0, 100);
            dataGridView.BackgroundColor = Color.FromArgb(215, 231, 246);
            dataGridView.ReadOnly = true;

            dataGridView.RowTemplate.Height = 50;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(253, 255, 255);
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(38, 77, 118);
            dataGridView.ColumnHeadersDefaultCellStyle.Padding = new Padding(19);
            dataGridView.BorderStyle = BorderStyle.None;

            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView.RowsDefaultCellStyle.Font = new Font("Segoe UI", 18F, FontStyle.Regular);
            dataGridView.RowsDefaultCellStyle.ForeColor = Color.FromArgb(31, 63, 96);
            dataGridView.RowsDefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
            dataGridView.RowsDefaultCellStyle.BackColor = Color.FromArgb(201, 223, 243);
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(231, 240, 249);
            dataGridView.RowHeadersVisible = false;

            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 227, 130);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(58, 65, 73);
            dataGridView.ScrollBars = ScrollBars.Both;


            //dataGridView.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom);
            dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        }

        public DataGridView GetGrid()
        {
            return dataGridView;
        }
    }
}
