import sys
import openpyxl
import fix_fill

sys.stdout.reconfigure(encoding='utf-8')
path = sys.argv[1]
row = int(sys.argv[2])
column = int(sys.argv[3])

with fix_fill.fix_fill(path) as new_path:
    book = openpyxl.load_workbook(new_path)
    sheet = book.active
    c = sheet.cell(row + 1, column + 1)
    print(c.value)
