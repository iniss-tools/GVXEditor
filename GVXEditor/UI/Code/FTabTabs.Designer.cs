namespace GVXEditor.UI.Code
{
    partial class FTabTabs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FTabTabs));
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.lbTables = new ExControls.ExListBox();
            this.tsTables = new System.Windows.Forms.ToolStrip();
            this.tsbTablesAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbTablesDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssTables1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbTablesUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbTablesDown = new ToolsCore.Icons.IconToolStripButton();
            this.splitTable = new System.Windows.Forms.SplitContainer();
            this.pTable = new System.Windows.Forms.Panel();
            this.lTableName = new System.Windows.Forms.Label();
            this.tbTableName = new ExControls.ExTextBox();
            this.lTableDescription = new System.Windows.Forms.Label();
            this.tbTableDescription = new ExControls.ExTextBox();
            this.cboxViewValues = new ExControls.ExCheckBox();
            this.cboxIgnoreCase = new ExControls.ExCheckBox();
            this.lExpression = new System.Windows.Forms.Label();
            this.codeExpression = new ToolsCore.CodeEditor.CodeEditorBox();
            this.dgvEntries = new System.Windows.Forms.DataGridView();
            this.cKeyText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cKeyFont = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cValueText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cValueFont = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsEntries = new System.Windows.Forms.ToolStrip();
            this.tsbEntriesAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbEntriesDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssEntries1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbEntriesUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbEntriesDown = new ToolsCore.Icons.IconToolStripButton();
            this.lEntries = new System.Windows.Forms.Label();
            this.flpFile = new System.Windows.Forms.FlowLayoutPanel();
            this.lFile = new System.Windows.Forms.Label();
            this.cboFile = new ExControls.ExComboBox();
            this.lFileInfo = new System.Windows.Forms.Label();
            this.lbProblems = new GVXEditor.UI.Controls.ProblemList();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.lCheck = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tsTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTable)).BeginInit();
            this.splitTable.Panel1.SuspendLayout();
            this.splitTable.Panel2.SuspendLayout();
            this.splitTable.SuspendLayout();
            this.pTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntries)).BeginInit();
            this.tsEntries.SuspendLayout();
            this.flpFile.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            resources.ApplyResources(this.splitMain, "splitMain");
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.lbTables);
            this.splitMain.Panel1.Controls.Add(this.tsTables);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.splitTable);
            // 
            // lbTables
            // 
            resources.ApplyResources(this.lbTables, "lbTables");
            this.lbTables.FormattingEnabled = true;
            this.lbTables.IntegralHeight = false;
            this.lbTables.Name = "lbTables";
            this.lbTables.SelectedIndexChanged += new System.EventHandler(this.lbTables_SelectedIndexChanged);
            // 
            // tsTables
            // 
            this.tsTables.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbTablesAdd,
            this.tsbTablesDelete,
            this.tssTables1,
            this.tsbTablesUp,
            this.tsbTablesDown});
            resources.ApplyResources(this.tsTables, "tsTables");
            this.tsTables.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsTables.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTables.Name = "tsTables";
            // 
            // tsbTablesAdd
            // 
            resources.ApplyResources(this.tsbTablesAdd, "tsbTablesAdd");
            this.tsbTablesAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTablesAdd.IconName = "add";
            this.tsbTablesAdd.Name = "tsbTablesAdd";
            this.tsbTablesAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbTablesDelete
            // 
            resources.ApplyResources(this.tsbTablesDelete, "tsbTablesDelete");
            this.tsbTablesDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTablesDelete.IconName = "delete";
            this.tsbTablesDelete.Name = "tsbTablesDelete";
            this.tsbTablesDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // tssTables1
            // 
            resources.ApplyResources(this.tssTables1, "tssTables1");
            this.tssTables1.Name = "tssTables1";
            // 
            // tsbTablesUp
            // 
            resources.ApplyResources(this.tsbTablesUp, "tsbTablesUp");
            this.tsbTablesUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTablesUp.IconName = "arrow-up";
            this.tsbTablesUp.Name = "tsbTablesUp";
            this.tsbTablesUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbTablesDown
            // 
            resources.ApplyResources(this.tsbTablesDown, "tsbTablesDown");
            this.tsbTablesDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTablesDown.IconName = "arrow-down";
            this.tsbTablesDown.Name = "tsbTablesDown";
            this.tsbTablesDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // splitTable
            // 
            resources.ApplyResources(this.splitTable, "splitTable");
            this.splitTable.Name = "splitTable";
            // 
            // splitTable.Panel1
            // 
            this.splitTable.Panel1.Controls.Add(this.pTable);
            // 
            // splitTable.Panel2
            // 
            this.splitTable.Panel2.Controls.Add(this.dgvEntries);
            this.splitTable.Panel2.Controls.Add(this.tsEntries);
            this.splitTable.Panel2.Controls.Add(this.lEntries);
            // 
            // pTable
            // 
            this.pTable.Controls.Add(this.lTableName);
            this.pTable.Controls.Add(this.tbTableName);
            this.pTable.Controls.Add(this.lTableDescription);
            this.pTable.Controls.Add(this.tbTableDescription);
            this.pTable.Controls.Add(this.cboxViewValues);
            this.pTable.Controls.Add(this.cboxIgnoreCase);
            this.pTable.Controls.Add(this.lExpression);
            this.pTable.Controls.Add(this.codeExpression);
            resources.ApplyResources(this.pTable, "pTable");
            this.pTable.Name = "pTable";
            // 
            // lTableName
            // 
            resources.ApplyResources(this.lTableName, "lTableName");
            this.lTableName.Name = "lTableName";
            // 
            // tbTableName
            // 
            resources.ApplyResources(this.tbTableName, "tbTableName");
            this.tbTableName.Name = "tbTableName";
            this.tbTableName.TextChanged += new System.EventHandler(this.Table_Changed);
            // 
            // lTableDescription
            // 
            resources.ApplyResources(this.lTableDescription, "lTableDescription");
            this.lTableDescription.Name = "lTableDescription";
            // 
            // tbTableDescription
            // 
            resources.ApplyResources(this.tbTableDescription, "tbTableDescription");
            this.tbTableDescription.Name = "tbTableDescription";
            this.tbTableDescription.TextChanged += new System.EventHandler(this.Table_Changed);
            // 
            // cboxViewValues
            // 
            resources.ApplyResources(this.cboxViewValues, "cboxViewValues");
            this.cboxViewValues.Name = "cboxViewValues";
            this.cboxViewValues.CheckedChanged += new System.EventHandler(this.Table_Changed);
            // 
            // cboxIgnoreCase
            // 
            resources.ApplyResources(this.cboxIgnoreCase, "cboxIgnoreCase");
            this.cboxIgnoreCase.Name = "cboxIgnoreCase";
            this.cboxIgnoreCase.CheckedChanged += new System.EventHandler(this.Table_Changed);
            // 
            // lExpression
            // 
            resources.ApplyResources(this.lExpression, "lExpression");
            this.lExpression.Name = "lExpression";
            // 
            // codeExpression
            // 
            resources.ApplyResources(this.codeExpression, "codeExpression");
            this.codeExpression.Name = "codeExpression";
            this.codeExpression.TextChanged += new System.EventHandler(this.Table_Changed);
            // 
            // dgvEntries
            // 
            this.dgvEntries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cKeyText,
            this.cKeyFont,
            this.cValueText,
            this.cValueFont});
            resources.ApplyResources(this.dgvEntries, "dgvEntries");
            this.dgvEntries.AllowUserToAddRows = false;
            this.dgvEntries.AllowUserToDeleteRows = false;
            this.dgvEntries.AllowUserToResizeRows = false;
            this.dgvEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEntries.MultiSelect = false;
            this.dgvEntries.RowHeadersVisible = false;
            this.dgvEntries.Name = "dgvEntries";
            this.dgvEntries.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Entries_CellValueChanged);
            // 
            // tsEntries
            // 
            this.tsEntries.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbEntriesAdd,
            this.tsbEntriesDelete,
            this.tssEntries1,
            this.tsbEntriesUp,
            this.tsbEntriesDown});
            resources.ApplyResources(this.tsEntries, "tsEntries");
            this.tsEntries.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsEntries.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsEntries.Name = "tsEntries";
            // 
            // tsbEntriesAdd
            // 
            resources.ApplyResources(this.tsbEntriesAdd, "tsbEntriesAdd");
            this.tsbEntriesAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEntriesAdd.IconName = "add";
            this.tsbEntriesAdd.Name = "tsbEntriesAdd";
            this.tsbEntriesAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbEntriesDelete
            // 
            resources.ApplyResources(this.tsbEntriesDelete, "tsbEntriesDelete");
            this.tsbEntriesDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEntriesDelete.IconName = "delete";
            this.tsbEntriesDelete.Name = "tsbEntriesDelete";
            this.tsbEntriesDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // tssEntries1
            // 
            resources.ApplyResources(this.tssEntries1, "tssEntries1");
            this.tssEntries1.Name = "tssEntries1";
            // 
            // tsbEntriesUp
            // 
            resources.ApplyResources(this.tsbEntriesUp, "tsbEntriesUp");
            this.tsbEntriesUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEntriesUp.IconName = "arrow-up";
            this.tsbEntriesUp.Name = "tsbEntriesUp";
            this.tsbEntriesUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbEntriesDown
            // 
            resources.ApplyResources(this.tsbEntriesDown, "tsbEntriesDown");
            this.tsbEntriesDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEntriesDown.IconName = "arrow-down";
            this.tsbEntriesDown.Name = "tsbEntriesDown";
            this.tsbEntriesDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // lEntries
            // 
            resources.ApplyResources(this.lEntries, "lEntries");
            this.lEntries.Name = "lEntries";
            // 
            // flpFile
            // 
            this.flpFile.Controls.Add(this.lFile);
            this.flpFile.Controls.Add(this.cboFile);
            this.flpFile.Controls.Add(this.lFileInfo);
            resources.ApplyResources(this.flpFile, "flpFile");
            this.flpFile.Name = "flpFile";
            // 
            // lFile
            // 
            resources.ApplyResources(this.lFile, "lFile");
            this.lFile.Name = "lFile";
            // 
            // cboFile
            // 
            resources.ApplyResources(this.cboFile, "cboFile");
            this.cboFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFile.FormattingEnabled = true;
            this.cboFile.Name = "cboFile";
            this.cboFile.SelectedIndexChanged += new System.EventHandler(this.cboFile_SelectedIndexChanged);
            // 
            // lFileInfo
            // 
            resources.ApplyResources(this.lFileInfo, "lFileInfo");
            this.lFileInfo.Name = "lFileInfo";
            // 
            // lbProblems
            // 
            resources.ApplyResources(this.lbProblems, "lbProblems");
            this.lbProblems.FormattingEnabled = true;
            this.lbProblems.IntegralHeight = false;
            this.lbProblems.Name = "lbProblems";
            this.lbProblems.DoubleClick += new System.EventHandler(this.lbProblems_DoubleClick);
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.bCancel);
            this.flpButtons.Controls.Add(this.bOk);
            this.flpButtons.Controls.Add(this.lCheck);
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
            // lCheck
            // 
            resources.ApplyResources(this.lCheck, "lCheck");
            this.lCheck.Name = "lCheck";
            // 
            // FTabTabs
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 740);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(820, 560);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.flpFile);
            this.Controls.Add(this.lbProblems);
            this.Controls.Add(this.flpButtons);
            this.Name = "FTabTabs";
            resources.ApplyResources(this, "$this");
            // 
            // cKeyText
            // 
            resources.ApplyResources(this.cKeyText, "cKeyText");
            this.cKeyText.DataPropertyName = "KeyText";
            this.cKeyText.Name = "cKeyText";
            // 
            // cKeyFont
            // 
            resources.ApplyResources(this.cKeyFont, "cKeyFont");
            this.cKeyFont.DataPropertyName = "KeyFont";
            this.cKeyFont.Name = "cKeyFont";
            // 
            // cValueText
            // 
            resources.ApplyResources(this.cValueText, "cValueText");
            this.cValueText.DataPropertyName = "ValueText";
            this.cValueText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cValueText.Name = "cValueText";
            // 
            // cValueFont
            // 
            resources.ApplyResources(this.cValueFont, "cValueFont");
            this.cValueFont.DataPropertyName = "ValueFont";
            this.cValueFont.Name = "cValueFont";
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.flpFile.ResumeLayout(false);
            this.flpFile.PerformLayout();
            this.tsEntries.ResumeLayout(false);
            this.tsEntries.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntries)).EndInit();
            this.pTable.ResumeLayout(false);
            this.pTable.PerformLayout();
            this.splitTable.Panel1.ResumeLayout(false);
            this.splitTable.Panel1.PerformLayout();
            this.splitTable.Panel2.ResumeLayout(false);
            this.splitTable.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTable)).EndInit();
            this.splitTable.ResumeLayout(false);
            this.splitTable.PerformLayout();
            this.tsTables.ResumeLayout(false);
            this.tsTables.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private ExControls.ExListBox lbTables;
        private System.Windows.Forms.ToolStrip tsTables;
        private ToolsCore.Icons.IconToolStripButton tsbTablesAdd;
        private ToolsCore.Icons.IconToolStripButton tsbTablesDelete;
        private System.Windows.Forms.ToolStripSeparator tssTables1;
        private ToolsCore.Icons.IconToolStripButton tsbTablesUp;
        private ToolsCore.Icons.IconToolStripButton tsbTablesDown;
        private System.Windows.Forms.SplitContainer splitTable;
        private System.Windows.Forms.Panel pTable;
        private System.Windows.Forms.Label lTableName;
        private ExControls.ExTextBox tbTableName;
        private System.Windows.Forms.Label lTableDescription;
        private ExControls.ExTextBox tbTableDescription;
        private ExControls.ExCheckBox cboxViewValues;
        private ExControls.ExCheckBox cboxIgnoreCase;
        private System.Windows.Forms.Label lExpression;
        private ToolsCore.CodeEditor.CodeEditorBox codeExpression;
        private System.Windows.Forms.DataGridView dgvEntries;
        private System.Windows.Forms.DataGridViewTextBoxColumn cKeyText;
        private System.Windows.Forms.DataGridViewTextBoxColumn cKeyFont;
        private System.Windows.Forms.DataGridViewTextBoxColumn cValueText;
        private System.Windows.Forms.DataGridViewTextBoxColumn cValueFont;
        private System.Windows.Forms.ToolStrip tsEntries;
        private ToolsCore.Icons.IconToolStripButton tsbEntriesAdd;
        private ToolsCore.Icons.IconToolStripButton tsbEntriesDelete;
        private System.Windows.Forms.ToolStripSeparator tssEntries1;
        private ToolsCore.Icons.IconToolStripButton tsbEntriesUp;
        private ToolsCore.Icons.IconToolStripButton tsbEntriesDown;
        private System.Windows.Forms.Label lEntries;
        private System.Windows.Forms.FlowLayoutPanel flpFile;
        private System.Windows.Forms.Label lFile;
        private ExControls.ExComboBox cboFile;
        private System.Windows.Forms.Label lFileInfo;
        private GVXEditor.UI.Controls.ProblemList lbProblems;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private ExControls.ExButton bCancel;
        private ExControls.ExButton bOk;
        private System.Windows.Forms.Label lCheck;
    }
}
