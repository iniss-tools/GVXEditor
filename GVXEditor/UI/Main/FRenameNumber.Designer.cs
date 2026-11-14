namespace GVXEditor.UI.Main
{
    partial class FRenameNumber
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRenameNumber));
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.lInfo = new System.Windows.Forms.Label();
            this.cboxAllTrains = new ExControls.ExCheckBox();
            this.tbNew = new ExControls.ExTextBox();
            this.lNew = new System.Windows.Forms.Label();
            this.cboNumber = new ExControls.ExComboBox();
            this.lNumber = new System.Windows.Forms.Label();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
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
            // lInfo
            // 
            resources.ApplyResources(this.lInfo, "lInfo");
            this.lInfo.MaximumSize = new System.Drawing.Size(400, 0);
            this.lInfo.Name = "lInfo";
            // 
            // cboxAllTrains
            // 
            resources.ApplyResources(this.cboxAllTrains, "cboxAllTrains");
            this.cboxAllTrains.MaximumSize = new System.Drawing.Size(400, 0);
            this.cboxAllTrains.Name = "cboxAllTrains";
            this.cboxAllTrains.CheckedChanged += new System.EventHandler(this.Input_Changed);
            // 
            // tbNew
            // 
            resources.ApplyResources(this.tbNew, "tbNew");
            this.tbNew.Name = "tbNew";
            this.tbNew.TextChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lNew
            // 
            resources.ApplyResources(this.lNew, "lNew");
            this.lNew.Name = "lNew";
            // 
            // cboNumber
            // 
            resources.ApplyResources(this.cboNumber, "cboNumber");
            this.cboNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNumber.FormattingEnabled = true;
            this.cboNumber.Name = "cboNumber";
            this.cboNumber.SelectedIndexChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lNumber
            // 
            resources.ApplyResources(this.lNumber, "lNumber");
            this.lNumber.Name = "lNumber";
            // 
            // FRenameNumber
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(424, 190);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.flpButtons);
            this.Controls.Add(this.lInfo);
            this.Controls.Add(this.cboxAllTrains);
            this.Controls.Add(this.tbNew);
            this.Controls.Add(this.lNew);
            this.Controls.Add(this.cboNumber);
            this.Controls.Add(this.lNumber);
            this.Name = "FRenameNumber";
            resources.ApplyResources(this, "$this");
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private ExControls.ExButton bCancel;
        private ExControls.ExButton bOk;
        private System.Windows.Forms.Label lInfo;
        private ExControls.ExCheckBox cboxAllTrains;
        private ExControls.ExTextBox tbNew;
        private System.Windows.Forms.Label lNew;
        private ExControls.ExComboBox cboNumber;
        private System.Windows.Forms.Label lNumber;
    }
}
