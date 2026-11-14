"""Panel Kontrola instalacie (zoznam zisteni v hlavnom okne): UI/Analysis/AnalysisView.Designer.cs, .resx, .cs.resx.
Navrh pri Segoe UI 9 (7x15).

Panel vznikol tymto skriptom; kym sa Designer.cs neupravi rucne, mozno ho znova vygenerovat.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
from formgen import column, control, generate, item

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor')
OUT = os.path.join(ROOT, 'UI', 'Analysis')
ICONS = 'ToolsCore.Icons.'
EH = 'System.EventHandler'
P = 'System.Drawing.Point'
S = 'System.Drawing.Size'


# filter zavaznosti ako vo VS: prepinac s ikonou a poctom (text nastavuje kod)
def severity(name, tip, icon, handler):
    return item(ICONS + 'IconToolStripButton', name, code=[
        'Checked = true;',
        'CheckOnClick = true;',
        'CheckState = System.Windows.Forms.CheckState.Checked;',
        'DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;',
        f'IconName = "{icon}";',
    ], text=('', ''), tooltip=tip, events=[('CheckedChanged', EH, handler)])


def action(name, text, icon, handler):
    return item(ICONS + 'IconToolStripButton', name, code=[
        'DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;',
        f'IconName = "{icon}";',
    ], text=text, events=[('Click', EH, handler)])


tool_strip = control('System.Windows.Forms.ToolStrip', 'tsFindings', items=[
    severity('tsbErrors', ('Zobraziť chyby', 'Zobrazit chyby'), 'status-error', 'Filter_CheckedChanged'),
    severity('tsbWarnings', ('Zobraziť upozornenia', 'Zobrazit upozornění'), 'status-warning', 'Filter_CheckedChanged'),
    severity('tsbInfos', ('Zobraziť informácie', 'Zobrazit informace'), 'status-info', 'Filter_CheckedChanged'),
    item('System.Windows.Forms.ToolStripSeparator', 'tssFindings1'),
    action('tsbRefresh', ('Znova skontrolovať', 'Znovu zkontrolovat'), 'rerun', 'tsbRefresh_Click'),
    action('tsbVerify', ('Overiť kódom servera', 'Ověřit kódem serveru'), 'document-checkmark', 'tsbVerify_Click'),
    item('System.Windows.Forms.ToolStripLabel', 'tslStatus', text=('', '')),
], code=['GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;', 'ImageScalingSize = new System.Drawing.Size(16, 16);',
         'Padding = new System.Windows.Forms.Padding(3, 0, 1, 0);'],
    text=('toolStrip1', 'toolStrip1'),
    res={'Location': ('0, 0', P), 'Size': ('900, 25', S), 'TabIndex': ('1', 'int')})

findings = control('System.Windows.Forms.DataGridView', 'dgvFindings', init=True, columns=[
    column('cSeverity', ('', ''), None, 32, image=True),
    column('cPlace', ('Miesto', 'Místo'), 'Place', 260),
    column('cMessage', ('Popis', 'Popis'), 'Message', 400, fill=True),
], events=[('CellDoubleClick', 'System.Windows.Forms.DataGridViewCellEventHandler', 'dgvFindings_CellDoubleClick'),
          ('CellFormatting', 'System.Windows.Forms.DataGridViewCellFormattingEventHandler', 'dgvFindings_CellFormatting'),
          ('KeyDown', 'System.Windows.Forms.KeyEventHandler', 'dgvFindings_KeyDown')], code=[
    'AllowUserToAddRows = false;',
    'AllowUserToDeleteRows = false;',
    'AllowUserToResizeRows = false;',
    'ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;',
    'MultiSelect = false;',
    'ReadOnly = true;',
    'RowHeadersVisible = false;',
    'SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;',
], res={'Dock': ('Fill', 'Dock'), 'Location': ('0, 25', P), 'Size': ('900, 195', S), 'TabIndex': ('0', 'int')})

view = control('System.Windows.Forms.UserControl', 'AnalysisView', children=[findings, tool_strip])

generate(view, 'GVXEditor.UI.Analysis', OUT, 'System.Windows.Forms.UserControl', form_code=[
    'AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);',
    'AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;',
], form_res={'Size': ('900, 220', S)}, form_text=('', ''))
print('ok')
