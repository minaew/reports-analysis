import unittest
import fix_fill


class Tests(unittest.TestCase):

    def test_do_not_remove_non_empty_fill(self):
        input = b'<?xml version="1.0" encoding="UTF-8" standalone="yes"?>' \
            b'<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">' \
            b'<numFmts count="2">' \
            b'<numFmt numFmtId="164" formatCode="General"/>' \
            b'<numFmt numFmtId="165" formatCode="#,##0.00"/>' \
            b'</numFmts>' \
            b'<fonts count="6">' \
            b'<font><sz val="10"/><name val="Arial"/><family val="0"/></font>' \
            b'<font><sz val="10"/><name val="Arial"/><family val="0"/></font>' \
            b'<font><sz val="10"/><name val="Arial"/><family val="0"/></font>' \
            b'<font><sz val="10"/><name val="Arial"/><family val="0"/></font>' \
            b'<font><b val="true"/><sz val="10"/><name val="Arial"/><family val="0"/></font>' \
            b'<font><sz val="10"/><color rgb="FFFF0000"/><name val="Arial"/><family val="0"/></font>' \
            b'</fonts>' \
            b'<fills count="2">' \
            b'<fill><patternFill patternType="none"/></fill>' \
            b'<fill><patternFill patternType="gray125"/></fill>' \
            b'</fills>' \
            b'<borders count="2">' \
            b'<border diagonalUp="false" diagonalDown="false"><left/><right/><top/><bottom/><diagonal/></border>' \
            b'<border diagonalUp="false" diagonalDown="false"><left style="thin"/><right style="thin"/><top style="thin"/><bottom style="thin"/><diagonal/></border>' \
            b'</borders>' \
            b'<cellStyleXfs count="20">' \
            b'<xf numFmtId="164" fontId="0" fillId="0" borderId="0" applyFont="true" applyBorder="true" applyAlignment="true" applyProtection="true">' \
            b'<alignment horizontal="general" vertical="bottom" textRotation="0" wrapText="false" indent="0" shrinkToFit="false"/>' \
            b'<protection locked="true" hidden="false"/>' \
            b'</xf>' \
            b'</cellStyleXfs>' \
            b'<cellXfs count="7">' \
            b'<xf numFmtId="164" fontId="0" fillId="0" borderId="0" xfId="0" applyFont="false" applyBorder="false" applyAlignment="false" applyProtection="false">' \
            b'<alignment horizontal="general" vertical="bottom" textRotation="0" wrapText="false" indent="0" shrinkToFit="false"/><protection locked="true" hidden="false"/>' \
            b'</xf>' \
            b'<xf numFmtId="164" fontId="0" fillId="0" borderId="1" xfId="0" applyFont="false" applyBorder="true" applyAlignment="true" applyProtection="false">' \
            b'<alignment horizontal="general" vertical="center" textRotation="0" wrapText="false" indent="0" shrinkToFit="false"/><protection locked="true" hidden="false"/>' \
            b'</xf>' \
            b'<xf numFmtId="164" fontId="0" fillId="0" borderId="1" xfId="0" applyFont="true" applyBorder="true" applyAlignment="true" applyProtection="false">' \
            b'<alignment horizontal="general" vertical="center" textRotation="0" wrapText="false" indent="0" shrinkToFit="false"/><protection locked="true" hidden="false"/>' \
            b'</xf>' \
            b'<xf numFmtId="164" fontId="4" fillId="0" borderId="1" xfId="0" applyFont="true" applyBorder="true" applyAlignment="true" applyProtection="false">' \
            b'<alignment horizontal="general" vertical="center" textRotation="0" wrapText="false" indent="0" shrinkToFit="false"/><protection locked="true" hidden="false"/>' \
            b'</xf>' \
            b'<xf numFmtId="164" fontId="0" fillId="0" borderId="1" xfId="0" applyFont="true" applyBorder="true" applyAlignment="true" applyProtection="false">' \
            b'<alignment horizontal="right" vertical="center" textRotation="0" wrapText="false" indent="0" shrinkToFit="false"/><protection locked="true" hidden="false"/>' \
            b'</xf>' \
            b'<xf numFmtId="165" fontId="5" fillId="0" borderId="1" xfId="0" applyFont="true" applyBorder="true" applyAlignment="true" applyProtection="false">' \
            b'<alignment horizontal="general" vertical="center" textRotation="0" wrapText="false" indent="0" shrinkToFit="false"/><protection locked="true" hidden="false"/>' \
            b'</xf>' \
            b'<xf numFmtId="165" fontId="0" fillId="0" borderId="1" xfId="0" applyFont="false" applyBorder="true" applyAlignment="true" applyProtection="false">' \
            b'<alignment horizontal="general" vertical="center" textRotation="0" wrapText="false" indent="0" shrinkToFit="false"/><protection locked="true" hidden="false"/>' \
            b'</xf>' \
            b'</cellXfs>' \
            b'<cellStyles count="6"><cellStyle name="Normal" xfId="0" builtinId="0"/><cellStyle name="Comma" xfId="15" builtinId="3"/>' \
            b'<cellStyle name="Comma [0]" xfId="16" builtinId="6"/><cellStyle name="Currency" xfId="17" builtinId="4"/>' \
            b'<cellStyle name="Currency [0]" xfId="18" builtinId="7"/><cellStyle name="Percent" xfId="19" builtinId="5"/>' \
            b'</cellStyles>' \
            b'</styleSheet>'

        self.assertEqual(input,
                         fix_fill.remove_empty_fill(input))

    def test_remove_empty_fill(self):
        input = b'<?xml version="1.0" encoding="utf-8"?>' \
            b'<x:styleSheet xmlns:x="http://schemas.openxmlformats.org/spreadsheetml/2006/main">' \
            b'<x:numFmts>' \
            b'<x:numFmt numFmtId="7732" formatCode="#,##0.##"/>' \
            b'<x:numFmt numFmtId="7733" formatCode="#,##0"/>' \
            b'</x:numFmts>' \
            b'<x:fonts>' \
            b'<x:font/>' \
            b'<x:font>' \
            b'<x:b/>' \
            b'</x:font>' \
            b'</x:fonts>' \
            b'<x:fills>' \
            b'<x:fill/>' \
            b'</x:fills>' \
            b'<x:borders>' \
            b'<x:border/>' \
            b'</x:borders>' \
            b'<x:cellXfs>' \
            b'<x:xf fontId="0" fillId="0" borderId="0"/>' \
            b'<x:xf fontId="1"/>' \
            b'<x:xf numFmtId="7732" fontId="0" applyNumberFormat="1"/>' \
            b'<x:xf numFmtId="7732" fontId="1" applyNumberFormat="1"/>' \
            b'<x:xf numFmtId="7733" fontId="0" applyNumberFormat="1"/>' \
            b'<x:xf numFmtId="7733" fontId="1" applyNumberFormat="1"/>' \
            b'</x:cellXfs>' \
            b'</x:styleSheet>'

        output = b'<?xml version="1.0" encoding="utf-8"?>' \
            b'<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">' \
            b'<numFmts>' \
            b'<numFmt numFmtId="7732" formatCode="#,##0.##"/>' \
            b'<numFmt numFmtId="7733" formatCode="#,##0"/>' \
            b'</numFmts>' \
            b'<fonts>' \
            b'<font/>' \
            b'<font>' \
            b'<b/>' \
            b'</font>' \
            b'</fonts>' \
            b'<fills/>' \
            b'<borders>' \
            b'<border/>' \
            b'</borders>' \
            b'<cellXfs>' \
            b'<xf fontId="0" fillId="0" borderId="0"/>' \
            b'<xf fontId="1"/>' \
            b'<xf numFmtId="7732" fontId="0" applyNumberFormat="1"/>' \
            b'<xf numFmtId="7732" fontId="1" applyNumberFormat="1"/>' \
            b'<xf numFmtId="7733" fontId="0" applyNumberFormat="1"/>' \
            b'<xf numFmtId="7733" fontId="1" applyNumberFormat="1"/>' \
            b'</cellXfs>' \
            b'</styleSheet>'

        self.assertEqual(output,
                         fix_fill.remove_empty_fill(input))
