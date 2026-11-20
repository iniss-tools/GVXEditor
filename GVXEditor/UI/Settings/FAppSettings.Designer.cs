namespace GVXEditor.UI.Settings
{
    partial class FAppSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FAppSettings));
            ExControls.OptionsNode optionsNodeMcp = new ExControls.OptionsNode();
            this.pMcp = new ExControls.ExOptionsPanel(this.optionsView);
            this.mcpPage = new ToolsCore.Mcp.McpSettingsPage();
            ((System.ComponentModel.ISupportInitialize)(this.optionsView)).BeginInit();
            this.pMcp.SuspendLayout();
            this.SuspendLayout();
            this.optionsView.Panels.Add(this.pMcp);
            //
            // pMcp
            //
            this.pMcp.Controls.Add(this.mcpPage);
            this.pMcp.Name = "pMcp";
            optionsNodeMcp.Name = "";
            optionsNodeMcp.Text = resources.GetString("optionsNodeMcp.Text");
            this.pMcp.Node = optionsNodeMcp;
            this.pMcp.NodeText = resources.GetString("pMcp.NodeText");
            this.pMcp.ParentNode = null;
            //
            // mcpPage
            //
            resources.ApplyResources(this.mcpPage, "mcpPage");
            this.mcpPage.Name = "mcpPage";
            //
            // FAppSettings
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 475);
            this.Name = "FAppSettings";
            ((System.ComponentModel.ISupportInitialize)(this.optionsView)).EndInit();
            this.pMcp.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ExControls.ExOptionsPanel pMcp;
        private ToolsCore.Mcp.McpSettingsPage mcpPage;
    }
}
