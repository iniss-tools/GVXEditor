namespace GVXEditor.UI.Code
{
    partial class FLogText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FLogText));
            this.codeText = new ToolsCore.CodeEditor.CodeEditorBox();
            this.tsText = new System.Windows.Forms.ToolStrip();
            this.tsbFind = new ToolsCore.Icons.IconToolStripButton();
            this.tsbReplace = new ToolsCore.Icons.IconToolStripButton();
            this.lbProblems = new GVXEditor.UI.Controls.ProblemList();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.lCheck = new System.Windows.Forms.Label();
            this.tsText.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // codeText
            // 
            resources.ApplyResources(this.codeText, "codeText");
            this.codeText.Name = "codeText";
            this.codeText.TextChanged += new System.EventHandler(this.Text_Changed);
            // 
            // tsText
            // 
            this.tsText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFind,
            this.tsbReplace});
            resources.ApplyResources(this.tsText, "tsText");
            this.tsText.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsText.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsText.Name = "tsText";
            // 
            // tsbFind
            // 
            resources.ApplyResources(this.tsbFind, "tsbFind");
            this.tsbFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFind.IconName = "search";
            this.tsbFind.Name = "tsbFind";
            this.tsbFind.Click += new System.EventHandler(this.tsbFind_Click);
            // 
            // tsbReplace
            // 
            resources.ApplyResources(this.tsbReplace, "tsbReplace");
            this.tsbReplace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReplace.IconName = "search";
            this.tsbReplace.IconBadge = "edit";
            this.tsbReplace.Name = "tsbReplace";
            this.tsbReplace.Click += new System.EventHandler(this.tsbReplace_Click);
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
            // FLogText
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 420);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.codeText);
            this.Controls.Add(this.tsText);
            this.Controls.Add(this.lbProblems);
            this.Controls.Add(this.flpButtons);
            this.Name = "FLogText";
            resources.ApplyResources(this, "$this");
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.tsText.ResumeLayout(false);
            this.tsText.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolsCore.CodeEditor.CodeEditorBox codeText;
        private System.Windows.Forms.ToolStrip tsText;
        private ToolsCore.Icons.IconToolStripButton tsbFind;
        private ToolsCore.Icons.IconToolStripButton tsbReplace;
        private GVXEditor.UI.Controls.ProblemList lbProblems;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private ExControls.ExButton bCancel;
        private ExControls.ExButton bOk;
        private System.Windows.Forms.Label lCheck;
    }
}
