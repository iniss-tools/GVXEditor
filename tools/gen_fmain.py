"""Hlavne okno GVXEditora: UI/Main/FMain.Designer.cs, FMain.resx, FMain.cs.resx. Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, zmeny patria sem a okno sa vygeneruje znova (pred tym
porovnat vystup so suborom). Vzor pre nove okna (formgen.py: strom prvkov -> Designer.cs, .resx a .cs.resx).
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import Node, column, control, generate, item

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Main')
ICON = os.path.join(ROOT, 'gvx.ico')
ICONS = 'ToolsCore.Icons.'
EH = 'System.EventHandler'


def menu(name, text, icon=None, children=(), badge=None):
    type_ = ICONS + 'IconToolStripMenuItem' if icon else 'System.Windows.Forms.ToolStripMenuItem'
    code = [f'IconName = "{icon}";'] if icon else []
    if badge:
        code.append(f'IconBadge = "{badge}";')
    return item(type_, name, code=code, text=text, items=children)


def button(name, tip, icon, type_='IconToolStripButton', events=(), badge=None):
    code = ['DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;', f'IconName = "{icon}";']
    if badge:
        code.append(f'IconBadge = "{badge}";')
    return item(ICONS + type_, name, code=code, text=tip, events=events)


def sep(name):
    return item('System.Windows.Forms.ToolStripSeparator', name)


def grid(name, columns, events=(), code=(), multi=False):
    return control('System.Windows.Forms.DataGridView', name, init=True, columns=columns, events=events, code=[*code,
        'AllowUserToAddRows = false;',
        'AllowUserToDeleteRows = false;',
        'AllowUserToResizeRows = false;',
        'ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;',
        f'MultiSelect = {"true" if multi else "false"};',
        'ReadOnly = true;',
        'RowHeadersVisible = false;',
        'SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;',
    ], res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', 'System.Drawing.Point'), 'Size': ('400, 200', 'System.Drawing.Size'),
            'TabIndex': ('0', 'int')})


def caption(name, text, index=1):
    return control('System.Windows.Forms.Label', name, text=text, res={
        'Dock': ('Top', 'Dock'), 'Location': ('0, 0', 'System.Drawing.Point'), 'Size': ('400, 24', 'System.Drawing.Size'),
        'TabIndex': (str(index), 'int'), 'TextAlign': ('MiddleLeft', 'Align'), 'Padding': ('3, 0, 0, 0', 'Padding')})


def split(name, panel1, panel2, horizontal=False, distance=200, fixed=None, index=0):
    res = {'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', 'System.Drawing.Point'), 'Size': ('900, 600', 'System.Drawing.Size'),
           'SplitterDistance': (str(distance), 'int'), 'TabIndex': (str(index), 'int')}
    if horizontal:
        res['Orientation'] = ('Horizontal', 'Orientation')
    if fixed:
        res['FixedPanel'] = (fixed, 'FixedPanel')
    return control('System.Windows.Forms.SplitContainer', name, init=True, panel1=panel1, panel2=panel2, res=res)


# ponuka a panel nastrojov
file_menu = menu('tsmiFile', ('&Súbor', '&Soubor'), children=[
    menu('tsmimOpen', ('&Otvoriť inštaláciu...', '&Otevřít instalaci...'), 'folder-open'),
    menu('tsmimRecent', ('&Nedávne', '&Nedávné'), 'history'),
    sep('tssFile1'),
    menu('tsmimSave', ('&Uložiť', '&Uložit'), 'save'),
    menu('tsmimCheck', ('&Skontrolovať inštaláciu', '&Zkontrolovat instalaci'), 'document-search'),
    menu('tsmimVerifyServer', ('Overiť &kódom servera', 'Ověřit &kódem serveru'), 'document-checkmark'),
    menu('tsmimReload', ('O&bnoviť', 'O&bnovit'), 'arrow-reset'),
])


def train_items(prefix, separator):
    return [
        menu(prefix + 'NewTrain', ('&Nový vlak...', '&Nový vlak...'), 'train', badge='add'),
        menu(prefix + 'EditTrain', ('&Upraviť vlak...', '&Upravit vlak...'), 'train', badge='edit'),
        menu(prefix + 'CopyTrain', ('&Kopírovať vlak', '&Kopírovat vlak'), 'train', badge='copy'),
        menu(prefix + 'DeleteTrain', ('&Zmazať vlak', '&Smazat vlak'), 'train', badge='delete'),
        menu(prefix + 'RenameNumber', ('&Premenovať číslo vlaku...', '&Přejmenovat číslo vlaku...'), 'rename'),
        sep(separator),
        menu(prefix + 'ShiftValidity', ('&Posunúť platnosť grafikonu...', '&Posunout platnost grafikonu...'), 'calendar'),
    ]


edit_menu = menu('tsmiEdit', ('Úp&ravy', 'Úp&ravy'), children=[
    menu('tsmimUndo', ('&Späť', '&Zpět'), 'undo'),
    menu('tsmimRedo', ('&Znova', '&Znovu'), 'redo'),
    sep('tssEdit1'),
    *train_items('tsmim', 'tssEdit2'),
])
group_menu = menu('tsmiGroup', ('&Grafikon', '&Grafikon'), children=[
    menu('tsmimGroupSetup', ('&Nastavenie grafikonu...', '&Nastavení grafikonu...'), 'wrench'),
    menu('tsmimConstReports', ('&Hlásenia obsluhy...', '&Hlášení obsluhy...'), 'speaker'),
    sep('tssGroup2'),
    menu('tsmimStateDiagram', ('&Stavový diagram...', '&Stavový diagram...'), 'flowchart'),
    menu('tsmimTabTab', ('&Tabuľky TabTab...', '&Tabulky TabTab...'), 'table'),
    menu('tsmimReports', ('H&lásenia...', 'H&lášení...'), 'speaker-edit'),
    menu('tsmimLogText', ('S&kript hlásení...', 'S&kript hlášení...'), 'code'),
    menu('tsmimBoards', ('Ta&bule...', 'Ta&bule...'), 'departure'),
    menu('tsmimGenerateTexts', ('&Generovať texty tabúľ...', '&Generovat texty tabulí...'), 'sparkle'),
    sep('tssGroup1'),
    menu('tsmimNewGroup', ('Nový &grafikon...', 'Nový &grafikon...'), 'folder', badge='add'),
    menu('tsmimAddStation', ('&Pridať stanicu...', '&Přidat stanici...'), 'building', badge='add'),
])
# panely okna (ExDockManager vytvara FMain.Docking.cs)
view_menu = menu('tsmiView', ('&Zobraziť', '&Zobrazit'), children=[
    menu('tsmimViewInstallation', ('&Inštalácia', '&Instalace'), 'folder'),
    menu('tsmimViewAnalysis', ('&Kontrola inštalácie', '&Kontrola instalace'), 'document-search'),
    sep('tssView1'),
    menu('tsmimResetLayout', ('&Obnoviť rozloženie okna', '&Obnovit rozložení okna')),
])
settings_menu = menu('tsmiSettings', ('&Nastavenia', '&Nastavení'), children=[
    menu('tsmimAppSettings', ('Nastavenia &programu...', 'Nastavení &programu...'), 'settings'),
])
help_menu = menu('tsmiHelp', ('&Pomocník', '&Nápověda'), children=[
    menu('tsmimInfoApp', ('&O programe', '&O programu'), 'info'),
    menu('tsmimCheckUpdates', ('&Skontrolovať aktualizácie...', '&Zkontrolovat aktualizace...')),
    menu('tsmimShowLogs', ('Otvoriť priečinok s logmi', 'Otevřít složku s logy')),
])
menu_strip = control('System.Windows.Forms.MenuStrip', 'menuStripMain', items=[file_menu, edit_menu, view_menu, group_menu, settings_menu, help_menu],
                     code=['BackColor = System.Drawing.SystemColors.Control;', 'ImageScalingSize = new System.Drawing.Size(20, 20);',
                           'Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);'],
                     text=('menuStrip1', 'menuStrip1'),
                     res={'Dock': ('Left', 'Dock'), 'Location': ('36, 0', 'System.Drawing.Point'), 'Size': ('320, 32', 'System.Drawing.Size'),
                          'TabIndex': ('0', 'int')})
title_bar = control('ExControls.ExTitleBar', 'titleBar', children=[menu_strip],
                    res={'Location': ('0, 0', 'System.Drawing.Point'), 'Size': ('1200, 32', 'System.Drawing.Size'), 'TabIndex': ('3', 'int')})

tool_strip = control('System.Windows.Forms.ToolStrip', 'toolStripMain', items=[
    button('tsbOpen', ('Otvoriť inštaláciu', 'Otevřít instalaci'), 'folder-open'),
    button('tssbRecent', ('Nedávne', 'Nedávné'), 'history', 'IconToolStripDropDownButton'),
    button('tsbSave', ('Uložiť', 'Uložit'), 'save'),
    sep('tssTool1'),
    button('tsbReload', ('Obnoviť', 'Obnovit'), 'arrow-reset'),
    sep('tssTool2'),
    button('tssbUndo', ('Späť', 'Zpět'), 'undo', 'IconToolStripSplitButton',
           events=[('DropDownOpening', EH, 'tssbUndo_DropDownOpening')]),
    button('tssbRedo', ('Znova', 'Znovu'), 'redo', 'IconToolStripSplitButton',
           events=[('DropDownOpening', EH, 'tssbRedo_DropDownOpening')]),
    sep('tssTool3'),
    button('tsbNewTrain', ('Nový vlak', 'Nový vlak'), 'train', badge='add'),
    button('tsbEditTrain', ('Upraviť vlak', 'Upravit vlak'), 'train', badge='edit'),
    button('tsbCopyTrain', ('Kopírovať vlak', 'Kopírovat vlak'), 'train', badge='copy'),
    button('tsbDeleteTrain', ('Zmazať vlak', 'Smazat vlak'), 'train', badge='delete'),
    sep('tssTool5'),
    button('tsbGroupSetup', ('Nastavenie grafikonu', 'Nastavení grafikonu'), 'wrench'),
    button('tsbConstReports', ('Hlásenia obsluhy', 'Hlášení obsluhy'), 'speaker'),
    sep('tssTool4'),
    button('tsbAppSettings', ('Nastavenia programu', 'Nastavení programu'), 'settings'),
    button('tsbInfoApp', ('Informácie o programe', 'Informace o programu'), 'info'),
], code=['GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;', 'ImageScalingSize = new System.Drawing.Size(20, 20);',
         'Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);'],
    text=('toolStrip1', 'toolStrip1'),
    res={'Location': ('0, 32', 'System.Drawing.Point'), 'Size': ('1200, 27', 'System.Drawing.Size'), 'TabIndex': ('2', 'int')})

status_strip = control('System.Windows.Forms.StatusStrip', 'statusStripMain', items=[
    item(ICONS + 'IconToolStripStatusLabel', 'tsslServer', text=('', '')),
    item('System.Windows.Forms.ToolStripStatusLabel', 'tsslCount', code=['Spring = true;',
                                                                         'TextAlign = System.Drawing.ContentAlignment.MiddleRight;'], text=('', '')),
], code=['ImageScalingSize = new System.Drawing.Size(20, 20);'], text=('statusStrip1', 'statusStrip1'),
    res={'Location': ('0, 678', 'System.Drawing.Point'), 'Size': ('1200, 22', 'System.Drawing.Size'), 'TabIndex': ('1', 'int')})

# strom grafikonov a stanic
tree = control('ExControls.ExTreeView', 'tvData', code=['ContextMenuStrip = this.cmsTree;', 'FullRowSelect = true;', 'HideSelection = false;',
                                                       'ItemHeight = 24;', 'ShowLines = false;', 'Style = ExControls.ExTreeViewStyle.Light;'],
               events=[('AfterSelect', 'System.Windows.Forms.TreeViewEventHandler', 'tvData_AfterSelect'),
                       ('NodeMouseClick', 'System.Windows.Forms.TreeNodeMouseClickEventHandler', 'tvData_NodeMouseClick')],
               res={'Dock': ('Left', 'Dock'), 'Location': ('0, 59', 'System.Drawing.Point'), 'Size': ('260, 619', 'System.Drawing.Size'),
                    'TabIndex': ('4', 'int')})

# vlaky grafikonu
trains = grid('dgvTrains', [
    column('cNumbers', ('Číslo', 'Číslo'), 'Numbers', 110, sort=True),
    column('cType', ('Druh', 'Druh'), 'Type', 50, sort=True),
    column('cName', ('Názov', 'Název'), 'Name', 120, sort=True),
    column('cFrom', ('Z', 'Z'), 'From', 160, sort=True),
    column('cDeparture', ('Odchod', 'Odjezd'), 'Departure', 70, sort=True),
    column('cTo', ('Do', 'Do'), 'To', 160, sort=True),
    column('cArrival', ('Príchod', 'Příjezd'), 'Arrival', 70, sort=True),
    column('cValidity', ('Platnosť', 'Platnost'), 'Validity', 160, sort=True),
    column('cVariants', ('Varianty', 'Varianty'), 'Variants', 65, sort=True),
    column('cOperator', ('Dopravca', 'Dopravce'), 'Operator', 120, fill=True, sort=True),
], events=[('SelectionChanged', EH, 'dgvTrains_SelectionChanged'),
           ('CurrentCellChanged', EH, 'dgvTrains_CurrentCellChanged'),
           ('CellDoubleClick', 'System.Windows.Forms.DataGridViewCellEventHandler', 'dgvTrains_CellDoubleClick'),
           ('CellMouseDown', 'System.Windows.Forms.DataGridViewCellMouseEventHandler', 'dgvTrains_CellMouseDown'),
           ('CellFormatting', 'System.Windows.Forms.DataGridViewCellFormattingEventHandler', 'dgvTrains_CellFormatting'),
           ('ColumnHeaderMouseClick', 'System.Windows.Forms.DataGridViewCellMouseEventHandler', 'dgvTrains_ColumnHeaderMouseClick')],
   code=['ContextMenuStrip = this.cmsTrains;'], multi=True)

# kontextova ponuka vlakov
context_trains = Node('component', 'System.Windows.Forms.ContextMenuStrip', 'cmsTrains', items=train_items('tsmic', 'tssTrains1'),
                      code=['ImageScalingSize = new System.Drawing.Size(20, 20);'],
                      res={'Size': ('260, 140', 'System.Drawing.Size')})
# kontextova ponuka stromu
context_tree = Node('component', 'System.Windows.Forms.ContextMenuStrip', 'cmsTree', items=[
    menu('tsmitGroupSetup', ('&Nastavenie grafikonu...', '&Nastavení grafikonu...'), 'wrench'),
    menu('tsmitStateDiagram', ('&Stavový diagram...', '&Stavový diagram...'), 'flowchart'),
    menu('tsmitTabTab', ('&Tabuľky TabTab...', '&Tabulky TabTab...'), 'table'),
    menu('tsmitBoards', ('Ta&bule...', 'Ta&bule...'), 'departure'),
    menu('tsmitGenerateTexts', ('&Generovať texty tabúľ...', '&Generovat texty tabulí...'), 'sparkle'),
    sep('tssTree1'),
    menu('tsmitAddStation', ('&Pridať stanicu...', '&Přidat stanici...'), 'building', badge='add'),
], code=['ImageScalingSize = new System.Drawing.Size(20, 20);'], res={'Size': ('220, 136', 'System.Drawing.Size')})
variants = grid('dgvVariants', [
    column('cVarIndex', ('#', '#'), 'Index', 35),
    column('cVarValidity', ('Jazdí', 'Jede'), 'Validity', 180),
    column('cVarDays', ('Dní', 'Dní'), 'Days', 50),
    column('cVarOperator', ('Dopravca', 'Dopravce'), 'Operator', 110),
    column('cVarFlags', ('Príznaky', 'Příznaky'), 'Flags', 150),
    column('cVarNote', ('Poznámka', 'Poznámka'), 'Note', 100, fill=True),
], events=[('SelectionChanged', EH, 'dgvVariants_SelectionChanged')])
stops = grid('dgvStops', [
    column('cStopStation', ('Stanica', 'Stanice'), 'Station', 170),
    column('cStopCode', ('SR70', 'SR70'), 'Code', 70),
    column('cStopArrival', ('Príchod', 'Příjezd'), 'Arrival', 70),
    column('cStopDeparture', ('Odchod', 'Odjezd'), 'Departure', 70),
    column('cStopFlags', ('Príznaky', 'Příznaky'), 'Flags', 130),
    column('cStopNumber', ('Číslo', 'Číslo'), 'Number', 70),
    column('cStopDisplay', ('Hlási sa', 'Hlásí se'), 'DisplayNumber', 70),
    column('cStopType', ('Druh', 'Druh'), 'Type', 50),
    column('cStopName', ('Názov', 'Název'), 'Name', 100),
    column('cStopLine', ('Linka IDS', 'Linka IDS'), 'Line', 70),
    column('cStopReplacement', ('Linka NAD', 'Linka NAD'), 'ReplacementLine', 70, fill=True),
])
split_detail = split('splitDetail', [variants, caption('lVariants', ('Varianty vlaku', 'Varianty vlaku'))],
                     [stops, caption('lStops', ('Trasa varianty', 'Trasa varianty'))], distance=480)
split_trains = split('splitTrains', [trains], [split_detail], horizontal=True, distance=330)
trains_panel = control('System.Windows.Forms.Panel', 'pTrains', children=[split_trains],
                       res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', 'System.Drawing.Point'), 'Size': ('936, 619', 'System.Drawing.Size'),
                            'TabIndex': ('0', 'int')})

# stanica na den
station = grid('dgvStation', [
    column('cDayArrival', ('Príchod', 'Příjezd'), 'Arrival', 70),
    column('cDayDeparture', ('Odchod', 'Odjezd'), 'Departure', 70),
    column('cDayArrivalNumber', ('Číslo pri príchode', 'Číslo při příjezdu'), 'ArrivalNumber', 90),
    column('cDayDepartureNumber', ('Číslo pri odchode', 'Číslo při odjezdu'), 'DepartureNumber', 90),
    column('cDayType', ('Druh', 'Druh'), 'Type', 50),
    column('cDayName', ('Názov', 'Název'), 'Name', 110),
    column('cDayFrom', ('Z', 'Z'), 'From', 160),
    column('cDayTo', ('Do', 'Do'), 'To', 160),
    column('cDayTrack', ('Koľaj', 'Kolej'), 'Track', 70),
    column('cDayVariant', ('Varianta', 'Varianta'), 'Variant', 65, fill=True),
])
day_label = control('System.Windows.Forms.Label', 'lDay', text=('Deň:', 'Den:'), res={
    'AutoSize': ('True', 'bool'), 'Location': ('3, 7', 'System.Drawing.Point'), 'Margin': ('3, 7, 3, 0', 'Padding'),
    'Size': ('29, 15', 'System.Drawing.Size'), 'TabIndex': ('0', 'int')})
day_picker = control('ExControls.ExDateTimePicker', 'dtpDay', code=['Format = System.Windows.Forms.DateTimePickerFormat.Short;'],
                     events=[('ValueChanged', EH, 'dtpDay_ValueChanged')],
                     res={'Location': ('38, 3', 'System.Drawing.Point'), 'Size': ('120, 23', 'System.Drawing.Size'), 'TabIndex': ('1', 'int')})
station_label = control('System.Windows.Forms.Label', 'lStation', text=('', ''), res={
    'AutoSize': ('True', 'bool'), 'Location': ('170, 7', 'System.Drawing.Point'), 'Margin': ('9, 7, 3, 0', 'Padding'),
    'Size': ('0, 15', 'System.Drawing.Size'), 'TabIndex': ('2', 'int')})
station_top = control('System.Windows.Forms.FlowLayoutPanel', 'pStationTop', children=[day_label, day_picker, station_label],
                      res={'AutoSize': ('True', 'bool'), 'Dock': ('Top', 'Dock'), 'Location': ('0, 0', 'System.Drawing.Point'),
                           'Size': ('936, 29', 'System.Drawing.Size'), 'TabIndex': ('1', 'int')})
station_panel = control('System.Windows.Forms.Panel', 'pStation', children=[station, station_top],
                        res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', 'System.Drawing.Point'), 'Size': ('936, 619', 'System.Drawing.Size'),
                             'TabIndex': ('1', 'int'), 'Visible': ('False', 'bool')})

empty = control('System.Windows.Forms.Label', 'lEmpty', text=('Otvorte inštaláciu INISS2 cez Súbor, Otvoriť inštaláciu.',
                                                             'Otevřete instalaci INISS2 přes Soubor, Otevřít instalaci.'),
                res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', 'System.Drawing.Point'), 'Size': ('936, 619', 'System.Drawing.Size'),
                     'TabIndex': ('2', 'int'), 'TextAlign': ('MiddleCenter', 'Align')})

# dokument okna: za behu ho FMain.Docking.cs presunie do ExDockManager.Document a strom do panela Instalacia
document = control('System.Windows.Forms.Panel', 'pDocument', children=[trains_panel, station_panel, empty],
                   res={'Dock': ('Fill', 'Dock'), 'Location': ('260, 59', 'System.Drawing.Point'), 'Size': ('940, 619', 'System.Drawing.Size'),
                        'TabIndex': ('0', 'int')})

form = control('ExControls.ExForm', 'FMain', children=[document, tree, status_strip, tool_strip, title_bar], events=[
    ('FormClosing', 'System.Windows.Forms.FormClosingEventHandler', 'FMain_FormClosing'),
    ('Load', EH, 'FMain_Load'),
])

generate(form, 'GVXEditor.UI.Main', OUT, 'ExControls.ExForm', form_code=[
    'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
    'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
    'ClientSize = new System.Drawing.Size(1200, 700);',
    'KeyPreview = true;',
    'MainMenuStrip = this.menuStripMain;',
    'UseCompositedPainting = false;',
    'WindowState = System.Windows.Forms.FormWindowState.Maximized;',
], form_res={}, form_text=('GVXEditor', 'GVXEditor'), icon=ICON, components=[context_trains, context_tree])
print('ok')
