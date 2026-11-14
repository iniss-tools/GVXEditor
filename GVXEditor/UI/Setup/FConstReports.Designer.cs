namespace GVXEditor.UI.Setup
{
    partial class FConstReports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FConstReports));
            this.pReports = new System.Windows.Forms.Panel();
            this.dgvReports = new System.Windows.Forms.DataGridView();
            this.cRepKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepFunction = new ExControls.DataGridViewExComboBoxColumn();
            this.cRepStations = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsReports = new System.Windows.Forms.ToolStrip();
            this.tsbReportsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbReportsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssReports1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbReportsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbReportsDown = new ToolsCore.Icons.IconToolStripButton();
            this.lReports = new System.Windows.Forms.Label();
            this.lbProblems = new GVXEditor.UI.Controls.ProblemList();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.pReports.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            this.tsReports.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pReports
            // 
            this.pReports.Controls.Add(this.dgvReports);
            this.pReports.Controls.Add(this.tsReports);
            this.pReports.Controls.Add(this.lReports);
            resources.ApplyResources(this.pReports, "pReports");
            this.pReports.Name = "pReports";
            // 
            // dgvReports
            // 
            this.dgvReports.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cRepKey,
            this.cRepName,
            this.cRepFunction,
            this.cRepStations});
            resources.ApplyResources(this.dgvReports, "dgvReports");
            this.dgvReports.AllowUserToAddRows = false;
            this.dgvReports.AllowUserToDeleteRows = false;
            this.dgvReports.AllowUserToResizeRows = false;
            this.dgvReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReports.MultiSelect = false;
            this.dgvReports.RowHeadersVisible = false;
            this.dgvReports.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.List_CellValueChanged);
            this.dgvReports.CurrentCellDirtyStateChanged += new System.EventHandler(this.List_CurrentCellDirtyStateChanged);
            this.dgvReports.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.List_DataError);
            this.dgvReports.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.List_CellFormatting);
            // 
            // tsReports
            // 
            this.tsReports.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbReportsAdd,
            this.tsbReportsDelete,
            this.tssReports1,
            this.tsbReportsUp,
            this.tsbReportsDown});
            resources.ApplyResources(this.tsReports, "tsReports");
            this.tsReports.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsReports.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsReports.Name = "tsReports";
            // 
            // tsbReportsAdd
            // 
            resources.ApplyResources(this.tsbReportsAdd, "tsbReportsAdd");
            this.tsbReportsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReportsAdd.IconName = "add";
            this.tsbReportsAdd.Name = "tsbReportsAdd";
            this.tsbReportsAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbReportsDelete
            // 
            resources.ApplyResources(this.tsbReportsDelete, "tsbReportsDelete");
            this.tsbReportsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReportsDelete.IconName = "delete";
            this.tsbReportsDelete.Name = "tsbReportsDelete";
            this.tsbReportsDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // tssReports1
            // 
            resources.ApplyResources(this.tssReports1, "tssReports1");
            this.tssReports1.Name = "tssReports1";
            // 
            // tsbReportsUp
            // 
            resources.ApplyResources(this.tsbReportsUp, "tsbReportsUp");
            this.tsbReportsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReportsUp.IconName = "arrow-up";
            this.tsbReportsUp.Name = "tsbReportsUp";
            this.tsbReportsUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbReportsDown
            // 
            resources.ApplyResources(this.tsbReportsDown, "tsbReportsDown");
            this.tsbReportsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReportsDown.IconName = "arrow-down";
            this.tsbReportsDown.Name = "tsbReportsDown";
            this.tsbReportsDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // lReports
            // 
            resources.ApplyResources(this.lReports, "lReports");
            this.lReports.Name = "lReports";
            // 
            // lbProblems
            // 
            resources.ApplyResources(this.lbProblems, "lbProblems");
            this.lbProblems.FormattingEnabled = true;
            this.lbProblems.IntegralHeight = false;
            this.lbProblems.Name = "lbProblems";
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.bCancel);
            this.flpButtons.Controls.Add(this.bOk);
            resources.ApplyResources(this.flpButtons, "flpButtons");
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Name = "flpButtons";
            // 
            // bCancel
            // 
            resources.ApplyResources(this.bCancel, "bCancel");
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Name = "bCancel";
            // 
            // bOk
            // 
            resources.ApplyResources(this.bOk, "bOk");
            this.bOk.Name = "bOk";
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // FConstReports
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 539);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.pReports);
            this.Controls.Add(this.lbProblems);
            this.Controls.Add(this.flpButtons);
            this.Name = "FConstReports";
            resources.ApplyResources(this, "$this");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FConstReports_FormClosing);
            // 
            // cRepKey
            // 
            resources.ApplyResources(this.cRepKey, "cRepKey");
            this.cRepKey.DataPropertyName = "Key";
            this.cRepKey.Name = "cRepKey";
            // 
            // cRepName
            // 
            resources.ApplyResources(this.cRepName, "cRepName");
            this.cRepName.DataPropertyName = "Name";
            this.cRepName.Name = "cRepName";
            // 
            // cRepFunction
            // 
            resources.ApplyResources(this.cRepFunction, "cRepFunction");
            this.cRepFunction.DataPropertyName = "Function";
            this.cRepFunction.Name = "cRepFunction";
            // 
            // cRepStations
            // 
            resources.ApplyResources(this.cRepStations, "cRepStations");
            this.cRepStations.DataPropertyName = "Stations";
            this.cRepStations.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cRepStations.Name = "cRepStations";
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.tsReports.ResumeLayout(false);
            this.tsReports.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
            this.pReports.ResumeLayout(false);
            this.pReports.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pReports;
        private System.Windows.Forms.DataGridView dgvReports;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepName;
        private ExControls.DataGridViewExComboBoxColumn cRepFunction;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepStations;
        private System.Windows.Forms.ToolStrip tsReports;
        private ToolsCore.Icons.IconToolStripButton tsbReportsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbReportsDelete;
        private System.Windows.Forms.ToolStripSeparator tssReports1;
        private ToolsCore.Icons.IconToolStripButton tsbReportsUp;
        private ToolsCore.Icons.IconToolStripButton tsbReportsDown;
        private System.Windows.Forms.Label lReports;
        private GVXEditor.UI.Controls.ProblemList lbProblems;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private ExControls.ExButton bCancel;
        private ExControls.ExButton bOk;
    }
}
