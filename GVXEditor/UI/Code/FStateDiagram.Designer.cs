namespace GVXEditor.UI.Code
{
    partial class FStateDiagram
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FStateDiagram));
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.tvGraphs = new ExControls.ExTreeView();
            this.tsGraphs = new System.Windows.Forms.ToolStrip();
            this.tsbAddGraph = new ToolsCore.Icons.IconToolStripButton();
            this.tsbAddNode = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDeleteItem = new ToolsCore.Icons.IconToolStripButton();
            this.tssGraphs1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbItemUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbItemDown = new ToolsCore.Icons.IconToolStripButton();
            this.pNode = new System.Windows.Forms.Panel();
            this.lKey = new System.Windows.Forms.Label();
            this.tbKey = new ExControls.ExTextBox();
            this.lName = new System.Windows.Forms.Label();
            this.tbName = new ExControls.ExTextBox();
            this.lReport = new System.Windows.Forms.Label();
            this.cboReport = new ExControls.ExComboBox();
            this.lFlags = new System.Windows.Forms.Label();
            this.clbFlags = new ExControls.ExCheckedListBox();
            this.lCondition = new System.Windows.Forms.Label();
            this.codeCondition = new ToolsCore.CodeEditor.CodeEditorBox();
            this.cboxButton = new ExControls.ExCheckBox();
            this.lButtonText = new System.Windows.Forms.Label();
            this.tbButtonText = new ExControls.ExTextBox();
            this.lButtonPosition = new System.Windows.Forms.Label();
            this.tbButtonPosition = new ExControls.ExTextBox();
            this.cboxAutomation = new ExControls.ExCheckBox();
            this.lTime = new System.Windows.Forms.Label();
            this.codeTime = new ToolsCore.CodeEditor.CodeEditorBox();
            this.lAutoCondition = new System.Windows.Forms.Label();
            this.codeAutoCondition = new ToolsCore.CodeEditor.CodeEditorBox();
            this.lMode = new System.Windows.Forms.Label();
            this.codeMode = new ToolsCore.CodeEditor.CodeEditorBox();
            this.lVariant = new System.Windows.Forms.Label();
            this.cboVariant = new ExControls.ExComboBox();
            this.lRepeat = new System.Windows.Forms.Label();
            this.codeRepeat = new ToolsCore.CodeEditor.CodeEditorBox();
            this.lEnd = new System.Windows.Forms.Label();
            this.codeEnd = new ToolsCore.CodeEditor.CodeEditorBox();
            this.lAutoFlags = new System.Windows.Forms.Label();
            this.clbAutoFlags = new ExControls.ExCheckedListBox();
            this.pGraph = new System.Windows.Forms.Panel();
            this.lGraphDescription = new System.Windows.Forms.Label();
            this.tbGraphDescription = new ExControls.ExTextBox();
            this.lGraphHint = new System.Windows.Forms.Label();
            this.pIndex = new System.Windows.Forms.Panel();
            this.codeIndex = new ToolsCore.CodeEditor.CodeEditorBox();
            this.lIndex = new System.Windows.Forms.Label();
            this.pStation = new System.Windows.Forms.Panel();
            this.lStation = new System.Windows.Forms.Label();
            this.cboStation = new ExControls.ExComboBox();
            this.lStationInfo = new System.Windows.Forms.Label();
            this.lbProblems = new GVXEditor.UI.Controls.ProblemList();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.lCheck = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tsGraphs.SuspendLayout();
            this.pNode.SuspendLayout();
            this.pGraph.SuspendLayout();
            this.pIndex.SuspendLayout();
            this.pStation.SuspendLayout();
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
            this.splitMain.Panel1.Controls.Add(this.tvGraphs);
            this.splitMain.Panel1.Controls.Add(this.tsGraphs);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.pNode);
            this.splitMain.Panel2.Controls.Add(this.pGraph);
            // 
            // tvGraphs
            // 
            resources.ApplyResources(this.tvGraphs, "tvGraphs");
            this.tvGraphs.FullRowSelect = true;
            this.tvGraphs.HideSelection = false;
            this.tvGraphs.ShowLines = false;
            this.tvGraphs.Style = ExControls.ExTreeViewStyle.Light;
            this.tvGraphs.Name = "tvGraphs";
            this.tvGraphs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvGraphs_AfterSelect);
            // 
            // tsGraphs
            // 
            this.tsGraphs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAddGraph,
            this.tsbAddNode,
            this.tsbDeleteItem,
            this.tssGraphs1,
            this.tsbItemUp,
            this.tsbItemDown});
            resources.ApplyResources(this.tsGraphs, "tsGraphs");
            this.tsGraphs.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsGraphs.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsGraphs.Name = "tsGraphs";
            // 
            // tsbAddGraph
            // 
            resources.ApplyResources(this.tsbAddGraph, "tsbAddGraph");
            this.tsbAddGraph.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddGraph.IconName = "flowchart";
            this.tsbAddGraph.IconBadge = "add";
            this.tsbAddGraph.Name = "tsbAddGraph";
            this.tsbAddGraph.Click += new System.EventHandler(this.tsbAddGraph_Click);
            // 
            // tsbAddNode
            // 
            resources.ApplyResources(this.tsbAddNode, "tsbAddNode");
            this.tsbAddNode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddNode.IconName = "add";
            this.tsbAddNode.Name = "tsbAddNode";
            this.tsbAddNode.Click += new System.EventHandler(this.tsbAddNode_Click);
            // 
            // tsbDeleteItem
            // 
            resources.ApplyResources(this.tsbDeleteItem, "tsbDeleteItem");
            this.tsbDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteItem.IconName = "delete";
            this.tsbDeleteItem.Name = "tsbDeleteItem";
            this.tsbDeleteItem.Click += new System.EventHandler(this.tsbDeleteItem_Click);
            // 
            // tssGraphs1
            // 
            resources.ApplyResources(this.tssGraphs1, "tssGraphs1");
            this.tssGraphs1.Name = "tssGraphs1";
            // 
            // tsbItemUp
            // 
            resources.ApplyResources(this.tsbItemUp, "tsbItemUp");
            this.tsbItemUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbItemUp.IconName = "arrow-up";
            this.tsbItemUp.Name = "tsbItemUp";
            this.tsbItemUp.Click += new System.EventHandler(this.tsbItemUp_Click);
            // 
            // tsbItemDown
            // 
            resources.ApplyResources(this.tsbItemDown, "tsbItemDown");
            this.tsbItemDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbItemDown.IconName = "arrow-down";
            this.tsbItemDown.Name = "tsbItemDown";
            this.tsbItemDown.Click += new System.EventHandler(this.tsbItemDown_Click);
            // 
            // pNode
            // 
            this.pNode.Controls.Add(this.lKey);
            this.pNode.Controls.Add(this.tbKey);
            this.pNode.Controls.Add(this.lName);
            this.pNode.Controls.Add(this.tbName);
            this.pNode.Controls.Add(this.lReport);
            this.pNode.Controls.Add(this.cboReport);
            this.pNode.Controls.Add(this.lFlags);
            this.pNode.Controls.Add(this.clbFlags);
            this.pNode.Controls.Add(this.lCondition);
            this.pNode.Controls.Add(this.codeCondition);
            this.pNode.Controls.Add(this.cboxButton);
            this.pNode.Controls.Add(this.lButtonText);
            this.pNode.Controls.Add(this.tbButtonText);
            this.pNode.Controls.Add(this.lButtonPosition);
            this.pNode.Controls.Add(this.tbButtonPosition);
            this.pNode.Controls.Add(this.cboxAutomation);
            this.pNode.Controls.Add(this.lTime);
            this.pNode.Controls.Add(this.codeTime);
            this.pNode.Controls.Add(this.lAutoCondition);
            this.pNode.Controls.Add(this.codeAutoCondition);
            this.pNode.Controls.Add(this.lMode);
            this.pNode.Controls.Add(this.codeMode);
            this.pNode.Controls.Add(this.lVariant);
            this.pNode.Controls.Add(this.cboVariant);
            this.pNode.Controls.Add(this.lRepeat);
            this.pNode.Controls.Add(this.codeRepeat);
            this.pNode.Controls.Add(this.lEnd);
            this.pNode.Controls.Add(this.codeEnd);
            this.pNode.Controls.Add(this.lAutoFlags);
            this.pNode.Controls.Add(this.clbAutoFlags);
            resources.ApplyResources(this.pNode, "pNode");
            this.pNode.Name = "pNode";
            // 
            // lKey
            // 
            resources.ApplyResources(this.lKey, "lKey");
            this.lKey.Name = "lKey";
            // 
            // tbKey
            // 
            resources.ApplyResources(this.tbKey, "tbKey");
            this.tbKey.Name = "tbKey";
            this.tbKey.TextChanged += new System.EventHandler(this.Node_Changed);
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
            this.tbName.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lReport
            // 
            resources.ApplyResources(this.lReport, "lReport");
            this.lReport.Name = "lReport";
            // 
            // cboReport
            // 
            resources.ApplyResources(this.cboReport, "cboReport");
            this.cboReport.FormattingEnabled = true;
            this.cboReport.Name = "cboReport";
            this.cboReport.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lFlags
            // 
            resources.ApplyResources(this.lFlags, "lFlags");
            this.lFlags.Name = "lFlags";
            // 
            // clbFlags
            // 
            resources.ApplyResources(this.clbFlags, "clbFlags");
            this.clbFlags.CheckOnClick = true;
            this.clbFlags.FormattingEnabled = true;
            this.clbFlags.IntegralHeight = false;
            this.clbFlags.MultiColumn = true;
            this.clbFlags.ColumnWidth = 205;
            this.clbFlags.Name = "clbFlags";
            this.clbFlags.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Flags_ItemCheck);
            this.clbFlags.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Flags_MouseMove);
            // 
            // lCondition
            // 
            resources.ApplyResources(this.lCondition, "lCondition");
            this.lCondition.Name = "lCondition";
            // 
            // codeCondition
            // 
            resources.ApplyResources(this.codeCondition, "codeCondition");
            this.codeCondition.Name = "codeCondition";
            this.codeCondition.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // cboxButton
            // 
            resources.ApplyResources(this.cboxButton, "cboxButton");
            this.cboxButton.Name = "cboxButton";
            this.cboxButton.CheckedChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lButtonText
            // 
            resources.ApplyResources(this.lButtonText, "lButtonText");
            this.lButtonText.Name = "lButtonText";
            // 
            // tbButtonText
            // 
            resources.ApplyResources(this.tbButtonText, "tbButtonText");
            this.tbButtonText.Name = "tbButtonText";
            this.tbButtonText.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lButtonPosition
            // 
            resources.ApplyResources(this.lButtonPosition, "lButtonPosition");
            this.lButtonPosition.Name = "lButtonPosition";
            // 
            // tbButtonPosition
            // 
            resources.ApplyResources(this.tbButtonPosition, "tbButtonPosition");
            this.tbButtonPosition.Name = "tbButtonPosition";
            this.tbButtonPosition.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // cboxAutomation
            // 
            resources.ApplyResources(this.cboxAutomation, "cboxAutomation");
            this.cboxAutomation.Name = "cboxAutomation";
            this.cboxAutomation.CheckedChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lTime
            // 
            resources.ApplyResources(this.lTime, "lTime");
            this.lTime.Name = "lTime";
            // 
            // codeTime
            // 
            resources.ApplyResources(this.codeTime, "codeTime");
            this.codeTime.Name = "codeTime";
            this.codeTime.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lAutoCondition
            // 
            resources.ApplyResources(this.lAutoCondition, "lAutoCondition");
            this.lAutoCondition.Name = "lAutoCondition";
            // 
            // codeAutoCondition
            // 
            resources.ApplyResources(this.codeAutoCondition, "codeAutoCondition");
            this.codeAutoCondition.Name = "codeAutoCondition";
            this.codeAutoCondition.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lMode
            // 
            resources.ApplyResources(this.lMode, "lMode");
            this.lMode.Name = "lMode";
            // 
            // codeMode
            // 
            resources.ApplyResources(this.codeMode, "codeMode");
            this.codeMode.Name = "codeMode";
            this.codeMode.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lVariant
            // 
            resources.ApplyResources(this.lVariant, "lVariant");
            this.lVariant.Name = "lVariant";
            // 
            // cboVariant
            // 
            resources.ApplyResources(this.cboVariant, "cboVariant");
            this.cboVariant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVariant.FormattingEnabled = true;
            this.cboVariant.Name = "cboVariant";
            this.cboVariant.SelectedIndexChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lRepeat
            // 
            resources.ApplyResources(this.lRepeat, "lRepeat");
            this.lRepeat.Name = "lRepeat";
            // 
            // codeRepeat
            // 
            resources.ApplyResources(this.codeRepeat, "codeRepeat");
            this.codeRepeat.Name = "codeRepeat";
            this.codeRepeat.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lEnd
            // 
            resources.ApplyResources(this.lEnd, "lEnd");
            this.lEnd.Name = "lEnd";
            // 
            // codeEnd
            // 
            resources.ApplyResources(this.codeEnd, "codeEnd");
            this.codeEnd.Name = "codeEnd";
            this.codeEnd.TextChanged += new System.EventHandler(this.Node_Changed);
            // 
            // lAutoFlags
            // 
            resources.ApplyResources(this.lAutoFlags, "lAutoFlags");
            this.lAutoFlags.Name = "lAutoFlags";
            // 
            // clbAutoFlags
            // 
            resources.ApplyResources(this.clbAutoFlags, "clbAutoFlags");
            this.clbAutoFlags.CheckOnClick = true;
            this.clbAutoFlags.FormattingEnabled = true;
            this.clbAutoFlags.IntegralHeight = false;
            this.clbAutoFlags.MultiColumn = true;
            this.clbAutoFlags.ColumnWidth = 205;
            this.clbAutoFlags.Name = "clbAutoFlags";
            this.clbAutoFlags.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Flags_ItemCheck);
            this.clbAutoFlags.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Flags_MouseMove);
            // 
            // pGraph
            // 
            this.pGraph.Controls.Add(this.lGraphDescription);
            this.pGraph.Controls.Add(this.tbGraphDescription);
            this.pGraph.Controls.Add(this.lGraphHint);
            resources.ApplyResources(this.pGraph, "pGraph");
            this.pGraph.Name = "pGraph";
            // 
            // lGraphDescription
            // 
            resources.ApplyResources(this.lGraphDescription, "lGraphDescription");
            this.lGraphDescription.Name = "lGraphDescription";
            // 
            // tbGraphDescription
            // 
            resources.ApplyResources(this.tbGraphDescription, "tbGraphDescription");
            this.tbGraphDescription.Name = "tbGraphDescription";
            this.tbGraphDescription.TextChanged += new System.EventHandler(this.Graph_Changed);
            // 
            // lGraphHint
            // 
            resources.ApplyResources(this.lGraphHint, "lGraphHint");
            this.lGraphHint.Name = "lGraphHint";
            // 
            // pIndex
            // 
            this.pIndex.Controls.Add(this.codeIndex);
            this.pIndex.Controls.Add(this.lIndex);
            resources.ApplyResources(this.pIndex, "pIndex");
            this.pIndex.Name = "pIndex";
            // 
            // codeIndex
            // 
            resources.ApplyResources(this.codeIndex, "codeIndex");
            this.codeIndex.Name = "codeIndex";
            this.codeIndex.TextChanged += new System.EventHandler(this.Index_Changed);
            // 
            // lIndex
            // 
            resources.ApplyResources(this.lIndex, "lIndex");
            this.lIndex.Name = "lIndex";
            // 
            // pStation
            // 
            this.pStation.Controls.Add(this.lStation);
            this.pStation.Controls.Add(this.cboStation);
            this.pStation.Controls.Add(this.lStationInfo);
            resources.ApplyResources(this.pStation, "pStation");
            this.pStation.Name = "pStation";
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
            this.cboStation.SelectedIndexChanged += new System.EventHandler(this.cboStation_SelectedIndexChanged);
            // 
            // lStationInfo
            // 
            resources.ApplyResources(this.lStationInfo, "lStationInfo");
            this.lStationInfo.Name = "lStationInfo";
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
            // FStateDiagram
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 760);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(820, 560);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pIndex);
            this.Controls.Add(this.pStation);
            this.Controls.Add(this.lbProblems);
            this.Controls.Add(this.flpButtons);
            this.Name = "FStateDiagram";
            resources.ApplyResources(this, "$this");
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.pStation.ResumeLayout(false);
            this.pStation.PerformLayout();
            this.pIndex.ResumeLayout(false);
            this.pIndex.PerformLayout();
            this.pGraph.ResumeLayout(false);
            this.pGraph.PerformLayout();
            this.pNode.ResumeLayout(false);
            this.pNode.PerformLayout();
            this.tsGraphs.ResumeLayout(false);
            this.tsGraphs.PerformLayout();
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
        private ExControls.ExTreeView tvGraphs;
        private System.Windows.Forms.ToolStrip tsGraphs;
        private ToolsCore.Icons.IconToolStripButton tsbAddGraph;
        private ToolsCore.Icons.IconToolStripButton tsbAddNode;
        private ToolsCore.Icons.IconToolStripButton tsbDeleteItem;
        private System.Windows.Forms.ToolStripSeparator tssGraphs1;
        private ToolsCore.Icons.IconToolStripButton tsbItemUp;
        private ToolsCore.Icons.IconToolStripButton tsbItemDown;
        private System.Windows.Forms.Panel pNode;
        private System.Windows.Forms.Label lKey;
        private ExControls.ExTextBox tbKey;
        private System.Windows.Forms.Label lName;
        private ExControls.ExTextBox tbName;
        private System.Windows.Forms.Label lReport;
        private ExControls.ExComboBox cboReport;
        private System.Windows.Forms.Label lFlags;
        private ExControls.ExCheckedListBox clbFlags;
        private System.Windows.Forms.Label lCondition;
        private ToolsCore.CodeEditor.CodeEditorBox codeCondition;
        private ExControls.ExCheckBox cboxButton;
        private System.Windows.Forms.Label lButtonText;
        private ExControls.ExTextBox tbButtonText;
        private System.Windows.Forms.Label lButtonPosition;
        private ExControls.ExTextBox tbButtonPosition;
        private ExControls.ExCheckBox cboxAutomation;
        private System.Windows.Forms.Label lTime;
        private ToolsCore.CodeEditor.CodeEditorBox codeTime;
        private System.Windows.Forms.Label lAutoCondition;
        private ToolsCore.CodeEditor.CodeEditorBox codeAutoCondition;
        private System.Windows.Forms.Label lMode;
        private ToolsCore.CodeEditor.CodeEditorBox codeMode;
        private System.Windows.Forms.Label lVariant;
        private ExControls.ExComboBox cboVariant;
        private System.Windows.Forms.Label lRepeat;
        private ToolsCore.CodeEditor.CodeEditorBox codeRepeat;
        private System.Windows.Forms.Label lEnd;
        private ToolsCore.CodeEditor.CodeEditorBox codeEnd;
        private System.Windows.Forms.Label lAutoFlags;
        private ExControls.ExCheckedListBox clbAutoFlags;
        private System.Windows.Forms.Panel pGraph;
        private System.Windows.Forms.Label lGraphDescription;
        private ExControls.ExTextBox tbGraphDescription;
        private System.Windows.Forms.Label lGraphHint;
        private System.Windows.Forms.Panel pIndex;
        private ToolsCore.CodeEditor.CodeEditorBox codeIndex;
        private System.Windows.Forms.Label lIndex;
        private System.Windows.Forms.Panel pStation;
        private System.Windows.Forms.Label lStation;
        private ExControls.ExComboBox cboStation;
        private System.Windows.Forms.Label lStationInfo;
        private GVXEditor.UI.Controls.ProblemList lbProblems;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private ExControls.ExButton bCancel;
        private ExControls.ExButton bOk;
        private System.Windows.Forms.Label lCheck;
    }
}
