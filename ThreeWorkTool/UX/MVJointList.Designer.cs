namespace ThreeWorkTool.UX
{
    partial class MVJointList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlJointListTop = new System.Windows.Forms.Panel();
            this.lblJointList = new System.Windows.Forms.Label();
            this.listJointList = new System.Windows.Forms.ListBox();
            this.pnlList = new System.Windows.Forms.Panel();
            this.pnlJointListTop.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlJointListTop
            // 
            this.pnlJointListTop.Controls.Add(this.lblJointList);
            this.pnlJointListTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlJointListTop.Location = new System.Drawing.Point(0, 0);
            this.pnlJointListTop.Name = "pnlJointListTop";
            this.pnlJointListTop.Size = new System.Drawing.Size(120, 20);
            this.pnlJointListTop.TabIndex = 19;
            // 
            // lblJointList
            // 
            this.lblJointList.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblJointList.AutoSize = true;
            this.lblJointList.Location = new System.Drawing.Point(33, 4);
            this.lblJointList.Name = "lblJointList";
            this.lblJointList.Size = new System.Drawing.Size(48, 13);
            this.lblJointList.TabIndex = 0;
            this.lblJointList.Text = "Joint List";
            this.lblJointList.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listJointList
            // 
            this.listJointList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.listJointList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listJointList.ForeColor = System.Drawing.SystemColors.Window;
            this.listJointList.HorizontalScrollbar = true;
            this.listJointList.Location = new System.Drawing.Point(0, 0);
            this.listJointList.Name = "listJointList";
            this.listJointList.Size = new System.Drawing.Size(120, 300);
            this.listJointList.TabIndex = 20;
            // 
            // pnlList
            // 
            this.pnlList.Controls.Add(this.listJointList);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlList.Location = new System.Drawing.Point(0, 20);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(120, 300);
            this.pnlList.TabIndex = 21;
            // 
            // MVJointList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.pnlJointListTop);
            this.Name = "MVJointList";
            this.Size = new System.Drawing.Size(120, 320);
            this.pnlJointListTop.ResumeLayout(false);
            this.pnlJointListTop.PerformLayout();
            this.pnlList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlJointListTop;
        private System.Windows.Forms.Label lblJointList;
        public System.Windows.Forms.ListBox listJointList;
        private System.Windows.Forms.Panel pnlList;
    }
}
