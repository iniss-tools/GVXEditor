"""Okno Posunut platnost grafikonu: UI/Main/FShiftValidity.Designer.cs, .resx, .cs.resx. Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import control, generate

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Main')
P = 'System.Drawing.Point'
S = 'System.Drawing.Size'


def at(x, y, w, h, index, autosize=False):
    res = {'Location': (f'{x}, {y}', P), 'Size': (f'{w}, {h}', S), 'TabIndex': (str(index), 'int')}
    if autosize:
        res['AutoSize'] = ('True', 'bool')
    return res


prompt = control('System.Windows.Forms.Label', 'lPrompt', text=('', ''), code=['MaximumSize = new System.Drawing.Size(400, 0);'],
                 res=at(12, 12, 0, 15, 0, autosize=True))
days = control('ExControls.ExNumericUpDown', 'nudDays', code=[
    'Maximum = new decimal(new int[] { 3650, 0, 0, 0 });',
    'Minimum = new decimal(new int[] { 3650, 0, 0, -2147483648 });',
    'Value = new decimal(new int[] { 364, 0, 0, 0 });',
], res=at(12, 66, 100, 23, 1))
unit = control('System.Windows.Forms.Label', 'lDays', text=('dní', 'dní'), res=at(118, 70, 24, 15, 2, autosize=True))

buttons = control('System.Windows.Forms.FlowLayoutPanel', 'flpButtons', children=[
    control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
            res=at(333, 6, 88, 27, 1)),
    control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), code=['DialogResult = System.Windows.Forms.DialogResult.OK;'],
            res=at(239, 6, 88, 27, 0)),
], code=['FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;'],
    res={'AutoSize': ('True', 'bool'), 'Dock': ('Bottom', 'Dock'), 'Location': ('0, 101', P), 'Size': ('424, 39', S), 'TabIndex': ('3', 'int'),
         'Padding': ('3, 3, 3, 3', 'Padding')})

form = control('System.Windows.Forms.Form', 'FShiftValidity', children=[buttons, unit, days, prompt])

generate(form, 'GVXEditor.UI.Main', OUT, 'System.Windows.Forms.Form', form_code=[
    'AcceptButton = this.bOk;',
    'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
    'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
    'CancelButton = this.bCancel;',
    'ClientSize = new System.Drawing.Size(424, 140);',
    'FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;',
    'MaximizeBox = false;',
    'MinimizeBox = false;',
    'ShowIcon = false;',
    'ShowInTaskbar = false;',
    'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
], form_res={}, form_text=('Posunúť platnosť grafikonu', 'Posunout platnost grafikonu'))
print('ok')
