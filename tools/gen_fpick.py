"""Okna vyberu hodnot: UI/Controls/FPickOrdered (zoznam v poradi - stanice, jazyky) a FPickChecked (viac hodnot
zaskrtnutim - pismena hlaseni, kategorie). Navrh pri Segoe UI 9 (7x15).

Okna vznikli tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ich znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import control, generate

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Controls')
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


def buttons(w, y):
    return control('System.Windows.Forms.FlowLayoutPanel', 'flpButtons', children=[
        control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
                res=at(w - 91, 6, 88, 27, 1)),
        control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), code=['DialogResult = System.Windows.Forms.DialogResult.OK;'], res=at(w - 185, 6, 88, 27, 0)),
    ], code=['FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;'],
        res={'AutoSize': ('True', 'bool'), 'Dock': ('Bottom', 'Dock'), 'Location': (f'0, {y}', P), 'Size': (f'{w}, 39', S), 'TabIndex': ('9', 'int'),
             'Padding': ('3, 3, 3, 3', 'Padding')})


def form_code(w, h, min_w, min_h):
    return [
        'AcceptButton = this.bOk;',
        'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
        'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
        'CancelButton = this.bCancel;',
        f'ClientSize = new System.Drawing.Size({w}, {h});',
        'MaximizeBox = false;',
        'MinimizeBox = false;',
        f'MinimumSize = new System.Drawing.Size({min_w}, {min_h});',
        'ShowIcon = false;',
        'ShowInTaskbar = false;',
        'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
    ]


LIST = ['FormattingEnabled = true;', 'IntegralHeight = false;']

# --- zoznam v poradi ---
ordered_controls = [
    control('System.Windows.Forms.Label', 'lAvailable', text=('Dostupné:', 'Dostupné:'), res=at(9, 9, 60, 15, 0, autosize=True)),
    control('ExControls.ExTextBox', 'tbFilter', events=[('TextChanged', EH, 'tbFilter_TextChanged')], res=at(9, 30, 270, 23, 1)),
    control('ExControls.ExListBox', 'lbAvailable', code=LIST + ['SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;'],
            events=[('DoubleClick', EH, 'bAdd_Click')], res=at(9, 59, 270, 330, 2, 'Top, Bottom, Left')),
    control('ExControls.ExButton', 'bAdd', text=('Pridať >', 'Přidat >'), events=[('Click', EH, 'bAdd_Click')], res=at(288, 120, 98, 27, 3)),
    control('ExControls.ExButton', 'bRemove', text=('< Odobrať', '< Odebrat'), events=[('Click', EH, 'bRemove_Click')], res=at(288, 153, 98, 27, 4)),
    control('ExControls.ExButton', 'bUp', text=('Vyššie', 'Výš'), events=[('Click', EH, 'bUp_Click')], res=at(288, 205, 98, 27, 5)),
    control('ExControls.ExButton', 'bDown', text=('Nižšie', 'Níž'), events=[('Click', EH, 'bDown_Click')], res=at(288, 238, 98, 27, 6)),
    control('System.Windows.Forms.Label', 'lSelected', text=('Vybrané v poradí:', 'Vybrané v pořadí:'), res=at(395, 9, 100, 15, 7, autosize=True)),
    control('ExControls.ExListBox', 'lbSelected', code=LIST + ['SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;'],
            events=[('DoubleClick', EH, 'bRemove_Click')], res=at(395, 30, 270, 359, 8, 'Top, Bottom, Left, Right')),
]
ordered_form = control('System.Windows.Forms.Form', 'FPickOrdered', children=[buttons(674, 400), *reversed(ordered_controls)])

# --- viac hodnot zaskrtnutim ---
checked_controls = [
    control('System.Windows.Forms.Label', 'lHint', text=('', ''), code=['MaximumSize = new System.Drawing.Size(340, 0);'],
            res=at(9, 9, 0, 15, 0, autosize=True)),
    control('ExControls.ExCheckedListBox', 'clbItems', code=['CheckOnClick = true;'] + LIST, res=at(9, 52, 342, 280, 1, 'Top, Bottom, Left, Right')),
]
checked_form = control('System.Windows.Forms.Form', 'FPickChecked', children=[buttons(360, 341), *reversed(checked_controls)])

# --- zvuky zo zvukovej banky: strom skupin a zvukov, vybrane zvuky v poradi (radenie) alebo jeden zvuk (kolaj) ---
sound_controls = [
    control('System.Windows.Forms.Label', 'lLanguage', text=('Jazyk banky:', 'Jazyk banky:'), res=at(9, 12, 70, 15, 0, autosize=True)),
    control('ExControls.ExComboBox', 'cboLanguage', code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
            events=[('SelectedIndexChanged', EH, 'cboLanguage_SelectedIndexChanged')], res=at(100, 9, 150, 23, 1)),
    control('ExControls.ExTextBox', 'tbSoundFilter', events=[('TextChanged', EH, 'tbSoundFilter_TextChanged')], res=at(258, 9, 152, 23, 2)),
    control('ExControls.ExTreeView', 'tvSounds', code=['FullRowSelect = true;', 'HideSelection = false;', 'ShowLines = false;',
                                                        'Style = ExControls.ExTreeViewStyle.Light;'],
            events=[('NodeMouseDoubleClick', 'System.Windows.Forms.TreeNodeMouseClickEventHandler', 'tvSounds_NodeMouseDoubleClick')],
            res=at(9, 40, 401, 350, 3, 'Top, Bottom, Left')),
    control('ExControls.ExButton', 'bSoundAdd', text=('Pridať >', 'Přidat >'), events=[('Click', EH, 'bSoundAdd_Click')], res=at(419, 120, 98, 27, 4)),
    control('ExControls.ExButton', 'bSoundRemove', text=('< Odobrať', '< Odebrat'), events=[('Click', EH, 'bSoundRemove_Click')], res=at(419, 153, 98, 27, 5)),
    control('ExControls.ExButton', 'bSoundUp', text=('Vyššie', 'Výš'), events=[('Click', EH, 'bSoundUp_Click')], res=at(419, 205, 98, 27, 6)),
    control('ExControls.ExButton', 'bSoundDown', text=('Nižšie', 'Níž'), events=[('Click', EH, 'bSoundDown_Click')], res=at(419, 238, 98, 27, 7)),
    control('ExControls.ExButton', 'bPlay', text=('Prehrať', 'Přehrát'), events=[('Click', EH, 'bPlay_Click')], res=at(419, 300, 98, 27, 8)),
    control('System.Windows.Forms.Label', 'lSounds', text=('Zvuky v poradí:', 'Zvuky v pořadí:'), res=at(526, 12, 90, 15, 9, autosize=True)),
    control('ExControls.ExListBox', 'lbSounds', code=LIST + ['SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;'],
            events=[('DoubleClick', EH, 'bSoundRemove_Click')], res=at(526, 40, 270, 324, 10, 'Top, Bottom, Left, Right')),
    control('ExControls.ExCheckBox', 'cboxAnyLanguage', text=('Zvuk v jazyku hlásenia', 'Zvuk v jazyce hlášení'), code=['Checked = true;',
            'CheckState = System.Windows.Forms.CheckState.Checked;'], res=at(526, 370, 80, 19, 11, 'Bottom, Left', autosize=True)),
]
sound_form = control('System.Windows.Forms.Form', 'FPickSound', children=[buttons(805, 400), *reversed(sound_controls)])

if __name__ == '__main__':
    os.makedirs(OUT, exist_ok=True)
    generate(ordered_form, 'GVXEditor.UI.Controls', OUT, 'System.Windows.Forms.Form', form_code=form_code(674, 439, 560, 320), form_res={},
             form_text=('Výber', 'Výběr'))
    generate(checked_form, 'GVXEditor.UI.Controls', OUT, 'System.Windows.Forms.Form', form_code=form_code(360, 380, 300, 260), form_res={},
             form_text=('Výber', 'Výběr'))
    generate(sound_form, 'GVXEditor.UI.Controls', OUT, 'System.Windows.Forms.Form', form_code=form_code(805, 439, 640, 320), form_res={},
             form_text=('Zvuky', 'Zvuky'))
    print('ok')
