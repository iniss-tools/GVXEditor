"""Okno Generovat texty tabul: UI/Boards/FGenerateTexts.* (Designer.cs, .resx, .cs.resx). Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import column, control, generate

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Boards')
EH = 'System.EventHandler'
P = 'System.Drawing.Point'
S = 'System.Drawing.Size'
FIELD_X = 150
FIELD_W = 360
WIDTH = 760


def at(x, y, w, h, index, anchor=None, autosize=False):
    res = {'Location': (f'{x}, {y}', P), 'Size': (f'{w}, {h}', S), 'TabIndex': (str(index), 'int')}
    if anchor:
        res['Anchor'] = (anchor, 'Anchor')
    if autosize:
        res['AutoSize'] = ('True', 'bool')
    return res


def row_label(name, text, y, index):
    return control('System.Windows.Forms.Label', name, text=text, res=at(12, y + 4, 100, 15, index, autosize=True))


def combo(name, y, index):
    return control('ExControls.ExComboBox', name, code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
                   res=at(FIELD_X, y, FIELD_W, 23, index), events=[('SelectedIndexChanged', EH, 'Input_Changed')])


def radio(name, text, x, y, index, checked=False):
    return control('ExControls.ExRadioButton', name, text=text, code=['Checked = true;'] if checked else [],
                   res=at(x, y, 200, 19, index, autosize=True), events=[('CheckedChanged', EH, 'Input_Changed')])


def group(name, y, h, index, children):
    # vlastny kontajner, aby boli prepinace dvoch volieb nezavisle
    return control('System.Windows.Forms.Panel', name, children=children, res=at(FIELD_X, y, WIDTH - FIELD_X - 12, h, index, 'Top, Left, Right'))


preview = control('System.Windows.Forms.DataGridView', 'dgvPreview', init=True, columns=[
    column('cNumber', ('Vlak', 'Vlak'), 'Number', 70),
    column('cValidity', ('Platnosť', 'Platnost'), 'Validity', 90),
    column('cText', ('Text', 'Text'), 'Text', 180),
    column('cCurrent', ('Teraz', 'Nyní'), 'Current', 180),
    column('cChange', ('Zmena', 'Změna'), 'Change', 200, fill=True),
], code=['AllowUserToAddRows = false;', 'AllowUserToDeleteRows = false;', 'AllowUserToResizeRows = false;',
         'ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;',
         'MultiSelect = false;', 'ReadOnly = true;', 'RowHeadersVisible = false;',
         'SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;'],
    res=at(12, 228, WIDTH - 24, 230, 9, 'Top, Bottom, Left, Right'))

form = control('System.Windows.Forms.Form', 'FGenerateTexts', children=[
    row_label('lStation', ('Stanica:', 'Stanice:'), 12, 0),
    combo('cboStation', 12, 1),
    row_label('lSet', ('Sada textov:', 'Sada textů:'), 41, 2),
    combo('cboSet', 41, 3),
    row_label('lKind', ('Text:', 'Text:'), 70, 4),
    combo('cboKind', 70, 5),
    row_label('lReport', ('Stanice z hlásenia:', 'Stanice z hlášení:'), 101, 6),
    group('pReport', 99, 23, 7, [
        radio('rbShort', ('krátkeho', 'krátkého'), 0, 2, 0, checked=True),
        radio('rbLong', ('dlhého', 'dlouhého'), 110, 2, 1),
    ]),
    row_label('lTrains', ('Vlaky:', 'Vlaky:'), 130, 8),
    group('pTrains', 128, 46, 9, [
        radio('rbMissing', ('Len vlaky, ktoré v sade text nemajú', 'Jen vlaky, které v sadě text nemají'), 0, 2, 0, checked=True),
        radio('rbAll', ('Všetky vlaky stanice (nahradí texty v sade)', 'Všechny vlaky stanice (nahradí texty v sadě)'), 0, 25, 1),
    ]),
    control('System.Windows.Forms.Label', 'lHint', text=(
        'Text pri vlaku má na serveri prednosť pred textami podľa trasy a stanice. Vlak, ktorému by server ukázal rovnaký text aj bez neho, záznam nedostane.',
        'Text u vlaku má na serveru přednost před texty podle trasy a stanice. Vlak, kterému by server ukázal stejný text i bez něj, záznam nedostane.'),
            res=at(12, 184, WIDTH - 24, 36, 10, 'Top, Left, Right')),
    preview,
    control('System.Windows.Forms.Label', 'lSummary', text=('', ''), res=at(12, 470, 480, 15, 11, 'Bottom, Left, Right')),
    control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), events=[('Click', EH, 'bOk_Click')], res=at(WIDTH - 200, 466, 88, 27, 12, 'Bottom, Right')),
    control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
            res=at(WIDTH - 106, 466, 88, 27, 13, 'Bottom, Right')),
])

if __name__ == '__main__':
    generate(form, 'GVXEditor.UI.Boards', OUT, 'System.Windows.Forms.Form', form_code=[
        'AcceptButton = this.bOk;',
        'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
        'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
        'CancelButton = this.bCancel;',
        f'ClientSize = new System.Drawing.Size({WIDTH}, 505);',
        f'MinimumSize = new System.Drawing.Size({WIDTH - 140}, 420);',
        'MinimizeBox = false;',
        'ShowIcon = false;',
        'ShowInTaskbar = false;',
        'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
    ], form_res={}, form_text=('Generovať texty tabúľ', 'Generovat texty tabulí'))
    print('ok')
