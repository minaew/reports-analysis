import zipfile
import tempfile
import os
import xml.etree.ElementTree as ET


def fix_fill(path):
    tempdir = tempfile.mkdtemp()
    tempname = os.path.join(tempdir, 'new.xlsx')
    with zipfile.ZipFile(path, 'r') as zipread:
        with zipfile.ZipFile(tempname, 'w') as zipwrite:
            for item in zipread.infolist():
                data = zipread.read(item.filename)
                if item.filename == 'xl/styles.xml':
                    root = ET.fromstring(data)
                    for fills in root.findall('{http://schemas.openxmlformats.org/spreadsheetml/2006/main}fills'):
                        for fill in fills.findall('{http://schemas.openxmlformats.org/spreadsheetml/2006/main}fill'):
                            if len(fill.attrib) == 0:
                                fills.remove(fill)
                    new_data = ET.tostring(root)
                else:
                    new_data = data
                zipwrite.writestr(item, new_data)
    return tempname
