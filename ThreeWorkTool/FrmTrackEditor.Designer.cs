namespace ThreeWorkTool
{
    partial class FrmTrackEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TrackGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.TrackGridView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1275, 607);
            this.panel2.TabIndex = 1;
            // 
            // TrackGridView
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.TrackGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.TrackGridView.BackgroundColor = System.Drawing.SystemColors.GrayText;
            this.TrackGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TrackGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TrackGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.TrackGridView.ColumnHeadersHeight = 30;
            this.TrackGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column9,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.InfoText;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.TrackGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.TrackGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrackGridView.Location = new System.Drawing.Point(0, 0);
            this.TrackGridView.Name = "TrackGridView";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.AppWorkspace;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TrackGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.TrackGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackGridView.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.TrackGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.TrackGridView.Size = new System.Drawing.Size(1275, 607);
            this.TrackGridView.TabIndex = 0;
            this.TrackGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.TrackGridView_CellValidating);
            // 
            // Column1
            // 
            this.Column1.FillWeight = 56.33803F;
            this.Column1.HeaderText = "Track#";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 80;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "BufferType";
            this.Column9.Name = "Column9";
            this.Column9.Width = 95;
            // 
            // Column2
            // 
            this.Column2.FillWeight = 59.3503F;
            this.Column2.HeaderText = "BoneID";
            this.Column2.Name = "Column2";
            this.Column2.Width = 84;
            // 
            // Column3
            // 
            this.Column3.FillWeight = 100.9945F;
            this.Column3.HeaderText = "ReferenceType";
            this.Column3.Name = "Column3";
            this.Column3.Width = 144;
            // 
            // Column4
            // 
            this.Column4.FillWeight = 116.2835F;
            this.Column4.HeaderText = "RefrenceDataW";
            this.Column4.Name = "Column4";
            this.Column4.Width = 165;
            // 
            // Column5
            // 
            this.Column5.FillWeight = 114.0162F;
            this.Column5.HeaderText = "RefrenceDataX";
            this.Column5.Name = "Column5";
            this.Column5.Width = 162;
            // 
            // Column6
            // 
            this.Column6.FillWeight = 112.0482F;
            this.Column6.HeaderText = "RefrenceDataY";
            this.Column6.Name = "Column6";
            this.Column6.Width = 159;
            // 
            // Column7
            // 
            this.Column7.FillWeight = 110.3401F;
            this.Column7.HeaderText = "RefrenceDataZ";
            this.Column7.Name = "Column7";
            this.Column7.Width = 157;
            // 
            // Column8
            // 
            this.Column8.FillWeight = 130.6291F;
            this.Column8.HeaderText = "ReferenceDataOffset";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 185;
            // 
            // FrmTrackEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1275, 607);
            this.Controls.Add(this.panel2);
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Name = "FrmTrackEditor";
            this.Text = "FrmTrackEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTrackEditor_FormClosing);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrackGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView TrackGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
    }
}