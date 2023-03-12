import zipfile
import tempfile
import shutil
import os
import xml.etree.ElementTree as ET


def remove_empty_fill(byte_array):
    root = ET.fromstring(byte_array)
    ET.register_namespace(
        '', 'http://schemas.openxmlformats.org/spreadsheetml/2006/main')
    for fills in root.findall('{http://schemas.openxmlformats.org/spreadsheetml/2006/main}fills'):
        for fill in fills.findall('{http://schemas.openxmlformats.org/spreadsheetml/2006/main}fill'):
            if len(fill.attrib) == 0:
                c = 0
                for i in fill.iter():  # todo: refuck
                    c = c + 1
                if c == 1:
                    fills.remove(fill)
    decl = byte_array[0: byte_array.find(b'>') + 1]
    return decl + ET.tostring(root).replace(b' />', b'/>')


class fix_fill(object):
    def __init__(self, file_name):
        self.file_name = file_name

    def __enter__(self):
        self.temp_dir = tempfile.mkdtemp()
        temp_path = os.path.join(self.temp_dir, 'new' + '.xlsx')
        with zipfile.ZipFile(self.file_name, 'r') as zipread:
            with zipfile.ZipFile(temp_path, 'w') as zipwrite:
                for item in zipread.infolist():
                    data = zipread.read(item.filename)
                    if item.filename == 'xl/styles.xml':
                        data = remove_empty_fill(data)
                    zipwrite.writestr(item, data)
        return temp_path

    def __exit__(self, *args):
        shutil.rmtree(self.temp_dir)
