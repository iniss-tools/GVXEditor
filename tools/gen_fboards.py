"""Okno Tabule: UI/Boards/FBoards (Designer.cs, .resx, .cs.resx). Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import column, control, generate, item
from gen_fcode import EH, P, S, at, buttons, check, code, form_code, label, problems, split, station_bar, strip, textbox, tool

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Boards')
COMBO_CODE = ['FormattingEnabled = true;']
LIST_CODE = ['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;']


def combo(name, x, y, w, index, events, editable=True, anchor=None):
    return control('ExControls.ExComboBox', name, code=COMBO_CODE if editable else LIST_CODE, events=events, res=at(x, y, w, 23, index, anchor))


def hint(name, text, x, y, index):
    return control('System.Windows.Forms.Label', name, text=text, res=at(x, y + 4, 80, 15, index, autosize=True))


def grid(name, columns, events, y, h, index):
    return control('System.Windows.Forms.DataGridView', name, init=True, columns=columns, events=events,
                   code=['AllowUserToAddRows = false;', 'AllowUserToDeleteRows = false;', 'AllowUserToResizeRows = false;',
                         'ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;', 'MultiSelect = false;',
                         'RowHeadersVisible = false;'],
                   res={'Dock': ('Fill', 'Dock'), 'Location': (f'0, {y}', P), 'Size': (f'600, {h}', S), 'TabIndex': (str(index), 'int')})


def list_tools(prefix, index=1):
    return strip(f'ts{prefix}', [
        tool(f'tsb{prefix}Add', ('Pridať', 'Přidat'), 'add', 'tsbAdd_Click'),
        tool(f'tsb{prefix}Delete', ('Zmazať', 'Smazat'), 'delete', 'tsbDelete_Click'),
        item('System.Windows.Forms.ToolStripSeparator', f'tss{prefix}1'),
        tool(f'tsb{prefix}Up', ('Posunúť vyššie', 'Posunout výš'), 'arrow-up', 'tsbUp_Click'),
        tool(f'tsb{prefix}Down', ('Posunúť nižšie', 'Posunout níž'), 'arrow-down', 'tsbDown_Click'),
    ], index)


def caption(name, text, index):
    return control('System.Windows.Forms.Label', name, text=text, res={
        'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('600, 24', S), 'TabIndex': (str(index), 'int'), 'TextAlign': ('MiddleLeft', 'Align'),
        'Padding': ('3, 0, 0, 0', 'Padding')})


def page(name, text, children, index):
    return control('System.Windows.Forms.TabPage', name, text=text, code=['UseVisualStyleBackColor = true;'], children=children,
                   res={'Location': ('4, 24', P), 'Size': ('1172, 560', S), 'TabIndex': (str(index), 'int'), 'Padding': ('3, 3, 3, 3', 'Padding')})


# --- tabula ---
board = [('TextChanged', EH, 'Board_Changed')]
X1, X2, W = 190, 520, 230
table_panel = control('System.Windows.Forms.Panel', 'pTable', children=[
    label('lKey', ('Kľúč:', 'Klíč:'), 6, 6, 0),
    textbox('tbKey', X1, 6, W, 1, events=board),
    label('lName', ('Názov:', 'Název:'), 6, 35, 2),
    textbox('tbName', X1, 35, 520, 3, events=board),
    label('lLine', ('Linka:', 'Linka:'), 6, 64, 4),
    combo('cboLine', X1, 64, W, 5, board),
    hint('lLineInfo', ('', ''), X1 + W + 10, 64, 6),
    label('lSubLine', ('Podriadená linka:', 'Podřízená linka:'), 6, 93, 7),
    textbox('tbSubLine', X1, 93, W, 8, events=board),
    label('lAddress', ('Adresa:', 'Adresa:'), 6, 122, 9),
    textbox('tbAddress', X1, 122, 80, 10, events=board),
    hint('lAddressHint', ('-1 znamená, že tabuľa so zariadením nekomunikuje.', '-1 znamená, že tabule se zařízením nekomunikuje.'), X1 + 90, 122, 11),
    label('lSave', ('Súbor s obsahom:', 'Soubor s obsahem:'), 6, 151, 12),
    textbox('tbSave', X1, 151, W, 13, events=board),
    label('lArrows', ('Otočené šípky:', 'Otočené šipky:'), 6, 180, 14),
    textbox('tbArrows', X1, 180, W, 15, events=board),
    hint('lArrowsHint', ('Čísla záznamov od 0 cez všetky časti.', 'Čísla záznamů od 0 přes všechny části.'), X1 + W + 10, 180, 16),
    check('cboxNoScroll', ('Nerolovať záznamy v zariadení', 'Nerolovat záznamy v zařízení'), X1, 209, 17, [('CheckedChanged', EH, 'Board_Changed')]),
    # jas: stlpce cez den a v noci, prazdne pole = hodnota z linky (napoveda v poli)
    control('System.Windows.Forms.Label', 'lLights', text=('Jas tabule', 'Jas tabule'), res=at(6, 244, 80, 15, 18, autosize=True)),
    label('lLightTime', ('Deň od, do:', 'Den od, do:'), 6, 266, 19),
    textbox('tbLightTime0', X1, 266, 100, 20, events=board),
    textbox('tbLightTime1', X1 + 110, 266, 100, 21, events=board),
    control('System.Windows.Forms.Label', 'lDay', text=('Cez deň', 'Ve dne'), res=at(X1, 298, 80, 15, 22, autosize=True)),
    control('System.Windows.Forms.Label', 'lNight', text=('V noci', 'V noci'), res=at(X1 + 110, 298, 80, 15, 23, autosize=True)),
    label('lLight', ('Jas:', 'Jas:'), 6, 316, 24),
    textbox('tbLight0', X1, 316, 100, 25, events=board),
    textbox('tbLight1', X1 + 110, 316, 100, 26, events=board),
    label('lLightOff', ('Jas bez vlakov:', 'Jas bez vlaků:'), 6, 345, 27),
    textbox('tbLightOff0', X1, 345, 100, 28, events=board),
    textbox('tbLightOff1', X1 + 110, 345, 100, 29, events=board),
    label('lLightIfEmpty', ('Bez vlakov plným jasom:', 'Bez vlaků plným jasem:'), 6, 374, 30),
    combo('cboLightIfEmpty0', X1, 374, 100, 31, [('SelectedIndexChanged', EH, 'Board_Changed')], editable=False),
    combo('cboLightIfEmpty1', X1 + 110, 374, 100, 32, [('SelectedIndexChanged', EH, 'Board_Changed')], editable=False),
    label('lLightOnTime', ('Čas medzi zmenami (ms):', 'Čas mezi změnami (ms):'), 6, 403, 33),
    textbox('tbLightOnTime', X1, 403, 100, 34, events=board),
    hint('lLightsHint', ('Prázdne pole znamená hodnotu z linky.', 'Prázdné pole znamená hodnotu z linky.'), X1 + 230, 266, 35),
], res={'AutoScroll': ('True', 'bool'), 'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('820, 560', S), 'TabIndex': ('0', 'int')})

# --- cast tabule ---
part = [('TextChanged', EH, 'Part_Changed')]
tracks = grid('dgvTracks', [
    column('cTrackKey', ('Koľaj', 'Kolej'), 'Key', 160, readonly=False, combo=True),
    column('cTrackPriority', ('Priorita', 'Priorita'), 'Priority', 80, readonly=False),
], [('CellValueChanged', 'System.Windows.Forms.DataGridViewCellEventHandler', 'Tracks_CellValueChanged'),
    ('DataError', 'System.Windows.Forms.DataGridViewDataErrorEventHandler', 'Grid_DataError'),
    ('CurrentCellDirtyStateChanged', EH, 'Grid_CurrentCellDirtyStateChanged')], 27, 130, 0)
tracks_panel = control('System.Windows.Forms.Panel', 'pTracks', children=[
    tracks, list_tools('Tracks'),
], res=at(X1, 340, 380, 160, 23))
part_panel = control('System.Windows.Forms.Panel', 'pPart', children=[
    label('lPartName', ('Názov:', 'Název:'), 6, 6, 0),
    textbox('tbPartName', X1, 6, W, 1, events=part),
    hint('lPartNameHint', ('Pri logických tabuliach určuje skupinu.', 'U logických tabulí určuje skupinu.'), X1 + W + 10, 6, 2),
    label('lTemplate', ('Predloha:', 'Předloha:'), 6, 35, 3),
    combo('cboTemplate', X1, 35, W, 4, part),
    label('lKind', ('Druh:', 'Druh:'), 6, 64, 5),
    combo('cboKind', X1, 64, W, 6, [('SelectedIndexChanged', EH, 'Part_Changed')], editable=False),
    label('lRecords', ('Počet záznamov:', 'Počet záznamů:'), 6, 93, 7),
    textbox('tbRecords', X1, 93, 80, 8, events=part),
    label('lStations', ('Stanice:', 'Stanice:'), 6, 122, 9),
    textbox('tbStations', X1, 122, W - 34, 10, events=part),
    control('ExControls.ExButton', 'bStations', text=('…', '…'), events=[('Click', EH, 'bStations_Click')], res=at(X1 + W - 30, 121, 30, 25, 36)),
    hint('lStationsHint', ('Prázdne pole je stanica tabule.', 'Prázdné pole je stanice tabule.'), X1 + W + 10, 122, 11),
    label('lPrevious', ('Predchádzajúca tabuľa:', 'Předchozí tabule:'), 6, 151, 12),
    combo('cboPrevious', X1, 151, W, 13, part),
    label('lWebSource', ('Webový zdroj:', 'Webový zdroj:'), 6, 180, 14),
    textbox('tbWebSource', X1, 180, W, 15, events=part),
    label('lWindows', ('Multiokná:', 'Multiokna:'), 6, 209, 16),
    textbox('tbWindows', X1, 209, W, 17, events=part),
    hint('lWindowsHint', ('Napríklad 0;1:2,4-6 (len HERMANMULTIOKNO).', 'Například 0;1:2,4-6 (jen HERMANMULTIOKNO).'), X1 + W + 10, 209, 18),
    label('lCondition', ('Podmienka zobrazenia:', 'Podmínka zobrazení:'), 6, 238, 19),
    code('codeCondition', X1, 238, 600, 90, 20, events=part),
    label('lTracks', ('Koľaje:', 'Koleje:'), 6, 340, 21),
    tracks_panel,
], res={'AutoScroll': ('True', 'bool'), 'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('820, 560', S), 'TabIndex': ('1', 'int')})

tree_tools = strip('tsBoards', [
    tool('tsbAddTable', ('Pridať tabuľu', 'Přidat tabuli'), 'table', 'tsbAddTable_Click', 'add'),
    tool('tsbAddPart', ('Pridať časť', 'Přidat část'), 'add', 'tsbAddPart_Click'),
    tool('tsbDeleteItem', ('Zmazať', 'Smazat'), 'delete', 'tsbDeleteItem_Click'),
    item('System.Windows.Forms.ToolStripSeparator', 'tssBoards1'),
    tool('tsbItemUp', ('Posunúť vyššie', 'Posunout výš'), 'arrow-up', 'tsbItemUp_Click'),
    tool('tsbItemDown', ('Posunúť nižšie', 'Posunout níž'), 'arrow-down', 'tsbItemDown_Click'),
])
tree = control('ExControls.ExTreeView', 'tvBoards', code=['FullRowSelect = true;', 'HideSelection = false;', 'ShowLines = false;',
                                                        'Style = ExControls.ExTreeViewStyle.Light;'],
               events=[('AfterSelect', 'System.Windows.Forms.TreeViewEventHandler', 'tvBoards_AfterSelect')],
               res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 27', P), 'Size': ('300, 500', S), 'TabIndex': ('0', 'int')})
page_tables = page('tpTables', ('Tabule', 'Tabule'), [split('splitTables', [tree, tree_tools], [part_panel, table_panel], distance=300)], 0)

# --- predlohy ---
template = [('TextChanged', EH, 'Template_Changed')]
template_panel = control('System.Windows.Forms.Panel', 'pTemplate', children=[
    label('lTemplateKey', ('Kľúč:', 'Klíč:'), 6, 6, 0),
    textbox('tbTemplateKey', 120, 6, W, 1, events=template),
    label('lTemplateName', ('Názov:', 'Název:'), 370, 6, 2),
    textbox('tbTemplateName', 430, 6, 300, 3, events=template),
    label('lClass', ('Druh zariadenia:', 'Druh zařízení:'), 6, 35, 4),
    combo('cboClass', 120, 35, W, 5, template, editable=False),
    label('lFontBank', ('Banka písem:', 'Banka písem:'), 370, 35, 6),
    textbox('tbFontBank', 470, 35, 260, 7, events=template),
], res={'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('860, 66', S), 'TabIndex': ('1', 'int')})
columns = grid('dgvColumns', [
    column('cColumnKey', ('Kľúč', 'Klíč'), 'Key', 90, readonly=False),
    column('cColumnName', ('Názov', 'Název'), 'Name', 120, readonly=False, fill=True, min_width=120),
    column('cColumnContent', ('Obsah', 'Obsah'), 'Content', 185, readonly=False, combo=True),
    column('cColumnRow', ('Riadok', 'Řádek'), 'Row', 50, readonly=False),
    column('cColumnStart', ('Od', 'Od'), 'Start', 40, readonly=False),
    column('cColumnEnd', ('Do', 'Do'), 'End', 40, readonly=False),
    column('cColumnFont', ('Písmo', 'Písmo'), 'Font', 50, readonly=False),
    column('cColumnAlign', ('Zarovnanie', 'Zarovnání'), 'Align', 125, readonly=False, combo=True),
    column('cColumnTranslation', ('Preklad', 'Překlad'), 'Translation', 155, readonly=False, combo=True),
    column('cColumnTable1', ('Tabuľka 1', 'Tabulka 1'), 'Table1', 110, readonly=False, combo=True),
    column('cColumnTable2', ('Tabuľka 2', 'Tabulka 2'), 'Table2', 110, readonly=False, combo=True),
], [('CellValueChanged', 'System.Windows.Forms.DataGridViewCellEventHandler', 'Columns_CellValueChanged'),
    ('DataError', 'System.Windows.Forms.DataGridViewDataErrorEventHandler', 'Grid_DataError'),
    ('CurrentCellDirtyStateChanged', EH, 'Grid_CurrentCellDirtyStateChanged')], 117, 400, 0)
columns_caption = caption('lColumns', ('Údaje predlohy. Od a Do sú znaky, body alebo súradnice okna podľa druhu zariadenia, Do je prvá pozícia za údajom.',
                                       'Údaje předlohy. Od a Do jsou znaky, body nebo souřadnice okna podle druhu zařízení, Do je první pozice za údajem.'), 3)
templates_list = control('ExControls.ExListBox', 'lbTemplates', code=['FormattingEnabled = true;', 'IntegralHeight = false;'],
                         events=[('SelectedIndexChanged', EH, 'lbTemplates_SelectedIndexChanged')],
                         res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 27', P), 'Size': ('300, 500', S), 'TabIndex': ('0', 'int')})
page_templates = page('tpTemplates', ('Predlohy', 'Předlohy'), [
    split('splitTemplates', [templates_list, list_tools('Templates')], [columns, list_tools('Columns', 2), columns_caption, template_panel], distance=300)], 1)

tabs = control('ExControls.ExTabControl', 'tcMain', children=[page_tables, page_templates], code=['SelectedIndex = 0;'],
               res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 29', P), 'Size': ('1380, 588', S), 'TabIndex': ('0', 'int')})

boards_form = control('System.Windows.Forms.Form', 'FBoards', children=[
    tabs,
    station_bar('flpStation', 'lStation', 'cboStation', 'lStationInfo', ('Stanica:', 'Stanice:'), 'cboStation_SelectedIndexChanged', 1),
    problems(617, 1380),
    buttons(707, 1380),
])

if __name__ == '__main__':
    os.makedirs(OUT, exist_ok=True)
    generate(boards_form, 'GVXEditor.UI.Boards', OUT, 'System.Windows.Forms.Form', form_code=form_code(1380, 746, 900, 600), form_res={},
             form_text=('Tabule', 'Tabule'))
    print('ok')
