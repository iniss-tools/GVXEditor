namespace GVXEditor.UI.Controls
{
    partial class FPickSound
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FPickSound));
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.cboxAnyLanguage = new ExControls.ExCheckBox();
            this.lbSounds = new ExControls.ExListBox();
            this.lSounds = new System.Windows.Forms.Label();
            this.bPlay = new ExControls.ExButton();
            this.bSoundDown = new ExControls.ExButton();
            this.bSoundUp = new ExControls.ExButton();
            this.bSoundRemove = new ExControls.ExButton();
            this.bSoundAdd = new ExControls.ExButton();
            this.tvSounds = new ExControls.ExTreeView();
            this.tbSoundFilter = new ExControls.ExTextBox();
            this.cboLanguage = new ExControls.ExComboBox();
            this.lLanguage = new System.Windows.Forms.Label();
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
            // cboxAnyLanguage
            // 
            resources.ApplyResources(this.cboxAnyLanguage, "cboxAnyLanguage");
            this.cboxAnyLanguage.Checked = true;
            this.cboxAnyLanguage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxAnyLanguage.Name = "cboxAnyLanguage";
            // 
            // lbSounds
            // 
            resources.ApplyResources(this.lbSounds, "lbSounds");
            this.lbSounds.FormattingEnabled = true;
            this.lbSounds.IntegralHeight = false;
            this.lbSounds.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSounds.Name = "lbSounds";
            this.lbSounds.DoubleClick += new System.EventHandler(this.bSoundRemove_Click);
            // 
            // lSounds
            // 
            resources.ApplyResources(this.lSounds, "lSounds");
            this.lSounds.Name = "lSounds";
            // 
            // bPlay
            // 
            resources.ApplyResources(this.bPlay, "bPlay");
            this.bPlay.Name = "bPlay";
            this.bPlay.Click += new System.EventHandler(this.bPlay_Click);
            // 
            // bSoundDown
            // 
            resources.ApplyResources(this.bSoundDown, "bSoundDown");
            this.bSoundDown.Name = "bSoundDown";
            this.bSoundDown.Click += new System.EventHandler(this.bSoundDown_Click);
            // 
            // bSoundUp
            // 
            resources.ApplyResources(this.bSoundUp, "bSoundUp");
            this.bSoundUp.Name = "bSoundUp";
            this.bSoundUp.Click += new System.EventHandler(this.bSoundUp_Click);
            // 
            // bSoundRemove
            // 
            resources.ApplyResources(this.bSoundRemove, "bSoundRemove");
            this.bSoundRemove.Name = "bSoundRemove";
            this.bSoundRemove.Click += new System.EventHandler(this.bSoundRemove_Click);
            // 
            // bSoundAdd
            // 
            resources.ApplyResources(this.bSoundAdd, "bSoundAdd");
            this.bSoundAdd.Name = "bSoundAdd";
            this.bSoundAdd.Click += new System.EventHandler(this.bSoundAdd_Click);
            // 
            // tvSounds
            // 
            resources.ApplyResources(this.tvSounds, "tvSounds");
            this.tvSounds.FullRowSelect = true;
            this.tvSounds.HideSelection = false;
            this.tvSounds.ShowLines = false;
            this.tvSounds.Style = ExControls.ExTreeViewStyle.Light;
            this.tvSounds.Name = "tvSounds";
            this.tvSounds.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvSounds_NodeMouseDoubleClick);
            // 
            // tbSoundFilter
            // 
            resources.ApplyResources(this.tbSoundFilter, "tbSoundFilter");
            this.tbSoundFilter.Name = "tbSoundFilter";
            this.tbSoundFilter.TextChanged += new System.EventHandler(this.tbSoundFilter_TextChanged);
            // 
            // cboLanguage
            // 
            resources.ApplyResources(this.cboLanguage, "cboLanguage");
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.SelectedIndexChanged += new System.EventHandler(this.cboLanguage_SelectedIndexChanged);
            // 
            // lLanguage
            // 
            resources.ApplyResources(this.lLanguage, "lLanguage");
            this.lLanguage.Name = "lLanguage";
            // 
            // FPickSound
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(805, 439);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 320);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.flpButtons);
            this.Controls.Add(this.cboxAnyLanguage);
            this.Controls.Add(this.lbSounds);
            this.Controls.Add(this.lSounds);
            this.Controls.Add(this.bPlay);
            this.Controls.Add(this.bSoundDown);
            this.Controls.Add(this.bSoundUp);
            this.Controls.Add(this.bSoundRemove);
            this.Controls.Add(this.bSoundAdd);
            this.Controls.Add(this.tvSounds);
            this.Controls.Add(this.tbSoundFilter);
            this.Controls.Add(this.cboLanguage);
            this.Controls.Add(this.lLanguage);
            this.Name = "FPickSound";
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
        private ExControls.ExCheckBox cboxAnyLanguage;
        private ExControls.ExListBox lbSounds;
        private System.Windows.Forms.Label lSounds;
        private ExControls.ExButton bPlay;
        private ExControls.ExButton bSoundDown;
        private ExControls.ExButton bSoundUp;
        private ExControls.ExButton bSoundRemove;
        private ExControls.ExButton bSoundAdd;
        private ExControls.ExTreeView tvSounds;
        private ExControls.ExTextBox tbSoundFilter;
        private ExControls.ExComboBox cboLanguage;
        private System.Windows.Forms.Label lLanguage;
    }
}
