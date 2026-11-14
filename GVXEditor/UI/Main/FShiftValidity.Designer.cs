namespace GVXEditor.UI.Main
{
    partial class FShiftValidity
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FShiftValidity));
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.lDays = new System.Windows.Forms.Label();
            this.nudDays = new ExControls.ExNumericUpDown();
            this.lPrompt = new System.Windows.Forms.Label();
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
            this.bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOk.Name = "bOk";
            // 
            // lDays
            // 
            resources.ApplyResources(this.lDays, "lDays");
            this.lDays.Name = "lDays";
            // 
            // nudDays
            // 
            resources.ApplyResources(this.nudDays, "nudDays");
            this.nudDays.Maximum = new decimal(new int[] { 3650, 0, 0, 0 });
            this.nudDays.Minimum = new decimal(new int[] { 3650, 0, 0, -2147483648 });
            this.nudDays.Value = new decimal(new int[] { 364, 0, 0, 0 });
            this.nudDays.Name = "nudDays";
            // 
            // lPrompt
            // 
            resources.ApplyResources(this.lPrompt, "lPrompt");
            this.lPrompt.MaximumSize = new System.Drawing.Size(400, 0);
            this.lPrompt.Name = "lPrompt";
            // 
            // FShiftValidity
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(424, 140);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.flpButtons);
            this.Controls.Add(this.lDays);
            this.Controls.Add(this.nudDays);
            this.Controls.Add(this.lPrompt);
            this.Name = "FShiftValidity";
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
        private System.Windows.Forms.Label lDays;
        private ExControls.ExNumericUpDown nudDays;
        private System.Windows.Forms.Label lPrompt;
    }
}
