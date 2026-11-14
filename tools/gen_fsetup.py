"""Okno Nastavenie grafikonu: UI/Setup/FSetup.Designer.cs, FSetup.resx, FSetup.cs.resx. Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import column, control, generate, item

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Setup')
ICONS = 'ToolsCore.Icons.'
EH = 'System.EventHandler'
P = 'System.Drawing.Point'
S = 'System.Drawing.Size'

LIST_EVENTS = [
    ('CellValueChanged', 'System.Windows.Forms.DataGridViewCellEventHandler', 'List_CellValueChanged'),
    ('CurrentCellDirtyStateChanged', EH, 'List_CurrentCellDirtyStateChanged'),
    ('DataError', 'System.Windows.Forms.DataGridViewDataErrorEventHandler', 'List_DataError'),
    ('CellFormatting', 'System.Windows.Forms.DataGridViewCellFormattingEventHandler', 'List_CellFormatting'),
]
MASTER_EVENTS = LIST_EVENTS + [('CurrentCellChanged', EH, 'Master_CurrentCellChanged')]


def at(x, y, w, h, index, anchor=None, autosize=False):
    res = {'Location': (f'{x}, {y}', P), 'Size': (f'{w}, {h}', S), 'TabIndex': (str(index), 'int')}
    if anchor:
        res['Anchor'] = (anchor, 'Anchor')
    if autosize:
        res['AutoSize'] = ('True', 'bool')
    return res


def tool(name, tip, icon, handler):
    return item(ICONS + 'IconToolStripButton', name, code=['DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;', f'IconName = "{icon}";'],
                text=tip, events=[('Click', EH, handler)])


def list_tools(prefix, index=1):
    """Panel nastrojov zoznamu: pridat, zmazat, vyssie, nizsie (spolocne obsluhy podla panela)."""
    return control('System.Windows.Forms.ToolStrip', f'ts{prefix}', items=[
        tool(f'tsb{prefix}Add', ('Pridať', 'Přidat'), 'add', 'tsbAdd_Click'),
        tool(f'tsb{prefix}Delete', ('Zmazať', 'Smazat'), 'delete', 'tsbDelete_Click'),
        item('System.Windows.Forms.ToolStripSeparator', f'tss{prefix}1'),
        tool(f'tsb{prefix}Up', ('Posunúť vyššie', 'Posunout výš'), 'arrow-up', 'tsbUp_Click'),
        tool(f'tsb{prefix}Down', ('Posunúť nižšie', 'Posunout níž'), 'arrow-down', 'tsbDown_Click'),
    ], text=(f'ts{prefix}', f'ts{prefix}'),
        code=['GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;', 'ImageScalingSize = new System.Drawing.Size(20, 20);'],
        res={'Location': ('0, 0', P), 'Size': ('500, 27', S), 'TabIndex': (str(index), 'int')})


def grid(prefix, columns, master=False):
    code = ['AllowUserToAddRows = false;', 'AllowUserToDeleteRows = false;', 'AllowUserToResizeRows = false;',
            'ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;',
            'MultiSelect = false;', 'RowHeadersVisible = false;']
    return control('System.Windows.Forms.DataGridView', f'dgv{prefix}', init=True, columns=columns, events=MASTER_EVENTS if master else LIST_EVENTS,
                   code=code, res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 27', P), 'Size': ('500, 200', S), 'TabIndex': ('0', 'int')})


def caption(name, text, index=2):
    return control('System.Windows.Forms.Label', name, text=text, res={
        'Dock': ('Top', 'Dock'), 'Location': ('0, 0', P), 'Size': ('500, 24', S), 'TabIndex': (str(index), 'int'), 'TextAlign': ('MiddleLeft', 'Align'),
        'Padding': ('3, 0, 0, 0', 'Padding')})


def list_block(prefix, columns, title=None, master=False):
    """Tabulka s panelom nastrojov a nadpisom - poradie deti: tabulka (Fill) sa dokuje posledna."""
    kids = [grid(prefix, columns, master), list_tools(prefix)]
    if title:
        kids.append(caption(f'l{prefix}', title))
    return kids


def split(name, panel1, panel2, horizontal=False, distance=200, index=0):
    res = {'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('980, 520', S), 'SplitterDistance': (str(distance), 'int'),
           'TabIndex': (str(index), 'int')}
    if horizontal:
        res['Orientation'] = ('Horizontal', 'Orientation')
    return control('System.Windows.Forms.SplitContainer', name, init=True, panel1=panel1, panel2=panel2, res=res)


def page(name, text, children, index, padding=True):
    res = {'Location': ('4, 24', P), 'Size': ('972, 520', S), 'TabIndex': (str(index), 'int')}
    if padding:
        res['Padding'] = ('3, 3, 3, 3', 'Padding')
    return control('System.Windows.Forms.TabPage', name, text=text, code=['UseVisualStyleBackColor = true;'], children=children, res=res)


def col(name, header, prop, width, **kw):
    """Upravitelny stlpec (readonly=False), ak nie je povedane inak."""
    kw.setdefault('readonly', False)
    return column(name, header, prop, width, **kw)


def name_col(name, width=180, fill=False):
    """Nazov stanice ako vyber viazany na cislo stanice (rovnaka vlastnost ako stlpec cisla)."""
    return column(name, ('Názov stanice', 'Název stanice'), 'Station', width, fill=fill, readonly=False, combo=True)


# grafikon: nazvy stanic a predvolene stanice hlasenia
page_names = page('tpNames', ('Názvy staníc', 'Názvy stanic'), list_block('Names', [
    col('cNameStation', ('Číslo stanice', 'Číslo stanice'), 'Station', 100),
    col('cNameName', ('Názov', 'Název'), 'Name', 300, fill=True),
], ('Názvy staníc grafikonu. Server použije prvý grafikon v poradí, ktorý stanicu má.',
    'Názvy stanic grafikonu. Server použije první grafikon v pořadí, který stanici má.')), 0)

page_defaults = page('tpDefaults', ('Predvolené stanice hlásenia', 'Výchozí stanice hlášení'), list_block('Defaults', [
    col('cDefStation', ('Číslo stanice', 'Číslo stanice'), 'Station', 100),
    name_col('cDefStationName', 200),
    col('cDefName', ('Názov v súbore', 'Název v souboru'), 'Name', 200),
    col('cDefShort', ('Krátke hlásenie', 'Krátké hlášení'), 'Short', 100, check=True),
    col('cDefLong', ('Dlhé hlásenie', 'Dlouhé hlášení'), 'Long', 100, check=True, fill=True),
], ('Stanice, ktoré sa predvolene povedia v hlásení do smeru. Názov v súbore server nepoužíva.',
    'Stanice, které se ve výchozím stavu řeknou v hlášení do směru. Název v souboru server nepoužívá.')), 1)

# stanica: kolaje
page_tracks = page('tpTracks', ('Koľaje', 'Koleje'), list_block('Tracks', [
    col('cTrackKey', ('Kľúč', 'Klíč'), 'Key', 60),
    col('cTrackName', ('Názov', 'Název'), 'Name', 80),
    col('cTrackSector', ('Sektor', 'Sektor'), 'Sector', 60),
    col('cTrackText', ('Tabuľa: koľaj', 'Tabule: kolej'), 'TrackText', 90),
    col('cTrackSectorText', ('Tabuľa: sektor', 'Tabule: sektor'), 'SectorText', 90),
    col('cTrackPlatform', ('Tabuľa: nástupište', 'Tabule: nástupiště'), 'PlatformText', 110),
    col('cTrackPlatformTrack', ('Tabuľa: nástupište a koľaj', 'Tabule: nástupiště a kolej'), 'PlatformTrackText', 110),
    col('cTrackAlt', ('Tabuľa: náhradný text', 'Tabule: náhradní text'), 'AltTrackText', 110),
    col('cTrackSound', ('Zvuk koľaje', 'Zvuk koleje'), 'TrackSound', 80),
    col('cTrackPlatformSound', ('Zvuk nástupišťa', 'Zvuk nástupiště'), 'PlatformSound', 90),
    col('cTrackSectorSound', ('Zvuk sektora', 'Zvuk sektoru'), 'SectorSound', 80),
    col('cTrackSeparator', ('Oddeľovač', 'Oddělovač'), 'Separator', 75, check=True),
    col('cTrackCommon', ('Nie je koľaj', 'Není kolej'), 'Common', 80, check=True),
    col('cTrackNoAudio', ('Bez hlásenia', 'Bez hlášení'), 'NoAudio', 85, check=True),
    col('cTrackAlterTran', ('ALTERTRAN', 'ALTERTRAN'), 'AlterTran', 85, check=True),
], ('Koľaje v poradí ponuky obsluhy. Vlaky sa na koľaj odkazujú kľúčom.',
    'Koleje v pořadí nabídky obsluhy. Vlaky se na kolej odkazují klíčem.')), 0)

page_keys = page('tpReportKeys', ('Typy hlásení', 'Typy hlášení'), list_block('ReportKeys', [
    col('cKeyName', ('Hlásenie', 'Hlášení'), 'Name', 200),
    col('cKeyLetter', ('Písmeno', 'Písmeno'), 'Key', 70),
    col('cKeyCategories', ('Kategórie vlakov', 'Kategorie vlaků'), 'Categories', 120, fill=True),
], ('Písmeno hlásenia vyberá dodatky a radenie vlaku. Kategórie sú V, P a K oddelené medzerou.',
    'Písmeno hlášení vybírá dodatky a řazení vlaku. Kategorie jsou V, P a K oddělené mezerou.')), 1)

# stanica: vzory stanic v hlaseni
patterns_bar = control('System.Windows.Forms.FlowLayoutPanel', 'flpPatterns', children=[
    control('ExControls.ExRadioButton', 'rbShortPatterns', text=('Krátke hlásenie', 'Krátké hlášení'), code=['Checked = true;'],
            events=[('CheckedChanged', EH, 'rbPatterns_CheckedChanged')], res={**at(3, 3, 110, 19, 0, autosize=True), 'Margin': ('3, 5, 3, 0', 'Padding')}),
    control('ExControls.ExRadioButton', 'rbLongPatterns', text=('Dlhé hlásenie', 'Dlouhé hlášení'),
            events=[('CheckedChanged', EH, 'rbPatterns_CheckedChanged')], res={**at(119, 3, 100, 19, 1, autosize=True), 'Margin': ('9, 5, 3, 0', 'Padding')}),
    control('System.Windows.Forms.Label', 'lPatternsHint', text=('Server použije prvý vzor so stanicami vlaku v rovnakom poradí. Povedia sa označené stanice.',
                                                                 'Server použije první vzor se stanicemi vlaku ve stejném pořadí. Řeknou se označené stanice.'),
            res={**at(230, 7, 0, 15, 2, autosize=True), 'Margin': ('12, 7, 3, 0', 'Padding')}),
], res={'AutoSize': ('True', 'bool'), 'Dock': ('Top', 'Dock'), 'Location': ('3, 3', P), 'Size': ('966, 29', S), 'TabIndex': ('1', 'int')})

split_patterns = split('splitPatterns', list_block('Patterns', [
    column('cPatIndex', ('#', '#'), None, 35),
    column('cPatStations', ('Stanice vzoru', 'Stanice vzoru'), None, 300, fill=True),
], ('Vzory', 'Vzory'), master=True), list_block('PatternStops', [
    col('cPatStop', ('Číslo stanice', 'Číslo stanice'), 'Station', 100),
    name_col('cPatStopName', 200, fill=True),
    col('cPatStopReported', ('Povie sa', 'Řekne se'), 'Flag', 70, check=True),
], ('Stanice vybraného vzoru v poradí jazdy', 'Stanice vybraného vzoru v pořadí jízdy')), distance=480)

page_patterns = page('tpPatterns', ('Stanice v hlásení', 'Stanice v hlášení'), [split_patterns, patterns_bar], 2)

# stanica: texty podla trasy a podla stanice
def set_columns(prefix):
    return [
        col(f'c{prefix}SetName', ('Sada', 'Sada'), 'Name', 200, fill=True),
        col(f'c{prefix}SetFonet', ('Tyfloservis', 'Tyfloservis'), 'Fonet', 80, check=True),
    ]


def target_columns(prefix):
    return [
        col(f'c{prefix}TargetCatalog', ('Predloha tabule', 'Předloha tabule'), 'Catalog', 200, combo=True),
        col(f'c{prefix}TargetColumn', ('Údaj', 'Údaj'), 'Column', 140, fill=True, combo=True),
    ]


def sets_split(prefix):
    return split(f'split{prefix}Sets', list_block(f'{prefix}Sets', set_columns(prefix), ('Sady textov', 'Sady textů'), master=True),
                 list_block(f'{prefix}Targets', target_columns(prefix), ('Údaje tabúľ, v ktorých sa texty sady prejavia',
                                                                            'Údaje tabulí, ve kterých se texty sady projeví')),
                 horizontal=True, distance=200)


split_route_texts = split('splitRouteTexts', list_block('Routes', [
    col('cRouteText', ('Text', 'Text'), 'Text', 260),
    col('cRouteFont', ('Písmo', 'Písmo'), 'Font', 55),
    column('cRouteStations', ('Stanice trasy', 'Stanice trasy'), None, 300, fill=True),
], ('Texty sady. Server použije prvý text so stanicami trasy vlaku v rovnakom poradí.',
    'Texty sady. Server použije první text se stanicemi trasy vlaku ve stejném pořadí.'), master=True),
    list_block('RouteStops', [
        col('cRouteStop', ('Číslo stanice', 'Číslo stanice'), 'Station', 100),
        name_col('cRouteStopName', 200, fill=True),
        col('cRouteStopSelected', ('Vybraná do textu', 'Vybraná do textu'), 'Flag', 110, check=True),
    ], ('Stanice trasy vybraného textu v poradí jazdy', 'Stanice trasy vybraného textu v pořadí jízdy')), horizontal=True, distance=250)

page_route_texts = page('tpRouteTexts', ('Texty podľa trasy', 'Texty podle trasy'),
                        [split('splitRoutes', [sets_split('Route')], [split_route_texts], distance=330)], 3)

page_station_texts = page('tpStationTexts', ('Texty podľa stanice', 'Texty podle stanice'), [
    split('splitStationTexts', [sets_split('Station')], list_block('StationTexts', [
        col('cStationTextStation', ('Číslo stanice', 'Číslo stanice'), 'Station', 100),
        name_col('cStationTextName', 180),
        col('cStationTextText', ('Text', 'Text'), 'Text', 220, fill=True),
        col('cStationTextFont', ('Písmo', 'Písmo'), 'Font', 55),
    ], ('Texty staníc v sade. Server použije prvý text stanice.', 'Texty stanic v sadě. Server použije první text stanice.')), distance=330)], 4)

station_tabs = control('ExControls.ExTabControl', 'tcStation', children=[page_tracks, page_keys, page_patterns, page_route_texts, page_station_texts],
                       code=['SelectedIndex = 0;'],
                       res={'Dock': ('Fill', 'Dock'), 'Location': ('3, 32', P), 'Size': ('966, 485', S), 'TabIndex': ('1', 'int')})

station_bar = control('System.Windows.Forms.FlowLayoutPanel', 'flpStation', children=[
    control('System.Windows.Forms.Label', 'lStation', text=('Stanica:', 'Stanice:'),
            res={**at(3, 7, 50, 15, 0, autosize=True), 'Margin': ('3, 7, 3, 0', 'Padding')}),
    control('ExControls.ExComboBox', 'cboStation', code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
            events=[('SelectedIndexChanged', EH, 'cboStation_SelectedIndexChanged')], res=at(59, 3, 320, 23, 1)),
    control('System.Windows.Forms.Label', 'lStationInfo', text=('', ''), res={**at(385, 7, 0, 15, 2, autosize=True), 'Margin': ('3, 7, 3, 0', 'Padding')}),
], res={'AutoSize': ('True', 'bool'), 'Dock': ('Top', 'Dock'), 'Location': ('3, 3', P), 'Size': ('966, 29', S), 'TabIndex': ('0', 'int')})

page_stations = page('tpStations', ('Stanice', 'Stanice'), [station_tabs, station_bar], 2)

tabs = control('ExControls.ExTabControl', 'tcMain', children=[page_names, page_defaults, page_stations], code=['SelectedIndex = 0;'],
               res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('980, 548', S), 'TabIndex': ('0', 'int')})

problems = control('GVXEditor.UI.Controls.ProblemList', 'lbProblems', code=['FormattingEnabled = true;', 'IntegralHeight = false;'],
                   res={'Dock': ('Bottom', 'Dock'), 'Location': ('0, 548', P), 'Size': ('980, 80', S), 'TabIndex': ('1', 'int')})

buttons = control('System.Windows.Forms.FlowLayoutPanel', 'flpButtons', children=[
    control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
            res=at(889, 6, 88, 27, 1)),
    control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), events=[('Click', EH, 'bOk_Click')], res=at(795, 6, 88, 27, 0)),
], code=['FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;'],
    res={'AutoSize': ('True', 'bool'), 'Dock': ('Bottom', 'Dock'), 'Location': ('0, 628', P), 'Size': ('980, 39', S), 'TabIndex': ('2', 'int'),
         'Padding': ('3, 3, 3, 3', 'Padding')})

form = control('System.Windows.Forms.Form', 'FSetup', children=[tabs, problems, buttons],
               events=[('FormClosing', 'System.Windows.Forms.FormClosingEventHandler', 'FSetup_FormClosing')])

if __name__ == '__main__':
    generate(form, 'GVXEditor.UI.Setup', OUT, 'System.Windows.Forms.Form', form_code=[
        'AcceptButton = this.bOk;',
        'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
        'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
        # bez CancelButton: Escape okno nezatvori, rozpisane upravy sa zahodia len tlacidlom Zrusit
        'ClientSize = new System.Drawing.Size(980, 667);',
        'MinimizeBox = false;',
        'MinimumSize = new System.Drawing.Size(760, 520);',
        'ShowIcon = false;',
        'ShowInTaskbar = false;',
        'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
    ], form_res={}, form_text=('Nastavenie grafikonu', 'Nastavení grafikonu'))
    print('ok')
