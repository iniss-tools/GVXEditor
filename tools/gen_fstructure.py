"""Okna Novy grafikon a Pridat stanicu: UI/Setup/FNewGroup.* a FAddStation.* (Designer.cs, .resx, .cs.resx). Navrh pri Segoe UI 9 (7x15).

Okna vznikli tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ich znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import control, generate

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Setup')
EH = 'System.EventHandler'
P = 'System.Drawing.Point'
S = 'System.Drawing.Size'
FIELD_X = 160
FIELD_W = 348


def at(x, y, w, h, index, anchor=None, autosize=False):
    res = {'Location': (f'{x}, {y}', P), 'Size': (f'{w}, {h}', S), 'TabIndex': (str(index), 'int')}
    if anchor:
        res['Anchor'] = (anchor, 'Anchor')
    if autosize:
        res['AutoSize'] = ('True', 'bool')
    return res


def row_label(name, text, y, index):
    return control('System.Windows.Forms.Label', name, text=text, res=at(12, y + 4, 100, 15, index, autosize=True))


def textbox(name, y, index, events=()):
    return control('ExControls.ExTextBox', name, res=at(FIELD_X, y, FIELD_W, 23, index, 'Top, Left, Right'), events=events)


def combo(name, y, index, events=()):
    return control('ExControls.ExComboBox', name, code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
                   res=at(FIELD_X, y, FIELD_W, 23, index, 'Top, Left, Right'), events=events)


def note(name, text, y, h, index):
    return control('System.Windows.Forms.Label', name, text=text, res=at(12, y, 496, h, index, 'Top, Left, Right'))


def buttons(y, index):
    return [
        control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), events=[('Click', EH, 'bOk_Click')], res=at(326, y, 88, 27, index, 'Bottom, Right')),
        control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
                res=at(420, y, 88, 27, index + 1, 'Bottom, Right')),
    ]


def form_code(height):
    return [
        'AcceptButton = this.bOk;',
        'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
        'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
        'CancelButton = this.bCancel;',
        f'ClientSize = new System.Drawing.Size(520, {height});',
        'FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;',
        'MaximizeBox = false;',
        'MinimizeBox = false;',
        'ShowIcon = false;',
        'ShowInTaskbar = false;',
        'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
    ]


changed = [('TextChanged', EH, 'Input_Changed')]

new_group = control('System.Windows.Forms.Form', 'FNewGroup', children=[
    row_label('lName', ('Názov grafikonu:', 'Název grafikonu:'), 12, 0),
    textbox('tbName', 12, 1, changed),
    row_label('lTemplate', ('Predloha:', 'Předloha:'), 41, 2),
    combo('cboTemplate', 41, 3, [('SelectedIndexChanged', EH, 'Input_Changed')]),
    control('ExControls.ExCheckBox', 'cboxCopyTrains', text=('Skopírovať aj vlaky, výluky, odklony a záznamy vlakov v staniciach',
                                                             'Zkopírovat i vlaky, výluky, odklony a záznamy vlaků ve stanicích'),
            res=at(FIELD_X, 72, 300, 19, 4, autosize=True)),
    note('lHint', ('Grafikon dostane kópiu súborov a staníc predlohy a pribudne na koniec zoznamu grafikonov. Zapíše sa hneď, bez kroku Späť.',
                   'Grafikon dostane kopii souborů a stanic předlohy a přibude na konec seznamu grafikonů. Zapíše se hned, bez kroku Zpět.'), 102, 34, 5),
    note('lProblem', ('', ''), 140, 34, 6),
    *buttons(180, 7),
])

add_station = control('System.Windows.Forms.Form', 'FAddStation', children=[
    row_label('lNumber', ('Číslo stanice:', 'Číslo stanice:'), 12, 0),
    textbox('tbNumber', 12, 1, changed),
    row_label('lName', ('Názov stanice:', 'Název stanice:'), 41, 2),
    textbox('tbName', 41, 3, changed),
    row_label('lDirectory', ('Priečinok:', 'Složka:'), 70, 4),
    textbox('tbDirectory', 70, 5, changed),
    row_label('lKind', ('Druh:', 'Druh:'), 99, 6),
    combo('cboKind', 99, 7),
    row_label('lTemplate', ('Nastavenie zo stanice:', 'Nastavení ze stanice:'), 128, 8),
    combo('cboTemplate', 128, 9, [('SelectedIndexChanged', EH, 'Input_Changed')]),
    note('lHint', ('Stanica dostane kópiu nastavenia predlohy bez záznamov vlakov a pribudne na koniec grafikonu. Zapíše sa hneď, bez kroku Späť.',
                   'Stanice dostane kopii nastavení předlohy bez záznamů vlaků a přibude na konec grafikonu. Zapíše se hned, bez kroku Zpět.'), 162, 34, 10),
    note('lProblem', ('', ''), 200, 34, 11),
    *buttons(240, 12),
])

if __name__ == '__main__':
    generate(new_group, 'GVXEditor.UI.Setup', OUT, 'System.Windows.Forms.Form', form_code=form_code(219), form_res={},
             form_text=('Nový grafikon', 'Nový grafikon'))
    generate(add_station, 'GVXEditor.UI.Setup', OUT, 'System.Windows.Forms.Form', form_code=form_code(279), form_res={},
             form_text=('Pridať stanicu', 'Přidat stanici'))
    print('ok')
