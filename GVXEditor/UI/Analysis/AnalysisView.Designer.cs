namespace GVXEditor.UI.Analysis
{
    partial class AnalysisView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalysisView));
            this.dgvFindings = new System.Windows.Forms.DataGridView();
            this.cSeverity = new System.Windows.Forms.DataGridViewImageColumn();
            this.cPlace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsFindings = new System.Windows.Forms.ToolStrip();
            this.tsbErrors = new ToolsCore.Icons.IconToolStripButton();
            this.tsbWarnings = new ToolsCore.Icons.IconToolStripButton();
            this.tsbInfos = new ToolsCore.Icons.IconToolStripButton();
            this.tssFindings1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRefresh = new ToolsCore.Icons.IconToolStripButton();
            this.tsbVerify = new ToolsCore.Icons.IconToolStripButton();
            this.tslStatus = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFindings)).BeginInit();
            this.tsFindings.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvFindings
            // 
            this.dgvFindings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cSeverity,
            this.cPlace,
            this.cMessage});
            resources.ApplyResources(this.dgvFindings, "dgvFindings");
            this.dgvFindings.AllowUserToAddRows = false;
            this.dgvFindings.AllowUserToDeleteRows = false;
            this.dgvFindings.AllowUserToResizeRows = false;
            this.dgvFindings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFindings.MultiSelect = false;
            this.dgvFindings.ReadOnly = true;
            this.dgvFindings.RowHeadersVisible = false;
            this.dgvFindings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFindings.Name = "dgvFindings";
            this.dgvFindings.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFindings_CellDoubleClick);
            this.dgvFindings.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvFindings_CellFormatting);
            this.dgvFindings.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvFindings_KeyDown);
            // 
            // tsFindings
            // 
            this.tsFindings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbErrors,
            this.tsbWarnings,
            this.tsbInfos,
            this.tssFindings1,
            this.tsbRefresh,
            this.tsbVerify,
            this.tslStatus});
            resources.ApplyResources(this.tsFindings, "tsFindings");
            this.tsFindings.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsFindings.ImageScalingSize = new System.Drawing.Size(16, 16);
            this.tsFindings.Padding = new System.Windows.Forms.Padding(3, 0, 1, 0);
            this.tsFindings.Name = "tsFindings";
            // 
            // tsbErrors
            // 
            resources.ApplyResources(this.tsbErrors, "tsbErrors");
            this.tsbErrors.Checked = true;
            this.tsbErrors.CheckOnClick = true;
            this.tsbErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbErrors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbErrors.IconName = "status-error";
            this.tsbErrors.Name = "tsbErrors";
            this.tsbErrors.CheckedChanged += new System.EventHandler(this.Filter_CheckedChanged);
            // 
            // tsbWarnings
            // 
            resources.ApplyResources(this.tsbWarnings, "tsbWarnings");
            this.tsbWarnings.Checked = true;
            this.tsbWarnings.CheckOnClick = true;
            this.tsbWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbWarnings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbWarnings.IconName = "status-warning";
            this.tsbWarnings.Name = "tsbWarnings";
            this.tsbWarnings.CheckedChanged += new System.EventHandler(this.Filter_CheckedChanged);
            // 
            // tsbInfos
            // 
            resources.ApplyResources(this.tsbInfos, "tsbInfos");
            this.tsbInfos.Checked = true;
            this.tsbInfos.CheckOnClick = true;
            this.tsbInfos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbInfos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbInfos.IconName = "status-info";
            this.tsbInfos.Name = "tsbInfos";
            this.tsbInfos.CheckedChanged += new System.EventHandler(this.Filter_CheckedChanged);
            // 
            // tssFindings1
            // 
            resources.ApplyResources(this.tssFindings1, "tssFindings1");
            this.tssFindings1.Name = "tssFindings1";
            // 
            // tsbRefresh
            // 
            resources.ApplyResources(this.tsbRefresh, "tsbRefresh");
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbRefresh.IconName = "rerun";
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);
            // 
            // tsbVerify
            // 
            resources.ApplyResources(this.tsbVerify, "tsbVerify");
            this.tsbVerify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbVerify.IconName = "document-checkmark";
            this.tsbVerify.Name = "tsbVerify";
            this.tsbVerify.Click += new System.EventHandler(this.tsbVerify_Click);
            // 
            // tslStatus
            // 
            resources.ApplyResources(this.tslStatus, "tslStatus");
            this.tslStatus.Name = "tslStatus";
            // 
            // AnalysisView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvFindings);
            this.Controls.Add(this.tsFindings);
            this.Name = "AnalysisView";
            resources.ApplyResources(this, "$this");
            // 
            // cSeverity
            // 
            resources.ApplyResources(this.cSeverity, "cSeverity");
            this.cSeverity.ReadOnly = true;
            this.cSeverity.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Normal;
            this.cSeverity.Name = "cSeverity";
            // 
            // cPlace
            // 
            resources.ApplyResources(this.cPlace, "cPlace");
            this.cPlace.DataPropertyName = "Place";
            this.cPlace.ReadOnly = true;
            this.cPlace.Name = "cPlace";
            // 
            // cMessage
            // 
            resources.ApplyResources(this.cMessage, "cMessage");
            this.cMessage.DataPropertyName = "Message";
            this.cMessage.ReadOnly = true;
            this.cMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cMessage.Name = "cMessage";
            this.tsFindings.ResumeLayout(false);
            this.tsFindings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFindings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFindings;
        private System.Windows.Forms.DataGridViewImageColumn cSeverity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPlace;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMessage;
        private System.Windows.Forms.ToolStrip tsFindings;
        private ToolsCore.Icons.IconToolStripButton tsbErrors;
        private ToolsCore.Icons.IconToolStripButton tsbWarnings;
        private ToolsCore.Icons.IconToolStripButton tsbInfos;
        private System.Windows.Forms.ToolStripSeparator tssFindings1;
        private ToolsCore.Icons.IconToolStripButton tsbRefresh;
        private ToolsCore.Icons.IconToolStripButton tsbVerify;
        private System.Windows.Forms.ToolStripLabel tslStatus;
    }
}
