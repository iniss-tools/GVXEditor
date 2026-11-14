"""Spolocne zapisovanie suborov .resx pre generatory GVXEditora (hlavicka ako vo VS, UTF-8 s BOM, CRLF)."""
import os
from xml.sax.saxutils import escape

# hlavicka .resx ako ju zapisuje Visual Studio (schema a resheader) - z hlavneho okna tohto programu
HEADER_SOURCE = os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'GVXEditor', 'UI', 'Main', 'FMain.resx')


def header():
    text = open(HEADER_SOURCE, encoding='utf-8-sig').read().replace('\r\n', '\n')
    end = text.rindex('</resheader>') + len('</resheader>')
    return text[:end] + '\n'


def data(name, value, type_=None, preserve=True, mimetype=None):
    attrs = f' name="{escape(name)}"'
    if type_:
        attrs += f' type="{type_}"'
    if mimetype:
        attrs += f' mimetype="{mimetype}"'
    if preserve and not type_:
        attrs += ' xml:space="preserve"'
    return f'  <data{attrs}>\n    <value>{escape(value)}</value>\n  </data>\n'


def metadata(name, value, type_):
    return f'  <metadata name="{escape(name)}" type="{type_}">\n    <value>{escape(value)}</value>\n  </metadata>\n'


def write(path, entries):
    text = header() + ''.join(entries) + '</root>'
    with open(path, 'w', encoding='utf-8-sig', newline='\r\n') as f:
        f.write(text)
