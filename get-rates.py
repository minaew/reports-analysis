import requests
from datetime import date
import os.path

year = 2022
content = ''
file_name = f'.{year}.cache'
if os.path.exists(file_name):
    with open(file_name) as file:
        for line in file.readlines():
            content += line
else :
    with open('.cookie.txt') as file:
        cookie = file.readline()
    x = requests.post('https://rate.am/en/armenian-dram-exchange-rates/central-bank-armenia',
                    data = f'__EVENTTARGET=ctl00%24Content%24dlYear&__VIEWSTATE=&__SCROLLPOSITIONX=0&__SCROLLPOSITIONY=0&__EVENTVALIDATION=%2FwEWSwL%2BraDpAgLC3NHXDgKTkL%2FQAgLpnNa3CwLFiYSZCwLE%2BqL2DQLOhILoBALbhILoBALSoPTdDgKWs5JfAuzMSgK2%2B%2Br3DQLf09GoDwKw%2B9r3DQKw%2B5r2DQKcs9JfApyAicMHAtmg7N0OAsXJuikCurb%2BNgKQs4ZfArv7gvYNAvHMXgL3%2F4OfAQLShObpBALShJboBALPyZ4pAubTiasPAvX%2F35wBAtmSo8IJAr77kvYNAo7W6YIPApuzkl8C5KDw3Q4CkvKb6QICwMnuNgKh8sv2AgKn8sf2AgLYhNLpBALVydY2Asb60vcNAqKz%2Fl8C8v%2BLnwEC7NOBqw8C%2BI2N6QcC8%2F%2FLnwECvMGrqQUCl9algg8CucvK0QkC58vzlAgC58uH8QMC58urqgsC58u%2FhwIC58vD4w0C%2Bty9yw4C%2BtzBpwYC%2BtyVzwQC%2Bty5qAwC%2BtzNhAcC%2BtzR4Q4C%2Btzl2gkC%2BtyJtwEC%2BtydkAgC%2BtyhzQMCkeWT1AQCkeWnsQwCkeWL2AoCkeWftQICkeWj7g0CkeW3ywQCkeXbpwwCkeXvgAcCkeXz%2FQ4CkeWH1gkC1YbuoAxuDx0R06%2FzAdIl%2FpIGvuBPqePSJA%3D%3D&ctl00%24Content%24cbInfo%24GoogleMapControl1%24hdgmLocations=&ctl00%24Content%24cbInfo%24GoogleMapControl1%24hdZoom=15&ctl00%24Content%24dlCurrency=USD&ctl00%24Content%24dlYear={year}',
                    headers={
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'Cookie': cookie,
                        })
    content = x.content.decode()
    with open(file_name, 'w') as file:
        file.write(content)

currenny_from = 'usd'
currency_to = 'amd'
rates = {}
chunks = content.split('<table class="cb"')[1].split('onmouseover="mouse_event_handler(event, 0, -1)">')
ch2 = chunks[1].split('</table>')
rows = ch2[0].replace('\n', '').split('<tr')
for str in range(2, 33):
    day = str - 1
    cells = rows[str].removesuffix('</tr>').split('<td>')
    for j in range(2, 14):
        month = j - 1
        cell = cells[j].removeprefix('>').removesuffix('</td>')
        if cell == '<span style="color:red">Sunday</span>' or cell == 'X':
            continue
        d = date(year, month, day)
        if day == 31 and month == 12:
            pass
        rates[d] = cell


with open(f'{year}-{currenny_from}-{currency_to}.csv', 'w') as file:
    file.write('date, from, to, value\n')
    s = dict(sorted(rates.items()))
    for d, v in s.items():
        file.write(f'{d}, {currenny_from}, {currency_to}, {v}\n')
