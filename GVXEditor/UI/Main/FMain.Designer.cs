namespace GVXEditor.UI.Main
{
    partial class FMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.pDocument = new System.Windows.Forms.Panel();
            this.pTrains = new System.Windows.Forms.Panel();
            this.splitTrains = new System.Windows.Forms.SplitContainer();
            this.dgvTrains = new System.Windows.Forms.DataGridView();
            this.cNumbers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDeparture = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cArrival = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVariants = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOperator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitDetail = new System.Windows.Forms.SplitContainer();
            this.dgvVariants = new System.Windows.Forms.DataGridView();
            this.cVarIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVarValidity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVarDays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVarOperator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVarFlags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVarNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lVariants = new System.Windows.Forms.Label();
            this.dgvStops = new System.Windows.Forms.DataGridView();
            this.cStopStation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopArrival = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopDeparture = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopFlags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopDisplay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStopReplacement = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lStops = new System.Windows.Forms.Label();
            this.pStation = new System.Windows.Forms.Panel();
            this.dgvStation = new System.Windows.Forms.DataGridView();
            this.cDayArrival = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayDeparture = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayArrivalNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayDepartureNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayTrack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDayVariant = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pStationTop = new System.Windows.Forms.FlowLayoutPanel();
            this.lDay = new System.Windows.Forms.Label();
            this.dtpDay = new ExControls.ExDateTimePicker();
            this.lStation = new System.Windows.Forms.Label();
            this.lEmpty = new System.Windows.Forms.Label();
            this.tvData = new ExControls.ExTreeView();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.tsslServer = new ToolsCore.Icons.IconToolStripStatusLabel();
            this.tsslCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tsbOpen = new ToolsCore.Icons.IconToolStripButton();
            this.tssbRecent = new ToolsCore.Icons.IconToolStripDropDownButton();
            this.tsbSave = new ToolsCore.Icons.IconToolStripButton();
            this.tssTool1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbReload = new ToolsCore.Icons.IconToolStripButton();
            this.tssTool2 = new System.Windows.Forms.ToolStripSeparator();
            this.tssbUndo = new ToolsCore.Icons.IconToolStripSplitButton();
            this.tssbRedo = new ToolsCore.Icons.IconToolStripSplitButton();
            this.tssTool3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbNewTrain = new ToolsCore.Icons.IconToolStripButton();
            this.tsbEditTrain = new ToolsCore.Icons.IconToolStripButton();
            this.tsbCopyTrain = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDeleteTrain = new ToolsCore.Icons.IconToolStripButton();
            this.tssTool5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbGroupSetup = new ToolsCore.Icons.IconToolStripButton();
            this.tsbConstReports = new ToolsCore.Icons.IconToolStripButton();
            this.tssTool4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAppSettings = new ToolsCore.Icons.IconToolStripButton();
            this.tsbInfoApp = new ToolsCore.Icons.IconToolStripButton();
            this.titleBar = new ExControls.ExTitleBar();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimOpen = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimRecent = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tssFile1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimSave = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimCheck = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimVerifyServer = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimReload = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimUndo = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimRedo = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tssEdit1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimNewTrain = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimEditTrain = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimCopyTrain = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimDeleteTrain = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimRenameNumber = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tssEdit2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimShiftValidity = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimViewInstallation = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimViewAnalysis = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tssView1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimResetLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimGroupSetup = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimConstReports = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tssGroup2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimStateDiagram = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimTabTab = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimReports = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimLogText = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimBoards = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimGenerateTexts = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tssGroup1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimNewGroup = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimAddStation = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimAppSettings = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimInfoApp = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimCheckUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimShowLogs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimReportBug = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimSuggestFeature = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsTrains = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmicNewTrain = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmicEditTrain = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmicCopyTrain = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmicDeleteTrain = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmicRenameNumber = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tssTrains1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmicShiftValidity = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmsTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmitGroupSetup = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmitStateDiagram = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmitTabTab = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmitBoards = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmitGenerateTexts = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tssTree1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmitAddStation = new ToolsCore.Icons.IconToolStripMenuItem();
            this.pDocument.SuspendLayout();
            this.pTrains.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTrains)).BeginInit();
            this.splitTrains.Panel1.SuspendLayout();
            this.splitTrains.Panel2.SuspendLayout();
            this.splitTrains.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTrains)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitDetail)).BeginInit();
            this.splitDetail.Panel1.SuspendLayout();
            this.splitDetail.Panel2.SuspendLayout();
            this.splitDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariants)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStops)).BeginInit();
            this.pStation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStation)).BeginInit();
            this.pStationTop.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.titleBar.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.cmsTrains.SuspendLayout();
            this.cmsTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // pDocument
            // 
            this.pDocument.Controls.Add(this.pTrains);
            this.pDocument.Controls.Add(this.pStation);
            this.pDocument.Controls.Add(this.lEmpty);
            resources.ApplyResources(this.pDocument, "pDocument");
            this.pDocument.Name = "pDocument";
            // 
            // pTrains
            // 
            this.pTrains.Controls.Add(this.splitTrains);
            resources.ApplyResources(this.pTrains, "pTrains");
            this.pTrains.Name = "pTrains";
            // 
            // splitTrains
            // 
            resources.ApplyResources(this.splitTrains, "splitTrains");
            this.splitTrains.Name = "splitTrains";
            // 
            // splitTrains.Panel1
            // 
            this.splitTrains.Panel1.Controls.Add(this.dgvTrains);
            // 
            // splitTrains.Panel2
            // 
            this.splitTrains.Panel2.Controls.Add(this.splitDetail);
            // 
            // dgvTrains
            // 
            this.dgvTrains.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cNumbers,
            this.cType,
            this.cName,
            this.cFrom,
            this.cDeparture,
            this.cTo,
            this.cArrival,
            this.cValidity,
            this.cVariants,
            this.cOperator});
            resources.ApplyResources(this.dgvTrains, "dgvTrains");
            this.dgvTrains.ContextMenuStrip = this.cmsTrains;
            this.dgvTrains.AllowUserToAddRows = false;
            this.dgvTrains.AllowUserToDeleteRows = false;
            this.dgvTrains.AllowUserToResizeRows = false;
            this.dgvTrains.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTrains.MultiSelect = true;
            this.dgvTrains.ReadOnly = true;
            this.dgvTrains.RowHeadersVisible = false;
            this.dgvTrains.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTrains.Name = "dgvTrains";
            this.dgvTrains.SelectionChanged += new System.EventHandler(this.dgvTrains_SelectionChanged);
            this.dgvTrains.CurrentCellChanged += new System.EventHandler(this.dgvTrains_CurrentCellChanged);
            this.dgvTrains.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTrains_CellDoubleClick);
            this.dgvTrains.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvTrains_CellMouseDown);
            this.dgvTrains.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvTrains_CellFormatting);
            this.dgvTrains.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvTrains_ColumnHeaderMouseClick);
            // 
            // splitDetail
            // 
            resources.ApplyResources(this.splitDetail, "splitDetail");
            this.splitDetail.Name = "splitDetail";
            // 
            // splitDetail.Panel1
            // 
            this.splitDetail.Panel1.Controls.Add(this.dgvVariants);
            this.splitDetail.Panel1.Controls.Add(this.lVariants);
            // 
            // splitDetail.Panel2
            // 
            this.splitDetail.Panel2.Controls.Add(this.dgvStops);
            this.splitDetail.Panel2.Controls.Add(this.lStops);
            // 
            // dgvVariants
            // 
            this.dgvVariants.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cVarIndex,
            this.cVarValidity,
            this.cVarDays,
            this.cVarOperator,
            this.cVarFlags,
            this.cVarNote});
            resources.ApplyResources(this.dgvVariants, "dgvVariants");
            this.dgvVariants.AllowUserToAddRows = false;
            this.dgvVariants.AllowUserToDeleteRows = false;
            this.dgvVariants.AllowUserToResizeRows = false;
            this.dgvVariants.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVariants.MultiSelect = false;
            this.dgvVariants.ReadOnly = true;
            this.dgvVariants.RowHeadersVisible = false;
            this.dgvVariants.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVariants.Name = "dgvVariants";
            this.dgvVariants.SelectionChanged += new System.EventHandler(this.dgvVariants_SelectionChanged);
            // 
            // lVariants
            // 
            resources.ApplyResources(this.lVariants, "lVariants");
            this.lVariants.Name = "lVariants";
            // 
            // dgvStops
            // 
            this.dgvStops.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cStopStation,
            this.cStopCode,
            this.cStopArrival,
            this.cStopDeparture,
            this.cStopFlags,
            this.cStopNumber,
            this.cStopDisplay,
            this.cStopType,
            this.cStopName,
            this.cStopLine,
            this.cStopReplacement});
            resources.ApplyResources(this.dgvStops, "dgvStops");
            this.dgvStops.AllowUserToAddRows = false;
            this.dgvStops.AllowUserToDeleteRows = false;
            this.dgvStops.AllowUserToResizeRows = false;
            this.dgvStops.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStops.MultiSelect = false;
            this.dgvStops.ReadOnly = true;
            this.dgvStops.RowHeadersVisible = false;
            this.dgvStops.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStops.Name = "dgvStops";
            // 
            // lStops
            // 
            resources.ApplyResources(this.lStops, "lStops");
            this.lStops.Name = "lStops";
            // 
            // pStation
            // 
            this.pStation.Controls.Add(this.dgvStation);
            this.pStation.Controls.Add(this.pStationTop);
            resources.ApplyResources(this.pStation, "pStation");
            this.pStation.Name = "pStation";
            // 
            // dgvStation
            // 
            this.dgvStation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cDayArrival,
            this.cDayDeparture,
            this.cDayArrivalNumber,
            this.cDayDepartureNumber,
            this.cDayType,
            this.cDayName,
            this.cDayFrom,
            this.cDayTo,
            this.cDayTrack,
            this.cDayVariant});
            resources.ApplyResources(this.dgvStation, "dgvStation");
            this.dgvStation.AllowUserToAddRows = false;
            this.dgvStation.AllowUserToDeleteRows = false;
            this.dgvStation.AllowUserToResizeRows = false;
            this.dgvStation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStation.MultiSelect = false;
            this.dgvStation.ReadOnly = true;
            this.dgvStation.RowHeadersVisible = false;
            this.dgvStation.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStation.Name = "dgvStation";
            // 
            // pStationTop
            // 
            this.pStationTop.Controls.Add(this.lDay);
            this.pStationTop.Controls.Add(this.dtpDay);
            this.pStationTop.Controls.Add(this.lStation);
            resources.ApplyResources(this.pStationTop, "pStationTop");
            this.pStationTop.Name = "pStationTop";
            // 
            // lDay
            // 
            resources.ApplyResources(this.lDay, "lDay");
            this.lDay.Name = "lDay";
            // 
            // dtpDay
            // 
            resources.ApplyResources(this.dtpDay, "dtpDay");
            this.dtpDay.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDay.Name = "dtpDay";
            this.dtpDay.ValueChanged += new System.EventHandler(this.dtpDay_ValueChanged);
            // 
            // lStation
            // 
            resources.ApplyResources(this.lStation, "lStation");
            this.lStation.Name = "lStation";
            // 
            // lEmpty
            // 
            resources.ApplyResources(this.lEmpty, "lEmpty");
            this.lEmpty.Name = "lEmpty";
            // 
            // tvData
            // 
            resources.ApplyResources(this.tvData, "tvData");
            this.tvData.ContextMenuStrip = this.cmsTree;
            this.tvData.FullRowSelect = true;
            this.tvData.HideSelection = false;
            this.tvData.ItemHeight = 24;
            this.tvData.ShowLines = false;
            this.tvData.Style = ExControls.ExTreeViewStyle.Light;
            this.tvData.Name = "tvData";
            this.tvData.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvData_AfterSelect);
            this.tvData.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvData_NodeMouseClick);
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslServer,
            this.tsslCount});
            resources.ApplyResources(this.statusStripMain, "statusStripMain");
            this.statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripMain.Name = "statusStripMain";
            // 
            // tsslServer
            // 
            resources.ApplyResources(this.tsslServer, "tsslServer");
            this.tsslServer.Name = "tsslServer";
            // 
            // tsslCount
            // 
            resources.ApplyResources(this.tsslCount, "tsslCount");
            this.tsslCount.Spring = true;
            this.tsslCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tsslCount.Name = "tsslCount";
            // 
            // toolStripMain
            // 
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpen,
            this.tssbRecent,
            this.tsbSave,
            this.tssTool1,
            this.tsbReload,
            this.tssTool2,
            this.tssbUndo,
            this.tssbRedo,
            this.tssTool3,
            this.tsbNewTrain,
            this.tsbEditTrain,
            this.tsbCopyTrain,
            this.tsbDeleteTrain,
            this.tssTool5,
            this.tsbGroupSetup,
            this.tsbConstReports,
            this.tssTool4,
            this.tsbAppSettings,
            this.tsbInfoApp});
            resources.ApplyResources(this.toolStripMain, "toolStripMain");
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripMain.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.toolStripMain.Name = "toolStripMain";
            // 
            // tsbOpen
            // 
            resources.ApplyResources(this.tsbOpen, "tsbOpen");
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.IconName = "folder-open";
            this.tsbOpen.Name = "tsbOpen";
            // 
            // tssbRecent
            // 
            resources.ApplyResources(this.tssbRecent, "tssbRecent");
            this.tssbRecent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tssbRecent.IconName = "history";
            this.tssbRecent.Name = "tssbRecent";
            // 
            // tsbSave
            // 
            resources.ApplyResources(this.tsbSave, "tsbSave");
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.IconName = "save";
            this.tsbSave.Name = "tsbSave";
            // 
            // tssTool1
            // 
            resources.ApplyResources(this.tssTool1, "tssTool1");
            this.tssTool1.Name = "tssTool1";
            // 
            // tsbReload
            // 
            resources.ApplyResources(this.tsbReload, "tsbReload");
            this.tsbReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReload.IconName = "arrow-reset";
            this.tsbReload.Name = "tsbReload";
            // 
            // tssTool2
            // 
            resources.ApplyResources(this.tssTool2, "tssTool2");
            this.tssTool2.Name = "tssTool2";
            // 
            // tssbUndo
            // 
            resources.ApplyResources(this.tssbUndo, "tssbUndo");
            this.tssbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tssbUndo.IconName = "undo";
            this.tssbUndo.Name = "tssbUndo";
            this.tssbUndo.DropDownOpening += new System.EventHandler(this.tssbUndo_DropDownOpening);
            // 
            // tssbRedo
            // 
            resources.ApplyResources(this.tssbRedo, "tssbRedo");
            this.tssbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tssbRedo.IconName = "redo";
            this.tssbRedo.Name = "tssbRedo";
            this.tssbRedo.DropDownOpening += new System.EventHandler(this.tssbRedo_DropDownOpening);
            // 
            // tssTool3
            // 
            resources.ApplyResources(this.tssTool3, "tssTool3");
            this.tssTool3.Name = "tssTool3";
            // 
            // tsbNewTrain
            // 
            resources.ApplyResources(this.tsbNewTrain, "tsbNewTrain");
            this.tsbNewTrain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNewTrain.IconName = "train";
            this.tsbNewTrain.IconBadge = "add";
            this.tsbNewTrain.Name = "tsbNewTrain";
            // 
            // tsbEditTrain
            // 
            resources.ApplyResources(this.tsbEditTrain, "tsbEditTrain");
            this.tsbEditTrain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEditTrain.IconName = "train";
            this.tsbEditTrain.IconBadge = "edit";
            this.tsbEditTrain.Name = "tsbEditTrain";
            // 
            // tsbCopyTrain
            // 
            resources.ApplyResources(this.tsbCopyTrain, "tsbCopyTrain");
            this.tsbCopyTrain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCopyTrain.IconName = "train";
            this.tsbCopyTrain.IconBadge = "copy";
            this.tsbCopyTrain.Name = "tsbCopyTrain";
            // 
            // tsbDeleteTrain
            // 
            resources.ApplyResources(this.tsbDeleteTrain, "tsbDeleteTrain");
            this.tsbDeleteTrain.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteTrain.IconName = "train";
            this.tsbDeleteTrain.IconBadge = "delete";
            this.tsbDeleteTrain.Name = "tsbDeleteTrain";
            // 
            // tssTool5
            // 
            resources.ApplyResources(this.tssTool5, "tssTool5");
            this.tssTool5.Name = "tssTool5";
            // 
            // tsbGroupSetup
            // 
            resources.ApplyResources(this.tsbGroupSetup, "tsbGroupSetup");
            this.tsbGroupSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGroupSetup.IconName = "wrench";
            this.tsbGroupSetup.Name = "tsbGroupSetup";
            // 
            // tsbConstReports
            // 
            resources.ApplyResources(this.tsbConstReports, "tsbConstReports");
            this.tsbConstReports.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConstReports.IconName = "speaker";
            this.tsbConstReports.Name = "tsbConstReports";
            // 
            // tssTool4
            // 
            resources.ApplyResources(this.tssTool4, "tssTool4");
            this.tssTool4.Name = "tssTool4";
            // 
            // tsbAppSettings
            // 
            resources.ApplyResources(this.tsbAppSettings, "tsbAppSettings");
            this.tsbAppSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAppSettings.IconName = "settings";
            this.tsbAppSettings.Name = "tsbAppSettings";
            // 
            // tsbInfoApp
            // 
            resources.ApplyResources(this.tsbInfoApp, "tsbInfoApp");
            this.tsbInfoApp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbInfoApp.IconName = "info";
            this.tsbInfoApp.Name = "tsbInfoApp";
            // 
            // titleBar
            // 
            this.titleBar.Controls.Add(this.menuStripMain);
            resources.ApplyResources(this.titleBar, "titleBar");
            this.titleBar.Name = "titleBar";
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiEdit,
            this.tsmiView,
            this.tsmiGroup,
            this.tsmiSettings,
            this.tsmiHelp});
            resources.ApplyResources(this.menuStripMain, "menuStripMain");
            this.menuStripMain.BackColor = System.Drawing.SystemColors.Control;
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStripMain.Name = "menuStripMain";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimOpen,
            this.tsmimRecent,
            this.tssFile1,
            this.tsmimSave,
            this.tsmimCheck,
            this.tsmimVerifyServer,
            this.tsmimReload});
            resources.ApplyResources(this.tsmiFile, "tsmiFile");
            this.tsmiFile.Name = "tsmiFile";
            // 
            // tsmimOpen
            // 
            resources.ApplyResources(this.tsmimOpen, "tsmimOpen");
            this.tsmimOpen.IconName = "folder-open";
            this.tsmimOpen.Name = "tsmimOpen";
            // 
            // tsmimRecent
            // 
            resources.ApplyResources(this.tsmimRecent, "tsmimRecent");
            this.tsmimRecent.IconName = "history";
            this.tsmimRecent.Name = "tsmimRecent";
            // 
            // tssFile1
            // 
            resources.ApplyResources(this.tssFile1, "tssFile1");
            this.tssFile1.Name = "tssFile1";
            // 
            // tsmimSave
            // 
            resources.ApplyResources(this.tsmimSave, "tsmimSave");
            this.tsmimSave.IconName = "save";
            this.tsmimSave.Name = "tsmimSave";
            // 
            // tsmimCheck
            // 
            resources.ApplyResources(this.tsmimCheck, "tsmimCheck");
            this.tsmimCheck.IconName = "document-search";
            this.tsmimCheck.Name = "tsmimCheck";
            // 
            // tsmimVerifyServer
            // 
            resources.ApplyResources(this.tsmimVerifyServer, "tsmimVerifyServer");
            this.tsmimVerifyServer.IconName = "document-checkmark";
            this.tsmimVerifyServer.Name = "tsmimVerifyServer";
            // 
            // tsmimReload
            // 
            resources.ApplyResources(this.tsmimReload, "tsmimReload");
            this.tsmimReload.IconName = "arrow-reset";
            this.tsmimReload.Name = "tsmimReload";
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimUndo,
            this.tsmimRedo,
            this.tssEdit1,
            this.tsmimNewTrain,
            this.tsmimEditTrain,
            this.tsmimCopyTrain,
            this.tsmimDeleteTrain,
            this.tsmimRenameNumber,
            this.tssEdit2,
            this.tsmimShiftValidity});
            resources.ApplyResources(this.tsmiEdit, "tsmiEdit");
            this.tsmiEdit.Name = "tsmiEdit";
            // 
            // tsmimUndo
            // 
            resources.ApplyResources(this.tsmimUndo, "tsmimUndo");
            this.tsmimUndo.IconName = "undo";
            this.tsmimUndo.Name = "tsmimUndo";
            // 
            // tsmimRedo
            // 
            resources.ApplyResources(this.tsmimRedo, "tsmimRedo");
            this.tsmimRedo.IconName = "redo";
            this.tsmimRedo.Name = "tsmimRedo";
            // 
            // tssEdit1
            // 
            resources.ApplyResources(this.tssEdit1, "tssEdit1");
            this.tssEdit1.Name = "tssEdit1";
            // 
            // tsmimNewTrain
            // 
            resources.ApplyResources(this.tsmimNewTrain, "tsmimNewTrain");
            this.tsmimNewTrain.IconName = "train";
            this.tsmimNewTrain.IconBadge = "add";
            this.tsmimNewTrain.Name = "tsmimNewTrain";
            // 
            // tsmimEditTrain
            // 
            resources.ApplyResources(this.tsmimEditTrain, "tsmimEditTrain");
            this.tsmimEditTrain.IconName = "train";
            this.tsmimEditTrain.IconBadge = "edit";
            this.tsmimEditTrain.Name = "tsmimEditTrain";
            // 
            // tsmimCopyTrain
            // 
            resources.ApplyResources(this.tsmimCopyTrain, "tsmimCopyTrain");
            this.tsmimCopyTrain.IconName = "train";
            this.tsmimCopyTrain.IconBadge = "copy";
            this.tsmimCopyTrain.Name = "tsmimCopyTrain";
            // 
            // tsmimDeleteTrain
            // 
            resources.ApplyResources(this.tsmimDeleteTrain, "tsmimDeleteTrain");
            this.tsmimDeleteTrain.IconName = "train";
            this.tsmimDeleteTrain.IconBadge = "delete";
            this.tsmimDeleteTrain.Name = "tsmimDeleteTrain";
            // 
            // tsmimRenameNumber
            // 
            resources.ApplyResources(this.tsmimRenameNumber, "tsmimRenameNumber");
            this.tsmimRenameNumber.IconName = "rename";
            this.tsmimRenameNumber.Name = "tsmimRenameNumber";
            // 
            // tssEdit2
            // 
            resources.ApplyResources(this.tssEdit2, "tssEdit2");
            this.tssEdit2.Name = "tssEdit2";
            // 
            // tsmimShiftValidity
            // 
            resources.ApplyResources(this.tsmimShiftValidity, "tsmimShiftValidity");
            this.tsmimShiftValidity.IconName = "calendar";
            this.tsmimShiftValidity.Name = "tsmimShiftValidity";
            // 
            // tsmiView
            // 
            this.tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimViewInstallation,
            this.tsmimViewAnalysis,
            this.tssView1,
            this.tsmimResetLayout});
            resources.ApplyResources(this.tsmiView, "tsmiView");
            this.tsmiView.Name = "tsmiView";
            // 
            // tsmimViewInstallation
            // 
            resources.ApplyResources(this.tsmimViewInstallation, "tsmimViewInstallation");
            this.tsmimViewInstallation.IconName = "folder";
            this.tsmimViewInstallation.Name = "tsmimViewInstallation";
            // 
            // tsmimViewAnalysis
            // 
            resources.ApplyResources(this.tsmimViewAnalysis, "tsmimViewAnalysis");
            this.tsmimViewAnalysis.IconName = "document-search";
            this.tsmimViewAnalysis.Name = "tsmimViewAnalysis";
            // 
            // tssView1
            // 
            resources.ApplyResources(this.tssView1, "tssView1");
            this.tssView1.Name = "tssView1";
            // 
            // tsmimResetLayout
            // 
            resources.ApplyResources(this.tsmimResetLayout, "tsmimResetLayout");
            this.tsmimResetLayout.Name = "tsmimResetLayout";
            // 
            // tsmiGroup
            // 
            this.tsmiGroup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimGroupSetup,
            this.tsmimConstReports,
            this.tssGroup2,
            this.tsmimStateDiagram,
            this.tsmimTabTab,
            this.tsmimReports,
            this.tsmimLogText,
            this.tsmimBoards,
            this.tsmimGenerateTexts,
            this.tssGroup1,
            this.tsmimNewGroup,
            this.tsmimAddStation});
            resources.ApplyResources(this.tsmiGroup, "tsmiGroup");
            this.tsmiGroup.Name = "tsmiGroup";
            // 
            // tsmimGroupSetup
            // 
            resources.ApplyResources(this.tsmimGroupSetup, "tsmimGroupSetup");
            this.tsmimGroupSetup.IconName = "wrench";
            this.tsmimGroupSetup.Name = "tsmimGroupSetup";
            // 
            // tsmimConstReports
            // 
            resources.ApplyResources(this.tsmimConstReports, "tsmimConstReports");
            this.tsmimConstReports.IconName = "speaker";
            this.tsmimConstReports.Name = "tsmimConstReports";
            // 
            // tssGroup2
            // 
            resources.ApplyResources(this.tssGroup2, "tssGroup2");
            this.tssGroup2.Name = "tssGroup2";
            // 
            // tsmimStateDiagram
            // 
            resources.ApplyResources(this.tsmimStateDiagram, "tsmimStateDiagram");
            this.tsmimStateDiagram.IconName = "flowchart";
            this.tsmimStateDiagram.Name = "tsmimStateDiagram";
            // 
            // tsmimTabTab
            // 
            resources.ApplyResources(this.tsmimTabTab, "tsmimTabTab");
            this.tsmimTabTab.IconName = "table";
            this.tsmimTabTab.Name = "tsmimTabTab";
            // 
            // tsmimReports
            // 
            resources.ApplyResources(this.tsmimReports, "tsmimReports");
            this.tsmimReports.IconName = "speaker-edit";
            this.tsmimReports.Name = "tsmimReports";
            // 
            // tsmimLogText
            // 
            resources.ApplyResources(this.tsmimLogText, "tsmimLogText");
            this.tsmimLogText.IconName = "code";
            this.tsmimLogText.Name = "tsmimLogText";
            // 
            // tsmimBoards
            // 
            resources.ApplyResources(this.tsmimBoards, "tsmimBoards");
            this.tsmimBoards.IconName = "departure";
            this.tsmimBoards.Name = "tsmimBoards";
            // 
            // tsmimGenerateTexts
            // 
            resources.ApplyResources(this.tsmimGenerateTexts, "tsmimGenerateTexts");
            this.tsmimGenerateTexts.IconName = "sparkle";
            this.tsmimGenerateTexts.Name = "tsmimGenerateTexts";
            // 
            // tssGroup1
            // 
            resources.ApplyResources(this.tssGroup1, "tssGroup1");
            this.tssGroup1.Name = "tssGroup1";
            // 
            // tsmimNewGroup
            // 
            resources.ApplyResources(this.tsmimNewGroup, "tsmimNewGroup");
            this.tsmimNewGroup.IconName = "folder";
            this.tsmimNewGroup.IconBadge = "add";
            this.tsmimNewGroup.Name = "tsmimNewGroup";
            // 
            // tsmimAddStation
            // 
            resources.ApplyResources(this.tsmimAddStation, "tsmimAddStation");
            this.tsmimAddStation.IconName = "building";
            this.tsmimAddStation.IconBadge = "add";
            this.tsmimAddStation.Name = "tsmimAddStation";
            // 
            // tsmiSettings
            // 
            this.tsmiSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimAppSettings});
            resources.ApplyResources(this.tsmiSettings, "tsmiSettings");
            this.tsmiSettings.Name = "tsmiSettings";
            // 
            // tsmimAppSettings
            // 
            resources.ApplyResources(this.tsmimAppSettings, "tsmimAppSettings");
            this.tsmimAppSettings.IconName = "settings";
            this.tsmimAppSettings.Name = "tsmimAppSettings";
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimInfoApp,
            this.tsmimCheckUpdates,
            this.tsmimShowLogs,
            this.tsmimReportBug,
            this.tsmimSuggestFeature});
            resources.ApplyResources(this.tsmiHelp, "tsmiHelp");
            this.tsmiHelp.Name = "tsmiHelp";
            // 
            // tsmimInfoApp
            // 
            resources.ApplyResources(this.tsmimInfoApp, "tsmimInfoApp");
            this.tsmimInfoApp.IconName = "info";
            this.tsmimInfoApp.Name = "tsmimInfoApp";
            // 
            // tsmimCheckUpdates
            // 
            resources.ApplyResources(this.tsmimCheckUpdates, "tsmimCheckUpdates");
            this.tsmimCheckUpdates.Name = "tsmimCheckUpdates";
            // 
            // tsmimShowLogs
            // 
            resources.ApplyResources(this.tsmimShowLogs, "tsmimShowLogs");
            this.tsmimShowLogs.Name = "tsmimShowLogs";
            // 
            // tsmimReportBug
            // 
            resources.ApplyResources(this.tsmimReportBug, "tsmimReportBug");
            this.tsmimReportBug.Name = "tsmimReportBug";
            // 
            // tsmimSuggestFeature
            // 
            resources.ApplyResources(this.tsmimSuggestFeature, "tsmimSuggestFeature");
            this.tsmimSuggestFeature.Name = "tsmimSuggestFeature";
            // 
            // cmsTrains
            // 
            this.cmsTrains.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmicNewTrain,
            this.tsmicEditTrain,
            this.tsmicCopyTrain,
            this.tsmicDeleteTrain,
            this.tsmicRenameNumber,
            this.tssTrains1,
            this.tsmicShiftValidity});
            resources.ApplyResources(this.cmsTrains, "cmsTrains");
            this.cmsTrains.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsTrains.Name = "cmsTrains";
            // 
            // tsmicNewTrain
            // 
            resources.ApplyResources(this.tsmicNewTrain, "tsmicNewTrain");
            this.tsmicNewTrain.IconName = "train";
            this.tsmicNewTrain.IconBadge = "add";
            this.tsmicNewTrain.Name = "tsmicNewTrain";
            // 
            // tsmicEditTrain
            // 
            resources.ApplyResources(this.tsmicEditTrain, "tsmicEditTrain");
            this.tsmicEditTrain.IconName = "train";
            this.tsmicEditTrain.IconBadge = "edit";
            this.tsmicEditTrain.Name = "tsmicEditTrain";
            // 
            // tsmicCopyTrain
            // 
            resources.ApplyResources(this.tsmicCopyTrain, "tsmicCopyTrain");
            this.tsmicCopyTrain.IconName = "train";
            this.tsmicCopyTrain.IconBadge = "copy";
            this.tsmicCopyTrain.Name = "tsmicCopyTrain";
            // 
            // tsmicDeleteTrain
            // 
            resources.ApplyResources(this.tsmicDeleteTrain, "tsmicDeleteTrain");
            this.tsmicDeleteTrain.IconName = "train";
            this.tsmicDeleteTrain.IconBadge = "delete";
            this.tsmicDeleteTrain.Name = "tsmicDeleteTrain";
            // 
            // tsmicRenameNumber
            // 
            resources.ApplyResources(this.tsmicRenameNumber, "tsmicRenameNumber");
            this.tsmicRenameNumber.IconName = "rename";
            this.tsmicRenameNumber.Name = "tsmicRenameNumber";
            // 
            // tssTrains1
            // 
            resources.ApplyResources(this.tssTrains1, "tssTrains1");
            this.tssTrains1.Name = "tssTrains1";
            // 
            // tsmicShiftValidity
            // 
            resources.ApplyResources(this.tsmicShiftValidity, "tsmicShiftValidity");
            this.tsmicShiftValidity.IconName = "calendar";
            this.tsmicShiftValidity.Name = "tsmicShiftValidity";
            // 
            // cmsTree
            // 
            this.cmsTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmitGroupSetup,
            this.tsmitStateDiagram,
            this.tsmitTabTab,
            this.tsmitBoards,
            this.tsmitGenerateTexts,
            this.tssTree1,
            this.tsmitAddStation});
            resources.ApplyResources(this.cmsTree, "cmsTree");
            this.cmsTree.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsTree.Name = "cmsTree";
            // 
            // tsmitGroupSetup
            // 
            resources.ApplyResources(this.tsmitGroupSetup, "tsmitGroupSetup");
            this.tsmitGroupSetup.IconName = "wrench";
            this.tsmitGroupSetup.Name = "tsmitGroupSetup";
            // 
            // tsmitStateDiagram
            // 
            resources.ApplyResources(this.tsmitStateDiagram, "tsmitStateDiagram");
            this.tsmitStateDiagram.IconName = "flowchart";
            this.tsmitStateDiagram.Name = "tsmitStateDiagram";
            // 
            // tsmitTabTab
            // 
            resources.ApplyResources(this.tsmitTabTab, "tsmitTabTab");
            this.tsmitTabTab.IconName = "table";
            this.tsmitTabTab.Name = "tsmitTabTab";
            // 
            // tsmitBoards
            // 
            resources.ApplyResources(this.tsmitBoards, "tsmitBoards");
            this.tsmitBoards.IconName = "departure";
            this.tsmitBoards.Name = "tsmitBoards";
            // 
            // tsmitGenerateTexts
            // 
            resources.ApplyResources(this.tsmitGenerateTexts, "tsmitGenerateTexts");
            this.tsmitGenerateTexts.IconName = "sparkle";
            this.tsmitGenerateTexts.Name = "tsmitGenerateTexts";
            // 
            // tssTree1
            // 
            resources.ApplyResources(this.tssTree1, "tssTree1");
            this.tssTree1.Name = "tssTree1";
            // 
            // tsmitAddStation
            // 
            resources.ApplyResources(this.tsmitAddStation, "tsmitAddStation");
            this.tsmitAddStation.IconName = "building";
            this.tsmitAddStation.IconBadge = "add";
            this.tsmitAddStation.Name = "tsmitAddStation";
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripMain;
            this.UseCompositedPainting = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Controls.Add(this.pDocument);
            this.Controls.Add(this.tvData);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.titleBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FMain";
            resources.ApplyResources(this, "$this");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FMain_FormClosing);
            this.Load += new System.EventHandler(this.FMain_Load);
            // 
            // cNumbers
            // 
            resources.ApplyResources(this.cNumbers, "cNumbers");
            this.cNumbers.DataPropertyName = "Numbers";
            this.cNumbers.ReadOnly = true;
            this.cNumbers.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cNumbers.Name = "cNumbers";
            // 
            // cType
            // 
            resources.ApplyResources(this.cType, "cType");
            this.cType.DataPropertyName = "Type";
            this.cType.ReadOnly = true;
            this.cType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cType.Name = "cType";
            // 
            // cName
            // 
            resources.ApplyResources(this.cName, "cName");
            this.cName.DataPropertyName = "Name";
            this.cName.ReadOnly = true;
            this.cName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cName.Name = "cName";
            // 
            // cFrom
            // 
            resources.ApplyResources(this.cFrom, "cFrom");
            this.cFrom.DataPropertyName = "From";
            this.cFrom.ReadOnly = true;
            this.cFrom.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cFrom.Name = "cFrom";
            // 
            // cDeparture
            // 
            resources.ApplyResources(this.cDeparture, "cDeparture");
            this.cDeparture.DataPropertyName = "Departure";
            this.cDeparture.ReadOnly = true;
            this.cDeparture.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cDeparture.Name = "cDeparture";
            // 
            // cTo
            // 
            resources.ApplyResources(this.cTo, "cTo");
            this.cTo.DataPropertyName = "To";
            this.cTo.ReadOnly = true;
            this.cTo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cTo.Name = "cTo";
            // 
            // cArrival
            // 
            resources.ApplyResources(this.cArrival, "cArrival");
            this.cArrival.DataPropertyName = "Arrival";
            this.cArrival.ReadOnly = true;
            this.cArrival.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cArrival.Name = "cArrival";
            // 
            // cValidity
            // 
            resources.ApplyResources(this.cValidity, "cValidity");
            this.cValidity.DataPropertyName = "Validity";
            this.cValidity.ReadOnly = true;
            this.cValidity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cValidity.Name = "cValidity";
            // 
            // cVariants
            // 
            resources.ApplyResources(this.cVariants, "cVariants");
            this.cVariants.DataPropertyName = "Variants";
            this.cVariants.ReadOnly = true;
            this.cVariants.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cVariants.Name = "cVariants";
            // 
            // cOperator
            // 
            resources.ApplyResources(this.cOperator, "cOperator");
            this.cOperator.DataPropertyName = "Operator";
            this.cOperator.ReadOnly = true;
            this.cOperator.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.cOperator.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cOperator.Name = "cOperator";
            // 
            // cVarIndex
            // 
            resources.ApplyResources(this.cVarIndex, "cVarIndex");
            this.cVarIndex.DataPropertyName = "Index";
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
            this.cVarDays.DataPropertyName = "Days";
            this.cVarDays.ReadOnly = true;
            this.cVarDays.Name = "cVarDays";
            // 
            // cVarOperator
            // 
            resources.ApplyResources(this.cVarOperator, "cVarOperator");
            this.cVarOperator.DataPropertyName = "Operator";
            this.cVarOperator.ReadOnly = true;
            this.cVarOperator.Name = "cVarOperator";
            // 
            // cVarFlags
            // 
            resources.ApplyResources(this.cVarFlags, "cVarFlags");
            this.cVarFlags.DataPropertyName = "Flags";
            this.cVarFlags.ReadOnly = true;
            this.cVarFlags.Name = "cVarFlags";
            // 
            // cVarNote
            // 
            resources.ApplyResources(this.cVarNote, "cVarNote");
            this.cVarNote.DataPropertyName = "Note";
            this.cVarNote.ReadOnly = true;
            this.cVarNote.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cVarNote.Name = "cVarNote";
            // 
            // cStopStation
            // 
            resources.ApplyResources(this.cStopStation, "cStopStation");
            this.cStopStation.DataPropertyName = "Station";
            this.cStopStation.ReadOnly = true;
            this.cStopStation.Name = "cStopStation";
            // 
            // cStopCode
            // 
            resources.ApplyResources(this.cStopCode, "cStopCode");
            this.cStopCode.DataPropertyName = "Code";
            this.cStopCode.ReadOnly = true;
            this.cStopCode.Name = "cStopCode";
            // 
            // cStopArrival
            // 
            resources.ApplyResources(this.cStopArrival, "cStopArrival");
            this.cStopArrival.DataPropertyName = "Arrival";
            this.cStopArrival.ReadOnly = true;
            this.cStopArrival.Name = "cStopArrival";
            // 
            // cStopDeparture
            // 
            resources.ApplyResources(this.cStopDeparture, "cStopDeparture");
            this.cStopDeparture.DataPropertyName = "Departure";
            this.cStopDeparture.ReadOnly = true;
            this.cStopDeparture.Name = "cStopDeparture";
            // 
            // cStopFlags
            // 
            resources.ApplyResources(this.cStopFlags, "cStopFlags");
            this.cStopFlags.DataPropertyName = "Flags";
            this.cStopFlags.ReadOnly = true;
            this.cStopFlags.Name = "cStopFlags";
            // 
            // cStopNumber
            // 
            resources.ApplyResources(this.cStopNumber, "cStopNumber");
            this.cStopNumber.DataPropertyName = "Number";
            this.cStopNumber.ReadOnly = true;
            this.cStopNumber.Name = "cStopNumber";
            // 
            // cStopDisplay
            // 
            resources.ApplyResources(this.cStopDisplay, "cStopDisplay");
            this.cStopDisplay.DataPropertyName = "DisplayNumber";
            this.cStopDisplay.ReadOnly = true;
            this.cStopDisplay.Name = "cStopDisplay";
            // 
            // cStopType
            // 
            resources.ApplyResources(this.cStopType, "cStopType");
            this.cStopType.DataPropertyName = "Type";
            this.cStopType.ReadOnly = true;
            this.cStopType.Name = "cStopType";
            // 
            // cStopName
            // 
            resources.ApplyResources(this.cStopName, "cStopName");
            this.cStopName.DataPropertyName = "Name";
            this.cStopName.ReadOnly = true;
            this.cStopName.Name = "cStopName";
            // 
            // cStopLine
            // 
            resources.ApplyResources(this.cStopLine, "cStopLine");
            this.cStopLine.DataPropertyName = "Line";
            this.cStopLine.ReadOnly = true;
            this.cStopLine.Name = "cStopLine";
            // 
            // cStopReplacement
            // 
            resources.ApplyResources(this.cStopReplacement, "cStopReplacement");
            this.cStopReplacement.DataPropertyName = "ReplacementLine";
            this.cStopReplacement.ReadOnly = true;
            this.cStopReplacement.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cStopReplacement.Name = "cStopReplacement";
            // 
            // cDayArrival
            // 
            resources.ApplyResources(this.cDayArrival, "cDayArrival");
            this.cDayArrival.DataPropertyName = "Arrival";
            this.cDayArrival.ReadOnly = true;
            this.cDayArrival.Name = "cDayArrival";
            // 
            // cDayDeparture
            // 
            resources.ApplyResources(this.cDayDeparture, "cDayDeparture");
            this.cDayDeparture.DataPropertyName = "Departure";
            this.cDayDeparture.ReadOnly = true;
            this.cDayDeparture.Name = "cDayDeparture";
            // 
            // cDayArrivalNumber
            // 
            resources.ApplyResources(this.cDayArrivalNumber, "cDayArrivalNumber");
            this.cDayArrivalNumber.DataPropertyName = "ArrivalNumber";
            this.cDayArrivalNumber.ReadOnly = true;
            this.cDayArrivalNumber.Name = "cDayArrivalNumber";
            // 
            // cDayDepartureNumber
            // 
            resources.ApplyResources(this.cDayDepartureNumber, "cDayDepartureNumber");
            this.cDayDepartureNumber.DataPropertyName = "DepartureNumber";
            this.cDayDepartureNumber.ReadOnly = true;
            this.cDayDepartureNumber.Name = "cDayDepartureNumber";
            // 
            // cDayType
            // 
            resources.ApplyResources(this.cDayType, "cDayType");
            this.cDayType.DataPropertyName = "Type";
            this.cDayType.ReadOnly = true;
            this.cDayType.Name = "cDayType";
            // 
            // cDayName
            // 
            resources.ApplyResources(this.cDayName, "cDayName");
            this.cDayName.DataPropertyName = "Name";
            this.cDayName.ReadOnly = true;
            this.cDayName.Name = "cDayName";
            // 
            // cDayFrom
            // 
            resources.ApplyResources(this.cDayFrom, "cDayFrom");
            this.cDayFrom.DataPropertyName = "From";
            this.cDayFrom.ReadOnly = true;
            this.cDayFrom.Name = "cDayFrom";
            // 
            // cDayTo
            // 
            resources.ApplyResources(this.cDayTo, "cDayTo");
            this.cDayTo.DataPropertyName = "To";
            this.cDayTo.ReadOnly = true;
            this.cDayTo.Name = "cDayTo";
            // 
            // cDayTrack
            // 
            resources.ApplyResources(this.cDayTrack, "cDayTrack");
            this.cDayTrack.DataPropertyName = "Track";
            this.cDayTrack.ReadOnly = true;
            this.cDayTrack.Name = "cDayTrack";
            // 
            // cDayVariant
            // 
            resources.ApplyResources(this.cDayVariant, "cDayVariant");
            this.cDayVariant.DataPropertyName = "Variant";
            this.cDayVariant.ReadOnly = true;
            this.cDayVariant.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cDayVariant.Name = "cDayVariant";
            this.cmsTree.ResumeLayout(false);
            this.cmsTree.PerformLayout();
            this.cmsTrains.ResumeLayout(false);
            this.cmsTrains.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.titleBar.ResumeLayout(false);
            this.titleBar.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.pStationTop.ResumeLayout(false);
            this.pStationTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStation)).EndInit();
            this.pStation.ResumeLayout(false);
            this.pStation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStops)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariants)).EndInit();
            this.splitDetail.Panel1.ResumeLayout(false);
            this.splitDetail.Panel1.PerformLayout();
            this.splitDetail.Panel2.ResumeLayout(false);
            this.splitDetail.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitDetail)).EndInit();
            this.splitDetail.ResumeLayout(false);
            this.splitDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTrains)).EndInit();
            this.splitTrains.Panel1.ResumeLayout(false);
            this.splitTrains.Panel1.PerformLayout();
            this.splitTrains.Panel2.ResumeLayout(false);
            this.splitTrains.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTrains)).EndInit();
            this.splitTrains.ResumeLayout(false);
            this.splitTrains.PerformLayout();
            this.pTrains.ResumeLayout(false);
            this.pTrains.PerformLayout();
            this.pDocument.ResumeLayout(false);
            this.pDocument.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pDocument;
        private System.Windows.Forms.Panel pTrains;
        private System.Windows.Forms.SplitContainer splitTrains;
        private System.Windows.Forms.DataGridView dgvTrains;
        private System.Windows.Forms.DataGridViewTextBoxColumn cNumbers;
        private System.Windows.Forms.DataGridViewTextBoxColumn cType;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDeparture;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn cArrival;
        private System.Windows.Forms.DataGridViewTextBoxColumn cValidity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVariants;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOperator;
        private System.Windows.Forms.SplitContainer splitDetail;
        private System.Windows.Forms.DataGridView dgvVariants;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarValidity;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarDays;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarOperator;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarFlags;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVarNote;
        private System.Windows.Forms.Label lVariants;
        private System.Windows.Forms.DataGridView dgvStops;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopStation;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopArrival;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopDeparture;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopFlags;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopDisplay;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopType;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStopReplacement;
        private System.Windows.Forms.Label lStops;
        private System.Windows.Forms.Panel pStation;
        private System.Windows.Forms.DataGridView dgvStation;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayArrival;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayDeparture;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayArrivalNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayDepartureNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayType;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayTrack;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDayVariant;
        private System.Windows.Forms.FlowLayoutPanel pStationTop;
        private System.Windows.Forms.Label lDay;
        private ExControls.ExDateTimePicker dtpDay;
        private System.Windows.Forms.Label lStation;
        private System.Windows.Forms.Label lEmpty;
        private ExControls.ExTreeView tvData;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private ToolsCore.Icons.IconToolStripStatusLabel tsslServer;
        private System.Windows.Forms.ToolStripStatusLabel tsslCount;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private ToolsCore.Icons.IconToolStripButton tsbOpen;
        private ToolsCore.Icons.IconToolStripDropDownButton tssbRecent;
        private ToolsCore.Icons.IconToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripSeparator tssTool1;
        private ToolsCore.Icons.IconToolStripButton tsbReload;
        private System.Windows.Forms.ToolStripSeparator tssTool2;
        private ToolsCore.Icons.IconToolStripSplitButton tssbUndo;
        private ToolsCore.Icons.IconToolStripSplitButton tssbRedo;
        private System.Windows.Forms.ToolStripSeparator tssTool3;
        private ToolsCore.Icons.IconToolStripButton tsbNewTrain;
        private ToolsCore.Icons.IconToolStripButton tsbEditTrain;
        private ToolsCore.Icons.IconToolStripButton tsbCopyTrain;
        private ToolsCore.Icons.IconToolStripButton tsbDeleteTrain;
        private System.Windows.Forms.ToolStripSeparator tssTool5;
        private ToolsCore.Icons.IconToolStripButton tsbGroupSetup;
        private ToolsCore.Icons.IconToolStripButton tsbConstReports;
        private System.Windows.Forms.ToolStripSeparator tssTool4;
        private ToolsCore.Icons.IconToolStripButton tsbAppSettings;
        private ToolsCore.Icons.IconToolStripButton tsbInfoApp;
        private ExControls.ExTitleBar titleBar;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimOpen;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimRecent;
        private System.Windows.Forms.ToolStripSeparator tssFile1;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimSave;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimCheck;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimVerifyServer;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimReload;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimUndo;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimRedo;
        private System.Windows.Forms.ToolStripSeparator tssEdit1;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimNewTrain;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimEditTrain;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimCopyTrain;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimDeleteTrain;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimRenameNumber;
        private System.Windows.Forms.ToolStripSeparator tssEdit2;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimShiftValidity;
        private System.Windows.Forms.ToolStripMenuItem tsmiView;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimViewInstallation;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimViewAnalysis;
        private System.Windows.Forms.ToolStripSeparator tssView1;
        private System.Windows.Forms.ToolStripMenuItem tsmimResetLayout;
        private System.Windows.Forms.ToolStripMenuItem tsmiGroup;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimGroupSetup;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimConstReports;
        private System.Windows.Forms.ToolStripSeparator tssGroup2;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimStateDiagram;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimTabTab;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimReports;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimLogText;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimBoards;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimGenerateTexts;
        private System.Windows.Forms.ToolStripSeparator tssGroup1;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimNewGroup;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimAddStation;
        private System.Windows.Forms.ToolStripMenuItem tsmiSettings;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimAppSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimInfoApp;
        private System.Windows.Forms.ToolStripMenuItem tsmimCheckUpdates;
        private System.Windows.Forms.ToolStripMenuItem tsmimShowLogs;
        private System.Windows.Forms.ToolStripMenuItem tsmimReportBug;
        private System.Windows.Forms.ToolStripMenuItem tsmimSuggestFeature;
        private System.Windows.Forms.ContextMenuStrip cmsTrains;
        private ToolsCore.Icons.IconToolStripMenuItem tsmicNewTrain;
        private ToolsCore.Icons.IconToolStripMenuItem tsmicEditTrain;
        private ToolsCore.Icons.IconToolStripMenuItem tsmicCopyTrain;
        private ToolsCore.Icons.IconToolStripMenuItem tsmicDeleteTrain;
        private ToolsCore.Icons.IconToolStripMenuItem tsmicRenameNumber;
        private System.Windows.Forms.ToolStripSeparator tssTrains1;
        private ToolsCore.Icons.IconToolStripMenuItem tsmicShiftValidity;
        private System.Windows.Forms.ContextMenuStrip cmsTree;
        private ToolsCore.Icons.IconToolStripMenuItem tsmitGroupSetup;
        private ToolsCore.Icons.IconToolStripMenuItem tsmitStateDiagram;
        private ToolsCore.Icons.IconToolStripMenuItem tsmitTabTab;
        private ToolsCore.Icons.IconToolStripMenuItem tsmitBoards;
        private ToolsCore.Icons.IconToolStripMenuItem tsmitGenerateTexts;
        private System.Windows.Forms.ToolStripSeparator tssTree1;
        private ToolsCore.Icons.IconToolStripMenuItem tsmitAddStation;
    }
}
