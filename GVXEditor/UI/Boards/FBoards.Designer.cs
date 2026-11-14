namespace GVXEditor.UI.Boards
{
    partial class FBoards
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FBoards));
            this.tcMain = new ExControls.ExTabControl();
            this.tpTables = new System.Windows.Forms.TabPage();
            this.splitTables = new System.Windows.Forms.SplitContainer();
            this.tvBoards = new ExControls.ExTreeView();
            this.tsBoards = new System.Windows.Forms.ToolStrip();
            this.tsbAddTable = new ToolsCore.Icons.IconToolStripButton();
            this.tsbAddPart = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDeleteItem = new ToolsCore.Icons.IconToolStripButton();
            this.tssBoards1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbItemUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbItemDown = new ToolsCore.Icons.IconToolStripButton();
            this.pPart = new System.Windows.Forms.Panel();
            this.lPartName = new System.Windows.Forms.Label();
            this.tbPartName = new ExControls.ExTextBox();
            this.lPartNameHint = new System.Windows.Forms.Label();
            this.lTemplate = new System.Windows.Forms.Label();
            this.cboTemplate = new ExControls.ExComboBox();
            this.lKind = new System.Windows.Forms.Label();
            this.cboKind = new ExControls.ExComboBox();
            this.lRecords = new System.Windows.Forms.Label();
            this.tbRecords = new ExControls.ExTextBox();
            this.lStations = new System.Windows.Forms.Label();
            this.tbStations = new ExControls.ExTextBox();
            this.bStations = new ExControls.ExButton();
            this.lStationsHint = new System.Windows.Forms.Label();
            this.lPrevious = new System.Windows.Forms.Label();
            this.cboPrevious = new ExControls.ExComboBox();
            this.lWebSource = new System.Windows.Forms.Label();
            this.tbWebSource = new ExControls.ExTextBox();
            this.lWindows = new System.Windows.Forms.Label();
            this.tbWindows = new ExControls.ExTextBox();
            this.lWindowsHint = new System.Windows.Forms.Label();
            this.lCondition = new System.Windows.Forms.Label();
            this.codeCondition = new ToolsCore.CodeEditor.CodeEditorBox();
            this.lTracks = new System.Windows.Forms.Label();
            this.pTracks = new System.Windows.Forms.Panel();
            this.dgvTracks = new System.Windows.Forms.DataGridView();
            this.cTrackKey = new ExControls.DataGridViewExComboBoxColumn();
            this.cTrackPriority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tsTracks = new System.Windows.Forms.ToolStrip();
            this.tsbTracksAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbTracksDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssTracks1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbTracksUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbTracksDown = new ToolsCore.Icons.IconToolStripButton();
            this.pTable = new System.Windows.Forms.Panel();
            this.lKey = new System.Windows.Forms.Label();
            this.tbKey = new ExControls.ExTextBox();
            this.lName = new System.Windows.Forms.Label();
            this.tbName = new ExControls.ExTextBox();
            this.lLine = new System.Windows.Forms.Label();
            this.cboLine = new ExControls.ExComboBox();
            this.lLineInfo = new System.Windows.Forms.Label();
            this.lSubLine = new System.Windows.Forms.Label();
            this.tbSubLine = new ExControls.ExTextBox();
            this.lAddress = new System.Windows.Forms.Label();
            this.tbAddress = new ExControls.ExTextBox();
            this.lAddressHint = new System.Windows.Forms.Label();
            this.lSave = new System.Windows.Forms.Label();
            this.tbSave = new ExControls.ExTextBox();
            this.lArrows = new System.Windows.Forms.Label();
            this.tbArrows = new ExControls.ExTextBox();
            this.lArrowsHint = new System.Windows.Forms.Label();
            this.cboxNoScroll = new ExControls.ExCheckBox();
            this.lLights = new System.Windows.Forms.Label();
            this.lLightTime = new System.Windows.Forms.Label();
            this.tbLightTime0 = new ExControls.ExTextBox();
            this.tbLightTime1 = new ExControls.ExTextBox();
            this.lDay = new System.Windows.Forms.Label();
            this.lNight = new System.Windows.Forms.Label();
            this.lLight = new System.Windows.Forms.Label();
            this.tbLight0 = new ExControls.ExTextBox();
            this.tbLight1 = new ExControls.ExTextBox();
            this.lLightOff = new System.Windows.Forms.Label();
            this.tbLightOff0 = new ExControls.ExTextBox();
            this.tbLightOff1 = new ExControls.ExTextBox();
            this.lLightIfEmpty = new System.Windows.Forms.Label();
            this.cboLightIfEmpty0 = new ExControls.ExComboBox();
            this.cboLightIfEmpty1 = new ExControls.ExComboBox();
            this.lLightOnTime = new System.Windows.Forms.Label();
            this.tbLightOnTime = new ExControls.ExTextBox();
            this.lLightsHint = new System.Windows.Forms.Label();
            this.tpTemplates = new System.Windows.Forms.TabPage();
            this.splitTemplates = new System.Windows.Forms.SplitContainer();
            this.lbTemplates = new ExControls.ExListBox();
            this.tsTemplates = new System.Windows.Forms.ToolStrip();
            this.tsbTemplatesAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbTemplatesDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssTemplates1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbTemplatesUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbTemplatesDown = new ToolsCore.Icons.IconToolStripButton();
            this.dgvColumns = new System.Windows.Forms.DataGridView();
            this.cColumnKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cColumnContent = new ExControls.DataGridViewExComboBoxColumn();
            this.cColumnRow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cColumnStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cColumnEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cColumnFont = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cColumnAlign = new ExControls.DataGridViewExComboBoxColumn();
            this.cColumnTranslation = new ExControls.DataGridViewExComboBoxColumn();
            this.cColumnTable1 = new ExControls.DataGridViewExComboBoxColumn();
            this.cColumnTable2 = new ExControls.DataGridViewExComboBoxColumn();
            this.tsColumns = new System.Windows.Forms.ToolStrip();
            this.tsbColumnsAdd = new ToolsCore.Icons.IconToolStripButton();
            this.tsbColumnsDelete = new ToolsCore.Icons.IconToolStripButton();
            this.tssColumns1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbColumnsUp = new ToolsCore.Icons.IconToolStripButton();
            this.tsbColumnsDown = new ToolsCore.Icons.IconToolStripButton();
            this.lColumns = new System.Windows.Forms.Label();
            this.pTemplate = new System.Windows.Forms.Panel();
            this.lTemplateKey = new System.Windows.Forms.Label();
            this.tbTemplateKey = new ExControls.ExTextBox();
            this.lTemplateName = new System.Windows.Forms.Label();
            this.tbTemplateName = new ExControls.ExTextBox();
            this.lClass = new System.Windows.Forms.Label();
            this.cboClass = new ExControls.ExComboBox();
            this.lFontBank = new System.Windows.Forms.Label();
            this.tbFontBank = new ExControls.ExTextBox();
            this.flpStation = new System.Windows.Forms.FlowLayoutPanel();
            this.lStation = new System.Windows.Forms.Label();
            this.cboStation = new ExControls.ExComboBox();
            this.lStationInfo = new System.Windows.Forms.Label();
            this.lbProblems = new GVXEditor.UI.Controls.ProblemList();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bCancel = new ExControls.ExButton();
            this.bOk = new ExControls.ExButton();
            this.lCheck = new System.Windows.Forms.Label();
            this.tcMain.SuspendLayout();
            this.tpTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTables)).BeginInit();
            this.splitTables.Panel1.SuspendLayout();
            this.splitTables.Panel2.SuspendLayout();
            this.splitTables.SuspendLayout();
            this.tsBoards.SuspendLayout();
            this.pPart.SuspendLayout();
            this.pTracks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTracks)).BeginInit();
            this.tsTracks.SuspendLayout();
            this.pTable.SuspendLayout();
            this.tpTemplates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTemplates)).BeginInit();
            this.splitTemplates.Panel1.SuspendLayout();
            this.splitTemplates.Panel2.SuspendLayout();
            this.splitTemplates.SuspendLayout();
            this.tsTemplates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumns)).BeginInit();
            this.tsColumns.SuspendLayout();
            this.pTemplate.SuspendLayout();
            this.flpStation.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpTables);
            this.tcMain.Controls.Add(this.tpTemplates);
            resources.ApplyResources(this.tcMain, "tcMain");
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Name = "tcMain";
            // 
            // tpTables
            // 
            this.tpTables.Controls.Add(this.splitTables);
            resources.ApplyResources(this.tpTables, "tpTables");
            this.tpTables.UseVisualStyleBackColor = true;
            this.tpTables.Name = "tpTables";
            // 
            // splitTables
            // 
            resources.ApplyResources(this.splitTables, "splitTables");
            this.splitTables.Name = "splitTables";
            // 
            // splitTables.Panel1
            // 
            this.splitTables.Panel1.Controls.Add(this.tvBoards);
            this.splitTables.Panel1.Controls.Add(this.tsBoards);
            // 
            // splitTables.Panel2
            // 
            this.splitTables.Panel2.Controls.Add(this.pPart);
            this.splitTables.Panel2.Controls.Add(this.pTable);
            // 
            // tvBoards
            // 
            resources.ApplyResources(this.tvBoards, "tvBoards");
            this.tvBoards.FullRowSelect = true;
            this.tvBoards.HideSelection = false;
            this.tvBoards.ShowLines = false;
            this.tvBoards.Style = ExControls.ExTreeViewStyle.Light;
            this.tvBoards.Name = "tvBoards";
            this.tvBoards.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvBoards_AfterSelect);
            // 
            // tsBoards
            // 
            this.tsBoards.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAddTable,
            this.tsbAddPart,
            this.tsbDeleteItem,
            this.tssBoards1,
            this.tsbItemUp,
            this.tsbItemDown});
            resources.ApplyResources(this.tsBoards, "tsBoards");
            this.tsBoards.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsBoards.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsBoards.Name = "tsBoards";
            // 
            // tsbAddTable
            // 
            resources.ApplyResources(this.tsbAddTable, "tsbAddTable");
            this.tsbAddTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddTable.IconName = "table";
            this.tsbAddTable.IconBadge = "add";
            this.tsbAddTable.Name = "tsbAddTable";
            this.tsbAddTable.Click += new System.EventHandler(this.tsbAddTable_Click);
            // 
            // tsbAddPart
            // 
            resources.ApplyResources(this.tsbAddPart, "tsbAddPart");
            this.tsbAddPart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddPart.IconName = "add";
            this.tsbAddPart.Name = "tsbAddPart";
            this.tsbAddPart.Click += new System.EventHandler(this.tsbAddPart_Click);
            // 
            // tsbDeleteItem
            // 
            resources.ApplyResources(this.tsbDeleteItem, "tsbDeleteItem");
            this.tsbDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteItem.IconName = "delete";
            this.tsbDeleteItem.Name = "tsbDeleteItem";
            this.tsbDeleteItem.Click += new System.EventHandler(this.tsbDeleteItem_Click);
            // 
            // tssBoards1
            // 
            resources.ApplyResources(this.tssBoards1, "tssBoards1");
            this.tssBoards1.Name = "tssBoards1";
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
            // pPart
            // 
            this.pPart.Controls.Add(this.lPartName);
            this.pPart.Controls.Add(this.tbPartName);
            this.pPart.Controls.Add(this.lPartNameHint);
            this.pPart.Controls.Add(this.lTemplate);
            this.pPart.Controls.Add(this.cboTemplate);
            this.pPart.Controls.Add(this.lKind);
            this.pPart.Controls.Add(this.cboKind);
            this.pPart.Controls.Add(this.lRecords);
            this.pPart.Controls.Add(this.tbRecords);
            this.pPart.Controls.Add(this.lStations);
            this.pPart.Controls.Add(this.tbStations);
            this.pPart.Controls.Add(this.bStations);
            this.pPart.Controls.Add(this.lStationsHint);
            this.pPart.Controls.Add(this.lPrevious);
            this.pPart.Controls.Add(this.cboPrevious);
            this.pPart.Controls.Add(this.lWebSource);
            this.pPart.Controls.Add(this.tbWebSource);
            this.pPart.Controls.Add(this.lWindows);
            this.pPart.Controls.Add(this.tbWindows);
            this.pPart.Controls.Add(this.lWindowsHint);
            this.pPart.Controls.Add(this.lCondition);
            this.pPart.Controls.Add(this.codeCondition);
            this.pPart.Controls.Add(this.lTracks);
            this.pPart.Controls.Add(this.pTracks);
            resources.ApplyResources(this.pPart, "pPart");
            this.pPart.Name = "pPart";
            // 
            // lPartName
            // 
            resources.ApplyResources(this.lPartName, "lPartName");
            this.lPartName.Name = "lPartName";
            // 
            // tbPartName
            // 
            resources.ApplyResources(this.tbPartName, "tbPartName");
            this.tbPartName.Name = "tbPartName";
            this.tbPartName.TextChanged += new System.EventHandler(this.Part_Changed);
            // 
            // lPartNameHint
            // 
            resources.ApplyResources(this.lPartNameHint, "lPartNameHint");
            this.lPartNameHint.Name = "lPartNameHint";
            // 
            // lTemplate
            // 
            resources.ApplyResources(this.lTemplate, "lTemplate");
            this.lTemplate.Name = "lTemplate";
            // 
            // cboTemplate
            // 
            resources.ApplyResources(this.cboTemplate, "cboTemplate");
            this.cboTemplate.FormattingEnabled = true;
            this.cboTemplate.Name = "cboTemplate";
            this.cboTemplate.TextChanged += new System.EventHandler(this.Part_Changed);
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
            this.cboKind.SelectedIndexChanged += new System.EventHandler(this.Part_Changed);
            // 
            // lRecords
            // 
            resources.ApplyResources(this.lRecords, "lRecords");
            this.lRecords.Name = "lRecords";
            // 
            // tbRecords
            // 
            resources.ApplyResources(this.tbRecords, "tbRecords");
            this.tbRecords.Name = "tbRecords";
            this.tbRecords.TextChanged += new System.EventHandler(this.Part_Changed);
            // 
            // lStations
            // 
            resources.ApplyResources(this.lStations, "lStations");
            this.lStations.Name = "lStations";
            // 
            // tbStations
            // 
            resources.ApplyResources(this.tbStations, "tbStations");
            this.tbStations.Name = "tbStations";
            this.tbStations.TextChanged += new System.EventHandler(this.Part_Changed);
            // 
            // bStations
            // 
            resources.ApplyResources(this.bStations, "bStations");
            this.bStations.Name = "bStations";
            this.bStations.Click += new System.EventHandler(this.bStations_Click);
            // 
            // lStationsHint
            // 
            resources.ApplyResources(this.lStationsHint, "lStationsHint");
            this.lStationsHint.Name = "lStationsHint";
            // 
            // lPrevious
            // 
            resources.ApplyResources(this.lPrevious, "lPrevious");
            this.lPrevious.Name = "lPrevious";
            // 
            // cboPrevious
            // 
            resources.ApplyResources(this.cboPrevious, "cboPrevious");
            this.cboPrevious.FormattingEnabled = true;
            this.cboPrevious.Name = "cboPrevious";
            this.cboPrevious.TextChanged += new System.EventHandler(this.Part_Changed);
            // 
            // lWebSource
            // 
            resources.ApplyResources(this.lWebSource, "lWebSource");
            this.lWebSource.Name = "lWebSource";
            // 
            // tbWebSource
            // 
            resources.ApplyResources(this.tbWebSource, "tbWebSource");
            this.tbWebSource.Name = "tbWebSource";
            this.tbWebSource.TextChanged += new System.EventHandler(this.Part_Changed);
            // 
            // lWindows
            // 
            resources.ApplyResources(this.lWindows, "lWindows");
            this.lWindows.Name = "lWindows";
            // 
            // tbWindows
            // 
            resources.ApplyResources(this.tbWindows, "tbWindows");
            this.tbWindows.Name = "tbWindows";
            this.tbWindows.TextChanged += new System.EventHandler(this.Part_Changed);
            // 
            // lWindowsHint
            // 
            resources.ApplyResources(this.lWindowsHint, "lWindowsHint");
            this.lWindowsHint.Name = "lWindowsHint";
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
            this.codeCondition.TextChanged += new System.EventHandler(this.Part_Changed);
            // 
            // lTracks
            // 
            resources.ApplyResources(this.lTracks, "lTracks");
            this.lTracks.Name = "lTracks";
            // 
            // pTracks
            // 
            this.pTracks.Controls.Add(this.dgvTracks);
            this.pTracks.Controls.Add(this.tsTracks);
            resources.ApplyResources(this.pTracks, "pTracks");
            this.pTracks.Name = "pTracks";
            // 
            // dgvTracks
            // 
            this.dgvTracks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cTrackKey,
            this.cTrackPriority});
            resources.ApplyResources(this.dgvTracks, "dgvTracks");
            this.dgvTracks.AllowUserToAddRows = false;
            this.dgvTracks.AllowUserToDeleteRows = false;
            this.dgvTracks.AllowUserToResizeRows = false;
            this.dgvTracks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTracks.MultiSelect = false;
            this.dgvTracks.RowHeadersVisible = false;
            this.dgvTracks.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvTracks.Name = "dgvTracks";
            this.dgvTracks.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Tracks_CellValueChanged);
            this.dgvTracks.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.dgvTracks.CurrentCellDirtyStateChanged += new System.EventHandler(this.Grid_CurrentCellDirtyStateChanged);
            // 
            // tsTracks
            // 
            this.tsTracks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbTracksAdd,
            this.tsbTracksDelete,
            this.tssTracks1,
            this.tsbTracksUp,
            this.tsbTracksDown});
            resources.ApplyResources(this.tsTracks, "tsTracks");
            this.tsTracks.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsTracks.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTracks.Name = "tsTracks";
            // 
            // tsbTracksAdd
            // 
            resources.ApplyResources(this.tsbTracksAdd, "tsbTracksAdd");
            this.tsbTracksAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTracksAdd.IconName = "add";
            this.tsbTracksAdd.Name = "tsbTracksAdd";
            this.tsbTracksAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbTracksDelete
            // 
            resources.ApplyResources(this.tsbTracksDelete, "tsbTracksDelete");
            this.tsbTracksDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTracksDelete.IconName = "delete";
            this.tsbTracksDelete.Name = "tsbTracksDelete";
            this.tsbTracksDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // tssTracks1
            // 
            resources.ApplyResources(this.tssTracks1, "tssTracks1");
            this.tssTracks1.Name = "tssTracks1";
            // 
            // tsbTracksUp
            // 
            resources.ApplyResources(this.tsbTracksUp, "tsbTracksUp");
            this.tsbTracksUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTracksUp.IconName = "arrow-up";
            this.tsbTracksUp.Name = "tsbTracksUp";
            this.tsbTracksUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbTracksDown
            // 
            resources.ApplyResources(this.tsbTracksDown, "tsbTracksDown");
            this.tsbTracksDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTracksDown.IconName = "arrow-down";
            this.tsbTracksDown.Name = "tsbTracksDown";
            this.tsbTracksDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // pTable
            // 
            this.pTable.Controls.Add(this.lKey);
            this.pTable.Controls.Add(this.tbKey);
            this.pTable.Controls.Add(this.lName);
            this.pTable.Controls.Add(this.tbName);
            this.pTable.Controls.Add(this.lLine);
            this.pTable.Controls.Add(this.cboLine);
            this.pTable.Controls.Add(this.lLineInfo);
            this.pTable.Controls.Add(this.lSubLine);
            this.pTable.Controls.Add(this.tbSubLine);
            this.pTable.Controls.Add(this.lAddress);
            this.pTable.Controls.Add(this.tbAddress);
            this.pTable.Controls.Add(this.lAddressHint);
            this.pTable.Controls.Add(this.lSave);
            this.pTable.Controls.Add(this.tbSave);
            this.pTable.Controls.Add(this.lArrows);
            this.pTable.Controls.Add(this.tbArrows);
            this.pTable.Controls.Add(this.lArrowsHint);
            this.pTable.Controls.Add(this.cboxNoScroll);
            this.pTable.Controls.Add(this.lLights);
            this.pTable.Controls.Add(this.lLightTime);
            this.pTable.Controls.Add(this.tbLightTime0);
            this.pTable.Controls.Add(this.tbLightTime1);
            this.pTable.Controls.Add(this.lDay);
            this.pTable.Controls.Add(this.lNight);
            this.pTable.Controls.Add(this.lLight);
            this.pTable.Controls.Add(this.tbLight0);
            this.pTable.Controls.Add(this.tbLight1);
            this.pTable.Controls.Add(this.lLightOff);
            this.pTable.Controls.Add(this.tbLightOff0);
            this.pTable.Controls.Add(this.tbLightOff1);
            this.pTable.Controls.Add(this.lLightIfEmpty);
            this.pTable.Controls.Add(this.cboLightIfEmpty0);
            this.pTable.Controls.Add(this.cboLightIfEmpty1);
            this.pTable.Controls.Add(this.lLightOnTime);
            this.pTable.Controls.Add(this.tbLightOnTime);
            this.pTable.Controls.Add(this.lLightsHint);
            resources.ApplyResources(this.pTable, "pTable");
            this.pTable.Name = "pTable";
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
            this.tbKey.TextChanged += new System.EventHandler(this.Board_Changed);
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
            this.tbName.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lLine
            // 
            resources.ApplyResources(this.lLine, "lLine");
            this.lLine.Name = "lLine";
            // 
            // cboLine
            // 
            resources.ApplyResources(this.cboLine, "cboLine");
            this.cboLine.FormattingEnabled = true;
            this.cboLine.Name = "cboLine";
            this.cboLine.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lLineInfo
            // 
            resources.ApplyResources(this.lLineInfo, "lLineInfo");
            this.lLineInfo.Name = "lLineInfo";
            // 
            // lSubLine
            // 
            resources.ApplyResources(this.lSubLine, "lSubLine");
            this.lSubLine.Name = "lSubLine";
            // 
            // tbSubLine
            // 
            resources.ApplyResources(this.tbSubLine, "tbSubLine");
            this.tbSubLine.Name = "tbSubLine";
            this.tbSubLine.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lAddress
            // 
            resources.ApplyResources(this.lAddress, "lAddress");
            this.lAddress.Name = "lAddress";
            // 
            // tbAddress
            // 
            resources.ApplyResources(this.tbAddress, "tbAddress");
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lAddressHint
            // 
            resources.ApplyResources(this.lAddressHint, "lAddressHint");
            this.lAddressHint.Name = "lAddressHint";
            // 
            // lSave
            // 
            resources.ApplyResources(this.lSave, "lSave");
            this.lSave.Name = "lSave";
            // 
            // tbSave
            // 
            resources.ApplyResources(this.tbSave, "tbSave");
            this.tbSave.Name = "tbSave";
            this.tbSave.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lArrows
            // 
            resources.ApplyResources(this.lArrows, "lArrows");
            this.lArrows.Name = "lArrows";
            // 
            // tbArrows
            // 
            resources.ApplyResources(this.tbArrows, "tbArrows");
            this.tbArrows.Name = "tbArrows";
            this.tbArrows.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lArrowsHint
            // 
            resources.ApplyResources(this.lArrowsHint, "lArrowsHint");
            this.lArrowsHint.Name = "lArrowsHint";
            // 
            // cboxNoScroll
            // 
            resources.ApplyResources(this.cboxNoScroll, "cboxNoScroll");
            this.cboxNoScroll.Name = "cboxNoScroll";
            this.cboxNoScroll.CheckedChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lLights
            // 
            resources.ApplyResources(this.lLights, "lLights");
            this.lLights.Name = "lLights";
            // 
            // lLightTime
            // 
            resources.ApplyResources(this.lLightTime, "lLightTime");
            this.lLightTime.Name = "lLightTime";
            // 
            // tbLightTime0
            // 
            resources.ApplyResources(this.tbLightTime0, "tbLightTime0");
            this.tbLightTime0.Name = "tbLightTime0";
            this.tbLightTime0.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // tbLightTime1
            // 
            resources.ApplyResources(this.tbLightTime1, "tbLightTime1");
            this.tbLightTime1.Name = "tbLightTime1";
            this.tbLightTime1.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lDay
            // 
            resources.ApplyResources(this.lDay, "lDay");
            this.lDay.Name = "lDay";
            // 
            // lNight
            // 
            resources.ApplyResources(this.lNight, "lNight");
            this.lNight.Name = "lNight";
            // 
            // lLight
            // 
            resources.ApplyResources(this.lLight, "lLight");
            this.lLight.Name = "lLight";
            // 
            // tbLight0
            // 
            resources.ApplyResources(this.tbLight0, "tbLight0");
            this.tbLight0.Name = "tbLight0";
            this.tbLight0.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // tbLight1
            // 
            resources.ApplyResources(this.tbLight1, "tbLight1");
            this.tbLight1.Name = "tbLight1";
            this.tbLight1.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lLightOff
            // 
            resources.ApplyResources(this.lLightOff, "lLightOff");
            this.lLightOff.Name = "lLightOff";
            // 
            // tbLightOff0
            // 
            resources.ApplyResources(this.tbLightOff0, "tbLightOff0");
            this.tbLightOff0.Name = "tbLightOff0";
            this.tbLightOff0.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // tbLightOff1
            // 
            resources.ApplyResources(this.tbLightOff1, "tbLightOff1");
            this.tbLightOff1.Name = "tbLightOff1";
            this.tbLightOff1.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lLightIfEmpty
            // 
            resources.ApplyResources(this.lLightIfEmpty, "lLightIfEmpty");
            this.lLightIfEmpty.Name = "lLightIfEmpty";
            // 
            // cboLightIfEmpty0
            // 
            resources.ApplyResources(this.cboLightIfEmpty0, "cboLightIfEmpty0");
            this.cboLightIfEmpty0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLightIfEmpty0.FormattingEnabled = true;
            this.cboLightIfEmpty0.Name = "cboLightIfEmpty0";
            this.cboLightIfEmpty0.SelectedIndexChanged += new System.EventHandler(this.Board_Changed);
            // 
            // cboLightIfEmpty1
            // 
            resources.ApplyResources(this.cboLightIfEmpty1, "cboLightIfEmpty1");
            this.cboLightIfEmpty1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLightIfEmpty1.FormattingEnabled = true;
            this.cboLightIfEmpty1.Name = "cboLightIfEmpty1";
            this.cboLightIfEmpty1.SelectedIndexChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lLightOnTime
            // 
            resources.ApplyResources(this.lLightOnTime, "lLightOnTime");
            this.lLightOnTime.Name = "lLightOnTime";
            // 
            // tbLightOnTime
            // 
            resources.ApplyResources(this.tbLightOnTime, "tbLightOnTime");
            this.tbLightOnTime.Name = "tbLightOnTime";
            this.tbLightOnTime.TextChanged += new System.EventHandler(this.Board_Changed);
            // 
            // lLightsHint
            // 
            resources.ApplyResources(this.lLightsHint, "lLightsHint");
            this.lLightsHint.Name = "lLightsHint";
            // 
            // tpTemplates
            // 
            this.tpTemplates.Controls.Add(this.splitTemplates);
            resources.ApplyResources(this.tpTemplates, "tpTemplates");
            this.tpTemplates.UseVisualStyleBackColor = true;
            this.tpTemplates.Name = "tpTemplates";
            // 
            // splitTemplates
            // 
            resources.ApplyResources(this.splitTemplates, "splitTemplates");
            this.splitTemplates.Name = "splitTemplates";
            // 
            // splitTemplates.Panel1
            // 
            this.splitTemplates.Panel1.Controls.Add(this.lbTemplates);
            this.splitTemplates.Panel1.Controls.Add(this.tsTemplates);
            // 
            // splitTemplates.Panel2
            // 
            this.splitTemplates.Panel2.Controls.Add(this.dgvColumns);
            this.splitTemplates.Panel2.Controls.Add(this.tsColumns);
            this.splitTemplates.Panel2.Controls.Add(this.lColumns);
            this.splitTemplates.Panel2.Controls.Add(this.pTemplate);
            // 
            // lbTemplates
            // 
            resources.ApplyResources(this.lbTemplates, "lbTemplates");
            this.lbTemplates.FormattingEnabled = true;
            this.lbTemplates.IntegralHeight = false;
            this.lbTemplates.Name = "lbTemplates";
            this.lbTemplates.SelectedIndexChanged += new System.EventHandler(this.lbTemplates_SelectedIndexChanged);
            // 
            // tsTemplates
            // 
            this.tsTemplates.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbTemplatesAdd,
            this.tsbTemplatesDelete,
            this.tssTemplates1,
            this.tsbTemplatesUp,
            this.tsbTemplatesDown});
            resources.ApplyResources(this.tsTemplates, "tsTemplates");
            this.tsTemplates.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsTemplates.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTemplates.Name = "tsTemplates";
            // 
            // tsbTemplatesAdd
            // 
            resources.ApplyResources(this.tsbTemplatesAdd, "tsbTemplatesAdd");
            this.tsbTemplatesAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTemplatesAdd.IconName = "add";
            this.tsbTemplatesAdd.Name = "tsbTemplatesAdd";
            this.tsbTemplatesAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbTemplatesDelete
            // 
            resources.ApplyResources(this.tsbTemplatesDelete, "tsbTemplatesDelete");
            this.tsbTemplatesDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTemplatesDelete.IconName = "delete";
            this.tsbTemplatesDelete.Name = "tsbTemplatesDelete";
            this.tsbTemplatesDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // tssTemplates1
            // 
            resources.ApplyResources(this.tssTemplates1, "tssTemplates1");
            this.tssTemplates1.Name = "tssTemplates1";
            // 
            // tsbTemplatesUp
            // 
            resources.ApplyResources(this.tsbTemplatesUp, "tsbTemplatesUp");
            this.tsbTemplatesUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTemplatesUp.IconName = "arrow-up";
            this.tsbTemplatesUp.Name = "tsbTemplatesUp";
            this.tsbTemplatesUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbTemplatesDown
            // 
            resources.ApplyResources(this.tsbTemplatesDown, "tsbTemplatesDown");
            this.tsbTemplatesDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbTemplatesDown.IconName = "arrow-down";
            this.tsbTemplatesDown.Name = "tsbTemplatesDown";
            this.tsbTemplatesDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // dgvColumns
            // 
            this.dgvColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cColumnKey,
            this.cColumnName,
            this.cColumnContent,
            this.cColumnRow,
            this.cColumnStart,
            this.cColumnEnd,
            this.cColumnFont,
            this.cColumnAlign,
            this.cColumnTranslation,
            this.cColumnTable1,
            this.cColumnTable2});
            resources.ApplyResources(this.dgvColumns, "dgvColumns");
            this.dgvColumns.AllowUserToAddRows = false;
            this.dgvColumns.AllowUserToDeleteRows = false;
            this.dgvColumns.AllowUserToResizeRows = false;
            this.dgvColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvColumns.MultiSelect = false;
            this.dgvColumns.RowHeadersVisible = false;
            this.dgvColumns.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvColumns.Name = "dgvColumns";
            this.dgvColumns.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Columns_CellValueChanged);
            this.dgvColumns.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.dgvColumns.CurrentCellDirtyStateChanged += new System.EventHandler(this.Grid_CurrentCellDirtyStateChanged);
            // 
            // tsColumns
            // 
            this.tsColumns.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbColumnsAdd,
            this.tsbColumnsDelete,
            this.tssColumns1,
            this.tsbColumnsUp,
            this.tsbColumnsDown});
            resources.ApplyResources(this.tsColumns, "tsColumns");
            this.tsColumns.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsColumns.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsColumns.Name = "tsColumns";
            // 
            // tsbColumnsAdd
            // 
            resources.ApplyResources(this.tsbColumnsAdd, "tsbColumnsAdd");
            this.tsbColumnsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbColumnsAdd.IconName = "add";
            this.tsbColumnsAdd.Name = "tsbColumnsAdd";
            this.tsbColumnsAdd.Click += new System.EventHandler(this.tsbAdd_Click);
            // 
            // tsbColumnsDelete
            // 
            resources.ApplyResources(this.tsbColumnsDelete, "tsbColumnsDelete");
            this.tsbColumnsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbColumnsDelete.IconName = "delete";
            this.tsbColumnsDelete.Name = "tsbColumnsDelete";
            this.tsbColumnsDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // tssColumns1
            // 
            resources.ApplyResources(this.tssColumns1, "tssColumns1");
            this.tssColumns1.Name = "tssColumns1";
            // 
            // tsbColumnsUp
            // 
            resources.ApplyResources(this.tsbColumnsUp, "tsbColumnsUp");
            this.tsbColumnsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbColumnsUp.IconName = "arrow-up";
            this.tsbColumnsUp.Name = "tsbColumnsUp";
            this.tsbColumnsUp.Click += new System.EventHandler(this.tsbUp_Click);
            // 
            // tsbColumnsDown
            // 
            resources.ApplyResources(this.tsbColumnsDown, "tsbColumnsDown");
            this.tsbColumnsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbColumnsDown.IconName = "arrow-down";
            this.tsbColumnsDown.Name = "tsbColumnsDown";
            this.tsbColumnsDown.Click += new System.EventHandler(this.tsbDown_Click);
            // 
            // lColumns
            // 
            resources.ApplyResources(this.lColumns, "lColumns");
            this.lColumns.Name = "lColumns";
            // 
            // pTemplate
            // 
            this.pTemplate.Controls.Add(this.lTemplateKey);
            this.pTemplate.Controls.Add(this.tbTemplateKey);
            this.pTemplate.Controls.Add(this.lTemplateName);
            this.pTemplate.Controls.Add(this.tbTemplateName);
            this.pTemplate.Controls.Add(this.lClass);
            this.pTemplate.Controls.Add(this.cboClass);
            this.pTemplate.Controls.Add(this.lFontBank);
            this.pTemplate.Controls.Add(this.tbFontBank);
            resources.ApplyResources(this.pTemplate, "pTemplate");
            this.pTemplate.Name = "pTemplate";
            // 
            // lTemplateKey
            // 
            resources.ApplyResources(this.lTemplateKey, "lTemplateKey");
            this.lTemplateKey.Name = "lTemplateKey";
            // 
            // tbTemplateKey
            // 
            resources.ApplyResources(this.tbTemplateKey, "tbTemplateKey");
            this.tbTemplateKey.Name = "tbTemplateKey";
            this.tbTemplateKey.TextChanged += new System.EventHandler(this.Template_Changed);
            // 
            // lTemplateName
            // 
            resources.ApplyResources(this.lTemplateName, "lTemplateName");
            this.lTemplateName.Name = "lTemplateName";
            // 
            // tbTemplateName
            // 
            resources.ApplyResources(this.tbTemplateName, "tbTemplateName");
            this.tbTemplateName.Name = "tbTemplateName";
            this.tbTemplateName.TextChanged += new System.EventHandler(this.Template_Changed);
            // 
            // lClass
            // 
            resources.ApplyResources(this.lClass, "lClass");
            this.lClass.Name = "lClass";
            // 
            // cboClass
            // 
            resources.ApplyResources(this.cboClass, "cboClass");
            this.cboClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClass.FormattingEnabled = true;
            this.cboClass.Name = "cboClass";
            this.cboClass.TextChanged += new System.EventHandler(this.Template_Changed);
            // 
            // lFontBank
            // 
            resources.ApplyResources(this.lFontBank, "lFontBank");
            this.lFontBank.Name = "lFontBank";
            // 
            // tbFontBank
            // 
            resources.ApplyResources(this.tbFontBank, "tbFontBank");
            this.tbFontBank.Name = "tbFontBank";
            this.tbFontBank.TextChanged += new System.EventHandler(this.Template_Changed);
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
            // FBoards
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1380, 746);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.flpStation);
            this.Controls.Add(this.lbProblems);
            this.Controls.Add(this.flpButtons);
            this.Name = "FBoards";
            resources.ApplyResources(this, "$this");
            // 
            // cTrackKey
            // 
            resources.ApplyResources(this.cTrackKey, "cTrackKey");
            this.cTrackKey.DataPropertyName = "Key";
            this.cTrackKey.Name = "cTrackKey";
            // 
            // cTrackPriority
            // 
            resources.ApplyResources(this.cTrackPriority, "cTrackPriority");
            this.cTrackPriority.DataPropertyName = "Priority";
            this.cTrackPriority.Name = "cTrackPriority";
            // 
            // cColumnKey
            // 
            resources.ApplyResources(this.cColumnKey, "cColumnKey");
            this.cColumnKey.DataPropertyName = "Key";
            this.cColumnKey.Name = "cColumnKey";
            // 
            // cColumnName
            // 
            resources.ApplyResources(this.cColumnName, "cColumnName");
            this.cColumnName.DataPropertyName = "Name";
            this.cColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cColumnName.MinimumWidth = 120;
            this.cColumnName.Name = "cColumnName";
            // 
            // cColumnContent
            // 
            resources.ApplyResources(this.cColumnContent, "cColumnContent");
            this.cColumnContent.DataPropertyName = "Content";
            this.cColumnContent.Name = "cColumnContent";
            // 
            // cColumnRow
            // 
            resources.ApplyResources(this.cColumnRow, "cColumnRow");
            this.cColumnRow.DataPropertyName = "Row";
            this.cColumnRow.Name = "cColumnRow";
            // 
            // cColumnStart
            // 
            resources.ApplyResources(this.cColumnStart, "cColumnStart");
            this.cColumnStart.DataPropertyName = "Start";
            this.cColumnStart.Name = "cColumnStart";
            // 
            // cColumnEnd
            // 
            resources.ApplyResources(this.cColumnEnd, "cColumnEnd");
            this.cColumnEnd.DataPropertyName = "End";
            this.cColumnEnd.Name = "cColumnEnd";
            // 
            // cColumnFont
            // 
            resources.ApplyResources(this.cColumnFont, "cColumnFont");
            this.cColumnFont.DataPropertyName = "Font";
            this.cColumnFont.Name = "cColumnFont";
            // 
            // cColumnAlign
            // 
            resources.ApplyResources(this.cColumnAlign, "cColumnAlign");
            this.cColumnAlign.DataPropertyName = "Align";
            this.cColumnAlign.Name = "cColumnAlign";
            // 
            // cColumnTranslation
            // 
            resources.ApplyResources(this.cColumnTranslation, "cColumnTranslation");
            this.cColumnTranslation.DataPropertyName = "Translation";
            this.cColumnTranslation.Name = "cColumnTranslation";
            // 
            // cColumnTable1
            // 
            resources.ApplyResources(this.cColumnTable1, "cColumnTable1");
            this.cColumnTable1.DataPropertyName = "Table1";
            this.cColumnTable1.Name = "cColumnTable1";
            // 
            // cColumnTable2
            // 
            resources.ApplyResources(this.cColumnTable2, "cColumnTable2");
            this.cColumnTable2.DataPropertyName = "Table2";
            this.cColumnTable2.Name = "cColumnTable2";
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.flpStation.ResumeLayout(false);
            this.flpStation.PerformLayout();
            this.pTemplate.ResumeLayout(false);
            this.pTemplate.PerformLayout();
            this.tsColumns.ResumeLayout(false);
            this.tsColumns.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumns)).EndInit();
            this.tsTemplates.ResumeLayout(false);
            this.tsTemplates.PerformLayout();
            this.splitTemplates.Panel1.ResumeLayout(false);
            this.splitTemplates.Panel1.PerformLayout();
            this.splitTemplates.Panel2.ResumeLayout(false);
            this.splitTemplates.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTemplates)).EndInit();
            this.splitTemplates.ResumeLayout(false);
            this.splitTemplates.PerformLayout();
            this.tpTemplates.ResumeLayout(false);
            this.tpTemplates.PerformLayout();
            this.pTable.ResumeLayout(false);
            this.pTable.PerformLayout();
            this.tsTracks.ResumeLayout(false);
            this.tsTracks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTracks)).EndInit();
            this.pTracks.ResumeLayout(false);
            this.pTracks.PerformLayout();
            this.pPart.ResumeLayout(false);
            this.pPart.PerformLayout();
            this.tsBoards.ResumeLayout(false);
            this.tsBoards.PerformLayout();
            this.splitTables.Panel1.ResumeLayout(false);
            this.splitTables.Panel1.PerformLayout();
            this.splitTables.Panel2.ResumeLayout(false);
            this.splitTables.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTables)).EndInit();
            this.splitTables.ResumeLayout(false);
            this.splitTables.PerformLayout();
            this.tpTables.ResumeLayout(false);
            this.tpTables.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tcMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExControls.ExTabControl tcMain;
        private System.Windows.Forms.TabPage tpTables;
        private System.Windows.Forms.SplitContainer splitTables;
        private ExControls.ExTreeView tvBoards;
        private System.Windows.Forms.ToolStrip tsBoards;
        private ToolsCore.Icons.IconToolStripButton tsbAddTable;
        private ToolsCore.Icons.IconToolStripButton tsbAddPart;
        private ToolsCore.Icons.IconToolStripButton tsbDeleteItem;
        private System.Windows.Forms.ToolStripSeparator tssBoards1;
        private ToolsCore.Icons.IconToolStripButton tsbItemUp;
        private ToolsCore.Icons.IconToolStripButton tsbItemDown;
        private System.Windows.Forms.Panel pPart;
        private System.Windows.Forms.Label lPartName;
        private ExControls.ExTextBox tbPartName;
        private System.Windows.Forms.Label lPartNameHint;
        private System.Windows.Forms.Label lTemplate;
        private ExControls.ExComboBox cboTemplate;
        private System.Windows.Forms.Label lKind;
        private ExControls.ExComboBox cboKind;
        private System.Windows.Forms.Label lRecords;
        private ExControls.ExTextBox tbRecords;
        private System.Windows.Forms.Label lStations;
        private ExControls.ExTextBox tbStations;
        private ExControls.ExButton bStations;
        private System.Windows.Forms.Label lStationsHint;
        private System.Windows.Forms.Label lPrevious;
        private ExControls.ExComboBox cboPrevious;
        private System.Windows.Forms.Label lWebSource;
        private ExControls.ExTextBox tbWebSource;
        private System.Windows.Forms.Label lWindows;
        private ExControls.ExTextBox tbWindows;
        private System.Windows.Forms.Label lWindowsHint;
        private System.Windows.Forms.Label lCondition;
        private ToolsCore.CodeEditor.CodeEditorBox codeCondition;
        private System.Windows.Forms.Label lTracks;
        private System.Windows.Forms.Panel pTracks;
        private System.Windows.Forms.DataGridView dgvTracks;
        private ExControls.DataGridViewExComboBoxColumn cTrackKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTrackPriority;
        private System.Windows.Forms.ToolStrip tsTracks;
        private ToolsCore.Icons.IconToolStripButton tsbTracksAdd;
        private ToolsCore.Icons.IconToolStripButton tsbTracksDelete;
        private System.Windows.Forms.ToolStripSeparator tssTracks1;
        private ToolsCore.Icons.IconToolStripButton tsbTracksUp;
        private ToolsCore.Icons.IconToolStripButton tsbTracksDown;
        private System.Windows.Forms.Panel pTable;
        private System.Windows.Forms.Label lKey;
        private ExControls.ExTextBox tbKey;
        private System.Windows.Forms.Label lName;
        private ExControls.ExTextBox tbName;
        private System.Windows.Forms.Label lLine;
        private ExControls.ExComboBox cboLine;
        private System.Windows.Forms.Label lLineInfo;
        private System.Windows.Forms.Label lSubLine;
        private ExControls.ExTextBox tbSubLine;
        private System.Windows.Forms.Label lAddress;
        private ExControls.ExTextBox tbAddress;
        private System.Windows.Forms.Label lAddressHint;
        private System.Windows.Forms.Label lSave;
        private ExControls.ExTextBox tbSave;
        private System.Windows.Forms.Label lArrows;
        private ExControls.ExTextBox tbArrows;
        private System.Windows.Forms.Label lArrowsHint;
        private ExControls.ExCheckBox cboxNoScroll;
        private System.Windows.Forms.Label lLights;
        private System.Windows.Forms.Label lLightTime;
        private ExControls.ExTextBox tbLightTime0;
        private ExControls.ExTextBox tbLightTime1;
        private System.Windows.Forms.Label lDay;
        private System.Windows.Forms.Label lNight;
        private System.Windows.Forms.Label lLight;
        private ExControls.ExTextBox tbLight0;
        private ExControls.ExTextBox tbLight1;
        private System.Windows.Forms.Label lLightOff;
        private ExControls.ExTextBox tbLightOff0;
        private ExControls.ExTextBox tbLightOff1;
        private System.Windows.Forms.Label lLightIfEmpty;
        private ExControls.ExComboBox cboLightIfEmpty0;
        private ExControls.ExComboBox cboLightIfEmpty1;
        private System.Windows.Forms.Label lLightOnTime;
        private ExControls.ExTextBox tbLightOnTime;
        private System.Windows.Forms.Label lLightsHint;
        private System.Windows.Forms.TabPage tpTemplates;
        private System.Windows.Forms.SplitContainer splitTemplates;
        private ExControls.ExListBox lbTemplates;
        private System.Windows.Forms.ToolStrip tsTemplates;
        private ToolsCore.Icons.IconToolStripButton tsbTemplatesAdd;
        private ToolsCore.Icons.IconToolStripButton tsbTemplatesDelete;
        private System.Windows.Forms.ToolStripSeparator tssTemplates1;
        private ToolsCore.Icons.IconToolStripButton tsbTemplatesUp;
        private ToolsCore.Icons.IconToolStripButton tsbTemplatesDown;
        private System.Windows.Forms.DataGridView dgvColumns;
        private System.Windows.Forms.DataGridViewTextBoxColumn cColumnKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cColumnName;
        private ExControls.DataGridViewExComboBoxColumn cColumnContent;
        private System.Windows.Forms.DataGridViewTextBoxColumn cColumnRow;
        private System.Windows.Forms.DataGridViewTextBoxColumn cColumnStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn cColumnEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn cColumnFont;
        private ExControls.DataGridViewExComboBoxColumn cColumnAlign;
        private ExControls.DataGridViewExComboBoxColumn cColumnTranslation;
        private ExControls.DataGridViewExComboBoxColumn cColumnTable1;
        private ExControls.DataGridViewExComboBoxColumn cColumnTable2;
        private System.Windows.Forms.ToolStrip tsColumns;
        private ToolsCore.Icons.IconToolStripButton tsbColumnsAdd;
        private ToolsCore.Icons.IconToolStripButton tsbColumnsDelete;
        private System.Windows.Forms.ToolStripSeparator tssColumns1;
        private ToolsCore.Icons.IconToolStripButton tsbColumnsUp;
        private ToolsCore.Icons.IconToolStripButton tsbColumnsDown;
        private System.Windows.Forms.Label lColumns;
        private System.Windows.Forms.Panel pTemplate;
        private System.Windows.Forms.Label lTemplateKey;
        private ExControls.ExTextBox tbTemplateKey;
        private System.Windows.Forms.Label lTemplateName;
        private ExControls.ExTextBox tbTemplateName;
        private System.Windows.Forms.Label lClass;
        private ExControls.ExComboBox cboClass;
        private System.Windows.Forms.Label lFontBank;
        private ExControls.ExTextBox tbFontBank;
        private System.Windows.Forms.FlowLayoutPanel flpStation;
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
