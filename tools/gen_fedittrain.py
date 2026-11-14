"""Okno Upravit vlak: UI/EditTrain/FEditTrain.Designer.cs, FEditTrain.resx, FEditTrain.cs.resx. Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import column, control, generate, item

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'EditTrain')
ICONS = 'ToolsCore.Icons.'
EH = 'System.EventHandler'
P = 'System.Drawing.Point'
S = 'System.Drawing.Size'


def at(x, y, w, h, index, anchor=None, autosize=False):
    res = {'Location': (f'{x}, {y}', P), 'Size': (f'{w}, {h}', S), 'TabIndex': (str(index), 'int')}
    if anchor:
        res['Anchor'] = (anchor, 'Anchor')
    if autosize:
        res['AutoSize'] = ('True', 'bool')
    return res


def label(name, text, x, y, index):
    return control('System.Windows.Forms.Label', name, text=text, res=at(x, y, 80, 15, index, autosize=True))


def textbox(name, x, y, w, index, anchor='Top, Left, Right', events=()):
    return control('ExControls.ExTextBox', name, res=at(x, y, w, 23, index, anchor), events=events)


def check(name, text, index, events=()):
    return control('ExControls.ExCheckBox', name, text=text, res=at(0, 0, 40, 19, index, autosize=True), events=events)


def tool(name, tip, icon, events):
    return item(ICONS + 'IconToolStripButton', name, code=['DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;', f'IconName = "{icon}";'],
                text=tip, events=[('Click', EH, events)])


def strip(name, items, index):
    return control('System.Windows.Forms.ToolStrip', name, items=items, text=(name, name),
                   code=['GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;', 'ImageScalingSize = new System.Drawing.Size(20, 20);'],
                   res={'Location': ('0, 0', P), 'Size': ('500, 27', S), 'TabIndex': (str(index), 'int')})


def grid(name, columns, events, editable=False):
    code = ['AllowUserToAddRows = false;', 'AllowUserToDeleteRows = false;', 'AllowUserToResizeRows = false;',
            'ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;',
            'MultiSelect = false;', 'RowHeadersVisible = false;']
    if not editable:
        code += ['ReadOnly = true;', 'SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;']
    return control('System.Windows.Forms.DataGridView', name, init=True, columns=columns, events=events, code=code,
                   res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 27', P), 'Size': ('500, 200', S), 'TabIndex': ('0', 'int')})


def split(name, panel1, panel2, horizontal=False, distance=200, index=0):
    res = {'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('1100, 560', S), 'SplitterDistance': (str(distance), 'int'),
           'TabIndex': (str(index), 'int')}
    if horizontal:
        res['Orientation'] = ('Horizontal', 'Orientation')
    return control('System.Windows.Forms.SplitContainer', name, init=True, panel1=panel1, panel2=panel2, res=res)


changed = [('TextChanged', EH, 'Variant_Changed')]

# obdobie platnosti
period = control('System.Windows.Forms.FlowLayoutPanel', 'flpPeriod', children=[
    control('System.Windows.Forms.Label', 'lFrom', text=('Platí od:', 'Platí od:'),
            res={**at(3, 7, 52, 15, 0, autosize=True), 'Margin': ('3, 7, 3, 0', 'Padding')}),
    control('ExControls.ExDateTimePicker', 'dtpFrom', code=['Format = System.Windows.Forms.DateTimePickerFormat.Short;'],
            events=[('ValueChanged', EH, 'Period_Changed')], res=at(61, 3, 110, 23, 1)),
    control('System.Windows.Forms.Label', 'lTo', text=('do:', 'do:'), res={**at(177, 7, 22, 15, 2, autosize=True), 'Margin': ('3, 7, 3, 0', 'Padding')}),
    control('ExControls.ExDateTimePicker', 'dtpTo', code=['Format = System.Windows.Forms.DateTimePickerFormat.Short;'],
            events=[('ValueChanged', EH, 'Period_Changed')], res=at(205, 3, 110, 23, 3)),
    control('ExControls.ExCheckBox', 'cboxNoEnd', text=('bez konca', 'bez konce'),
            events=[('CheckedChanged', EH, 'Period_Changed')], res={**at(321, 5, 80, 19, 4, autosize=True), 'Margin': ('3, 5, 3, 0', 'Padding')}),
], res={'AutoSize': ('True', 'bool'), 'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('1100, 29', S), 'TabIndex': ('1', 'int'),
        'Padding': ('3, 3, 3, 3', 'Padding')})

# varianty
variants = grid('dgvVariants', [
    column('cVarIndex', ('#', '#'), None, 35),
    column('cVarValidity', ('Jazdí', 'Jede'), 'Validity', 240),
    column('cVarDays', ('Dní', 'Dní'), None, 50, fill=True),
], events=[('SelectionChanged', EH, 'dgvVariants_SelectionChanged'),
           ('CellFormatting', 'System.Windows.Forms.DataGridViewCellFormattingEventHandler', 'dgvVariants_CellFormatting')])
variant_tools = strip('tsVariants', [
    tool('tsbVarAdd', ('Pridať variantu', 'Přidat variantu'), 'add', 'tsbVarAdd_Click'),
    tool('tsbVarCopy', ('Kopírovať variantu pred vybranú', 'Kopírovat variantu před vybranou'), 'document-copy', 'tsbVarCopy_Click'),
    tool('tsbVarDelete', ('Zmazať variantu', 'Smazat variantu'), 'delete', 'tsbVarDelete_Click'),
    item('System.Windows.Forms.ToolStripSeparator', 'tssVar1'),
    tool('tsbVarUp', ('Posunúť vyššie', 'Posunout výš'), 'arrow-up', 'tsbVarUp_Click'),
    tool('tsbVarDown', ('Posunúť nižšie', 'Posunout níž'), 'arrow-down', 'tsbVarDown_Click'),
], 1)

# texty a napovedu priznakov nastavi kod (ValueTexts), tu su len pre navrhar
flags = control('System.Windows.Forms.FlowLayoutPanel', 'flpFlags', children=[
    check(f'cboxFlag{key}', (key, key), i, events=[('CheckedChanged', EH, 'Variant_Changed')]) for i, key in enumerate(['D', 'X', 'N', 'NP', 'O', 'M', 'R', 'L'])
], res={**at(90, 63, 534, 50, 6, 'Top, Left, Right'), 'Margin': ('0, 0, 0, 0', 'Padding')})

calendar = control('ToolsCore.Forms.Calendar.CalendarStrip', 'calendarStrip', res=at(3, 153, 560, 60, 9, 'Top, Left, Right'))

variant_panel = control('System.Windows.Forms.Panel', 'pVariant', children=[
    label('lValidity', ('Jazdí:', 'Jede:'), 3, 9, 0),
    textbox('tbValidity', 90, 5, 440, 1, events=changed),
    control('ExControls.ExButton', 'bValidity', text=('Kalendár...', 'Kalendář...'), events=[('Click', EH, 'bValidity_Click')],
            res=at(536, 4, 88, 27, 2, 'Top, Right')),
    label('lOperator', ('Dopravca:', 'Dopravce:'), 3, 38, 3),
    textbox('tbOperator', 90, 34, 534, 4, events=changed),
    label('lFlags', ('Príznaky:', 'Příznaky:'), 3, 67, 5),
    flags,
    label('lNote', ('Poznámka:', 'Poznámka:'), 3, 121, 7),
    textbox('tbNote', 90, 117, 534, 8, events=changed),
    calendar,
], res={'AutoScroll': ('True', 'bool'), 'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('630, 300', S), 'TabIndex': ('0', 'int')})

split_variants = split('splitVariants', [variants, variant_tools], [variant_panel], distance=440)

# trasa
stops = grid('dgvStops', [
    column('cStation', ('Stanica', 'Stanice'), 'Station', 75, readonly=False),
    column('cStationName', ('Názov stanice', 'Název stanice'), 'Station', 170, readonly=False, combo=True),
    column('cArrival', ('Príchod', 'Příjezd'), 'Arrival', 60, readonly=False),
    column('cDeparture', ('Odchod', 'Odjezd'), 'Departure', 60, readonly=False),
    column('cPassing', ('Prejazd', 'Průjezd'), 'Passing', 60, check=True, readonly=False),
    column('cTransfer', ('Prestup', 'Přestup'), 'Transfer', 60, check=True, readonly=False),
    column('cOperating', ('Dopr. bod', 'Dopr. bod'), 'OperatingPoint', 66, check=True, readonly=False),
    column('cRequest', ('Na znamenie', 'Na znamení'), 'OnRequest', 84, check=True, readonly=False),
    column('cNumber', ('Číslo', 'Číslo'), 'Number', 60, readonly=False),
    column('cDisplay', ('Hlási sa', 'Hlásí se'), 'DisplayNumber', 60, readonly=False),
    column('cType', ('Druh', 'Druh'), 'TrainType', 70, readonly=False, combo=True),
    column('cName', ('Názov', 'Název'), 'TrainName', 90, readonly=False),
    column('cLine', ('Linka IDS', 'Linka IDS'), 'Line', 70, readonly=False),
    column('cReplacement', ('Linka NAD', 'Linka NAD'), 'ReplacementLine', 70, fill=True, readonly=False),
], events=[('CellValueChanged', 'System.Windows.Forms.DataGridViewCellEventHandler', 'dgvStops_CellValueChanged'),
           ('CurrentCellDirtyStateChanged', EH, 'dgvStops_CurrentCellDirtyStateChanged')], editable=True)
stop_tools = strip('tsStops', [
    tool('tsbStopInsert', ('Vložiť bod pred vybraný', 'Vložit bod před vybraný'), 'add', 'tsbStopInsert_Click'),
    tool('tsbStopAppend', ('Pridať bod na koniec', 'Přidat bod na konec'), 'arrow-down', 'tsbStopAppend_Click'),
    tool('tsbStopDelete', ('Zmazať bod', 'Smazat bod'), 'delete', 'tsbStopDelete_Click'),
    item('System.Windows.Forms.ToolStripSeparator', 'tssStop1'),
    item('System.Windows.Forms.ToolStripLabel', 'tslStopsHint', text=('Kľúč Z a dni cez polnoc sa dopočítajú z časov a staníc grafikonu.',
                                                                       'Klíč Z a dny přes půlnoc se dopočítají z časů a stanic grafikonu.')),
], 1)

# varianta s tromi riadkami priznakov (FitFlags) a kalendarom sa zmesti bez posuvnika
split_main = split('splitMain', [split_variants], [stops, stop_tools], horizontal=True, distance=260)

problems = control('GVXEditor.UI.Controls.ProblemList', 'lbProblems', code=['FormattingEnabled = true;', 'IntegralHeight = false;'],
                   res={'Dock': ('Bottom', 'Dock'), 'Location': ('0, 590', P), 'Size': ('1100, 80', S), 'TabIndex': ('2', 'int')})

buttons = control('System.Windows.Forms.FlowLayoutPanel', 'flpButtons', children=[
    control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
            res=at(1009, 6, 88, 27, 1)),
    control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), events=[('Click', EH, 'bOk_Click')], res=at(915, 6, 88, 27, 0)),
], code=['FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;'],
    res={'AutoSize': ('True', 'bool'), 'Dock': ('Bottom', 'Dock'), 'Location': ('0, 670', P), 'Size': ('1100, 39', S), 'TabIndex': ('3', 'int'),
         'Padding': ('3, 3, 3, 3', 'Padding')})

# zaznamy suborov stanic a vyluky: tabulka s tlacidlami pridat, zmazat, vyssie, nizsie
RECORD_EVENTS = [
    ('CellValueChanged', 'System.Windows.Forms.DataGridViewCellEventHandler', 'Records_CellValueChanged'),
    ('CurrentCellDirtyStateChanged', EH, 'Records_CurrentCellDirtyStateChanged'),
    ('DataError', 'System.Windows.Forms.DataGridViewDataErrorEventHandler', 'Records_DataError'),
    ('CellFormatting', 'System.Windows.Forms.DataGridViewCellFormattingEventHandler', 'Records_CellFormatting'),
]


def record_tools(prefix, index=1, extra=()):
    return strip(f'ts{prefix}', [
        tool(f'tsb{prefix}Add', ('Pridať záznam', 'Přidat záznam'), 'add', 'tsbRecordAdd_Click'),
        tool(f'tsb{prefix}Delete', ('Zmazať záznam', 'Smazat záznam'), 'delete', 'tsbRecordDelete_Click'),
        item('System.Windows.Forms.ToolStripSeparator', f'tss{prefix}1'),
        tool(f'tsb{prefix}Up', ('Posunúť vyššie', 'Posunout výš'), 'arrow-up', 'tsbRecordUp_Click'),
        tool(f'tsb{prefix}Down', ('Posunúť nižšie', 'Posunout níž'), 'arrow-down', 'tsbRecordDown_Click'),
        *extra,
    ], index)


def validity(prefix, width=180, fill=False):
    return column(f'c{prefix}Validity', ('Jazdí', 'Jede'), 'Validity', width, fill=fill, readonly=False)


def day(prefix):
    return column(f'c{prefix}Day', ('Na deň', 'Na den'), 'EditedForDay', 55, check=True, readonly=False)


def record_grid(prefix, columns):
    return grid(f'dgv{prefix}', columns, RECORD_EVENTS, editable=True)


def record_page(prefix, title, columns, index, extra=()):
    return control('System.Windows.Forms.TabPage', f'tp{prefix}', text=title, code=['UseVisualStyleBackColor = true;'],
                   children=[record_grid(prefix, columns), record_tools(prefix, extra=extra)],
                   res={'Location': ('4, 24', P), 'Size': ('1092, 480', S), 'TabIndex': (str(index), 'int'), 'Padding': ('3, 3, 3, 3', 'Padding')})


station_pages = [
    record_page('Positions', ('Koľaje', 'Koleje'), [
        validity('Pos'),
        column('cPosArrival', ('Príchod', 'Příjezd'), 'Arrival', 90, readonly=False),
        column('cPosDeparture', ('Odchod', 'Odjezd'), 'Departure', 90, readonly=False),
        column('cPosTracks', ('Názvy koľají', 'Názvy kolejí'), None, 200, fill=True),
        day('Pos'),
    ], 0),
    record_page('Additions', ('Dodatky', 'Dodatky'), [
        validity('Add'),
        column('cAddKey', ('Dodatok', 'Dodatek'), 'Key', 340, fill=True, readonly=False, combo=True),
        column('cAddReports', ('Hlásenia', 'Hlášení'), 'Reports', 180, readonly=False),
        day('Add'),
    ], 1, extra=[
        item('System.Windows.Forms.ToolStripSeparator', 'tssAdditions2'),
        # novy dodatok v skripte hlaseni (okno Hlasenia) a jeho kluc do zaznamu
        item(ICONS + 'IconToolStripButton', 'tsbAdditionsNew', text=('Nový dodatok v skripte hlásení...', 'Nový dodatek ve skriptu hlášení...'),
             code=['DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;', 'IconName = "speaker-edit";', 'IconBadge = "add";'],
             events=[('Click', EH, 'tsbAdditionsNew_Click')]),
    ]),
    record_page('Compositions', ('Radenie', 'Řazení'), [
        validity('Comp'),
        column('cCompEnd', ('Cieľová stanica', 'Cílová stanice'), 'EndStation', 170, readonly=False, combo=True),
        column('cCompReports', ('Hlásenia', 'Hlášení'), 'Reports', 160, readonly=False),
        column('cCompSounds', ('Zvuky', 'Zvuky'), 'Sounds', 300, fill=True, readonly=False),
        day('Comp'),
    ], 2),
    record_page('Languages', ('Jazyky', 'Jazyky'), [
        validity('Lang'),
        column('cLangLanguages', ('Jazyky', 'Jazyky'), 'Languages', 200, fill=True, readonly=False),
        day('Lang'),
    ], 3),
    record_page('ReportStations', ('Stanice v hlásení', 'Stanice v hlášení'), [
        validity('Rep'),
        column('cRepShortFrom', ('Krátke zo smeru', 'Krátké ze směru'), 'ShortFrom', 160, readonly=False),
        column('cRepLongFrom', ('Dlhé zo smeru', 'Dlouhé ze směru'), 'LongFrom', 160, readonly=False),
        column('cRepShortTo', ('Krátke do smeru', 'Krátké do směru'), 'ShortTo', 160, readonly=False),
        column('cRepLongTo', ('Dlhé do smeru', 'Dlouhé do směru'), 'LongTo', 160, fill=True, readonly=False),
        day('Rep'),
    ], 4),
    record_page('Texts', ('Texty tabúľ', 'Texty tabulí'), [
        column('cTextSet', ('Sada', 'Sada'), 'Set', 180, readonly=False, combo=True),
        validity('Text'),
        column('cTextText', ('Text', 'Text'), 'Text', 300, fill=True, readonly=False),
        column('cTextFont', ('Písmo', 'Písmo'), 'Font', 60, readonly=False),
        day('Text'),
    ], 5),
    record_page('Mos', ('MOs', 'MOs'), [
        validity('Mos', fill=True),
    ], 6),
]

station_tabs = control('ExControls.ExTabControl', 'tcStation', children=station_pages, code=['SelectedIndex = 0;'],
                       res={'Dock': ('Fill', 'Dock'), 'Location': ('3, 32', P), 'Size': ('1086, 460', S), 'TabIndex': ('1', 'int')})

station_bar = control('System.Windows.Forms.FlowLayoutPanel', 'flpStation', children=[
    control('System.Windows.Forms.Label', 'lStation', text=('Stanica:', 'Stanice:'),
            res={**at(3, 7, 50, 15, 0, autosize=True), 'Margin': ('3, 7, 3, 0', 'Padding')}),
    control('ExControls.ExComboBox', 'cboStation', code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
            events=[('SelectedIndexChanged', EH, 'cboStation_SelectedIndexChanged')], res=at(59, 3, 320, 23, 1)),
    control('System.Windows.Forms.Label', 'lStationInfo', text=('', ''), res={**at(385, 7, 0, 15, 2, autosize=True), 'Margin': ('3, 7, 3, 0', 'Padding')}),
], res={'AutoSize': ('True', 'bool'), 'Dock': ('Top', 'Dock'), 'Location': ('3, 3', P), 'Size': ('1086, 29', S), 'TabIndex': ('0', 'int')})


def caption_label(name, index):
    return control('System.Windows.Forms.Label', name, text=('', ''), res={
        'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('1086, 24', S), 'TabIndex': (str(index), 'int'), 'TextAlign': ('MiddleLeft', 'Align'),
        'Padding': ('3, 0, 0, 0', 'Padding')})


lockouts = record_grid('Lockouts', [
    validity('Lock'),
    column('cLockKey', ('Výluka', 'Výluka'), 'Key', 340, fill=True, readonly=False, combo=True),
    column('cLockFrom', ('Od', 'Od'), 'From', 90, readonly=False),
    column('cLockTo', ('Do', 'Do'), 'To', 90, readonly=False),
    day('Lock'),
])
diversions = record_grid('Diversions', [
    column('cDivStation', ('Stanica', 'Stanice'), 'Station', 200, readonly=False, combo=True),
    validity('Div'),
    column('cDivKey', ('Odklon', 'Odklon'), 'Key', 320, fill=True, readonly=False, combo=True),
    column('cDivFrom', ('Od', 'Od'), 'From', 90, readonly=False),
    column('cDivTo', ('Do', 'Do'), 'To', 90, readonly=False),
    day('Div'),
])
split_notices = split('splitNotices', [lockouts, record_tools('Lockouts'), caption_label('lLockouts', 2)],
                      [diversions, record_tools('Diversions'), caption_label('lDiversions', 2)], horizontal=True, distance=240)

page_train = control('System.Windows.Forms.TabPage', 'tpTrain', text=('Vlak', 'Vlak'), code=['UseVisualStyleBackColor = true;'],
                     children=[split_main, period],
                     res={'Location': ('4, 24', P), 'Size': ('1092, 562', S), 'TabIndex': ('0', 'int')})
page_stations = control('System.Windows.Forms.TabPage', 'tpStations', text=('V staniciach', 'Ve stanicích'), code=['UseVisualStyleBackColor = true;'],
                        children=[station_tabs, station_bar],
                        res={'Location': ('4, 24', P), 'Size': ('1092, 562', S), 'TabIndex': ('1', 'int'), 'Padding': ('3, 3, 3, 3', 'Padding')})
page_notices = control('System.Windows.Forms.TabPage', 'tpNotices', text=('Výluky a odklony', 'Výluky a odklony'), code=['UseVisualStyleBackColor = true;'],
                       children=[split_notices],
                       res={'Location': ('4, 24', P), 'Size': ('1092, 562', S), 'TabIndex': ('2', 'int'), 'Padding': ('3, 3, 3, 3', 'Padding')})

tabs = control('ExControls.ExTabControl', 'tcMain', children=[page_train, page_stations, page_notices], code=['SelectedIndex = 0;'],
               events=[('SelectedIndexChanged', EH, 'tcMain_SelectedIndexChanged')],
               res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('1100, 590', S), 'TabIndex': ('0', 'int')})

form = control('System.Windows.Forms.Form', 'FEditTrain', children=[tabs, problems, buttons],
               events=[('FormClosing', 'System.Windows.Forms.FormClosingEventHandler', 'FEditTrain_FormClosing')])

generate(form, 'GVXEditor.UI.EditTrain', OUT, 'System.Windows.Forms.Form', form_code=[
    'AcceptButton = this.bOk;',
    'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
    'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
    # bez CancelButton: Escape okno nezatvori, rozpisane upravy sa zahodia len tlacidlom Zrusit
    'ClientSize = new System.Drawing.Size(1100, 709);',
    'MinimizeBox = false;',
    'MinimumSize = new System.Drawing.Size(800, 560);',
    'ShowIcon = false;',
    'ShowInTaskbar = false;',
    'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
], form_res={}, form_text=('Upraviť vlak', 'Upravit vlak'))
print('ok')
