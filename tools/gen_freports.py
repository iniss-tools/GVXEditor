"""Okno Hlasenia: UI/Code/FReports (Designer.cs, .resx, .cs.resx). Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat. Pomocne funkcie su z gen_fcode.py.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import column, control, generate, item
from gen_fcode import EH, ICONS, OUT, P, S, at, buttons, form_code, label, problems, split, strip, textbox, tool

GRID = ['AllowUserToAddRows = false;', 'AllowUserToDeleteRows = false;', 'AllowUserToResizeRows = false;',
        'ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;',
        'MultiSelect = false;', 'RowHeadersVisible = false;']
CELL = 'System.Windows.Forms.DataGridViewCellEventHandler'
COMBO = ['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;']


def caption(name, text, index):
    return control('System.Windows.Forms.Label', name, text=text, res={
        'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('700, 24', S), 'TabIndex': (str(index), 'int'), 'TextAlign': ('MiddleLeft', 'Align'),
        'Padding': ('3, 0, 0, 0', 'Padding')})


def combo(name, x, y, w, index, handler, anchor=None):
    return control('ExControls.ExComboBox', name, code=COMBO, events=[('SelectedIndexChanged', EH, handler)], res=at(x, y, w, 23, index, anchor))


def pick(name, x, y, index, handler):
    return control('ExControls.ExButton', name, text=('…', '…'), events=[('Click', EH, handler)], res=at(x, y - 1, 34, 25, index, 'Top, Right'))


# --- zoznam hlaseni ---
report_list = control('System.Windows.Forms.DataGridView', 'dgvReports', init=True, columns=[
    column('cRepName', ('Názov', 'Název'), 'Name', 150),
    column('cRepKind', ('Druh', 'Druh'), 'KindText', 100),
    column('cRepDescription', ('Popis', 'Popis'), 'Description', 200, fill=True),
    column('cRepState', ('Úprava', 'Úprava'), 'StateText', 75),
    column('cRepRefs', ('Odkazy', 'Odkazy'), 'References', 55),
], events=[('SelectionChanged', EH, 'dgvReports_SelectionChanged'),
           ('CellFormatting', 'System.Windows.Forms.DataGridViewCellFormattingEventHandler', 'dgvReports_CellFormatting')],
    code=GRID + ['ReadOnly = true;', 'SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;'],
    res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 58', P), 'Size': ('470, 500', S), 'TabIndex': ('0', 'int')})

report_tools = strip('tsReports', [
    tool('tsbNew', ('Nové hlásenie', 'Nové hlášení'), 'add', 'tsbNew_Click'),
    tool('tsbCopy', ('Kopírovať hlásenie', 'Kopírovat hlášení'), 'document-copy', 'tsbCopy_Click'),
    tool('tsbDelete', ('Zmazať hlásenie', 'Smazat hlášení'), 'delete', 'tsbDelete_Click'),
    item('System.Windows.Forms.ToolStripSeparator', 'tssReports1'),
    tool('tsbOpenCode', ('Otvoriť v skripte hlásení', 'Otevřít ve skriptu hlášení'), 'code', 'tsbOpenCode_Click'),
], index=2)

filter_panel = control('System.Windows.Forms.Panel', 'pFilter', children=[
    label('lFilterKind', ('Druh:', 'Druh:'), 6, 4, 0),
    combo('cboFilterKind', 50, 4, 160, 1, 'Filter_Changed'),
    textbox('tbFilter', 220, 4, 244, 2, 'Top, Left, Right', events=[('TextChanged', EH, 'Filter_Changed')]),
], res={'Dock': ('Top', 'Dock'), 'Location': ('0, 27', P), 'Size': ('470, 31', S), 'TabIndex': ('1', 'int')})

# --- hlasenie ---
changed = [('TextChanged', EH, 'Report_Changed')]
header = control('System.Windows.Forms.Panel', 'pHeader', children=[
    label('lName', ('Názov funkcie:', 'Název funkce:'), 6, 6, 0),
    textbox('tbName', 130, 6, 220, 1, events=changed),
    label('lKind', ('Druh:', 'Druh:'), 366, 6, 2),
    combo('cboKind', 420, 6, 270, 3, 'Report_Changed', 'Top, Left, Right'),
    label('lDescription', ('Popis:', 'Popis:'), 6, 35, 4),
    textbox('tbDescription', 130, 35, 560, 5, 'Top, Left, Right', events=changed),
    label('lForStations', ('Len pre stanice:', 'Jen pro stanice:'), 6, 64, 6),
    textbox('tbForStations', 130, 64, 520, 7, 'Top, Left, Right', events=changed),
    pick('bForStations', 656, 64, 8, 'bForStations_Click'),
    label('lNotStations', ('Okrem staníc:', 'Kromě stanic:'), 6, 93, 9),
    textbox('tbNotStations', 130, 93, 520, 10, 'Top, Left, Right', events=changed),
    pick('bNotStations', 656, 93, 11, 'bNotStations_Click'),
    label('lExclusion', ('Typ výluky:', 'Typ výluky:'), 6, 122, 12),
    combo('cboExclusion', 130, 122, 220, 13, 'Report_Changed'),
    label('lLanguage', ('Len jazyk:', 'Jen jazyk:'), 366, 122, 14),
    combo('cboLanguage', 440, 122, 250, 15, 'Report_Changed', 'Top, Left, Right'),
    label('lGuards', ('Nevyhlásiť, keď:', 'Nevyhlásit, když:'), 6, 151, 16),
    control('ExControls.ExTextBox', 'tbGuards', code=['AcceptsReturn = true;', 'Multiline = true;', 'ScrollBars = System.Windows.Forms.ScrollBars.Vertical;'],
            events=changed, res=at(130, 151, 520, 46, 17, 'Top, Left, Right')),
    pick('bGuards', 656, 151, 18, 'bGuards_Click'),
    control('ExControls.ExCheckBox', 'cboxSection', text=('Pri chybe zvuku vrátiť celé hlásenie (sekcia)', 'Při chybě zvuku vrátit celé hlášení (sekce)'),
            events=[('CheckedChanged', EH, 'Report_Changed')], res=at(130, 203, 80, 19, 19, autosize=True)),
], res={'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('700, 230', S), 'TabIndex': ('2', 'int')})

steps = control('System.Windows.Forms.DataGridView', 'dgvSteps', init=True, columns=[
    column('cStepBlock', ('Jazyk bloku', 'Jazyk bloku'), 'Block', 80, readonly=False, combo=True),
    column('cStepCondition', ('Podmienka', 'Podmínka'), 'Condition', 140, readonly=False),
    column('cStepKind', ('Krok', 'Krok'), 'Kind', 105, readonly=False, combo=True),
    column('cStepValue', ('Zvuk, pauza (ms) alebo riadok', 'Zvuk, pauza (ms) nebo řádek'), 'Value', 175, readonly=False),
    column('cStepBefore', ('Pauza pred', 'Pauza před'), 'Before', 55, readonly=False),
    column('cStepAfter', ('Pauza po', 'Pauza po'), 'After', 55, readonly=False),
    column('cStepText', ('Text zvuku', 'Text zvuku'), None, 160, fill=True),
    column('cStepComment', ('Poznámka', 'Poznámka'), 'Comment', 80, readonly=False),
], events=[('CellValueChanged', CELL, 'Steps_CellValueChanged'),
           ('CurrentCellDirtyStateChanged', EH, 'Steps_CurrentCellDirtyStateChanged'),
           ('DataError', 'System.Windows.Forms.DataGridViewDataErrorEventHandler', 'Steps_DataError'),
           ('CellFormatting', 'System.Windows.Forms.DataGridViewCellFormattingEventHandler', 'Steps_CellFormatting')],
    code=[line for line in GRID if line != 'MultiSelect = false;'], res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 51', P), 'Size': ('700, 250', S),
                                                                       'TabIndex': ('0', 'int')})

step_tools = strip('tsSteps', [
    tool('tsbAddSounds', ('Pridať zvuky z banky', 'Přidat zvuky z banky'), 'speaker', 'tsbAddSounds_Click', 'add'),
    tool('tsbAddPause', ('Pridať pauzu', 'Přidat pauzu'), 'pause', 'tsbAddPause_Click', 'add'),
    item(ICONS + 'IconToolStripDropDownButton', 'tsbAddBuiltin', text=('Pridať vstavanú položku', 'Přidat vestavěnou položku'),
         code=['DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;', 'IconName = "train";', 'IconBadge = "add";']),
    tool('tsbAddCall', ('Pridať volanie funkcie', 'Přidat volání funkce'), 'code', 'tsbAddCall_Click', 'add'),
    item(ICONS + 'IconToolStripDropDownButton', 'tsbCondition', text=('Podmienka vybraných krokov', 'Podmínka vybraných kroků'),
         code=['DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;', 'IconName = "filter";']),
    tool('tsbToggleStep', ('Vypnúť alebo zapnúť krok', 'Vypnout nebo zapnout krok'), 'eye', 'tsbToggleStep_Click'),
    tool('tsbDeleteStep', ('Zmazať krok', 'Smazat krok'), 'delete', 'tsbDeleteStep_Click'),
    item('System.Windows.Forms.ToolStripSeparator', 'tssSteps1'),
    tool('tsbStepUp', ('Posunúť vyššie', 'Posunout výš'), 'arrow-up', 'tsbStepUp_Click'),
    tool('tsbStepDown', ('Posunúť nižšie', 'Posunout níž'), 'arrow-down', 'tsbStepDown_Click'),
], index=1)
step_tools.res['Location'] = ('0, 24', P)

steps_panel = control('System.Windows.Forms.Panel', 'pSteps', children=[
    steps, step_tools,
    caption('lSteps', ('Kroky v poradí hlásenia. Zvuk iného jazyka server preskočí aj s jeho pauzami, samostatná pauza platí vo všetkých jazykoch.',
                       'Kroky v pořadí hlášení. Zvuk jiného jazyka server přeskočí i s jeho pauzami, samostatná pauza platí ve všech jazycích.'), 2),
], res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 230', P), 'Size': ('700, 224', S), 'TabIndex': ('0', 'int')})

preview = control('System.Windows.Forms.Panel', 'pPreview', children=[
    control('ExControls.ExTextBox', 'tbPreview', code=['Multiline = true;', 'ReadOnly = true;', 'ScrollBars = System.Windows.Forms.ScrollBars.Vertical;'],
            res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 24', P), 'Size': ('700, 82', S), 'TabIndex': ('0', 'int')}),
    caption('lPreview', ('Text hlásenia z textov zvukov banky:', 'Text hlášení z textů zvuků banky:'), 1),
], res={'Dock': ('Bottom', 'Dock'), 'Location': ('0, 454', P), 'Size': ('700, 106', S), 'TabIndex': ('1', 'int')})

report_panel = control('System.Windows.Forms.Panel', 'pReport', children=[steps_panel, preview, header],
                       res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('700, 560', S), 'TabIndex': ('0', 'int')})

manual_panel = control('System.Windows.Forms.Panel', 'pManual', children=[
    control('System.Windows.Forms.Label', 'lManual', text=('', ''), res=at(6, 6, 680, 60, 0, 'Top, Left, Right')),
    control('ExControls.ExButton', 'bOpenManual', text=('Otvoriť v skripte hlásení', 'Otevřít ve skriptu hlášení'),
            events=[('Click', EH, 'tsbOpenCode_Click')], res=at(6, 72, 200, 27, 1, autosize=True)),
], code=['Visible = false;'], res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('700, 560', S), 'TabIndex': ('1', 'int')})

main_split = split('splitMain', [report_list, filter_panel, report_tools], [report_panel, manual_panel], distance=540)
# zoznam hlaseni ma pevnu sirku, pri vacsom okne rastie cast s krokmi
main_split.res['FixedPanel'] = ('Panel1', 'FixedPanel')
main_split.res['Size'] = ('1400, 651', S)

reports_form = control('System.Windows.Forms.Form', 'FReports', children=[
    main_split,
    problems(651, 1400),
    buttons(741, 1400),
])

if __name__ == '__main__':
    generate(reports_form, 'GVXEditor.UI.Code', OUT, 'System.Windows.Forms.Form', form_code=form_code(1400, 800, 1000, 600), form_res={},
             form_text=('Hlásenia', 'Hlášení'))
    print('ok')
