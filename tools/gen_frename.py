"""Okno Premenovat cislo vlaku: UI/Main/FRenameNumber.Designer.cs, .resx, .cs.resx. Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import control, generate

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Main')
EH = 'System.EventHandler'
P = 'System.Drawing.Point'
S = 'System.Drawing.Size'


def at(x, y, w, h, index, autosize=False):
    res = {'Location': (f'{x}, {y}', P), 'Size': (f'{w}, {h}', S), 'TabIndex': (str(index), 'int')}
    if autosize:
        res['AutoSize'] = ('True', 'bool')
    return res


controls = [
    control('System.Windows.Forms.Label', 'lNumber', text=('Číslo vlaku:', 'Číslo vlaku:'), res=at(12, 15, 75, 15, 0, autosize=True)),
    control('ExControls.ExComboBox', 'cboNumber', code=['DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;', 'FormattingEnabled = true;'],
            events=[('SelectedIndexChanged', EH, 'Input_Changed')], res=at(110, 11, 150, 23, 1)),
    control('System.Windows.Forms.Label', 'lNew', text=('Nové číslo:', 'Nové číslo:'), res=at(12, 47, 70, 15, 2, autosize=True)),
    control('ExControls.ExTextBox', 'tbNew', events=[('TextChanged', EH, 'Input_Changed')], res=at(110, 43, 150, 23, 3)),
    control('ExControls.ExCheckBox', 'cboxAllTrains', text=('', ''), code=['MaximumSize = new System.Drawing.Size(400, 0);'],
            events=[('CheckedChanged', EH, 'Input_Changed')], res=at(12, 78, 0, 19, 4, autosize=True)),
    control('System.Windows.Forms.Label', 'lInfo', text=('', ''), code=['MaximumSize = new System.Drawing.Size(400, 0);'],
            res=at(12, 106, 0, 15, 5, autosize=True)),
]

buttons = control('System.Windows.Forms.FlowLayoutPanel', 'flpButtons', children=[
    control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
            res=at(333, 6, 88, 27, 1)),
    control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), events=[('Click', EH, 'bOk_Click')], res=at(239, 6, 88, 27, 0)),
], code=['FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;'],
    res={'AutoSize': ('True', 'bool'), 'Dock': ('Bottom', 'Dock'), 'Location': ('0, 151', P), 'Size': ('424, 39', S), 'TabIndex': ('6', 'int'),
         'Padding': ('3, 3, 3, 3', 'Padding')})

form = control('System.Windows.Forms.Form', 'FRenameNumber', children=[buttons, *reversed(controls)])

generate(form, 'GVXEditor.UI.Main', OUT, 'System.Windows.Forms.Form', form_code=[
    'AcceptButton = this.bOk;',
    'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
    'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
    'CancelButton = this.bCancel;',
    'ClientSize = new System.Drawing.Size(424, 190);',
    'FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;',
    'MaximizeBox = false;',
    'MinimizeBox = false;',
    'ShowIcon = false;',
    'ShowInTaskbar = false;',
    'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
], form_res={}, form_text=('Premenovať číslo vlaku', 'Přejmenovat číslo vlaku'))
print('ok')
