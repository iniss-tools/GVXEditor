namespace GVXEditor.UI.Boards
{
    partial class FGenerateTexts
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FGenerateTexts));
            this.lStation = new System.Windows.Forms.Label();
            this.cboStation = new ExControls.ExComboBox();
            this.lSet = new System.Windows.Forms.Label();
            this.cboSet = new ExControls.ExComboBox();
            this.lKind = new System.Windows.Forms.Label();
            this.cboKind = new ExControls.ExComboBox();
            this.lReport = new System.Windows.Forms.Label();
            this.pReport = new System.Windows.Forms.Panel();
            this.rbShort = new ExControls.ExRadioButton();
            this.rbLong = new ExControls.ExRadioButton();
            this.lTrains = new System.Windows.Forms.Label();
            this.pTrains = new System.Windows.Forms.Panel();
            this.rbMissing = new ExControls.ExRadioButton();
            this.rbAll = new ExControls.ExRadioButton();
            this.lHint = new System.Windows.Forms.Label();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            this.cNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cChange = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lSummary = new System.Windows.Forms.Label();
            this.bOk = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.pReport.SuspendLayout();
            this.pTrains.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // lStation
            // 
            resources.ApplyResources(this.lStation, "lStation");
            this.lStation.Name = "lStation";
            // 
            // cboStation
            // 
            resources.ApplyResources(this.cboStation, "cboStation");
            this.cboStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStation.FormattingEnabled = true;
            this.cboStation.Name = "cboStation";
            this.cboStation.SelectedIndexChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lSet
            // 
            resources.ApplyResources(this.lSet, "lSet");
            this.lSet.Name = "lSet";
            // 
            // cboSet
            // 
            resources.ApplyResources(this.cboSet, "cboSet");
            this.cboSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSet.FormattingEnabled = true;
            this.cboSet.Name = "cboSet";
            this.cboSet.SelectedIndexChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lKind
            // 
            resources.ApplyResources(this.lKind, "lKind");
            this.lKind.Name = "lKind";
            // 
            // cboKind
            // 
            resources.ApplyResources(this.cboKind, "cboKind");
            this.cboKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKind.FormattingEnabled = true;
            this.cboKind.Name = "cboKind";
            this.cboKind.SelectedIndexChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lReport
            // 
            resources.ApplyResources(this.lReport, "lReport");
            this.lReport.Name = "lReport";
            // 
            // pReport
            // 
            this.pReport.Controls.Add(this.rbShort);
            this.pReport.Controls.Add(this.rbLong);
            resources.ApplyResources(this.pReport, "pReport");
            this.pReport.Name = "pReport";
            // 
            // rbShort
            // 
            resources.ApplyResources(this.rbShort, "rbShort");
            this.rbShort.Checked = true;
            this.rbShort.Name = "rbShort";
            this.rbShort.CheckedChanged += new System.EventHandler(this.Input_Changed);
            // 
            // rbLong
            // 
            resources.ApplyResources(this.rbLong, "rbLong");
            this.rbLong.Name = "rbLong";
            this.rbLong.CheckedChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lTrains
            // 
            resources.ApplyResources(this.lTrains, "lTrains");
            this.lTrains.Name = "lTrains";
            // 
            // pTrains
            // 
            this.pTrains.Controls.Add(this.rbMissing);
            this.pTrains.Controls.Add(this.rbAll);
            resources.ApplyResources(this.pTrains, "pTrains");
            this.pTrains.Name = "pTrains";
            // 
            // rbMissing
            // 
            resources.ApplyResources(this.rbMissing, "rbMissing");
            this.rbMissing.Checked = true;
            this.rbMissing.Name = "rbMissing";
            this.rbMissing.CheckedChanged += new System.EventHandler(this.Input_Changed);
            // 
            // rbAll
            // 
            resources.ApplyResources(this.rbAll, "rbAll");
            this.rbAll.Name = "rbAll";
            this.rbAll.CheckedChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // dgvPreview
            // 
            this.dgvPreview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cNumber,
            this.cValidity,
            this.cText,
            this.cCurrent,
            this.cChange});
            resources.ApplyResources(this.dgvPreview, "dgvPreview");
            this.dgvPreview.AllowUserToAddRows = false;
            this.dgvPreview.AllowUserToDeleteRows = false;
            this.dgvPreview.AllowUserToResizeRows = false;
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.MultiSelect = false;
            this.dgvPreview.ReadOnly = true;
            this.dgvPreview.RowHeadersVisible = false;
            this.dgvPreview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPreview.Name = "dgvPreview";
            // 
            // lSummary
            // 
            resources.ApplyResources(this.lSummary, "lSummary");
            this.lSummary.Name = "lSummary";
            // 
            // bOk
            // 
            resources.ApplyResources(this.bOk, "bOk");
            this.bOk.Name = "bOk";
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // bCancel
            // 
            resources.ApplyResources(this.bCancel, "bCancel");
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Name = "bCancel";
            // 
            // FGenerateTexts
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(760, 505);
            this.MinimumSize = new System.Drawing.Size(620, 420);
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.lStation);
            this.Controls.Add(this.cboStation);
            this.Controls.Add(this.lSet);
            this.Controls.Add(this.cboSet);
            this.Controls.Add(this.lKind);
            this.Controls.Add(this.cboKind);
            this.Controls.Add(this.lReport);
            this.Controls.Add(this.pReport);
            this.Controls.Add(this.lTrains);
            this.Controls.Add(this.pTrains);
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.lSummary);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.bCancel);
            this.Name = "FGenerateTexts";
            resources.ApplyResources(this, "$this");
            // 
            // cNumber
            // 
            resources.ApplyResources(this.cNumber, "cNumber");
            this.cNumber.DataPropertyName = "Number";
            this.cNumber.ReadOnly = true;
            this.cNumber.Name = "cNumber";
            // 
            // cValidity
            // 
            resources.ApplyResources(this.cValidity, "cValidity");
            this.cValidity.DataPropertyName = "Validity";
            this.cValidity.ReadOnly = true;
            this.cValidity.Name = "cValidity";
            // 
            // cText
            // 
            resources.ApplyResources(this.cText, "cText");
            this.cText.DataPropertyName = "Text";
            this.cText.ReadOnly = true;
            this.cText.Name = "cText";
            // 
            // cCurrent
            // 
            resources.ApplyResources(this.cCurrent, "cCurrent");
            this.cCurrent.DataPropertyName = "Current";
            this.cCurrent.ReadOnly = true;
            this.cCurrent.Name = "cCurrent";
            // 
            // cChange
            // 
            resources.ApplyResources(this.cChange, "cChange");
            this.cChange.DataPropertyName = "Change";
            this.cChange.ReadOnly = true;
            this.cChange.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cChange.Name = "cChange";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.pTrains.ResumeLayout(false);
            this.pTrains.PerformLayout();
            this.pReport.ResumeLayout(false);
            this.pReport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lStation;
        private ExControls.ExComboBox cboStation;
        private System.Windows.Forms.Label lSet;
        private ExControls.ExComboBox cboSet;
        private System.Windows.Forms.Label lKind;
        private ExControls.ExComboBox cboKind;
        private System.Windows.Forms.Label lReport;
        private System.Windows.Forms.Panel pReport;
        private ExControls.ExRadioButton rbShort;
        private ExControls.ExRadioButton rbLong;
        private System.Windows.Forms.Label lTrains;
        private System.Windows.Forms.Panel pTrains;
        private ExControls.ExRadioButton rbMissing;
        private ExControls.ExRadioButton rbAll;
        private System.Windows.Forms.Label lHint;
        private System.Windows.Forms.DataGridView dgvPreview;
        private System.Windows.Forms.DataGridViewTextBoxColumn cNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn cValidity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cText;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCurrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn cChange;
        private System.Windows.Forms.Label lSummary;
        private ExControls.ExButton bOk;
        private ExControls.ExButton bCancel;
    }
}
