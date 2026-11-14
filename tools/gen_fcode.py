"""Okna kodu: UI/Code/FStateDiagram, FTabTabs, FLogText (Designer.cs, .resx, .cs.resx). Navrh pri Segoe UI 9 (7x15).

Okna vznikli tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ich znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import column, control, generate, item

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Code')
ICONS = 'ToolsCore.Icons.'
CODE = 'ToolsCore.CodeEditor.CodeEditorBox'
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
    return control('System.Windows.Forms.Label', name, text=text, res=at(x, y + 4, 80, 15, index, autosize=True))


def textbox(name, x, y, w, index, anchor=None, events=()):
    return control('ExControls.ExTextBox', name, res=at(x, y, w, 23, index, anchor), events=events)


def code(name, x, y, w, h, index, anchor='Top, Left, Right', events=()):
    return control(CODE, name, res=at(x, y, w, h, index, anchor), events=events)


def check(name, text, x, y, index, events=()):
    return control('ExControls.ExCheckBox', name, text=text, res=at(x, y, 80, 19, index, autosize=True), events=events)


def tool(name, tip, icon, handler, badge=None):
    code_ = ['DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;', f'IconName = "{icon}";']
    if badge:
        code_.append(f'IconBadge = "{badge}";')
    return item(ICONS + 'IconToolStripButton', name, code=code_, text=tip, events=[('Click', EH, handler)])


def strip(name, items, index=1):
    return control('System.Windows.Forms.ToolStrip', name, items=items, text=(name, name),
                   code=['GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;', 'ImageScalingSize = new System.Drawing.Size(20, 20);'],
                   res={'Location': ('0, 0', P), 'Size': ('300, 27', S), 'TabIndex': (str(index), 'int')})


def list_tools(prefix):
    return strip(f'ts{prefix}', [
        tool(f'tsb{prefix}Add', ('Pridať', 'Přidat'), 'add', 'tsbAdd_Click'),
        tool(f'tsb{prefix}Delete', ('Zmazať', 'Smazat'), 'delete', 'tsbDelete_Click'),
        item('System.Windows.Forms.ToolStripSeparator', f'tss{prefix}1'),
        tool(f'tsb{prefix}Up', ('Posunúť vyššie', 'Posunout výš'), 'arrow-up', 'tsbUp_Click'),
        tool(f'tsb{prefix}Down', ('Posunúť nižšie', 'Posunout níž'), 'arrow-down', 'tsbDown_Click'),
    ])


def split(name, panel1, panel2, horizontal=False, distance=300, index=0):
    res = {'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('1100, 560', S), 'SplitterDistance': (str(distance), 'int'),
           'TabIndex': (str(index), 'int')}
    if horizontal:
        res['Orientation'] = ('Horizontal', 'Orientation')
    return control('System.Windows.Forms.SplitContainer', name, init=True, panel1=panel1, panel2=panel2, res=res)


def problems(y, w):
    return control('GVXEditor.UI.Controls.ProblemList', 'lbProblems', code=['FormattingEnabled = true;', 'IntegralHeight = false;'],
                   events=[('DoubleClick', EH, 'lbProblems_DoubleClick')],
                   res={'Dock': ('Bottom', 'Dock'), 'Location': (f'0, {y}', P), 'Size': (f'{w}, 90', S), 'TabIndex': ('8', 'int')})


def buttons(y, w):
    return control('System.Windows.Forms.FlowLayoutPanel', 'flpButtons', children=[
        control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
                res=at(w - 91, 6, 88, 27, 1)),
        control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), events=[('Click', EH, 'bOk_Click')], res=at(w - 185, 6, 88, 27, 0)),
        control('System.Windows.Forms.Label', 'lCheck', text=('', ''), res={**at(w - 200, 13, 0, 15, 2, autosize=True), 'Margin': ('3, 9, 9, 0', 'Padding')}),
    ], code=['FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;'],
        res={'AutoSize': ('True', 'bool'), 'Dock': ('Bottom', 'Dock'), 'Location': (f'0, {y}', P), 'Size': (f'{w}, 39', S), 'TabIndex': ('9', 'int'),
             'Padding': ('3, 3, 3, 3', 'Padding')})


def station_bar(name, label_name, combo_name, info_name, text, handler, index):
    return control('System.Windows.Forms.FlowLayoutPanel', name, children=[
        control('System.Windows.Forms.Label', label_name, text=text, res={**at(3, 7, 50, 15, 0, autosize=True), 'Margin': ('3, 7, 3, 0', 'Padding')}),
        control('ExControls.ExComboBox', combo_name, code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
                events=[('SelectedIndexChanged', EH, handler)], res=at(59, 3, 360, 23, 1)),
        control('System.Windows.Forms.Label', info_name, text=('', ''), res={**at(425, 7, 0, 15, 2, autosize=True), 'Margin': ('3, 7, 3, 0', 'Padding')}),
    ], res={'AutoSize': ('True', 'bool'), 'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('1100, 29', S), 'TabIndex': (str(index), 'int'),
            'Padding': ('3, 0, 3, 0', 'Padding')})


def station_row(name, label_name, combo_name, info_name, text, handler, index, x=130):
    """Riadok vyberu stanice s pevnym zaciatkom pola - zarovnany s dalsimi riadkami nad stromom (cislo grafu)."""
    return control('System.Windows.Forms.Panel', name, children=[
        control('System.Windows.Forms.Label', label_name, text=text, res=at(6, 8, 50, 15, 0, autosize=True)),
        control('ExControls.ExComboBox', combo_name, code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
                events=[('SelectedIndexChanged', EH, handler)], res=at(x, 4, 360, 23, 1)),
        control('System.Windows.Forms.Label', info_name, text=('', ''), res=at(x + 370, 8, 0, 15, 2, autosize=True)),
    ], res={'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('1100, 31', S), 'TabIndex': (str(index), 'int')})


def form_code(width, height, min_w, min_h):
    return [
        'AcceptButton = this.bOk;',
        'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
        'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
        # bez CancelButton: Escape okno nezatvori, rozpisane upravy sa zahodia len tlacidlom Zrusit
        f'ClientSize = new System.Drawing.Size({width}, {height});',
        'MinimizeBox = false;',
        f'MinimumSize = new System.Drawing.Size({min_w}, {min_h});',
        'ShowIcon = false;',
        'ShowInTaskbar = false;',
        'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
    ]


# --- stavovy diagram ---
changed = [('TextChanged', EH, 'Node_Changed')]
node_panel = control('System.Windows.Forms.Panel', 'pNode', children=[
    label('lKey', ('Kľúč:', 'Klíč:'), 6, 6, 0),
    textbox('tbKey', 150, 6, 200, 1, events=changed),
    label('lName', ('Názov:', 'Název:'), 370, 6, 2),
    textbox('tbName', 430, 6, 348, 3, 'Top, Left, Right', events=changed),
    label('lReport', ('Hlásenie:', 'Hlášení:'), 6, 35, 4),
    control('ExControls.ExComboBox', 'cboReport', code=['FormattingEnabled = true;'], events=changed, res=at(150, 35, 200, 23, 5)),
    label('lFlags', ('Príznaky:', 'Příznaky:'), 6, 64, 6),
    # priznaky slovami, kluc ukaze napoveda pri mysi
    control('ExControls.ExCheckedListBox', 'clbFlags', code=['CheckOnClick = true;', 'FormattingEnabled = true;', 'IntegralHeight = false;',
                                                             'MultiColumn = true;', 'ColumnWidth = 205;'],
            events=[('ItemCheck', 'System.Windows.Forms.ItemCheckEventHandler', 'Flags_ItemCheck'),
                    ('MouseMove', 'System.Windows.Forms.MouseEventHandler', 'Flags_MouseMove')], res=at(150, 64, 628, 132, 7, 'Top, Left, Right')),
    label('lCondition', ('Podmienka:', 'Podmínka:'), 6, 204, 8),
    code('codeCondition', 150, 204, 628, 64, 9, events=changed),
    check('cboxButton', ('Tlačidlo', 'Tlačítko'), 6, 278, 10, [('CheckedChanged', EH, 'Node_Changed')]),
    label('lButtonText', ('Text:', 'Text:'), 150, 276, 11),
    textbox('tbButtonText', 190, 276, 200, 12, events=changed),
    label('lButtonPosition', ('Poradie:', 'Pořadí:'), 410, 276, 13),
    textbox('tbButtonPosition', 470, 276, 60, 14, events=changed),
    check('cboxAutomation', ('Automatika', 'Automatika'), 6, 310, 15, [('CheckedChanged', EH, 'Node_Changed')]),
    label('lTime', ('Čas spustenia:', 'Čas spuštění:'), 24, 338, 16),
    code('codeTime', 150, 338, 628, 24, 17, events=changed),
    label('lAutoCondition', ('Podmienka:', 'Podmínka:'), 24, 368, 18),
    code('codeAutoCondition', 150, 368, 628, 50, 19, events=changed),
    label('lMode', ('Režim:', 'Režim:'), 24, 424, 20),
    code('codeMode', 150, 424, 200, 24, 21, anchor=None, events=changed),
    label('lVariant', ('Variant hlásenia:', 'Varianta hlášení:'), 370, 424, 22),
    control('ExControls.ExComboBox', 'cboVariant', code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
            events=[('SelectedIndexChanged', EH, 'Node_Changed')], res=at(490, 424, 160, 23, 23)),
    label('lRepeat', ('Opakovanie (s):', 'Opakování (s):'), 24, 454, 24),
    code('codeRepeat', 150, 454, 200, 24, 25, anchor=None, events=changed),
    label('lEnd', ('Koniec opakovania:', 'Konec opakování:'), 24, 484, 26),
    code('codeEnd', 150, 484, 628, 24, 27, events=changed),
    label('lAutoFlags', ('Príznaky:', 'Příznaky:'), 24, 514, 28),
    control('ExControls.ExCheckedListBox', 'clbAutoFlags', code=['CheckOnClick = true;', 'FormattingEnabled = true;', 'IntegralHeight = false;',
                                                                 'MultiColumn = true;', 'ColumnWidth = 205;'],
            events=[('ItemCheck', 'System.Windows.Forms.ItemCheckEventHandler', 'Flags_ItemCheck'),
                    ('MouseMove', 'System.Windows.Forms.MouseEventHandler', 'Flags_MouseMove')], res=at(150, 514, 628, 24, 29, 'Top, Left, Right')),
], res={'AutoScroll': ('True', 'bool'), 'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('790, 560', S), 'TabIndex': ('0', 'int')})

graph_panel = control('System.Windows.Forms.Panel', 'pGraph', children=[
    label('lGraphDescription', ('Popis grafu:', 'Popis grafu:'), 6, 6, 0),
    textbox('tbGraphDescription', 130, 6, 400, 1, events=[('TextChanged', EH, 'Graph_Changed')]),
    control('System.Windows.Forms.Label', 'lGraphHint', text=('Popis sa objaví v chybách prekladu. Vlak postupuje stavmi grafu v poradí zoznamu.',
                                                               'Popis se objeví v chybách překladu. Vlak postupuje stavy grafu v pořadí seznamu.'),
            res=at(6, 40, 80, 15, 2, autosize=True)),
], res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('790, 560', S), 'TabIndex': ('1', 'int')})

graph_tools = strip('tsGraphs', [
    tool('tsbAddGraph', ('Pridať graf', 'Přidat graf'), 'flowchart', 'tsbAddGraph_Click', 'add'),
    tool('tsbAddNode', ('Pridať stav', 'Přidat stav'), 'add', 'tsbAddNode_Click'),
    tool('tsbDeleteItem', ('Zmazať', 'Smazat'), 'delete', 'tsbDeleteItem_Click'),
    item('System.Windows.Forms.ToolStripSeparator', 'tssGraphs1'),
    tool('tsbItemUp', ('Posunúť vyššie', 'Posunout výš'), 'arrow-up', 'tsbItemUp_Click'),
    tool('tsbItemDown', ('Posunúť nižšie', 'Posunout níž'), 'arrow-down', 'tsbItemDown_Click'),
])
tree = control('ExControls.ExTreeView', 'tvGraphs', code=['FullRowSelect = true;', 'HideSelection = false;', 'ShowLines = false;',
                                                         'Style = ExControls.ExTreeViewStyle.Light;'],
               events=[('AfterSelect', 'System.Windows.Forms.TreeViewEventHandler', 'tvGraphs_AfterSelect')],
               res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 27', P), 'Size': ('300, 500', S), 'TabIndex': ('0', 'int')})

index_panel = control('System.Windows.Forms.Panel', 'pIndex', children=[
    code('codeIndex', 130, 4, 960, 24, 1, events=[('TextChanged', EH, 'Index_Changed')]),
    label('lIndex', ('Číslo grafu:', 'Číslo grafu:'), 6, 4, 0),
], res={'Dock': ('Top', 'Dock'), 'Location': ('0, 29', P), 'Size': ('1100, 34', S), 'TabIndex': ('1', 'int')})

state_form = control('System.Windows.Forms.Form', 'FStateDiagram', children=[
    split('splitMain', [tree, graph_tools], [node_panel, graph_panel], distance=300),
    index_panel,
    station_row('pStation', 'lStation', 'cboStation', 'lStationInfo', ('Stanica:', 'Stanice:'), 'cboStation_SelectedIndexChanged', 2),
    problems(611, 1100),
    buttons(701, 1100),
])

# --- tabulky TabTab ---
table_panel = control('System.Windows.Forms.Panel', 'pTable', children=[
    label('lTableName', ('Názov:', 'Název:'), 6, 6, 0),
    textbox('tbTableName', 90, 6, 250, 1, events=[('TextChanged', EH, 'Table_Changed')]),
    label('lTableDescription', ('Popis:', 'Popis:'), 6, 35, 2),
    textbox('tbTableDescription', 90, 35, 660, 3, 'Top, Left, Right', events=[('TextChanged', EH, 'Table_Changed')]),
    check('cboxViewValues', ('Obsluha vidí prekódovaný text', 'Obsluha vidí překódovaný text'), 90, 66, 4, [('CheckedChanged', EH, 'Table_Changed')]),
    check('cboxIgnoreCase', ('Slovník nerozlišuje veľkosť písmen', 'Slovník nerozlišuje velikost písmen'), 340, 66, 5, [('CheckedChanged', EH, 'Table_Changed')]),
    label('lExpression', ('Výraz:', 'Výraz:'), 6, 92, 6),
    code('codeExpression', 6, 116, 744, 220, 7, anchor='Top, Bottom, Left, Right', events=[('TextChanged', EH, 'Table_Changed')]),
], res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('756, 340', S), 'TabIndex': ('0', 'int')})

entries = control('System.Windows.Forms.DataGridView', 'dgvEntries', init=True, columns=[
    column('cKeyText', ('Text kľúča', 'Text klíče'), 'KeyText', 220, readonly=False),
    column('cKeyFont', ('Písmo kľúča', 'Písmo klíče'), 'KeyFont', 90, readonly=False),
    column('cValueText', ('Text hodnoty', 'Text hodnoty'), 'ValueText', 220, readonly=False, fill=True),
    column('cValueFont', ('Písmo hodnoty', 'Písmo hodnoty'), 'ValueFont', 90, readonly=False),
], events=[('CellValueChanged', 'System.Windows.Forms.DataGridViewCellEventHandler', 'Entries_CellValueChanged')],
    code=['AllowUserToAddRows = false;', 'AllowUserToDeleteRows = false;', 'AllowUserToResizeRows = false;',
          'ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;', 'MultiSelect = false;',
          'RowHeadersVisible = false;'],
    res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 51', P), 'Size': ('756, 160', S), 'TabIndex': ('0', 'int')})
entries_caption = control('System.Windows.Forms.Label', 'lEntries', text=(
    'Slovník prekóduje text stĺpca. Písmo prázdne znamená bez písma, * akékoľvek písmo v kľúči a písmo vstupu v hodnote.',
    'Slovník překóduje text sloupce. Písmo prázdné znamená bez písma, * jakékoli písmo v klíči a písmo vstupu v hodnotě.'),
    res={'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('756, 24', S), 'TabIndex': ('2', 'int'), 'TextAlign': ('MiddleLeft', 'Align'),
         'Padding': ('3, 0, 0, 0', 'Padding')})

tables_list = control('ExControls.ExListBox', 'lbTables', code=['FormattingEnabled = true;', 'IntegralHeight = false;'],
                      events=[('SelectedIndexChanged', EH, 'lbTables_SelectedIndexChanged')],
                      res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 27', P), 'Size': ('300, 500', S), 'TabIndex': ('0', 'int')})

tabtab_form = control('System.Windows.Forms.Form', 'FTabTabs', children=[
    split('splitMain', [tables_list, list_tools('Tables')],
          [split('splitTable', [table_panel], [entries, list_tools('Entries'), entries_caption], horizontal=True, distance=340)], distance=300),
    station_bar('flpFile', 'lFile', 'cboFile', 'lFileInfo', ('Súbor:', 'Soubor:'), 'cboFile_SelectedIndexChanged', 1),
    problems(611, 1100),
    buttons(701, 1100),
])

# --- skript hlaseni ---
text_tools = strip('tsText', [
    tool('tsbFind', ('Hľadať (Ctrl+F)', 'Hledat (Ctrl+F)'), 'search', 'tsbFind_Click'),
    tool('tsbReplace', ('Nahradiť (Ctrl+H)', 'Nahradit (Ctrl+H)'), 'search', 'tsbReplace_Click', 'edit'),
])
logtext_form = control('System.Windows.Forms.Form', 'FLogText', children=[
    code('codeText', 0, 27, 1000, 544, 0, anchor=None),
    text_tools,
    problems(571, 1000),
    buttons(661, 1000),
])
logtext_form.children[0].res['Dock'] = ('Fill', 'Dock')
logtext_form.children[0].events = [('TextChanged', EH, 'Text_Changed')]

if __name__ == '__main__':
    os.makedirs(OUT, exist_ok=True)
    generate(state_form, 'GVXEditor.UI.Code', OUT, 'System.Windows.Forms.Form', form_code=form_code(1100, 760, 820, 560), form_res={},
             form_text=('Stavový diagram', 'Stavový diagram'))
    generate(tabtab_form, 'GVXEditor.UI.Code', OUT, 'System.Windows.Forms.Form', form_code=form_code(1100, 740, 820, 560), form_res={},
             form_text=('Tabuľky TabTab', 'Tabulky TabTab'))
    generate(logtext_form, 'GVXEditor.UI.Code', OUT, 'System.Windows.Forms.Form', form_code=form_code(1000, 700, 640, 420), form_res={},
             form_text=('Skript hlásení', 'Skript hlášení'))
    print('ok')
