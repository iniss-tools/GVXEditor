namespace GVXEditor.UI.EditTrain
{
    partial class FEditTrain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FEditTrain));
            this.tcMain = new ExControls.ExTabControl();
            this.tpTrain = new System.Windows.Forms.TabPage();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitVariants = new System.Windows.Forms.SplitContainer();
            this.dgvVariants = new System.Windows.Forms.DataGridView();
            this.cVarIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVarValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVarDays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsVariants = new System.Windows.Forms.ToolStrip();
            this.tsbVarAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbVarCopy = new ToolsCore.Icons.IconToolStripButton();
            this.tsbVarDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssVar1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbVarUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbVarDown = new ToolsCore.Icons.IconToolStripButton();
            this.pVariant = new System.Windows.Forms.Panel();
            this.lValidity = new System.Windows.Forms.Label();
            this.tbValidity = new ExControls.ExTextBox();
            this.bValidity = new ExControls.ExButton();
            this.lOperator = new System.Windows.Forms.Label();
            this.tbOperator = new ExControls.ExTextBox();
            this.lFlags = new System.Windows.Forms.Label();
            this.flpFlags = new System.Windows.Forms.FlowLayoutPanel();
            this.cboxFlagD = new ExControls.ExCheckBox();
            this.cboxFlagX = new ExControls.ExCheckBox();
            this.cboxFlagN = new ExControls.ExCheckBox();
            this.cboxFlagNP = new ExControls.ExCheckBox();
            this.cboxFlagO = new ExControls.ExCheckBox();
            this.cboxFlagM = new ExControls.ExCheckBox();
            this.cboxFlagR = new ExControls.ExCheckBox();
            this.cboxFlagL = new ExControls.ExCheckBox();
            this.lNote = new System.Windows.Forms.Label();
            this.tbNote = new ExControls.ExTextBox();
            this.calendarStrip = new ToolsCore.Forms.Calendar.CalendarStrip();
            this.dgvStops = new System.Windows.Forms.DataGridView();
            this.cStation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStationName = new ExControls.DataGridViewExComboBoxColumn();
            this.cArrival = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDeparture = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cPassing = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cTransfer = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cOperating = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cRequest = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDisplay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cType = new ExControls.DataGridViewExComboBoxColumn();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cReplacement = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsStops = new System.Windows.Forms.ToolStrip();
            this.tsbStopInsert = new ToolsCore.Icons.IconToolStripButton();
            this.tsbStopAppend = new ToolsCore.Icons.IconToolStripButton();
            this.tsbStopDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssStop1 = new System.Windows.Forms.ToolStripSeparator();
            this.tslStopsHint = new System.Windows.Forms.ToolStripLabel();
            this.flpPeriod = new System.Windows.Forms.FlowLayoutPanel();
            this.lFrom = new System.Windows.Forms.Label();
            this.dtpFrom = new ExControls.ExDateTimePicker();
            this.lTo = new System.Windows.Forms.Label();
            this.dtpTo = new ExControls.ExDateTimePicker();
            this.cboxNoEnd = new ExControls.ExCheckBox();
            this.tpStations = new System.Windows.Forms.TabPage();
            this.tcStation = new ExControls.ExTabControl();
            this.tpPositions = new System.Windows.Forms.TabPage();
            this.dgvPositions = new System.Windows.Forms.DataGridView();
            this.cPosValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cPosArrival = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cPosDeparture = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cPosTracks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cPosDay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tsPositions = new System.Windows.Forms.ToolStrip();
            this.tsbPositionsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbPositionsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssPositions1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbPositionsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbPositionsDown = new ToolsCore.Icons.IconToolStripButton();
            this.tpAdditions = new System.Windows.Forms.TabPage();
            this.dgvAdditions = new System.Windows.Forms.DataGridView();
            this.cAddValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cAddKey = new ExControls.DataGridViewExComboBoxColumn();
            this.cAddReports = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cAddDay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tsAdditions = new System.Windows.Forms.ToolStrip();
            this.tsbAdditionsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbAdditionsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssAdditions1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAdditionsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbAdditionsDown = new ToolsCore.Icons.IconToolStripButton();
            this.tssAdditions2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAdditionsNew = new ToolsCore.Icons.IconToolStripButton();
            this.tpCompositions = new System.Windows.Forms.TabPage();
            this.dgvCompositions = new System.Windows.Forms.DataGridView();
            this.cCompValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCompEnd = new ExControls.DataGridViewExComboBoxColumn();
            this.cCompReports = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCompSounds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCompDay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tsCompositions = new System.Windows.Forms.ToolStrip();
            this.tsbCompositionsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbCompositionsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssCompositions1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCompositionsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbCompositionsDown = new ToolsCore.Icons.IconToolStripButton();
            this.tpLanguages = new System.Windows.Forms.TabPage();
            this.dgvLanguages = new System.Windows.Forms.DataGridView();
            this.cLangValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLangLanguages = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLangDay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tsLanguages = new System.Windows.Forms.ToolStrip();
            this.tsbLanguagesAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbLanguagesDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssLanguages1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLanguagesUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbLanguagesDown = new ToolsCore.Icons.IconToolStripButton();
            this.tpReportStations = new System.Windows.Forms.TabPage();
            this.dgvReportStations = new System.Windows.Forms.DataGridView();
            this.cRepValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepShortFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepLongFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepShortTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepLongTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cRepDay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tsReportStations = new System.Windows.Forms.ToolStrip();
            this.tsbReportStationsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbReportStationsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssReportStations1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbReportStationsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbReportStationsDown = new ToolsCore.Icons.IconToolStripButton();
            this.tpTexts = new System.Windows.Forms.TabPage();
            this.dgvTexts = new System.Windows.Forms.DataGridView();
            this.cTextSet = new ExControls.DataGridViewExComboBoxColumn();
            this.cTextValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTextText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTextFont = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTextDay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tsTexts = new System.Windows.Forms.ToolStrip();
            this.tsbTextsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbTextsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssTexts1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbTextsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbTextsDown = new ToolsCore.Icons.IconToolStripButton();
            this.tpMos = new System.Windows.Forms.TabPage();
            this.dgvMos = new System.Windows.Forms.DataGridView();
            this.cMosValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsMos = new System.Windows.Forms.ToolStrip();
            this.tsbMosAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbMosDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssMos1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbMosUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbMosDown = new ToolsCore.Icons.IconToolStripButton();
            this.flpStation = new System.Windows.Forms.FlowLayoutPanel();
            this.lStation = new System.Windows.Forms.Label();
            this.cboStation = new ExControls.ExComboBox();
            this.lStationInfo = new System.Windows.Forms.Label();
            this.tpNotices = new System.Windows.Forms.TabPage();
            this.splitNotices = new System.Windows.Forms.SplitContainer();
            this.dgvLockouts = new System.Windows.Forms.DataGridView();
            this.cLockValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLockKey = new ExControls.DataGridViewExComboBoxColumn();
            this.cLockFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLockTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLockDay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tsLockouts = new System.Windows.Forms.ToolStrip();
            this.tsbLockoutsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbLockoutsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssLockouts1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLockoutsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbLockoutsDown = new ToolsCore.Icons.IconToolStripButton();
            this.lLockouts = new System.Windows.Forms.Label();
            this.dgvDiversions = new System.Windows.Forms.DataGridView();
            this.cDivStation = new ExControls.DataGridViewExComboBoxColumn();
            this.cDivValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDivKey = new ExControls.DataGridViewExComboBoxColumn();
            this.cDivFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDivTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDivDay = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tsDiversions = new System.Windows.Forms.ToolStrip();
            this.tsbDiversionsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDiversionsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssDiversions1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDiversionsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDiversionsDown = new ToolsCore.Icons.IconToolStripButton();
            this.lDiversions = new System.Windows.Forms.Label();
            this.lbProblems = new GVXEditor.UI.Controls.ProblemList();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.tcMain.SuspendLayout();
            this.tpTrain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitVariants)).BeginInit();
            this.splitVariants.Panel1.SuspendLayout();
            this.splitVariants.Panel2.SuspendLayout();
            this.splitVariants.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariants)).BeginInit();
            this.tsVariants.SuspendLayout();
            this.pVariant.SuspendLayout();
            this.flpFlags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStops)).BeginInit();
            this.tsStops.SuspendLayout();
            this.flpPeriod.SuspendLayout();
            this.tpStations.SuspendLayout();
            this.tcStation.SuspendLayout();
            this.tpPositions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPositions)).BeginInit();
            this.tsPositions.SuspendLayout();
            this.tpAdditions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdditions)).BeginInit();
            this.tsAdditions.SuspendLayout();
            this.tpCompositions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCompositions)).BeginInit();
            this.tsCompositions.SuspendLayout();
            this.tpLanguages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLanguages)).BeginInit();
            this.tsLanguages.SuspendLayout();
            this.tpReportStations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportStations)).BeginInit();
            this.tsReportStations.SuspendLayout();
            this.tpTexts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTexts)).BeginInit();
            this.tsTexts.SuspendLayout();
            this.tpMos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMos)).BeginInit();
            this.tsMos.SuspendLayout();
            this.flpStation.SuspendLayout();
            this.tpNotices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitNotices)).BeginInit();
            this.splitNotices.Panel1.SuspendLayout();
            this.splitNotices.Panel2.SuspendLayout();
            this.splitNotices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLockouts)).BeginInit();
            this.tsLockouts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDiversions)).BeginInit();
            this.tsDiversions.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpTrain);
            this.tcMain.Controls.Add(this.tpStations);
            this.tcMain.Controls.Add(this.tpNotices);
            resources.ApplyResources(this.tcMain, "tcMain");
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndexChanged += new System.EventHandler(this.tcMain_SelectedIndexChanged);
            // 
            // tpTrain
            // 
            this.tpTrain.Controls.Add(this.splitMain);
            this.tpTrain.Controls.Add(this.flpPeriod);
            resources.ApplyResources(this.tpTrain, "tpTrain");
            this.tpTrain.UseVisualStyleBackColor = true;
            this.tpTrain.Name = "tpTrain";
            // 
            // splitMain
            // 
            resources.ApplyResources(this.splitMain, "splitMain");
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.splitVariants);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.dgvStops);
            this.splitMain.Panel2.Controls.Add(this.tsStops);
            // 
            // splitVariants
            // 
            resources.ApplyResources(this.splitVariants, "splitVariants");
            this.splitVariants.Name = "splitVariants";
            // 
            // splitVariants.Panel1
            // 
            this.splitVariants.Panel1.Controls.Add(this.dgvVariants);
            this.splitVariants.Panel1.Controls.Add(this.tsVariants);
            // 
            // splitVariants.Panel2
            // 
            this.splitVariants.Panel2.Controls.Add(this.pVariant);
            // 
            // dgvVariants
            // 
            this.dgvVariants.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cVarIndex,
            this.cVarValidity,
            this.cVarDays});
            resources.ApplyResources(this.dgvVariants, "dgvVariants");
            this.dgvVariants.AllowUserToAddRows = false;
            this.dgvVariants.AllowUserToDeleteRows = false;
            this.dgvVariants.AllowUserToResizeRows = false;
            this.dgvVariants.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVariants.MultiSelect = false;
            this.dgvVariants.RowHeadersVisible = false;
            this.dgvVariants.ReadOnly = true;
            this.dgvVariants.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVariants.Name = "dgvVariants";
            this.dgvVariants.SelectionChanged += new System.EventHandler(this.dgvVariants_SelectionChanged);
            this.dgvVariants.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvVariants_CellFormatting);
            // 
            // tsVariants
            // 
            this.tsVariants.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbVarAdd,
            this.tsbVarCopy,
            this.tsbVarDelete,
            this.tssVar1,
            this.tsbVarUp,
            this.tsbVarDown});
            resources.ApplyResources(this.tsVariants, "tsVariants");
            this.tsVariants.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsVariants.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsVariants.Name = "tsVariants";
            // 
            // tsbVarAdd
            // 
            resources.ApplyResources(this.tsbVarAdd, "tsbVarAdd");
            this.tsbVarAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbVarAdd.IconName = "add";
            this.tsbVarAdd.Name = "tsbVarAdd";
            this.tsbVarAdd.Click += new System.EventHandler(this.tsbVarAdd_Click);
            // 
            // tsbVarCopy
            // 
            resources.ApplyResources(this.tsbVarCopy, "tsbVarCopy");
            this.tsbVarCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbVarCopy.IconName = "document-copy";
            this.tsbVarCopy.Name = "tsbVarCopy";
            this.tsbVarCopy.Click += new System.EventHandler(this.tsbVarCopy_Click);
            // 
            // tsbVarDelete
            // 
            resources.ApplyResources(this.tsbVarDelete, "tsbVarDelete");
            this.tsbVarDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbVarDelete.IconName = "delete";
            this.tsbVarDelete.Name = "tsbVarDelete";
            this.tsbVarDelete.Click += new System.EventHandler(this.tsbVarDelete_Click);
            // 
            // tssVar1
            // 
            resources.ApplyResources(this.tssVar1, "tssVar1");
            this.tssVar1.Name = "tssVar1";
            // 
            // tsbVarUp
            // 
            resources.ApplyResources(this.tsbVarUp, "tsbVarUp");
            this.tsbVarUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbVarUp.IconName = "arrow-up";
            this.tsbVarUp.Name = "tsbVarUp";
            this.tsbVarUp.Click += new System.EventHandler(this.tsbVarUp_Click);
            // 
            // tsbVarDown
            // 
            resources.ApplyResources(this.tsbVarDown, "tsbVarDown");
            this.tsbVarDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbVarDown.IconName = "arrow-down";
            this.tsbVarDown.Name = "tsbVarDown";
            this.tsbVarDown.Click += new System.EventHandler(this.tsbVarDown_Click);
            // 
            // pVariant
            // 
            this.pVariant.Controls.Add(this.lValidity);
            this.pVariant.Controls.Add(this.tbValidity);
            this.pVariant.Controls.Add(this.bValidity);
            this.pVariant.Controls.Add(this.lOperator);
            this.pVariant.Controls.Add(this.tbOperator);
            this.pVariant.Controls.Add(this.lFlags);
            this.pVariant.Controls.Add(this.flpFlags);
            this.pVariant.Controls.Add(this.lNote);
            this.pVariant.Controls.Add(this.tbNote);
            this.pVariant.Controls.Add(this.calendarStrip);
            resources.ApplyResources(this.pVariant, "pVariant");
            this.pVariant.Name = "pVariant";
            // 
            // lValidity
            // 
            resources.ApplyResources(this.lValidity, "lValidity");
            this.lValidity.Name = "lValidity";
            // 
            // tbValidity
            // 
            resources.ApplyResources(this.tbValidity, "tbValidity");
            this.tbValidity.Name = "tbValidity";
            this.tbValidity.TextChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // bValidity
            // 
            resources.ApplyResources(this.bValidity, "bValidity");
            this.bValidity.Name = "bValidity";
            this.bValidity.Click += new System.EventHandler(this.bValidity_Click);
            // 
            // lOperator
            // 
            resources.ApplyResources(this.lOperator, "lOperator");
            this.lOperator.Name = "lOperator";
            // 
            // tbOperator
            // 
            resources.ApplyResources(this.tbOperator, "tbOperator");
            this.tbOperator.Name = "tbOperator";
            this.tbOperator.TextChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // lFlags
            // 
            resources.ApplyResources(this.lFlags, "lFlags");
            this.lFlags.Name = "lFlags";
            // 
            // flpFlags
            // 
            this.flpFlags.Controls.Add(this.cboxFlagD);
            this.flpFlags.Controls.Add(this.cboxFlagX);
            this.flpFlags.Controls.Add(this.cboxFlagN);
            this.flpFlags.Controls.Add(this.cboxFlagNP);
            this.flpFlags.Controls.Add(this.cboxFlagO);
            this.flpFlags.Controls.Add(this.cboxFlagM);
            this.flpFlags.Controls.Add(this.cboxFlagR);
            this.flpFlags.Controls.Add(this.cboxFlagL);
            resources.ApplyResources(this.flpFlags, "flpFlags");
            this.flpFlags.Name = "flpFlags";
            // 
            // cboxFlagD
            // 
            resources.ApplyResources(this.cboxFlagD, "cboxFlagD");
            this.cboxFlagD.Name = "cboxFlagD";
            this.cboxFlagD.CheckedChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // cboxFlagX
            // 
            resources.ApplyResources(this.cboxFlagX, "cboxFlagX");
            this.cboxFlagX.Name = "cboxFlagX";
            this.cboxFlagX.CheckedChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // cboxFlagN
            // 
            resources.ApplyResources(this.cboxFlagN, "cboxFlagN");
            this.cboxFlagN.Name = "cboxFlagN";
            this.cboxFlagN.CheckedChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // cboxFlagNP
            // 
            resources.ApplyResources(this.cboxFlagNP, "cboxFlagNP");
            this.cboxFlagNP.Name = "cboxFlagNP";
            this.cboxFlagNP.CheckedChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // cboxFlagO
            // 
            resources.ApplyResources(this.cboxFlagO, "cboxFlagO");
            this.cboxFlagO.Name = "cboxFlagO";
            this.cboxFlagO.CheckedChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // cboxFlagM
            // 
            resources.ApplyResources(this.cboxFlagM, "cboxFlagM");
            this.cboxFlagM.Name = "cboxFlagM";
            this.cboxFlagM.CheckedChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // cboxFlagR
            // 
            resources.ApplyResources(this.cboxFlagR, "cboxFlagR");
            this.cboxFlagR.Name = "cboxFlagR";
            this.cboxFlagR.CheckedChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // cboxFlagL
            // 
            resources.ApplyResources(this.cboxFlagL, "cboxFlagL");
            this.cboxFlagL.Name = "cboxFlagL";
            this.cboxFlagL.CheckedChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // lNote
            // 
            resources.ApplyResources(this.lNote, "lNote");
            this.lNote.Name = "lNote";
            // 
            // tbNote
            // 
            resources.ApplyResources(this.tbNote, "tbNote");
            this.tbNote.Name = "tbNote";
            this.tbNote.TextChanged += new System.EventHandler(this.Variant_Changed);
            // 
            // calendarStrip
            // 
            resources.ApplyResources(this.calendarStrip, "calendarStrip");
            this.calendarStrip.Name = "calendarStrip";
            // 
            // dgvStops
            // 
            this.dgvStops.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cStation,
            this.cStationName,
            this.cArrival,
            this.cDeparture,
            this.cPassing,
            this.cTransfer,
            this.cOperating,
            this.cRequest,
            this.cNumber,
            this.cDisplay,
            this.cType,
            this.cName,
            this.cLine,
            this.cReplacement});
            resources.ApplyResources(this.dgvStops, "dgvStops");
            this.dgvStops.AllowUserToAddRows = false;
            this.dgvStops.AllowUserToDeleteRows = false;
            this.dgvStops.AllowUserToResizeRows = false;
            this.dgvStops.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStops.MultiSelect = false;
            this.dgvStops.RowHeadersVisible = false;
            this.dgvStops.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvStops.Name = "dgvStops";
            this.dgvStops.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStops_CellValueChanged);
            this.dgvStops.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvStops_CurrentCellDirtyStateChanged);
            // 
            // tsStops
            // 
            this.tsStops.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbStopInsert,
            this.tsbStopAppend,
            this.tsbStopDelete,
            this.tssStop1,
            this.tslStopsHint});
            resources.ApplyResources(this.tsStops, "tsStops");
            this.tsStops.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsStops.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsStops.Name = "tsStops";
            // 
            // tsbStopInsert
            // 
            resources.ApplyResources(this.tsbStopInsert, "tsbStopInsert");
            this.tsbStopInsert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStopInsert.IconName = "add";
            this.tsbStopInsert.Name = "tsbStopInsert";
            this.tsbStopInsert.Click += new System.EventHandler(this.tsbStopInsert_Click);
            // 
            // tsbStopAppend
            // 
            resources.ApplyResources(this.tsbStopAppend, "tsbStopAppend");
            this.tsbStopAppend.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStopAppend.IconName = "arrow-down";
            this.tsbStopAppend.Name = "tsbStopAppend";
            this.tsbStopAppend.Click += new System.EventHandler(this.tsbStopAppend_Click);
            // 
            // tsbStopDelete
            // 
            resources.ApplyResources(this.tsbStopDelete, "tsbStopDelete");
            this.tsbStopDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStopDelete.IconName = "delete";
            this.tsbStopDelete.Name = "tsbStopDelete";
            this.tsbStopDelete.Click += new System.EventHandler(this.tsbStopDelete_Click);
            // 
            // tssStop1
            // 
            resources.ApplyResources(this.tssStop1, "tssStop1");
            this.tssStop1.Name = "tssStop1";
            // 
            // tslStopsHint
            // 
            resources.ApplyResources(this.tslStopsHint, "tslStopsHint");
            this.tslStopsHint.Name = "tslStopsHint";
            // 
            // flpPeriod
            // 
            this.flpPeriod.Controls.Add(this.lFrom);
            this.flpPeriod.Controls.Add(this.dtpFrom);
            this.flpPeriod.Controls.Add(this.lTo);
            this.flpPeriod.Controls.Add(this.dtpTo);
            this.flpPeriod.Controls.Add(this.cboxNoEnd);
            resources.ApplyResources(this.flpPeriod, "flpPeriod");
            this.flpPeriod.Name = "flpPeriod";
            // 
            // lFrom
            // 
            resources.ApplyResources(this.lFrom, "lFrom");
            this.lFrom.Name = "lFrom";
            // 
            // dtpFrom
            // 
            resources.ApplyResources(this.dtpFrom, "dtpFrom");
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.ValueChanged += new System.EventHandler(this.Period_Changed);
            // 
            // lTo
            // 
            resources.ApplyResources(this.lTo, "lTo");
            this.lTo.Name = "lTo";
            // 
            // dtpTo
            // 
            resources.ApplyResources(this.dtpTo, "dtpTo");
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.ValueChanged += new System.EventHandler(this.Period_Changed);
            // 
            // cboxNoEnd
            // 
            resources.ApplyResources(this.cboxNoEnd, "cboxNoEnd");
            this.cboxNoEnd.Name = "cboxNoEnd";
            this.cboxNoEnd.CheckedChanged += new System.EventHandler(this.Period_Changed);
            // 
            // tpStations
            // 
            this.tpStations.Controls.Add(this.tcStation);
            this.tpStations.Controls.Add(this.flpStation);
            resources.ApplyResources(this.tpStations, "tpStations");
            this.tpStations.UseVisualStyleBackColor = true;
            this.tpStations.Name = "tpStations";
            // 
            // tcStation
            // 
            this.tcStation.Controls.Add(this.tpPositions);
            this.tcStation.Controls.Add(this.tpAdditions);
            this.tcStation.Controls.Add(this.tpCompositions);
            this.tcStation.Controls.Add(this.tpLanguages);
            this.tcStation.Controls.Add(this.tpReportStations);
            this.tcStation.Controls.Add(this.tpTexts);
            this.tcStation.Controls.Add(this.tpMos);
            resources.ApplyResources(this.tcStation, "tcStation");
            this.tcStation.SelectedIndex = 0;
            this.tcStation.Name = "tcStation";
            // 
            // tpPositions
            // 
            this.tpPositions.Controls.Add(this.dgvPositions);
            this.tpPositions.Controls.Add(this.tsPositions);
            resources.ApplyResources(this.tpPositions, "tpPositions");
            this.tpPositions.UseVisualStyleBackColor = true;
            this.tpPositions.Name = "tpPositions";
            // 
            // dgvPositions
            // 
            this.dgvPositions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cPosValidity,
            this.cPosArrival,
            this.cPosDeparture,
            this.cPosTracks,
            this.cPosDay});
            resources.ApplyResources(this.dgvPositions, "dgvPositions");
            this.dgvPositions.AllowUserToAddRows = false;
            this.dgvPositions.AllowUserToDeleteRows = false;
            this.dgvPositions.AllowUserToResizeRows = false;
            this.dgvPositions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPositions.MultiSelect = false;
            this.dgvPositions.RowHeadersVisible = false;
            this.dgvPositions.Name = "dgvPositions";
            this.dgvPositions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvPositions.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvPositions.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvPositions.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsPositions
            // 
            this.tsPositions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbPositionsAdd,
            this.tsbPositionsDelete,
            this.tssPositions1,
            this.tsbPositionsUp,
            this.tsbPositionsDown});
            resources.ApplyResources(this.tsPositions, "tsPositions");
            this.tsPositions.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsPositions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsPositions.Name = "tsPositions";
            // 
            // tsbPositionsAdd
            // 
            resources.ApplyResources(this.tsbPositionsAdd, "tsbPositionsAdd");
            this.tsbPositionsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPositionsAdd.IconName = "add";
            this.tsbPositionsAdd.Name = "tsbPositionsAdd";
            this.tsbPositionsAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbPositionsDelete
            // 
            resources.ApplyResources(this.tsbPositionsDelete, "tsbPositionsDelete");
            this.tsbPositionsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPositionsDelete.IconName = "delete";
            this.tsbPositionsDelete.Name = "tsbPositionsDelete";
            this.tsbPositionsDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssPositions1
            // 
            resources.ApplyResources(this.tssPositions1, "tssPositions1");
            this.tssPositions1.Name = "tssPositions1";
            // 
            // tsbPositionsUp
            // 
            resources.ApplyResources(this.tsbPositionsUp, "tsbPositionsUp");
            this.tsbPositionsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPositionsUp.IconName = "arrow-up";
            this.tsbPositionsUp.Name = "tsbPositionsUp";
            this.tsbPositionsUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbPositionsDown
            // 
            resources.ApplyResources(this.tsbPositionsDown, "tsbPositionsDown");
            this.tsbPositionsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPositionsDown.IconName = "arrow-down";
            this.tsbPositionsDown.Name = "tsbPositionsDown";
            this.tsbPositionsDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // tpAdditions
            // 
            this.tpAdditions.Controls.Add(this.dgvAdditions);
            this.tpAdditions.Controls.Add(this.tsAdditions);
            resources.ApplyResources(this.tpAdditions, "tpAdditions");
            this.tpAdditions.UseVisualStyleBackColor = true;
            this.tpAdditions.Name = "tpAdditions";
            // 
            // dgvAdditions
            // 
            this.dgvAdditions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cAddValidity,
            this.cAddKey,
            this.cAddReports,
            this.cAddDay});
            resources.ApplyResources(this.dgvAdditions, "dgvAdditions");
            this.dgvAdditions.AllowUserToAddRows = false;
            this.dgvAdditions.AllowUserToDeleteRows = false;
            this.dgvAdditions.AllowUserToResizeRows = false;
            this.dgvAdditions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAdditions.MultiSelect = false;
            this.dgvAdditions.RowHeadersVisible = false;
            this.dgvAdditions.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvAdditions.Name = "dgvAdditions";
            this.dgvAdditions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvAdditions.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvAdditions.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvAdditions.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsAdditions
            // 
            this.tsAdditions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAdditionsAdd,
            this.tsbAdditionsDelete,
            this.tssAdditions1,
            this.tsbAdditionsUp,
            this.tsbAdditionsDown,
            this.tssAdditions2,
            this.tsbAdditionsNew});
            resources.ApplyResources(this.tsAdditions, "tsAdditions");
            this.tsAdditions.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsAdditions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsAdditions.Name = "tsAdditions";
            // 
            // tsbAdditionsAdd
            // 
            resources.ApplyResources(this.tsbAdditionsAdd, "tsbAdditionsAdd");
            this.tsbAdditionsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAdditionsAdd.IconName = "add";
            this.tsbAdditionsAdd.Name = "tsbAdditionsAdd";
            this.tsbAdditionsAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbAdditionsDelete
            // 
            resources.ApplyResources(this.tsbAdditionsDelete, "tsbAdditionsDelete");
            this.tsbAdditionsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAdditionsDelete.IconName = "delete";
            this.tsbAdditionsDelete.Name = "tsbAdditionsDelete";
            this.tsbAdditionsDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssAdditions1
            // 
            resources.ApplyResources(this.tssAdditions1, "tssAdditions1");
            this.tssAdditions1.Name = "tssAdditions1";
            // 
            // tsbAdditionsUp
            // 
            resources.ApplyResources(this.tsbAdditionsUp, "tsbAdditionsUp");
            this.tsbAdditionsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAdditionsUp.IconName = "arrow-up";
            this.tsbAdditionsUp.Name = "tsbAdditionsUp";
            this.tsbAdditionsUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbAdditionsDown
            // 
            resources.ApplyResources(this.tsbAdditionsDown, "tsbAdditionsDown");
            this.tsbAdditionsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAdditionsDown.IconName = "arrow-down";
            this.tsbAdditionsDown.Name = "tsbAdditionsDown";
            this.tsbAdditionsDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // tssAdditions2
            // 
            resources.ApplyResources(this.tssAdditions2, "tssAdditions2");
            this.tssAdditions2.Name = "tssAdditions2";
            // 
            // tsbAdditionsNew
            // 
            resources.ApplyResources(this.tsbAdditionsNew, "tsbAdditionsNew");
            this.tsbAdditionsNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAdditionsNew.IconName = "speaker-edit";
            this.tsbAdditionsNew.IconBadge = "add";
            this.tsbAdditionsNew.Name = "tsbAdditionsNew";
            this.tsbAdditionsNew.Click += new System.EventHandler(this.tsbAdditionsNew_Click);
            // 
            // tpCompositions
            // 
            this.tpCompositions.Controls.Add(this.dgvCompositions);
            this.tpCompositions.Controls.Add(this.tsCompositions);
            resources.ApplyResources(this.tpCompositions, "tpCompositions");
            this.tpCompositions.UseVisualStyleBackColor = true;
            this.tpCompositions.Name = "tpCompositions";
            // 
            // dgvCompositions
            // 
            this.dgvCompositions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cCompValidity,
            this.cCompEnd,
            this.cCompReports,
            this.cCompSounds,
            this.cCompDay});
            resources.ApplyResources(this.dgvCompositions, "dgvCompositions");
            this.dgvCompositions.AllowUserToAddRows = false;
            this.dgvCompositions.AllowUserToDeleteRows = false;
            this.dgvCompositions.AllowUserToResizeRows = false;
            this.dgvCompositions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCompositions.MultiSelect = false;
            this.dgvCompositions.RowHeadersVisible = false;
            this.dgvCompositions.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvCompositions.Name = "dgvCompositions";
            this.dgvCompositions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvCompositions.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvCompositions.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvCompositions.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsCompositions
            // 
            this.tsCompositions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCompositionsAdd,
            this.tsbCompositionsDelete,
            this.tssCompositions1,
            this.tsbCompositionsUp,
            this.tsbCompositionsDown});
            resources.ApplyResources(this.tsCompositions, "tsCompositions");
            this.tsCompositions.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsCompositions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsCompositions.Name = "tsCompositions";
            // 
            // tsbCompositionsAdd
            // 
            resources.ApplyResources(this.tsbCompositionsAdd, "tsbCompositionsAdd");
            this.tsbCompositionsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCompositionsAdd.IconName = "add";
            this.tsbCompositionsAdd.Name = "tsbCompositionsAdd";
            this.tsbCompositionsAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbCompositionsDelete
            // 
            resources.ApplyResources(this.tsbCompositionsDelete, "tsbCompositionsDelete");
            this.tsbCompositionsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCompositionsDelete.IconName = "delete";
            this.tsbCompositionsDelete.Name = "tsbCompositionsDelete";
            this.tsbCompositionsDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssCompositions1
            // 
            resources.ApplyResources(this.tssCompositions1, "tssCompositions1");
            this.tssCompositions1.Name = "tssCompositions1";
            // 
            // tsbCompositionsUp
            // 
            resources.ApplyResources(this.tsbCompositionsUp, "tsbCompositionsUp");
            this.tsbCompositionsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCompositionsUp.IconName = "arrow-up";
            this.tsbCompositionsUp.Name = "tsbCompositionsUp";
            this.tsbCompositionsUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbCompositionsDown
            // 
            resources.ApplyResources(this.tsbCompositionsDown, "tsbCompositionsDown");
            this.tsbCompositionsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCompositionsDown.IconName = "arrow-down";
            this.tsbCompositionsDown.Name = "tsbCompositionsDown";
            this.tsbCompositionsDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // tpLanguages
            // 
            this.tpLanguages.Controls.Add(this.dgvLanguages);
            this.tpLanguages.Controls.Add(this.tsLanguages);
            resources.ApplyResources(this.tpLanguages, "tpLanguages");
            this.tpLanguages.UseVisualStyleBackColor = true;
            this.tpLanguages.Name = "tpLanguages";
            // 
            // dgvLanguages
            // 
            this.dgvLanguages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cLangValidity,
            this.cLangLanguages,
            this.cLangDay});
            resources.ApplyResources(this.dgvLanguages, "dgvLanguages");
            this.dgvLanguages.AllowUserToAddRows = false;
            this.dgvLanguages.AllowUserToDeleteRows = false;
            this.dgvLanguages.AllowUserToResizeRows = false;
            this.dgvLanguages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLanguages.MultiSelect = false;
            this.dgvLanguages.RowHeadersVisible = false;
            this.dgvLanguages.Name = "dgvLanguages";
            this.dgvLanguages.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvLanguages.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvLanguages.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvLanguages.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsLanguages
            // 
            this.tsLanguages.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLanguagesAdd,
            this.tsbLanguagesDelete,
            this.tssLanguages1,
            this.tsbLanguagesUp,
            this.tsbLanguagesDown});
            resources.ApplyResources(this.tsLanguages, "tsLanguages");
            this.tsLanguages.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsLanguages.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsLanguages.Name = "tsLanguages";
            // 
            // tsbLanguagesAdd
            // 
            resources.ApplyResources(this.tsbLanguagesAdd, "tsbLanguagesAdd");
            this.tsbLanguagesAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLanguagesAdd.IconName = "add";
            this.tsbLanguagesAdd.Name = "tsbLanguagesAdd";
            this.tsbLanguagesAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbLanguagesDelete
            // 
            resources.ApplyResources(this.tsbLanguagesDelete, "tsbLanguagesDelete");
            this.tsbLanguagesDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLanguagesDelete.IconName = "delete";
            this.tsbLanguagesDelete.Name = "tsbLanguagesDelete";
            this.tsbLanguagesDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssLanguages1
            // 
            resources.ApplyResources(this.tssLanguages1, "tssLanguages1");
            this.tssLanguages1.Name = "tssLanguages1";
            // 
            // tsbLanguagesUp
            // 
            resources.ApplyResources(this.tsbLanguagesUp, "tsbLanguagesUp");
            this.tsbLanguagesUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLanguagesUp.IconName = "arrow-up";
            this.tsbLanguagesUp.Name = "tsbLanguagesUp";
            this.tsbLanguagesUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbLanguagesDown
            // 
            resources.ApplyResources(this.tsbLanguagesDown, "tsbLanguagesDown");
            this.tsbLanguagesDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLanguagesDown.IconName = "arrow-down";
            this.tsbLanguagesDown.Name = "tsbLanguagesDown";
            this.tsbLanguagesDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // tpReportStations
            // 
            this.tpReportStations.Controls.Add(this.dgvReportStations);
            this.tpReportStations.Controls.Add(this.tsReportStations);
            resources.ApplyResources(this.tpReportStations, "tpReportStations");
            this.tpReportStations.UseVisualStyleBackColor = true;
            this.tpReportStations.Name = "tpReportStations";
            // 
            // dgvReportStations
            // 
            this.dgvReportStations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cRepValidity,
            this.cRepShortFrom,
            this.cRepLongFrom,
            this.cRepShortTo,
            this.cRepLongTo,
            this.cRepDay});
            resources.ApplyResources(this.dgvReportStations, "dgvReportStations");
            this.dgvReportStations.AllowUserToAddRows = false;
            this.dgvReportStations.AllowUserToDeleteRows = false;
            this.dgvReportStations.AllowUserToResizeRows = false;
            this.dgvReportStations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReportStations.MultiSelect = false;
            this.dgvReportStations.RowHeadersVisible = false;
            this.dgvReportStations.Name = "dgvReportStations";
            this.dgvReportStations.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvReportStations.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvReportStations.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvReportStations.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsReportStations
            // 
            this.tsReportStations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbReportStationsAdd,
            this.tsbReportStationsDelete,
            this.tssReportStations1,
            this.tsbReportStationsUp,
            this.tsbReportStationsDown});
            resources.ApplyResources(this.tsReportStations, "tsReportStations");
            this.tsReportStations.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsReportStations.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsReportStations.Name = "tsReportStations";
            // 
            // tsbReportStationsAdd
            // 
            resources.ApplyResources(this.tsbReportStationsAdd, "tsbReportStationsAdd");
            this.tsbReportStationsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReportStationsAdd.IconName = "add";
            this.tsbReportStationsAdd.Name = "tsbReportStationsAdd";
            this.tsbReportStationsAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbReportStationsDelete
            // 
            resources.ApplyResources(this.tsbReportStationsDelete, "tsbReportStationsDelete");
            this.tsbReportStationsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReportStationsDelete.IconName = "delete";
            this.tsbReportStationsDelete.Name = "tsbReportStationsDelete";
            this.tsbReportStationsDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssReportStations1
            // 
            resources.ApplyResources(this.tssReportStations1, "tssReportStations1");
            this.tssReportStations1.Name = "tssReportStations1";
            // 
            // tsbReportStationsUp
            // 
            resources.ApplyResources(this.tsbReportStationsUp, "tsbReportStationsUp");
            this.tsbReportStationsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReportStationsUp.IconName = "arrow-up";
            this.tsbReportStationsUp.Name = "tsbReportStationsUp";
            this.tsbReportStationsUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbReportStationsDown
            // 
            resources.ApplyResources(this.tsbReportStationsDown, "tsbReportStationsDown");
            this.tsbReportStationsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReportStationsDown.IconName = "arrow-down";
            this.tsbReportStationsDown.Name = "tsbReportStationsDown";
            this.tsbReportStationsDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // tpTexts
            // 
            this.tpTexts.Controls.Add(this.dgvTexts);
            this.tpTexts.Controls.Add(this.tsTexts);
            resources.ApplyResources(this.tpTexts, "tpTexts");
            this.tpTexts.UseVisualStyleBackColor = true;
            this.tpTexts.Name = "tpTexts";
            // 
            // dgvTexts
            // 
            this.dgvTexts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cTextSet,
            this.cTextValidity,
            this.cTextText,
            this.cTextFont,
            this.cTextDay});
            resources.ApplyResources(this.dgvTexts, "dgvTexts");
            this.dgvTexts.AllowUserToAddRows = false;
            this.dgvTexts.AllowUserToDeleteRows = false;
            this.dgvTexts.AllowUserToResizeRows = false;
            this.dgvTexts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTexts.MultiSelect = false;
            this.dgvTexts.RowHeadersVisible = false;
            this.dgvTexts.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvTexts.Name = "dgvTexts";
            this.dgvTexts.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvTexts.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvTexts.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvTexts.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsTexts
            // 
            this.tsTexts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbTextsAdd,
            this.tsbTextsDelete,
            this.tssTexts1,
            this.tsbTextsUp,
            this.tsbTextsDown});
            resources.ApplyResources(this.tsTexts, "tsTexts");
            this.tsTexts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsTexts.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTexts.Name = "tsTexts";
            // 
            // tsbTextsAdd
            // 
            resources.ApplyResources(this.tsbTextsAdd, "tsbTextsAdd");
            this.tsbTextsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTextsAdd.IconName = "add";
            this.tsbTextsAdd.Name = "tsbTextsAdd";
            this.tsbTextsAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbTextsDelete
            // 
            resources.ApplyResources(this.tsbTextsDelete, "tsbTextsDelete");
            this.tsbTextsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTextsDelete.IconName = "delete";
            this.tsbTextsDelete.Name = "tsbTextsDelete";
            this.tsbTextsDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssTexts1
            // 
            resources.ApplyResources(this.tssTexts1, "tssTexts1");
            this.tssTexts1.Name = "tssTexts1";
            // 
            // tsbTextsUp
            // 
            resources.ApplyResources(this.tsbTextsUp, "tsbTextsUp");
            this.tsbTextsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTextsUp.IconName = "arrow-up";
            this.tsbTextsUp.Name = "tsbTextsUp";
            this.tsbTextsUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbTextsDown
            // 
            resources.ApplyResources(this.tsbTextsDown, "tsbTextsDown");
            this.tsbTextsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTextsDown.IconName = "arrow-down";
            this.tsbTextsDown.Name = "tsbTextsDown";
            this.tsbTextsDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // tpMos
            // 
            this.tpMos.Controls.Add(this.dgvMos);
            this.tpMos.Controls.Add(this.tsMos);
            resources.ApplyResources(this.tpMos, "tpMos");
            this.tpMos.UseVisualStyleBackColor = true;
            this.tpMos.Name = "tpMos";
            // 
            // dgvMos
            // 
            this.dgvMos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cMosValidity});
            resources.ApplyResources(this.dgvMos, "dgvMos");
            this.dgvMos.AllowUserToAddRows = false;
            this.dgvMos.AllowUserToDeleteRows = false;
            this.dgvMos.AllowUserToResizeRows = false;
            this.dgvMos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMos.MultiSelect = false;
            this.dgvMos.RowHeadersVisible = false;
            this.dgvMos.Name = "dgvMos";
            this.dgvMos.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvMos.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvMos.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvMos.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsMos
            // 
            this.tsMos.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbMosAdd,
            this.tsbMosDelete,
            this.tssMos1,
            this.tsbMosUp,
            this.tsbMosDown});
            resources.ApplyResources(this.tsMos, "tsMos");
            this.tsMos.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMos.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsMos.Name = "tsMos";
            // 
            // tsbMosAdd
            // 
            resources.ApplyResources(this.tsbMosAdd, "tsbMosAdd");
            this.tsbMosAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMosAdd.IconName = "add";
            this.tsbMosAdd.Name = "tsbMosAdd";
            this.tsbMosAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbMosDelete
            // 
            resources.ApplyResources(this.tsbMosDelete, "tsbMosDelete");
            this.tsbMosDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMosDelete.IconName = "delete";
            this.tsbMosDelete.Name = "tsbMosDelete";
            this.tsbMosDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssMos1
            // 
            resources.ApplyResources(this.tssMos1, "tssMos1");
            this.tssMos1.Name = "tssMos1";
            // 
            // tsbMosUp
            // 
            resources.ApplyResources(this.tsbMosUp, "tsbMosUp");
            this.tsbMosUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMosUp.IconName = "arrow-up";
            this.tsbMosUp.Name = "tsbMosUp";
            this.tsbMosUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbMosDown
            // 
            resources.ApplyResources(this.tsbMosDown, "tsbMosDown");
            this.tsbMosDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMosDown.IconName = "arrow-down";
            this.tsbMosDown.Name = "tsbMosDown";
            this.tsbMosDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // flpStation
            // 
            this.flpStation.Controls.Add(this.lStation);
            this.flpStation.Controls.Add(this.cboStation);
            this.flpStation.Controls.Add(this.lStationInfo);
            resources.ApplyResources(this.flpStation, "flpStation");
            this.flpStation.Name = "flpStation";
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
            // tpNotices
            // 
            this.tpNotices.Controls.Add(this.splitNotices);
            resources.ApplyResources(this.tpNotices, "tpNotices");
            this.tpNotices.UseVisualStyleBackColor = true;
            this.tpNotices.Name = "tpNotices";
            // 
            // splitNotices
            // 
            resources.ApplyResources(this.splitNotices, "splitNotices");
            this.splitNotices.Name = "splitNotices";
            // 
            // splitNotices.Panel1
            // 
            this.splitNotices.Panel1.Controls.Add(this.dgvLockouts);
            this.splitNotices.Panel1.Controls.Add(this.tsLockouts);
            this.splitNotices.Panel1.Controls.Add(this.lLockouts);
            // 
            // splitNotices.Panel2
            // 
            this.splitNotices.Panel2.Controls.Add(this.dgvDiversions);
            this.splitNotices.Panel2.Controls.Add(this.tsDiversions);
            this.splitNotices.Panel2.Controls.Add(this.lDiversions);
            // 
            // dgvLockouts
            // 
            this.dgvLockouts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cLockValidity,
            this.cLockKey,
            this.cLockFrom,
            this.cLockTo,
            this.cLockDay});
            resources.ApplyResources(this.dgvLockouts, "dgvLockouts");
            this.dgvLockouts.AllowUserToAddRows = false;
            this.dgvLockouts.AllowUserToDeleteRows = false;
            this.dgvLockouts.AllowUserToResizeRows = false;
            this.dgvLockouts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLockouts.MultiSelect = false;
            this.dgvLockouts.RowHeadersVisible = false;
            this.dgvLockouts.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvLockouts.Name = "dgvLockouts";
            this.dgvLockouts.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvLockouts.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvLockouts.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvLockouts.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsLockouts
            // 
            this.tsLockouts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLockoutsAdd,
            this.tsbLockoutsDelete,
            this.tssLockouts1,
            this.tsbLockoutsUp,
            this.tsbLockoutsDown});
            resources.ApplyResources(this.tsLockouts, "tsLockouts");
            this.tsLockouts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsLockouts.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsLockouts.Name = "tsLockouts";
            // 
            // tsbLockoutsAdd
            // 
            resources.ApplyResources(this.tsbLockoutsAdd, "tsbLockoutsAdd");
            this.tsbLockoutsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLockoutsAdd.IconName = "add";
            this.tsbLockoutsAdd.Name = "tsbLockoutsAdd";
            this.tsbLockoutsAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbLockoutsDelete
            // 
            resources.ApplyResources(this.tsbLockoutsDelete, "tsbLockoutsDelete");
            this.tsbLockoutsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLockoutsDelete.IconName = "delete";
            this.tsbLockoutsDelete.Name = "tsbLockoutsDelete";
            this.tsbLockoutsDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssLockouts1
            // 
            resources.ApplyResources(this.tssLockouts1, "tssLockouts1");
            this.tssLockouts1.Name = "tssLockouts1";
            // 
            // tsbLockoutsUp
            // 
            resources.ApplyResources(this.tsbLockoutsUp, "tsbLockoutsUp");
            this.tsbLockoutsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLockoutsUp.IconName = "arrow-up";
            this.tsbLockoutsUp.Name = "tsbLockoutsUp";
            this.tsbLockoutsUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbLockoutsDown
            // 
            resources.ApplyResources(this.tsbLockoutsDown, "tsbLockoutsDown");
            this.tsbLockoutsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLockoutsDown.IconName = "arrow-down";
            this.tsbLockoutsDown.Name = "tsbLockoutsDown";
            this.tsbLockoutsDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // lLockouts
            // 
            resources.ApplyResources(this.lLockouts, "lLockouts");
            this.lLockouts.Name = "lLockouts";
            // 
            // dgvDiversions
            // 
            this.dgvDiversions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cDivStation,
            this.cDivValidity,
            this.cDivKey,
            this.cDivFrom,
            this.cDivTo,
            this.cDivDay});
            resources.ApplyResources(this.dgvDiversions, "dgvDiversions");
            this.dgvDiversions.AllowUserToAddRows = false;
            this.dgvDiversions.AllowUserToDeleteRows = false;
            this.dgvDiversions.AllowUserToResizeRows = false;
            this.dgvDiversions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDiversions.MultiSelect = false;
            this.dgvDiversions.RowHeadersVisible = false;
            this.dgvDiversions.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvDiversions.Name = "dgvDiversions";
            this.dgvDiversions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Records_CellValueChanged);
            this.dgvDiversions.CurrentCellDirtyStateChanged += new System.EventHandler(this.Records_CurrentCellDirtyStateChanged);
            this.dgvDiversions.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Records_DataError);
            this.dgvDiversions.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.Records_CellFormatting);
            // 
            // tsDiversions
            // 
            this.tsDiversions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbDiversionsAdd,
            this.tsbDiversionsDelete,
            this.tssDiversions1,
            this.tsbDiversionsUp,
            this.tsbDiversionsDown});
            resources.ApplyResources(this.tsDiversions, "tsDiversions");
            this.tsDiversions.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsDiversions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsDiversions.Name = "tsDiversions";
            // 
            // tsbDiversionsAdd
            // 
            resources.ApplyResources(this.tsbDiversionsAdd, "tsbDiversionsAdd");
            this.tsbDiversionsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDiversionsAdd.IconName = "add";
            this.tsbDiversionsAdd.Name = "tsbDiversionsAdd";
            this.tsbDiversionsAdd.Click += new System.EventHandler(this.tsbRecordAdd_Click);
            // 
            // tsbDiversionsDelete
            // 
            resources.ApplyResources(this.tsbDiversionsDelete, "tsbDiversionsDelete");
            this.tsbDiversionsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDiversionsDelete.IconName = "delete";
            this.tsbDiversionsDelete.Name = "tsbDiversionsDelete";
            this.tsbDiversionsDelete.Click += new System.EventHandler(this.tsbRecordDelete_Click);
            // 
            // tssDiversions1
            // 
            resources.ApplyResources(this.tssDiversions1, "tssDiversions1");
            this.tssDiversions1.Name = "tssDiversions1";
            // 
            // tsbDiversionsUp
            // 
            resources.ApplyResources(this.tsbDiversionsUp, "tsbDiversionsUp");
            this.tsbDiversionsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDiversionsUp.IconName = "arrow-up";
            this.tsbDiversionsUp.Name = "tsbDiversionsUp";
            this.tsbDiversionsUp.Click += new System.EventHandler(this.tsbRecordUp_Click);
            // 
            // tsbDiversionsDown
            // 
            resources.ApplyResources(this.tsbDiversionsDown, "tsbDiversionsDown");
            this.tsbDiversionsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDiversionsDown.IconName = "arrow-down";
            this.tsbDiversionsDown.Name = "tsbDiversionsDown";
            this.tsbDiversionsDown.Click += new System.EventHandler(this.tsbRecordDown_Click);
            // 
            // lDiversions
            // 
            resources.ApplyResources(this.lDiversions, "lDiversions");
            this.lDiversions.Name = "lDiversions";
            // 
            // lbProblems
            // 
            resources.ApplyResources(this.lbProblems, "lbProblems");
            this.lbProblems.FormattingEnabled = true;
            this.lbProblems.IntegralHeight = false;
            this.lbProblems.Name = "lbProblems";
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
            // FEditTrain
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 709);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 560);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.lbProblems);
            this.Controls.Add(this.flpButtons);
            this.Name = "FEditTrain";
            resources.ApplyResources(this, "$this");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FEditTrain_FormClosing);
            // 
            // cVarIndex
            // 
            resources.ApplyResources(this.cVarIndex, "cVarIndex");
            this.cVarIndex.ReadOnly = true;
            this.cVarIndex.Name = "cVarIndex";
            // 
            // cVarValidity
            // 
            resources.ApplyResources(this.cVarValidity, "cVarValidity");
            this.cVarValidity.DataPropertyName = "Validity";
            this.cVarValidity.ReadOnly = true;
            this.cVarValidity.Name = "cVarValidity";
            // 
            // cVarDays
            // 
            resources.ApplyResources(this.cVarDays, "cVarDays");
            this.cVarDays.ReadOnly = true;
            this.cVarDays.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cVarDays.Name = "cVarDays";
            // 
            // cStation
            // 
            resources.ApplyResources(this.cStation, "cStation");
            this.cStation.DataPropertyName = "Station";
            this.cStation.Name = "cStation";
            // 
            // cStationName
            // 
            resources.ApplyResources(this.cStationName, "cStationName");
            this.cStationName.DataPropertyName = "Station";
            this.cStationName.Name = "cStationName";
            // 
            // cArrival
            // 
            resources.ApplyResources(this.cArrival, "cArrival");
            this.cArrival.DataPropertyName = "Arrival";
            this.cArrival.Name = "cArrival";
            // 
            // cDeparture
            // 
            resources.ApplyResources(this.cDeparture, "cDeparture");
            this.cDeparture.DataPropertyName = "Departure";
            this.cDeparture.Name = "cDeparture";
            // 
            // cPassing
            // 
            resources.ApplyResources(this.cPassing, "cPassing");
            this.cPassing.DataPropertyName = "Passing";
            this.cPassing.Name = "cPassing";
            // 
            // cTransfer
            // 
            resources.ApplyResources(this.cTransfer, "cTransfer");
            this.cTransfer.DataPropertyName = "Transfer";
            this.cTransfer.Name = "cTransfer";
            // 
            // cOperating
            // 
            resources.ApplyResources(this.cOperating, "cOperating");
            this.cOperating.DataPropertyName = "OperatingPoint";
            this.cOperating.Name = "cOperating";
            // 
            // cRequest
            // 
            resources.ApplyResources(this.cRequest, "cRequest");
            this.cRequest.DataPropertyName = "OnRequest";
            this.cRequest.Name = "cRequest";
            // 
            // cNumber
            // 
            resources.ApplyResources(this.cNumber, "cNumber");
            this.cNumber.DataPropertyName = "Number";
            this.cNumber.Name = "cNumber";
            // 
            // cDisplay
            // 
            resources.ApplyResources(this.cDisplay, "cDisplay");
            this.cDisplay.DataPropertyName = "DisplayNumber";
            this.cDisplay.Name = "cDisplay";
            // 
            // cType
            // 
            resources.ApplyResources(this.cType, "cType");
            this.cType.DataPropertyName = "TrainType";
            this.cType.Name = "cType";
            // 
            // cName
            // 
            resources.ApplyResources(this.cName, "cName");
            this.cName.DataPropertyName = "TrainName";
            this.cName.Name = "cName";
            // 
            // cLine
            // 
            resources.ApplyResources(this.cLine, "cLine");
            this.cLine.DataPropertyName = "Line";
            this.cLine.Name = "cLine";
            // 
            // cReplacement
            // 
            resources.ApplyResources(this.cReplacement, "cReplacement");
            this.cReplacement.DataPropertyName = "ReplacementLine";
            this.cReplacement.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cReplacement.Name = "cReplacement";
            // 
            // cPosValidity
            // 
            resources.ApplyResources(this.cPosValidity, "cPosValidity");
            this.cPosValidity.DataPropertyName = "Validity";
            this.cPosValidity.Name = "cPosValidity";
            // 
            // cPosArrival
            // 
            resources.ApplyResources(this.cPosArrival, "cPosArrival");
            this.cPosArrival.DataPropertyName = "Arrival";
            this.cPosArrival.Name = "cPosArrival";
            // 
            // cPosDeparture
            // 
            resources.ApplyResources(this.cPosDeparture, "cPosDeparture");
            this.cPosDeparture.DataPropertyName = "Departure";
            this.cPosDeparture.Name = "cPosDeparture";
            // 
            // cPosTracks
            // 
            resources.ApplyResources(this.cPosTracks, "cPosTracks");
            this.cPosTracks.ReadOnly = true;
            this.cPosTracks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cPosTracks.Name = "cPosTracks";
            // 
            // cPosDay
            // 
            resources.ApplyResources(this.cPosDay, "cPosDay");
            this.cPosDay.DataPropertyName = "EditedForDay";
            this.cPosDay.Name = "cPosDay";
            // 
            // cAddValidity
            // 
            resources.ApplyResources(this.cAddValidity, "cAddValidity");
            this.cAddValidity.DataPropertyName = "Validity";
            this.cAddValidity.Name = "cAddValidity";
            // 
            // cAddKey
            // 
            resources.ApplyResources(this.cAddKey, "cAddKey");
            this.cAddKey.DataPropertyName = "Key";
            this.cAddKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cAddKey.Name = "cAddKey";
            // 
            // cAddReports
            // 
            resources.ApplyResources(this.cAddReports, "cAddReports");
            this.cAddReports.DataPropertyName = "Reports";
            this.cAddReports.Name = "cAddReports";
            // 
            // cAddDay
            // 
            resources.ApplyResources(this.cAddDay, "cAddDay");
            this.cAddDay.DataPropertyName = "EditedForDay";
            this.cAddDay.Name = "cAddDay";
            // 
            // cCompValidity
            // 
            resources.ApplyResources(this.cCompValidity, "cCompValidity");
            this.cCompValidity.DataPropertyName = "Validity";
            this.cCompValidity.Name = "cCompValidity";
            // 
            // cCompEnd
            // 
            resources.ApplyResources(this.cCompEnd, "cCompEnd");
            this.cCompEnd.DataPropertyName = "EndStation";
            this.cCompEnd.Name = "cCompEnd";
            // 
            // cCompReports
            // 
            resources.ApplyResources(this.cCompReports, "cCompReports");
            this.cCompReports.DataPropertyName = "Reports";
            this.cCompReports.Name = "cCompReports";
            // 
            // cCompSounds
            // 
            resources.ApplyResources(this.cCompSounds, "cCompSounds");
            this.cCompSounds.DataPropertyName = "Sounds";
            this.cCompSounds.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cCompSounds.Name = "cCompSounds";
            // 
            // cCompDay
            // 
            resources.ApplyResources(this.cCompDay, "cCompDay");
            this.cCompDay.DataPropertyName = "EditedForDay";
            this.cCompDay.Name = "cCompDay";
            // 
            // cLangValidity
            // 
            resources.ApplyResources(this.cLangValidity, "cLangValidity");
            this.cLangValidity.DataPropertyName = "Validity";
            this.cLangValidity.Name = "cLangValidity";
            // 
            // cLangLanguages
            // 
            resources.ApplyResources(this.cLangLanguages, "cLangLanguages");
            this.cLangLanguages.DataPropertyName = "Languages";
            this.cLangLanguages.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cLangLanguages.Name = "cLangLanguages";
            // 
            // cLangDay
            // 
            resources.ApplyResources(this.cLangDay, "cLangDay");
            this.cLangDay.DataPropertyName = "EditedForDay";
            this.cLangDay.Name = "cLangDay";
            // 
            // cRepValidity
            // 
            resources.ApplyResources(this.cRepValidity, "cRepValidity");
            this.cRepValidity.DataPropertyName = "Validity";
            this.cRepValidity.Name = "cRepValidity";
            // 
            // cRepShortFrom
            // 
            resources.ApplyResources(this.cRepShortFrom, "cRepShortFrom");
            this.cRepShortFrom.DataPropertyName = "ShortFrom";
            this.cRepShortFrom.Name = "cRepShortFrom";
            // 
            // cRepLongFrom
            // 
            resources.ApplyResources(this.cRepLongFrom, "cRepLongFrom");
            this.cRepLongFrom.DataPropertyName = "LongFrom";
            this.cRepLongFrom.Name = "cRepLongFrom";
            // 
            // cRepShortTo
            // 
            resources.ApplyResources(this.cRepShortTo, "cRepShortTo");
            this.cRepShortTo.DataPropertyName = "ShortTo";
            this.cRepShortTo.Name = "cRepShortTo";
            // 
            // cRepLongTo
            // 
            resources.ApplyResources(this.cRepLongTo, "cRepLongTo");
            this.cRepLongTo.DataPropertyName = "LongTo";
            this.cRepLongTo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cRepLongTo.Name = "cRepLongTo";
            // 
            // cRepDay
            // 
            resources.ApplyResources(this.cRepDay, "cRepDay");
            this.cRepDay.DataPropertyName = "EditedForDay";
            this.cRepDay.Name = "cRepDay";
            // 
            // cTextSet
            // 
            resources.ApplyResources(this.cTextSet, "cTextSet");
            this.cTextSet.DataPropertyName = "Set";
            this.cTextSet.Name = "cTextSet";
            // 
            // cTextValidity
            // 
            resources.ApplyResources(this.cTextValidity, "cTextValidity");
            this.cTextValidity.DataPropertyName = "Validity";
            this.cTextValidity.Name = "cTextValidity";
            // 
            // cTextText
            // 
            resources.ApplyResources(this.cTextText, "cTextText");
            this.cTextText.DataPropertyName = "Text";
            this.cTextText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cTextText.Name = "cTextText";
            // 
            // cTextFont
            // 
            resources.ApplyResources(this.cTextFont, "cTextFont");
            this.cTextFont.DataPropertyName = "Font";
            this.cTextFont.Name = "cTextFont";
            // 
            // cTextDay
            // 
            resources.ApplyResources(this.cTextDay, "cTextDay");
            this.cTextDay.DataPropertyName = "EditedForDay";
            this.cTextDay.Name = "cTextDay";
            // 
            // cMosValidity
            // 
            resources.ApplyResources(this.cMosValidity, "cMosValidity");
            this.cMosValidity.DataPropertyName = "Validity";
            this.cMosValidity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cMosValidity.Name = "cMosValidity";
            // 
            // cLockValidity
            // 
            resources.ApplyResources(this.cLockValidity, "cLockValidity");
            this.cLockValidity.DataPropertyName = "Validity";
            this.cLockValidity.Name = "cLockValidity";
            // 
            // cLockKey
            // 
            resources.ApplyResources(this.cLockKey, "cLockKey");
            this.cLockKey.DataPropertyName = "Key";
            this.cLockKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cLockKey.Name = "cLockKey";
            // 
            // cLockFrom
            // 
            resources.ApplyResources(this.cLockFrom, "cLockFrom");
            this.cLockFrom.DataPropertyName = "From";
            this.cLockFrom.Name = "cLockFrom";
            // 
            // cLockTo
            // 
            resources.ApplyResources(this.cLockTo, "cLockTo");
            this.cLockTo.DataPropertyName = "To";
            this.cLockTo.Name = "cLockTo";
            // 
            // cLockDay
            // 
            resources.ApplyResources(this.cLockDay, "cLockDay");
            this.cLockDay.DataPropertyName = "EditedForDay";
            this.cLockDay.Name = "cLockDay";
            // 
            // cDivStation
            // 
            resources.ApplyResources(this.cDivStation, "cDivStation");
            this.cDivStation.DataPropertyName = "Station";
            this.cDivStation.Name = "cDivStation";
            // 
            // cDivValidity
            // 
            resources.ApplyResources(this.cDivValidity, "cDivValidity");
            this.cDivValidity.DataPropertyName = "Validity";
            this.cDivValidity.Name = "cDivValidity";
            // 
            // cDivKey
            // 
            resources.ApplyResources(this.cDivKey, "cDivKey");
            this.cDivKey.DataPropertyName = "Key";
            this.cDivKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cDivKey.Name = "cDivKey";
            // 
            // cDivFrom
            // 
            resources.ApplyResources(this.cDivFrom, "cDivFrom");
            this.cDivFrom.DataPropertyName = "From";
            this.cDivFrom.Name = "cDivFrom";
            // 
            // cDivTo
            // 
            resources.ApplyResources(this.cDivTo, "cDivTo");
            this.cDivTo.DataPropertyName = "To";
            this.cDivTo.Name = "cDivTo";
            // 
            // cDivDay
            // 
            resources.ApplyResources(this.cDivDay, "cDivDay");
            this.cDivDay.DataPropertyName = "EditedForDay";
            this.cDivDay.Name = "cDivDay";
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.tsDiversions.ResumeLayout(false);
            this.tsDiversions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDiversions)).EndInit();
            this.tsLockouts.ResumeLayout(false);
            this.tsLockouts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLockouts)).EndInit();
            this.splitNotices.Panel1.ResumeLayout(false);
            this.splitNotices.Panel1.PerformLayout();
            this.splitNotices.Panel2.ResumeLayout(false);
            this.splitNotices.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitNotices)).EndInit();
            this.splitNotices.ResumeLayout(false);
            this.splitNotices.PerformLayout();
            this.tpNotices.ResumeLayout(false);
            this.tpNotices.PerformLayout();
            this.flpStation.ResumeLayout(false);
            this.flpStation.PerformLayout();
            this.tsMos.ResumeLayout(false);
            this.tsMos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMos)).EndInit();
            this.tpMos.ResumeLayout(false);
            this.tpMos.PerformLayout();
            this.tsTexts.ResumeLayout(false);
            this.tsTexts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTexts)).EndInit();
            this.tpTexts.ResumeLayout(false);
            this.tpTexts.PerformLayout();
            this.tsReportStations.ResumeLayout(false);
            this.tsReportStations.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReportStations)).EndInit();
            this.tpReportStations.ResumeLayout(false);
            this.tpReportStations.PerformLayout();
            this.tsLanguages.ResumeLayout(false);
            this.tsLanguages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLanguages)).EndInit();
            this.tpLanguages.ResumeLayout(false);
            this.tpLanguages.PerformLayout();
            this.tsCompositions.ResumeLayout(false);
            this.tsCompositions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCompositions)).EndInit();
            this.tpCompositions.ResumeLayout(false);
            this.tpCompositions.PerformLayout();
            this.tsAdditions.ResumeLayout(false);
            this.tsAdditions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdditions)).EndInit();
            this.tpAdditions.ResumeLayout(false);
            this.tpAdditions.PerformLayout();
            this.tsPositions.ResumeLayout(false);
            this.tsPositions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPositions)).EndInit();
            this.tpPositions.ResumeLayout(false);
            this.tpPositions.PerformLayout();
            this.tcStation.ResumeLayout(false);
            this.tcStation.PerformLayout();
            this.tpStations.ResumeLayout(false);
            this.tpStations.PerformLayout();
            this.flpPeriod.ResumeLayout(false);
            this.flpPeriod.PerformLayout();
            this.tsStops.ResumeLayout(false);
            this.tsStops.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStops)).EndInit();
            this.flpFlags.ResumeLayout(false);
            this.flpFlags.PerformLayout();
            this.pVariant.ResumeLayout(false);
            this.pVariant.PerformLayout();
            this.tsVariants.ResumeLayout(false);
            this.tsVariants.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariants)).EndInit();
            this.splitVariants.Panel1.ResumeLayout(false);
            this.splitVariants.Panel1.PerformLayout();
            this.splitVariants.Panel2.ResumeLayout(false);
            this.splitVariants.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitVariants)).EndInit();
            this.splitVariants.ResumeLayout(false);
            this.splitVariants.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitMain.PerformLayout();
            this.tpTrain.ResumeLayout(false);
            this.tpTrain.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tcMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExControls.ExTabControl tcMain;
        private System.Windows.Forms.TabPage tpTrain;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.SplitContainer splitVariants;
        private System.Windows.Forms.DataGridView dgvVariants;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarValidity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarDays;
        private System.Windows.Forms.ToolStrip tsVariants;
        private ToolsCore.Icons.IconToolStripButton tsbVarAdd;
        private ToolsCore.Icons.IconToolStripButton tsbVarCopy;
        private ToolsCore.Icons.IconToolStripButton tsbVarDelete;
        private System.Windows.Forms.ToolStripSeparator tssVar1;
        private ToolsCore.Icons.IconToolStripButton tsbVarUp;
        private ToolsCore.Icons.IconToolStripButton tsbVarDown;
        private System.Windows.Forms.Panel pVariant;
        private System.Windows.Forms.Label lValidity;
        private ExControls.ExTextBox tbValidity;
        private ExControls.ExButton bValidity;
        private System.Windows.Forms.Label lOperator;
        private ExControls.ExTextBox tbOperator;
        private System.Windows.Forms.Label lFlags;
        private System.Windows.Forms.FlowLayoutPanel flpFlags;
        private ExControls.ExCheckBox cboxFlagD;
        private ExControls.ExCheckBox cboxFlagX;
        private ExControls.ExCheckBox cboxFlagN;
        private ExControls.ExCheckBox cboxFlagNP;
        private ExControls.ExCheckBox cboxFlagO;
        private ExControls.ExCheckBox cboxFlagM;
        private ExControls.ExCheckBox cboxFlagR;
        private ExControls.ExCheckBox cboxFlagL;
        private System.Windows.Forms.Label lNote;
        private ExControls.ExTextBox tbNote;
        private ToolsCore.Forms.Calendar.CalendarStrip calendarStrip;
        private System.Windows.Forms.DataGridView dgvStops;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStation;
        private ExControls.DataGridViewExComboBoxColumn cStationName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cArrival;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDeparture;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cPassing;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cTransfer;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cOperating;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cRequest;
        private System.Windows.Forms.DataGridViewTextBoxColumn cNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDisplay;
        private ExControls.DataGridViewExComboBoxColumn cType;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn cReplacement;
        private System.Windows.Forms.ToolStrip tsStops;
        private ToolsCore.Icons.IconToolStripButton tsbStopInsert;
        private ToolsCore.Icons.IconToolStripButton tsbStopAppend;
        private ToolsCore.Icons.IconToolStripButton tsbStopDelete;
        private System.Windows.Forms.ToolStripSeparator tssStop1;
        private System.Windows.Forms.ToolStripLabel tslStopsHint;
        private System.Windows.Forms.FlowLayoutPanel flpPeriod;
        private System.Windows.Forms.Label lFrom;
        private ExControls.ExDateTimePicker dtpFrom;
        private System.Windows.Forms.Label lTo;
        private ExControls.ExDateTimePicker dtpTo;
        private ExControls.ExCheckBox cboxNoEnd;
        private System.Windows.Forms.TabPage tpStations;
        private ExControls.ExTabControl tcStation;
        private System.Windows.Forms.TabPage tpPositions;
        private System.Windows.Forms.DataGridView dgvPositions;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPosValidity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPosArrival;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPosDeparture;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPosTracks;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cPosDay;
        private System.Windows.Forms.ToolStrip tsPositions;
        private ToolsCore.Icons.IconToolStripButton tsbPositionsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbPositionsDelete;
        private System.Windows.Forms.ToolStripSeparator tssPositions1;
        private ToolsCore.Icons.IconToolStripButton tsbPositionsUp;
        private ToolsCore.Icons.IconToolStripButton tsbPositionsDown;
        private System.Windows.Forms.TabPage tpAdditions;
        private System.Windows.Forms.DataGridView dgvAdditions;
        private System.Windows.Forms.DataGridViewTextBoxColumn cAddValidity;
        private ExControls.DataGridViewExComboBoxColumn cAddKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cAddReports;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cAddDay;
        private System.Windows.Forms.ToolStrip tsAdditions;
        private ToolsCore.Icons.IconToolStripButton tsbAdditionsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbAdditionsDelete;
        private System.Windows.Forms.ToolStripSeparator tssAdditions1;
        private ToolsCore.Icons.IconToolStripButton tsbAdditionsUp;
        private ToolsCore.Icons.IconToolStripButton tsbAdditionsDown;
        private System.Windows.Forms.ToolStripSeparator tssAdditions2;
        private ToolsCore.Icons.IconToolStripButton tsbAdditionsNew;
        private System.Windows.Forms.TabPage tpCompositions;
        private System.Windows.Forms.DataGridView dgvCompositions;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCompValidity;
        private ExControls.DataGridViewExComboBoxColumn cCompEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCompReports;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCompSounds;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cCompDay;
        private System.Windows.Forms.ToolStrip tsCompositions;
        private ToolsCore.Icons.IconToolStripButton tsbCompositionsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbCompositionsDelete;
        private System.Windows.Forms.ToolStripSeparator tssCompositions1;
        private ToolsCore.Icons.IconToolStripButton tsbCompositionsUp;
        private ToolsCore.Icons.IconToolStripButton tsbCompositionsDown;
        private System.Windows.Forms.TabPage tpLanguages;
        private System.Windows.Forms.DataGridView dgvLanguages;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLangValidity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLangLanguages;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cLangDay;
        private System.Windows.Forms.ToolStrip tsLanguages;
        private ToolsCore.Icons.IconToolStripButton tsbLanguagesAdd;
        private ToolsCore.Icons.IconToolStripButton tsbLanguagesDelete;
        private System.Windows.Forms.ToolStripSeparator tssLanguages1;
        private ToolsCore.Icons.IconToolStripButton tsbLanguagesUp;
        private ToolsCore.Icons.IconToolStripButton tsbLanguagesDown;
        private System.Windows.Forms.TabPage tpReportStations;
        private System.Windows.Forms.DataGridView dgvReportStations;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepValidity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepShortFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepLongFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepShortTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn cRepLongTo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cRepDay;
        private System.Windows.Forms.ToolStrip tsReportStations;
        private ToolsCore.Icons.IconToolStripButton tsbReportStationsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbReportStationsDelete;
        private System.Windows.Forms.ToolStripSeparator tssReportStations1;
        private ToolsCore.Icons.IconToolStripButton tsbReportStationsUp;
        private ToolsCore.Icons.IconToolStripButton tsbReportStationsDown;
        private System.Windows.Forms.TabPage tpTexts;
        private System.Windows.Forms.DataGridView dgvTexts;
        private ExControls.DataGridViewExComboBoxColumn cTextSet;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTextValidity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTextText;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTextFont;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cTextDay;
        private System.Windows.Forms.ToolStrip tsTexts;
        private ToolsCore.Icons.IconToolStripButton tsbTextsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbTextsDelete;
        private System.Windows.Forms.ToolStripSeparator tssTexts1;
        private ToolsCore.Icons.IconToolStripButton tsbTextsUp;
        private ToolsCore.Icons.IconToolStripButton tsbTextsDown;
        private System.Windows.Forms.TabPage tpMos;
        private System.Windows.Forms.DataGridView dgvMos;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMosValidity;
        private System.Windows.Forms.ToolStrip tsMos;
        private ToolsCore.Icons.IconToolStripButton tsbMosAdd;
        private ToolsCore.Icons.IconToolStripButton tsbMosDelete;
        private System.Windows.Forms.ToolStripSeparator tssMos1;
        private ToolsCore.Icons.IconToolStripButton tsbMosUp;
        private ToolsCore.Icons.IconToolStripButton tsbMosDown;
        private System.Windows.Forms.FlowLayoutPanel flpStation;
        private System.Windows.Forms.Label lStation;
        private ExControls.ExComboBox cboStation;
        private System.Windows.Forms.Label lStationInfo;
        private System.Windows.Forms.TabPage tpNotices;
        private System.Windows.Forms.SplitContainer splitNotices;
        private System.Windows.Forms.DataGridView dgvLockouts;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLockValidity;
        private ExControls.DataGridViewExComboBoxColumn cLockKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLockFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLockTo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cLockDay;
        private System.Windows.Forms.ToolStrip tsLockouts;
        private ToolsCore.Icons.IconToolStripButton tsbLockoutsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbLockoutsDelete;
        private System.Windows.Forms.ToolStripSeparator tssLockouts1;
        private ToolsCore.Icons.IconToolStripButton tsbLockoutsUp;
        private ToolsCore.Icons.IconToolStripButton tsbLockoutsDown;
        private System.Windows.Forms.Label lLockouts;
        private System.Windows.Forms.DataGridView dgvDiversions;
        private ExControls.DataGridViewExComboBoxColumn cDivStation;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDivValidity;
        private ExControls.DataGridViewExComboBoxColumn cDivKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDivFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDivTo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cDivDay;
        private System.Windows.Forms.ToolStrip tsDiversions;
        private ToolsCore.Icons.IconToolStripButton tsbDiversionsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbDiversionsDelete;
        private System.Windows.Forms.ToolStripSeparator tssDiversions1;
        private ToolsCore.Icons.IconToolStripButton tsbDiversionsUp;
        private ToolsCore.Icons.IconToolStripButton tsbDiversionsDown;
        private System.Windows.Forms.Label lDiversions;
        private GVXEditor.UI.Controls.ProblemList lbProblems;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private ExControls.ExButton bCancel;
        private ExControls.ExButton bOk;
    }
}
