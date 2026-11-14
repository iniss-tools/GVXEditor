namespace GVXEditor.UI.Code
{
    partial class FReports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FReports));
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.dgvReports = new System.Windows.Forms.DataGridView();
            this.cRepName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepKind = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepRefs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pFilter = new System.Windows.Forms.Panel();
            this.lFilterKind = new System.Windows.Forms.Label();
            this.cboFilterKind = new ExControls.ExComboBox();
            this.tbFilter = new ExControls.ExTextBox();
            this.tsReports = new System.Windows.Forms.ToolStrip();
            this.tsbNew = new ToolsCore.Icons.IconToolStripButton();
            this.tsbCopy = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssReports1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbOpenCode = new ToolsCore.Icons.IconToolStripButton();
            this.pReport = new System.Windows.Forms.Panel();
            this.pSteps = new System.Windows.Forms.Panel();
            this.dgvSteps = new System.Windows.Forms.DataGridView();
            this.cStepBlock = new ExControls.DataGridViewExComboBoxColumn();
            this.cStepCondition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStepKind = new ExControls.DataGridViewExComboBoxColumn();
            this.cStepValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStepBefore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStepAfter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStepText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStepComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsSteps = new System.Windows.Forms.ToolStrip();
            this.tsbAddSounds = new ToolsCore.Icons.IconToolStripButton();
            this.tsbAddPause = new ToolsCore.Icons.IconToolStripButton();
            this.tsbAddBuiltin = new ToolsCore.Icons.IconToolStripDropDownButton();
            this.tsbAddCall = new ToolsCore.Icons.IconToolStripButton();
            this.tsbCondition = new ToolsCore.Icons.IconToolStripDropDownButton();
            this.tsbToggleStep = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDeleteStep = new ToolsCore.Icons.IconToolStripButton();
            this.tssSteps1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbStepUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbStepDown = new ToolsCore.Icons.IconToolStripButton();
            this.lSteps = new System.Windows.Forms.Label();
            this.pPreview = new System.Windows.Forms.Panel();
            this.tbPreview = new ExControls.ExTextBox();
            this.lPreview = new System.Windows.Forms.Label();
            this.pHeader = new System.Windows.Forms.Panel();
            this.lName = new System.Windows.Forms.Label();
            this.tbName = new ExControls.ExTextBox();
            this.lKind = new System.Windows.Forms.Label();
            this.cboKind = new ExControls.ExComboBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.tbDescription = new ExControls.ExTextBox();
            this.lForStations = new System.Windows.Forms.Label();
            this.tbForStations = new ExControls.ExTextBox();
            this.bForStations = new ExControls.ExButton();
            this.lNotStations = new System.Windows.Forms.Label();
            this.tbNotStations = new ExControls.ExTextBox();
            this.bNotStations = new ExControls.ExButton();
            this.lExclusion = new System.Windows.Forms.Label();
            this.cboExclusion = new ExControls.ExComboBox();
            this.lLanguage = new System.Windows.Forms.Label();
            this.cboLanguage = new ExControls.ExComboBox();
            this.lGuards = new System.Windows.Forms.Label();
            this.tbGuards = new ExControls.ExTextBox();
            this.bGuards = new ExControls.ExButton();
            this.cboxSection = new ExControls.ExCheckBox();
            this.pManual = new System.Windows.Forms.Panel();
            this.lManual = new System.Windows.Forms.Label();
            this.bOpenManual = new ExControls.ExButton();
            this.lbProblems = new GVXEditor.UI.Controls.ProblemList();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.lCheck = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            this.pFilter.SuspendLayout();
            this.tsReports.SuspendLayout();
            this.pReport.SuspendLayout();
            this.pSteps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSteps)).BeginInit();
            this.tsSteps.SuspendLayout();
            this.pPreview.SuspendLayout();
            this.pHeader.SuspendLayout();
            this.pManual.SuspendLayout();
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
            this.splitMain.Panel1.Controls.Add(this.dgvReports);
            this.splitMain.Panel1.Controls.Add(this.pFilter);
            this.splitMain.Panel1.Controls.Add(this.tsReports);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.pReport);
            this.splitMain.Panel2.Controls.Add(this.pManual);
            // 
            // dgvReports
            // 
            this.dgvReports.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cRepName,
            this.cRepKind,
            this.cRepDescription,
            this.cRepState,
            this.cRepRefs});
            resources.ApplyResources(this.dgvReports, "dgvReports");
            this.dgvReports.AllowUserToAddRows = false;
            this.dgvReports.AllowUserToDeleteRows = false;
            this.dgvReports.AllowUserToResizeRows = false;
            this.dgvReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReports.MultiSelect = false;
            this.dgvReports.RowHeadersVisible = false;
            this.dgvReports.ReadOnly = true;
            this.dgvReports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.SelectionChanged += new System.EventHandler(this.dgvReports_SelectionChanged);
            this.dgvReports.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvReports_CellFormatting);
            // 
            // pFilter
            // 
            this.pFilter.Controls.Add(this.lFilterKind);
            this.pFilter.Controls.Add(this.cboFilterKind);
            this.pFilter.Controls.Add(this.tbFilter);
            resources.ApplyResources(this.pFilter, "pFilter");
            this.pFilter.Name = "pFilter";
            // 
            // lFilterKind
            // 
            resources.ApplyResources(this.lFilterKind, "lFilterKind");
            this.lFilterKind.Name = "lFilterKind";
            // 
            // cboFilterKind
            // 
            resources.ApplyResources(this.cboFilterKind, "cboFilterKind");
            this.cboFilterKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterKind.FormattingEnabled = true;
            this.cboFilterKind.Name = "cboFilterKind";
            this.cboFilterKind.SelectedIndexChanged += new System.EventHandler(this.Filter_Changed);
            // 
            // tbFilter
            // 
            resources.ApplyResources(this.tbFilter, "tbFilter");
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.TextChanged += new System.EventHandler(this.Filter_Changed);
            // 
            // tsReports
            // 
            this.tsReports.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNew,
            this.tsbCopy,
            this.tsbDelete,
            this.tssReports1,
            this.tsbOpenCode});
            resources.ApplyResources(this.tsReports, "tsReports");
            this.tsReports.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsReports.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsReports.Name = "tsReports";
            // 
            // tsbNew
            // 
            resources.ApplyResources(this.tsbNew, "tsbNew");
            this.tsbNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNew.IconName = "add";
            this.tsbNew.Name = "tsbNew";
            this.tsbNew.Click += new System.EventHandler(this.tsbNew_Click);
            // 
            // tsbCopy
            // 
            resources.ApplyResources(this.tsbCopy, "tsbCopy");
            this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCopy.IconName = "document-copy";
            this.tsbCopy.Name = "tsbCopy";
            this.tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);
            // 
            // tsbDelete
            // 
            resources.ApplyResources(this.tsbDelete, "tsbDelete");
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelete.IconName = "delete";
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // tssReports1
            // 
            resources.ApplyResources(this.tssReports1, "tssReports1");
            this.tssReports1.Name = "tssReports1";
            // 
            // tsbOpenCode
            // 
            resources.ApplyResources(this.tsbOpenCode, "tsbOpenCode");
            this.tsbOpenCode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenCode.IconName = "code";
            this.tsbOpenCode.Name = "tsbOpenCode";
            this.tsbOpenCode.Click += new System.EventHandler(this.tsbOpenCode_Click);
            // 
            // pReport
            // 
            this.pReport.Controls.Add(this.pSteps);
            this.pReport.Controls.Add(this.pPreview);
            this.pReport.Controls.Add(this.pHeader);
            resources.ApplyResources(this.pReport, "pReport");
            this.pReport.Name = "pReport";
            // 
            // pSteps
            // 
            this.pSteps.Controls.Add(this.dgvSteps);
            this.pSteps.Controls.Add(this.tsSteps);
            this.pSteps.Controls.Add(this.lSteps);
            resources.ApplyResources(this.pSteps, "pSteps");
            this.pSteps.Name = "pSteps";
            // 
            // dgvSteps
            // 
            this.dgvSteps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cStepBlock,
            this.cStepCondition,
            this.cStepKind,
            this.cStepValue,
            this.cStepBefore,
            this.cStepAfter,
            this.cStepText,
            this.cStepComment});
            resources.ApplyResources(this.dgvSteps, "dgvSteps");
            this.dgvSteps.AllowUserToAddRows = false;
            this.dgvSteps.AllowUserToDeleteRows = false;
            this.dgvSteps.AllowUserToResizeRows = false;
            this.dgvSteps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSteps.RowHeadersVisible = false;
            this.dgvSteps.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvSteps.Name = "dgvSteps";
            this.dgvSteps.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Steps_CellValueChanged);
            this.dgvSteps.CurrentCellDirtyStateChanged += new System.EventHandler(this.Steps_CurrentCellDirtyStateChanged);
            this.dgvSteps.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Steps_DataError);
            this.dgvSteps.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Steps_CellFormatting);
            // 
            // tsSteps
            // 
            this.tsSteps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAddSounds,
            this.tsbAddPause,
            this.tsbAddBuiltin,
            this.tsbAddCall,
            this.tsbCondition,
            this.tsbToggleStep,
            this.tsbDeleteStep,
            this.tssSteps1,
            this.tsbStepUp,
            this.tsbStepDown});
            resources.ApplyResources(this.tsSteps, "tsSteps");
            this.tsSteps.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsSteps.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsSteps.Name = "tsSteps";
            // 
            // tsbAddSounds
            // 
            resources.ApplyResources(this.tsbAddSounds, "tsbAddSounds");
            this.tsbAddSounds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddSounds.IconName = "speaker";
            this.tsbAddSounds.IconBadge = "add";
            this.tsbAddSounds.Name = "tsbAddSounds";
            this.tsbAddSounds.Click += new System.EventHandler(this.tsbAddSounds_Click);
            // 
            // tsbAddPause
            // 
            resources.ApplyResources(this.tsbAddPause, "tsbAddPause");
            this.tsbAddPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddPause.IconName = "pause";
            this.tsbAddPause.IconBadge = "add";
            this.tsbAddPause.Name = "tsbAddPause";
            this.tsbAddPause.Click += new System.EventHandler(this.tsbAddPause_Click);
            // 
            // tsbAddBuiltin
            // 
            resources.ApplyResources(this.tsbAddBuiltin, "tsbAddBuiltin");
            this.tsbAddBuiltin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddBuiltin.IconName = "train";
            this.tsbAddBuiltin.IconBadge = "add";
            this.tsbAddBuiltin.Name = "tsbAddBuiltin";
            // 
            // tsbAddCall
            // 
            resources.ApplyResources(this.tsbAddCall, "tsbAddCall");
            this.tsbAddCall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddCall.IconName = "code";
            this.tsbAddCall.IconBadge = "add";
            this.tsbAddCall.Name = "tsbAddCall";
            this.tsbAddCall.Click += new System.EventHandler(this.tsbAddCall_Click);
            // 
            // tsbCondition
            // 
            resources.ApplyResources(this.tsbCondition, "tsbCondition");
            this.tsbCondition.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCondition.IconName = "filter";
            this.tsbCondition.Name = "tsbCondition";
            // 
            // tsbToggleStep
            // 
            resources.ApplyResources(this.tsbToggleStep, "tsbToggleStep");
            this.tsbToggleStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToggleStep.IconName = "eye";
            this.tsbToggleStep.Name = "tsbToggleStep";
            this.tsbToggleStep.Click += new System.EventHandler(this.tsbToggleStep_Click);
            // 
            // tsbDeleteStep
            // 
            resources.ApplyResources(this.tsbDeleteStep, "tsbDeleteStep");
            this.tsbDeleteStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteStep.IconName = "delete";
            this.tsbDeleteStep.Name = "tsbDeleteStep";
            this.tsbDeleteStep.Click += new System.EventHandler(this.tsbDeleteStep_Click);
            // 
            // tssSteps1
            // 
            resources.ApplyResources(this.tssSteps1, "tssSteps1");
            this.tssSteps1.Name = "tssSteps1";
            // 
            // tsbStepUp
            // 
            resources.ApplyResources(this.tsbStepUp, "tsbStepUp");
            this.tsbStepUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStepUp.IconName = "arrow-up";
            this.tsbStepUp.Name = "tsbStepUp";
            this.tsbStepUp.Click += new System.EventHandler(this.tsbStepUp_Click);
            // 
            // tsbStepDown
            // 
            resources.ApplyResources(this.tsbStepDown, "tsbStepDown");
            this.tsbStepDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStepDown.IconName = "arrow-down";
            this.tsbStepDown.Name = "tsbStepDown";
            this.tsbStepDown.Click += new System.EventHandler(this.tsbStepDown_Click);
            // 
            // lSteps
            // 
            resources.ApplyResources(this.lSteps, "lSteps");
            this.lSteps.Name = "lSteps";
            // 
            // pPreview
            // 
            this.pPreview.Controls.Add(this.tbPreview);
            this.pPreview.Controls.Add(this.lPreview);
            resources.ApplyResources(this.pPreview, "pPreview");
            this.pPreview.Name = "pPreview";
            // 
            // tbPreview
            // 
            resources.ApplyResources(this.tbPreview, "tbPreview");
            this.tbPreview.Multiline = true;
            this.tbPreview.ReadOnly = true;
            this.tbPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbPreview.Name = "tbPreview";
            // 
            // lPreview
            // 
            resources.ApplyResources(this.lPreview, "lPreview");
            this.lPreview.Name = "lPreview";
            // 
            // pHeader
            // 
            this.pHeader.Controls.Add(this.lName);
            this.pHeader.Controls.Add(this.tbName);
            this.pHeader.Controls.Add(this.lKind);
            this.pHeader.Controls.Add(this.cboKind);
            this.pHeader.Controls.Add(this.lDescription);
            this.pHeader.Controls.Add(this.tbDescription);
            this.pHeader.Controls.Add(this.lForStations);
            this.pHeader.Controls.Add(this.tbForStations);
            this.pHeader.Controls.Add(this.bForStations);
            this.pHeader.Controls.Add(this.lNotStations);
            this.pHeader.Controls.Add(this.tbNotStations);
            this.pHeader.Controls.Add(this.bNotStations);
            this.pHeader.Controls.Add(this.lExclusion);
            this.pHeader.Controls.Add(this.cboExclusion);
            this.pHeader.Controls.Add(this.lLanguage);
            this.pHeader.Controls.Add(this.cboLanguage);
            this.pHeader.Controls.Add(this.lGuards);
            this.pHeader.Controls.Add(this.tbGuards);
            this.pHeader.Controls.Add(this.bGuards);
            this.pHeader.Controls.Add(this.cboxSection);
            resources.ApplyResources(this.pHeader, "pHeader");
            this.pHeader.Name = "pHeader";
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
            this.tbName.TextChanged += new System.EventHandler(this.Report_Changed);
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
            this.cboKind.SelectedIndexChanged += new System.EventHandler(this.Report_Changed);
            // 
            // lDescription
            // 
            resources.ApplyResources(this.lDescription, "lDescription");
            this.lDescription.Name = "lDescription";
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.TextChanged += new System.EventHandler(this.Report_Changed);
            // 
            // lForStations
            // 
            resources.ApplyResources(this.lForStations, "lForStations");
            this.lForStations.Name = "lForStations";
            // 
            // tbForStations
            // 
            resources.ApplyResources(this.tbForStations, "tbForStations");
            this.tbForStations.Name = "tbForStations";
            this.tbForStations.TextChanged += new System.EventHandler(this.Report_Changed);
            // 
            // bForStations
            // 
            resources.ApplyResources(this.bForStations, "bForStations");
            this.bForStations.Name = "bForStations";
            this.bForStations.Click += new System.EventHandler(this.bForStations_Click);
            // 
            // lNotStations
            // 
            resources.ApplyResources(this.lNotStations, "lNotStations");
            this.lNotStations.Name = "lNotStations";
            // 
            // tbNotStations
            // 
            resources.ApplyResources(this.tbNotStations, "tbNotStations");
            this.tbNotStations.Name = "tbNotStations";
            this.tbNotStations.TextChanged += new System.EventHandler(this.Report_Changed);
            // 
            // bNotStations
            // 
            resources.ApplyResources(this.bNotStations, "bNotStations");
            this.bNotStations.Name = "bNotStations";
            this.bNotStations.Click += new System.EventHandler(this.bNotStations_Click);
            // 
            // lExclusion
            // 
            resources.ApplyResources(this.lExclusion, "lExclusion");
            this.lExclusion.Name = "lExclusion";
            // 
            // cboExclusion
            // 
            resources.ApplyResources(this.cboExclusion, "cboExclusion");
            this.cboExclusion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExclusion.FormattingEnabled = true;
            this.cboExclusion.Name = "cboExclusion";
            this.cboExclusion.SelectedIndexChanged += new System.EventHandler(this.Report_Changed);
            // 
            // lLanguage
            // 
            resources.ApplyResources(this.lLanguage, "lLanguage");
            this.lLanguage.Name = "lLanguage";
            // 
            // cboLanguage
            // 
            resources.ApplyResources(this.cboLanguage, "cboLanguage");
            this.cboLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.SelectedIndexChanged += new System.EventHandler(this.Report_Changed);
            // 
            // lGuards
            // 
            resources.ApplyResources(this.lGuards, "lGuards");
            this.lGuards.Name = "lGuards";
            // 
            // tbGuards
            // 
            resources.ApplyResources(this.tbGuards, "tbGuards");
            this.tbGuards.AcceptsReturn = true;
            this.tbGuards.Multiline = true;
            this.tbGuards.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbGuards.Name = "tbGuards";
            this.tbGuards.TextChanged += new System.EventHandler(this.Report_Changed);
            // 
            // bGuards
            // 
            resources.ApplyResources(this.bGuards, "bGuards");
            this.bGuards.Name = "bGuards";
            this.bGuards.Click += new System.EventHandler(this.bGuards_Click);
            // 
            // cboxSection
            // 
            resources.ApplyResources(this.cboxSection, "cboxSection");
            this.cboxSection.Name = "cboxSection";
            this.cboxSection.CheckedChanged += new System.EventHandler(this.Report_Changed);
            // 
            // pManual
            // 
            this.pManual.Controls.Add(this.lManual);
            this.pManual.Controls.Add(this.bOpenManual);
            resources.ApplyResources(this.pManual, "pManual");
            this.pManual.Visible = false;
            this.pManual.Name = "pManual";
            // 
            // lManual
            // 
            resources.ApplyResources(this.lManual, "lManual");
            this.lManual.Name = "lManual";
            // 
            // bOpenManual
            // 
            resources.ApplyResources(this.bOpenManual, "bOpenManual");
            this.bOpenManual.Name = "bOpenManual";
            this.bOpenManual.Click += new System.EventHandler(this.tsbOpenCode_Click);
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
            // FReports
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.lbProblems);
            this.Controls.Add(this.flpButtons);
            this.Name = "FReports";
            resources.ApplyResources(this, "$this");
            // 
            // cRepName
            // 
            resources.ApplyResources(this.cRepName, "cRepName");
            this.cRepName.DataPropertyName = "Name";
            this.cRepName.ReadOnly = true;
            this.cRepName.Name = "cRepName";
            // 
            // cRepKind
            // 
            resources.ApplyResources(this.cRepKind, "cRepKind");
            this.cRepKind.DataPropertyName = "KindText";
            this.cRepKind.ReadOnly = true;
            this.cRepKind.Name = "cRepKind";
            // 
            // cRepDescription
            // 
            resources.ApplyResources(this.cRepDescription, "cRepDescription");
            this.cRepDescription.DataPropertyName = "Description";
            this.cRepDescription.ReadOnly = true;
            this.cRepDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cRepDescription.Name = "cRepDescription";
            // 
            // cRepState
            // 
            resources.ApplyResources(this.cRepState, "cRepState");
            this.cRepState.DataPropertyName = "StateText";
            this.cRepState.ReadOnly = true;
            this.cRepState.Name = "cRepState";
            // 
            // cRepRefs
            // 
            resources.ApplyResources(this.cRepRefs, "cRepRefs");
            this.cRepRefs.DataPropertyName = "References";
            this.cRepRefs.ReadOnly = true;
            this.cRepRefs.Name = "cRepRefs";
            // 
            // cStepBlock
            // 
            resources.ApplyResources(this.cStepBlock, "cStepBlock");
            this.cStepBlock.DataPropertyName = "Block";
            this.cStepBlock.Name = "cStepBlock";
            // 
            // cStepCondition
            // 
            resources.ApplyResources(this.cStepCondition, "cStepCondition");
            this.cStepCondition.DataPropertyName = "Condition";
            this.cStepCondition.Name = "cStepCondition";
            // 
            // cStepKind
            // 
            resources.ApplyResources(this.cStepKind, "cStepKind");
            this.cStepKind.DataPropertyName = "Kind";
            this.cStepKind.Name = "cStepKind";
            // 
            // cStepValue
            // 
            resources.ApplyResources(this.cStepValue, "cStepValue");
            this.cStepValue.DataPropertyName = "Value";
            this.cStepValue.Name = "cStepValue";
            // 
            // cStepBefore
            // 
            resources.ApplyResources(this.cStepBefore, "cStepBefore");
            this.cStepBefore.DataPropertyName = "Before";
            this.cStepBefore.Name = "cStepBefore";
            // 
            // cStepAfter
            // 
            resources.ApplyResources(this.cStepAfter, "cStepAfter");
            this.cStepAfter.DataPropertyName = "After";
            this.cStepAfter.Name = "cStepAfter";
            // 
            // cStepText
            // 
            resources.ApplyResources(this.cStepText, "cStepText");
            this.cStepText.ReadOnly = true;
            this.cStepText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cStepText.Name = "cStepText";
            // 
            // cStepComment
            // 
            resources.ApplyResources(this.cStepComment, "cStepComment");
            this.cStepComment.DataPropertyName = "Comment";
            this.cStepComment.Name = "cStepComment";
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.pManual.ResumeLayout(false);
            this.pManual.PerformLayout();
            this.pHeader.ResumeLayout(false);
            this.pHeader.PerformLayout();
            this.pPreview.ResumeLayout(false);
            this.pPreview.PerformLayout();
            this.tsSteps.ResumeLayout(false);
            this.tsSteps.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSteps)).EndInit();
            this.pSteps.ResumeLayout(false);
            this.pSteps.PerformLayout();
            this.pReport.ResumeLayout(false);
            this.pReport.PerformLayout();
            this.tsReports.ResumeLayout(false);
            this.tsReports.PerformLayout();
            this.pFilter.ResumeLayout(false);
            this.pFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
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
        private System.Windows.Forms.DataGridView dgvReports;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepKind;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepState;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepRefs;
        private System.Windows.Forms.Panel pFilter;
        private System.Windows.Forms.Label lFilterKind;
        private ExControls.ExComboBox cboFilterKind;
        private ExControls.ExTextBox tbFilter;
        private System.Windows.Forms.ToolStrip tsReports;
        private ToolsCore.Icons.IconToolStripButton tsbNew;
        private ToolsCore.Icons.IconToolStripButton tsbCopy;
        private ToolsCore.Icons.IconToolStripButton tsbDelete;
        private System.Windows.Forms.ToolStripSeparator tssReports1;
        private ToolsCore.Icons.IconToolStripButton tsbOpenCode;
        private System.Windows.Forms.Panel pReport;
        private System.Windows.Forms.Panel pSteps;
        private System.Windows.Forms.DataGridView dgvSteps;
        private ExControls.DataGridViewExComboBoxColumn cStepBlock;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStepCondition;
        private ExControls.DataGridViewExComboBoxColumn cStepKind;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStepValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStepBefore;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStepAfter;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStepText;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStepComment;
        private System.Windows.Forms.ToolStrip tsSteps;
        private ToolsCore.Icons.IconToolStripButton tsbAddSounds;
        private ToolsCore.Icons.IconToolStripButton tsbAddPause;
        private ToolsCore.Icons.IconToolStripDropDownButton tsbAddBuiltin;
        private ToolsCore.Icons.IconToolStripButton tsbAddCall;
        private ToolsCore.Icons.IconToolStripDropDownButton tsbCondition;
        private ToolsCore.Icons.IconToolStripButton tsbToggleStep;
        private ToolsCore.Icons.IconToolStripButton tsbDeleteStep;
        private System.Windows.Forms.ToolStripSeparator tssSteps1;
        private ToolsCore.Icons.IconToolStripButton tsbStepUp;
        private ToolsCore.Icons.IconToolStripButton tsbStepDown;
        private System.Windows.Forms.Label lSteps;
        private System.Windows.Forms.Panel pPreview;
        private ExControls.ExTextBox tbPreview;
        private System.Windows.Forms.Label lPreview;
        private System.Windows.Forms.Panel pHeader;
        private System.Windows.Forms.Label lName;
        private ExControls.ExTextBox tbName;
        private System.Windows.Forms.Label lKind;
        private ExControls.ExComboBox cboKind;
        private System.Windows.Forms.Label lDescription;
        private ExControls.ExTextBox tbDescription;
        private System.Windows.Forms.Label lForStations;
        private ExControls.ExTextBox tbForStations;
        private ExControls.ExButton bForStations;
        private System.Windows.Forms.Label lNotStations;
        private ExControls.ExTextBox tbNotStations;
        private ExControls.ExButton bNotStations;
        private System.Windows.Forms.Label lExclusion;
        private ExControls.ExComboBox cboExclusion;
        private System.Windows.Forms.Label lLanguage;
        private ExControls.ExComboBox cboLanguage;
        private System.Windows.Forms.Label lGuards;
        private ExControls.ExTextBox tbGuards;
        private ExControls.ExButton bGuards;
        private ExControls.ExCheckBox cboxSection;
        private System.Windows.Forms.Panel pManual;
        private System.Windows.Forms.Label lManual;
        private ExControls.ExButton bOpenManual;
        private GVXEditor.UI.Controls.ProblemList lbProblems;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private ExControls.ExButton bCancel;
        private ExControls.ExButton bOk;
        private System.Windows.Forms.Label lCheck;
    }
}
