"""Okno Hlasenia obsluhy: UI/Setup/FConstReports.Designer.cs, FConstReports.resx, FConstReports.cs.resx. Navrh pri Segoe UI 9 (7x15).

Okno vzniklo tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import column, control, generate
from gen_fsetup import at, caption, grid, list_tools, OUT, EH, P, S

reports = grid('Reports', [
    column('cRepKey', ('Číslo', 'Číslo'), 'Key', 60, readonly=False),
    column('cRepName', ('Názov', 'Název'), 'Name', 200, readonly=False),
    column('cRepFunction', ('Vzor hlásenia', 'Vzor hlášení'), 'Function', 320, readonly=False, combo=True),
    column('cRepStations', ('Stanice', 'Stanice'), 'Stations', 300, fill=True, readonly=False),
])
hint = caption('lReports', ('Na číslo hlásenia sa odkazujú dodatky, výluky a odklony vlakov. Stanice vyberiete tlačidlom … v bunke.',
                            'Na číslo hlášení se odkazují dodatky, výluky a odklony vlaků. Stanice vyberete tlačítkem … v buňce.'))
panel = control('System.Windows.Forms.Panel', 'pReports', children=[reports, list_tools('Reports'), hint],
                res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 0', P), 'Size': ('900, 420', S), 'TabIndex': ('0', 'int')})

problems = control('GVXEditor.UI.Controls.ProblemList', 'lbProblems', code=['FormattingEnabled = true;', 'IntegralHeight = false;'],
                   res={'Dock': ('Bottom', 'Dock'), 'Location': ('0, 420', P), 'Size': ('900, 80', S), 'TabIndex': ('1', 'int')})

buttons = control('System.Windows.Forms.FlowLayoutPanel', 'flpButtons', children=[
    control('ExControls.ExButton', 'bCancel', text=('Zrušiť', 'Zrušit'), code=['DialogResult = System.Windows.Forms.DialogResult.Cancel;'],
            res=at(809, 6, 88, 27, 1)),
    control('ExControls.ExButton', 'bOk', text=('OK', 'OK'), events=[('Click', EH, 'bOk_Click')], res=at(715, 6, 88, 27, 0)),
], code=['FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;'],
    res={'AutoSize': ('True', 'bool'), 'Dock': ('Bottom', 'Dock'), 'Location': ('0, 500', P), 'Size': ('900, 39', S), 'TabIndex': ('2', 'int'),
         'Padding': ('3, 3, 3, 3', 'Padding')})

form = control('System.Windows.Forms.Form', 'FConstReports', children=[panel, problems, buttons],
               events=[('FormClosing', 'System.Windows.Forms.FormClosingEventHandler', 'FConstReports_FormClosing')])

generate(form, 'GVXEditor.UI.Setup', OUT, 'System.Windows.Forms.Form', form_code=[
    'AcceptButton = this.bOk;',
    'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
    'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
    # bez CancelButton: Escape okno nezatvori, rozpisane upravy sa zahodia len tlacidlom Zrusit
    'ClientSize = new System.Drawing.Size(900, 539);',
    'MinimizeBox = false;',
    'MinimumSize = new System.Drawing.Size(640, 400);',
    'ShowIcon = false;',
    'ShowInTaskbar = false;',
    'StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;',
], form_res={}, form_text=('Hlásenia obsluhy', 'Hlášení obsluhy'))
print('ok')
