namespace GVXEditor.UI.Setup
{
    partial class FAddStation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FAddStation));
            this.lNumber = new System.Windows.Forms.Label();
            this.tbNumber = new ExControls.ExTextBox();
            this.lName = new System.Windows.Forms.Label();
            this.tbName = new ExControls.ExTextBox();
            this.lDirectory = new System.Windows.Forms.Label();
            this.tbDirectory = new ExControls.ExTextBox();
            this.lKind = new System.Windows.Forms.Label();
            this.cboKind = new ExControls.ExComboBox();
            this.lTemplate = new System.Windows.Forms.Label();
            this.cboTemplate = new ExControls.ExComboBox();
            this.lHint = new System.Windows.Forms.Label();
            this.lProblem = new System.Windows.Forms.Label();
            this.bOk = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.SuspendLayout();
            // 
            // lNumber
            // 
            resources.ApplyResources(this.lNumber, "lNumber");
            this.lNumber.Name = "lNumber";
            // 
            // tbNumber
            // 
            resources.ApplyResources(this.tbNumber, "tbNumber");
            this.tbNumber.Name = "tbNumber";
            this.tbNumber.TextChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lName
            // 
            resources.ApplyResources(this.lName, "lName");
            this.lName.Name = "lName";
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.Name = "tbName";
            this.tbName.TextChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lDirectory
            // 
            resources.ApplyResources(this.lDirectory, "lDirectory");
            this.lDirectory.Name = "lDirectory";
            // 
            // tbDirectory
            // 
            resources.ApplyResources(this.tbDirectory, "tbDirectory");
            this.tbDirectory.Name = "tbDirectory";
            this.tbDirectory.TextChanged += new System.EventHandler(this.Input_Changed);
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
            // 
            // lTemplate
            // 
            resources.ApplyResources(this.lTemplate, "lTemplate");
            this.lTemplate.Name = "lTemplate";
            // 
            // cboTemplate
            // 
            resources.ApplyResources(this.cboTemplate, "cboTemplate");
            this.cboTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTemplate.FormattingEnabled = true;
            this.cboTemplate.Name = "cboTemplate";
            this.cboTemplate.SelectedIndexChanged += new System.EventHandler(this.Input_Changed);
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // lProblem
            // 
            resources.ApplyResources(this.lProblem, "lProblem");
            this.lProblem.Name = "lProblem";
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
            // FAddStation
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(520, 279);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.lNumber);
            this.Controls.Add(this.tbNumber);
            this.Controls.Add(this.lName);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lDirectory);
            this.Controls.Add(this.tbDirectory);
            this.Controls.Add(this.lKind);
            this.Controls.Add(this.cboKind);
            this.Controls.Add(this.lTemplate);
            this.Controls.Add(this.cboTemplate);
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.lProblem);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.bCancel);
            this.Name = "FAddStation";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lNumber;
        private ExControls.ExTextBox tbNumber;
        private System.Windows.Forms.Label lName;
        private ExControls.ExTextBox tbName;
        private System.Windows.Forms.Label lDirectory;
        private ExControls.ExTextBox tbDirectory;
        private System.Windows.Forms.Label lKind;
        private ExControls.ExComboBox cboKind;
        private System.Windows.Forms.Label lTemplate;
        private ExControls.ExComboBox cboTemplate;
        private System.Windows.Forms.Label lHint;
        private System.Windows.Forms.Label lProblem;
        private ExControls.ExButton bOk;
        private ExControls.ExButton bCancel;
    }
}
