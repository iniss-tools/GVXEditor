"""Generator rucneho Designer.cs a .resx (SK) / .cs.resx (CZ) pre formulare WinForms.

Prvky sa opisu stromom: kazdy prvok ma typ, nazov, vlastnosti kodu (riadky za `this.<nazov>.`), vlastnosti resx (rozlozenie),
lokalizovane texty (sk, cz) a deti. Poradie deti = poradie Controls.Add, teda ZOrder (prvy = 0 = Fill sa dokuje posledny).
"""
import base64
import os
import sys

sys.path.insert(0, os.path.dirname(__file__))
import resxlib

WF = 'System.Windows.Forms, Culture=neutral, PublicKeyToken=b77a5c561934e089'
TYPES = {
    'System.Drawing.Point': 'System.Drawing.Point, System.Drawing',
    'System.Drawing.Size': 'System.Drawing.Size, System.Drawing',
    'int': 'System.Int32, mscorlib',
    'bool': 'System.Boolean, mscorlib',
    'Dock': 'System.Windows.Forms.DockStyle, System.Windows.Forms',
    'Orientation': 'System.Windows.Forms.Orientation, System.Windows.Forms',
    'Padding': 'System.Windows.Forms.Padding, System.Windows.Forms',
    'FixedPanel': 'System.Windows.Forms.FixedPanel, System.Windows.Forms',
    'Align': 'System.Drawing.ContentAlignment, System.Drawing',
    'AutoSizeMode': 'System.Windows.Forms.DataGridViewAutoSizeColumnMode, System.Windows.Forms',
    'Anchor': 'System.Windows.Forms.AnchorStyles, System.Windows.Forms',
    'FlowDirection': 'System.Windows.Forms.FlowDirection, System.Windows.Forms',
}


def assembly_type(type_):
    if type_.startswith('System.Windows.Forms.'):
        return f'{type_}, {WF}'
    if type_.startswith('ExControls.'):
        return f'{type_}, ExControls, Culture=neutral, PublicKeyToken=null'
    if type_.startswith('ToolsCore.CodeEditor.'):
        return f'{type_}, ToolsCore.CodeEditor, Culture=neutral, PublicKeyToken=null'
    if type_.startswith('ToolsCore.'):
        return f'{type_}, ToolsCore, Culture=neutral, PublicKeyToken=null'
    if type_.startswith('GVXEditor.'):
        return f'{type_}, GVXEditor, Culture=neutral, PublicKeyToken=null'
    raise ValueError(type_)


class Node:
    """Prvok formulara."""

    def __init__(self, kind, type_, name, code=(), res=None, text=None, children=(), items=(), columns=(), panel1=(), panel2=(),
                 events=(), tooltip=None, init=False):
        self.kind = kind              # control | item | column | component
        self.type = type_
        self.name = name
        self.code = list(code)        # riadky kodu bez this.<nazov>.
        self.res = dict(res or {})    # vlastnost -> (hodnota, typ)
        self.text = text              # (sk, cz) pre Text / HeaderText
        self.tooltip = tooltip        # (sk, cz) pre ToolTipText
        self.children = list(children)
        self.items = list(items)
        self.columns = list(columns)
        self.panel1 = list(panel1)
        self.panel2 = list(panel2)
        self.events = list(events)    # (udalost, typ handlera, metoda)
        self.init = init              # ISupportInitialize


def control(type_, name, **kw):
    return Node('control', type_, name, **kw)


def item(type_, name, **kw):
    return Node('item', type_, name, **kw)


def column(name, header, prop, width=None, fill=False, check=False, readonly=True, combo=False, image=False, sort=False, min_width=None, **kw):
    """Stlpec tabulky; prop None = bez vazby (hodnotu nastavi kod), check = zaciarkavacie policko, combo = vyber (polozky nastavi kod),
    image = obrazok (ikonu nastavi kod pri formatovani bunky), sort = triedi kod po kliku na hlavicku, min_width = najmensia sirka
    stlpca fill (inak ho ostatne stlpce zuzia takmer na nic)."""
    code = [f'DataPropertyName = "{prop}";'] if prop else []
    if readonly:
        code.append('ReadOnly = true;')
    if sort:
        code.append('SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;')
    if fill:
        code.append('AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;')
    if min_width:
        code.append(f'MinimumWidth = {min_width};')
    res = {}
    if width:
        res['Width'] = (str(width), 'int')
    # vyber ako v GVDEditore: bunka vzdy vyzera ako rozbalovaci zoznam a ma temu programu
    type_ = ('System.Windows.Forms.DataGridViewCheckBoxColumn' if check else
             'ExControls.DataGridViewExComboBoxColumn' if combo else
             'System.Windows.Forms.DataGridViewImageColumn' if image else 'System.Windows.Forms.DataGridViewTextBoxColumn')
    if image:
        code.append('ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Normal;')
    return Node('column', type_, name, code=code, res=res, text=header, **kw)


def walk(node):
    yield node
    for child in node.children + node.items + node.panel1 + node.panel2 + node.columns:
        yield from walk(child)


def generate(form, namespace, out_dir, base_type, form_code, form_res, form_text, icon=None, fields_extra=(), components=()):
    """components = prvky mimo stromu ovladacich prvkov (napr. ContextMenuStrip), vytvoria sa s this.components."""
    nodes = [n for n in walk(form)][1:] + [n for c in components for n in walk(c)]
    lines = []
    w = lines.append
    w(f'namespace {namespace}')
    w('{')
    w(f'    partial class {form.name}')
    w('    {')
    w('        /// <summary>')
    w('        /// Required designer variable.')
    w('        /// </summary>')
    w('        private System.ComponentModel.IContainer components = null;')
    w('')
    w('        /// <summary>')
    w('        /// Clean up any resources being used.')
    w('        /// </summary>')
    w('        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>')
    w('        protected override void Dispose(bool disposing)')
    w('        {')
    w('            if (disposing && (components != null))')
    w('            {')
    w('                components.Dispose();')
    w('            }')
    w('            base.Dispose(disposing);')
    w('        }')
    w('')
    w('        #region Windows Form Designer generated code')
    w('')
    w('        /// <summary>')
    w('        /// Required method for Designer support - do not modify')
    w('        /// the contents of this method with the code editor.')
    w('        /// </summary>')
    w('        private void InitializeComponent()')
    w('        {')
    w('            this.components = new System.ComponentModel.Container();')
    w(f'            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof({form.name}));')
    # tabulka s vyberom: uprava hned po vstupe do bunky, zoznam sa rozbali druhym kliknutim (ako hlavne okno GVDEditora)
    for n in nodes:
        if n.type == 'System.Windows.Forms.DataGridView' and any(c.type == 'ExControls.DataGridViewExComboBoxColumn' for c in n.columns):
            edit_mode = 'EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;'
            if edit_mode not in n.code:
                n.code.append(edit_mode)
    for n in nodes:
        args = 'this.components' if n.kind == 'component' else ''
        w(f'            this.{n.name} = new {n.type}({args});')

    suspend = [n for n in nodes if n.kind in ('control', 'component') and (n.children or n.items or n.panel1 or n.panel2 or n.init)]
    for n in suspend:
        if n.init:
            w(f'            ((System.ComponentModel.ISupportInitialize)(this.{n.name})).BeginInit();')
        if n.panel1 or n.panel2:
            w(f'            this.{n.name}.Panel1.SuspendLayout();')
            w(f'            this.{n.name}.Panel2.SuspendLayout();')
        if n.children or n.items or n.panel1 or n.panel2:
            w(f'            this.{n.name}.SuspendLayout();')
    w('            this.SuspendLayout();')

    def block(n):
        w('            // ')
        w(f'            // {n.name}')
        w('            // ')
        for child in n.children:
            w(f'            this.{n.name}.Controls.Add(this.{child.name});')
        if n.items:
            prop = 'DropDownItems' if n.kind == 'item' else 'Items'
            w(f'            this.{n.name}.{prop}.AddRange(new System.Windows.Forms.ToolStripItem[] {{')
            w(',\n'.join(f'            this.{i.name}' for i in n.items) + '});')
        if n.columns:
            w(f'            this.{n.name}.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {{')
            w(',\n'.join(f'            this.{c.name}' for c in n.columns) + '});')
        w(f'            resources.ApplyResources(this.{n.name}, "{n.name}");')
        for code in n.code:
            w(f'            this.{n.name}.{code}')
        w(f'            this.{n.name}.Name = "{n.name}";')
        for event, handler, method in n.events:
            w(f'            this.{n.name}.{event} += new {handler}(this.{method});')
        for panel, kids in (('Panel1', n.panel1), ('Panel2', n.panel2)):
            if kids or n.panel1 or n.panel2:
                w('            // ')
                w(f'            // {n.name}.{panel}')
                w('            // ')
                for child in kids:
                    w(f'            this.{n.name}.{panel}.Controls.Add(this.{child.name});')

    for n in nodes:
        if n.kind != 'column':
            block(n)

    w('            // ')
    w(f'            // {form.name}')
    w('            // ')
    for code in form_code:
        w(f'            this.{code}')
    for child in form.children:
        w(f'            this.Controls.Add(this.{child.name});')
    if icon:
        w('            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));')
    w(f'            this.Name = "{form.name}";')
    w('            resources.ApplyResources(this, "$this");')
    for event, handler, method in form.events:
        w(f'            this.{event} += new {handler}(this.{method});')
    # stlpce na konci (pozri AGENTS.md: blok stlpca na konci InitializeComponent)
    for n in nodes:
        if n.kind == 'column':
            block(n)
    for n in reversed(suspend):
        if n.panel1 or n.panel2:
            w(f'            this.{n.name}.Panel1.ResumeLayout(false);')
            w(f'            this.{n.name}.Panel1.PerformLayout();')
            w(f'            this.{n.name}.Panel2.ResumeLayout(false);')
            w(f'            this.{n.name}.Panel2.PerformLayout();')
        if n.init:
            w(f'            ((System.ComponentModel.ISupportInitialize)(this.{n.name})).EndInit();')
        if n.children or n.items or n.panel1 or n.panel2:
            w(f'            this.{n.name}.ResumeLayout(false);')
            w(f'            this.{n.name}.PerformLayout();')
    w('            this.ResumeLayout(false);')
    w('            this.PerformLayout();')
    w('')
    w('        }')
    w('')
    w('        #endregion')
    w('')
    for n in nodes:
        w(f'        private {n.type} {n.name};')
    for field in fields_extra:
        w(f'        {field}')
    w('    }')
    w('}')

    with open(os.path.join(out_dir, f'{form.name}.Designer.cs'), 'w', encoding='utf-8-sig', newline='\r\n') as f:
        f.write('\n'.join(lines) + '\n')

    # resx
    sk, cz = [], []
    parents = {}
    zorder = {}
    for n in [form] + nodes:
        for i, child in enumerate(n.children):
            parents[child.name] = '$this' if n is form else n.name
            zorder[child.name] = i
        for panel, kids in (('Panel1', n.panel1), ('Panel2', n.panel2)):
            for i, child in enumerate(kids):
                parents[child.name] = f'{n.name}.{panel}'
                zorder[child.name] = i

    for n in nodes:
        for prop, (value, type_) in n.res.items():
            sk.append(resxlib.data(f'{n.name}.{prop}', value, TYPES[type_]))
        for prop, texts in (('HeaderText' if n.kind == 'column' else 'Text', n.text), ('ToolTipText', n.tooltip)):
            if texts:
                sk.append(resxlib.data(f'{n.name}.{prop}', texts[0]))
                if texts[0]:
                    cz.append(resxlib.data(f'{n.name}.{prop}', texts[1]))
        for panel in ('Panel1', 'Panel2'):
            if n.panel1 or n.panel2:
                sk.append(resxlib.data(f'>>{n.name}.{panel}.Name', f'{n.name}.{panel}'))
                sk.append(resxlib.data(f'>>{n.name}.{panel}.Type', assembly_type('System.Windows.Forms.SplitterPanel')))
                sk.append(resxlib.data(f'>>{n.name}.{panel}.Parent', n.name))
                sk.append(resxlib.data(f'>>{n.name}.{panel}.ZOrder', '0' if panel == 'Panel1' else '1'))
    for prop, (value, type_) in form_res.items():
        sk.append(resxlib.data(f'$this.{prop}', value, TYPES[type_]))
    if icon:
        encoded = base64.b64encode(open(icon, 'rb').read()).decode('ascii')
        wrapped = '\n' + '\n'.join('        ' + encoded[i:i + 80] for i in range(0, len(encoded), 80)) + '\n'
        sk.append(f'  <data name="$this.Icon" type="System.Drawing.Icon, System.Drawing" mimetype="application/x-microsoft.net.object.bytearray.base64">\n'
                  f'    <value>{wrapped}</value>\n  </data>\n')
    sk.append(resxlib.data('$this.Text', form_text[0]))
    for n in nodes:
        sk.append(resxlib.data(f'>>{n.name}.Name', n.name))
        sk.append(resxlib.data(f'>>{n.name}.Type', assembly_type(n.type)))
        if n.name in parents:
            sk.append(resxlib.data(f'>>{n.name}.Parent', parents[n.name]))
            sk.append(resxlib.data(f'>>{n.name}.ZOrder', str(zorder[n.name])))
    sk.append(resxlib.data('>>$this.Name', form.name))
    sk.append(resxlib.data('>>$this.Type', assembly_type(base_type)))
    for n in nodes:
        if n.kind == 'column':
            sk.append(resxlib.metadata(f'{n.name}.UserAddedColumn', 'True',
                                       'System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'))
    sk.append(resxlib.metadata('$this.Localizable', 'True', 'System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'))

    resxlib.write(os.path.join(out_dir, f'{form.name}.resx'), sk)
    resxlib.write(os.path.join(out_dir, f'{form.name}.cs.resx'), cz)
