namespace GVXEditor.UI.Controls
{
    partial class FPickOrdered
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FPickOrdered));
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.lbSelected = new ExControls.ExListBox();
            this.lSelected = new System.Windows.Forms.Label();
            this.bDown = new ExControls.ExButton();
            this.bUp = new ExControls.ExButton();
            this.bRemove = new ExControls.ExButton();
            this.bAdd = new ExControls.ExButton();
            this.lbAvailable = new ExControls.ExListBox();
            this.tbFilter = new ExControls.ExTextBox();
            this.lAvailable = new System.Windows.Forms.Label();
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
            // lbSelected
            // 
            resources.ApplyResources(this.lbSelected, "lbSelected");
            this.lbSelected.FormattingEnabled = true;
            this.lbSelected.IntegralHeight = false;
            this.lbSelected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSelected.Name = "lbSelected";
            this.lbSelected.DoubleClick += new System.EventHandler(this.bRemove_Click);
            // 
            // lSelected
            // 
            resources.ApplyResources(this.lSelected, "lSelected");
            this.lSelected.Name = "lSelected";
            // 
            // bDown
            // 
            resources.ApplyResources(this.bDown, "bDown");
            this.bDown.Name = "bDown";
            this.bDown.Click += new System.EventHandler(this.bDown_Click);
            // 
            // bUp
            // 
            resources.ApplyResources(this.bUp, "bUp");
            this.bUp.Name = "bUp";
            this.bUp.Click += new System.EventHandler(this.bUp_Click);
            // 
            // bRemove
            // 
            resources.ApplyResources(this.bRemove, "bRemove");
            this.bRemove.Name = "bRemove";
            this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
            // 
            // bAdd
            // 
            resources.ApplyResources(this.bAdd, "bAdd");
            this.bAdd.Name = "bAdd";
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // lbAvailable
            // 
            resources.ApplyResources(this.lbAvailable, "lbAvailable");
            this.lbAvailable.FormattingEnabled = true;
            this.lbAvailable.IntegralHeight = false;
            this.lbAvailable.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAvailable.Name = "lbAvailable";
            this.lbAvailable.DoubleClick += new System.EventHandler(this.bAdd_Click);
            // 
            // tbFilter
            // 
            resources.ApplyResources(this.tbFilter, "tbFilter");
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
            // 
            // lAvailable
            // 
            resources.ApplyResources(this.lAvailable, "lAvailable");
            this.lAvailable.Name = "lAvailable";
            // 
            // FPickOrdered
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(674, 439);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 320);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.flpButtons);
            this.Controls.Add(this.lbSelected);
            this.Controls.Add(this.lSelected);
            this.Controls.Add(this.bDown);
            this.Controls.Add(this.bUp);
            this.Controls.Add(this.bRemove);
            this.Controls.Add(this.bAdd);
            this.Controls.Add(this.lbAvailable);
            this.Controls.Add(this.tbFilter);
            this.Controls.Add(this.lAvailable);
            this.Name = "FPickOrdered";
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
        private ExControls.ExListBox lbSelected;
        private System.Windows.Forms.Label lSelected;
        private ExControls.ExButton bDown;
        private ExControls.ExButton bUp;
        private ExControls.ExButton bRemove;
        private ExControls.ExButton bAdd;
        private ExControls.ExListBox lbAvailable;
        private ExControls.ExTextBox tbFilter;
        private System.Windows.Forms.Label lAvailable;
    }
}
