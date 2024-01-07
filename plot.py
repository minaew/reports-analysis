import matplotlib.pyplot as plt
import numpy as np
import matplotlib.dates as mdates
from dataclasses import dataclass
import subprocess
import json


@dataclass
class item:
    start: np.datetime64
    end: np.datetime64
    label: str


def create_month_start(year: int, month: int) -> np.datetime64:
    month_str = str(month).rjust(2, '0')
    return np.datetime64(f'{year}-{month_str}-01')


def get_month_start(datetime: np.datetime64) -> np.datetime64:
    y = datetime.astype('datetime64[Y]').astype(int) + 1970
    m = datetime.astype('datetime64[M]').astype(int) % 12 + 1
    return create_month_start(y, m)


def get_month_incremented(datetime: np.datetime64) -> np.datetime64:
    y = datetime.astype('datetime64[Y]').astype(int) + 1970
    m = datetime.astype('datetime64[M]').astype(int) % 12 + 1
    if m == 12:
        return create_month_start(y + 1, 1)
    else:
        return create_month_start(y, m + 1)


def plot_coverage(items):
    _, ax = plt.subplots()
    plt.grid()

    ax.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%b'))
    for label in ax.get_xticklabels(which='major'):
        label.set(rotation=30, horizontalalignment='right')

    y = 0
    min_start = items[0].start  # fixme
    max_end = items[0].end  # fixme
    for item in items:
        if item.start < min_start:
            min_start = item.start
        if item.end > max_end:
            max_end = item.end

        y = y + 1
        ax.plot([np.datetime64(item.start), np.datetime64(item.end)], [y, y], 'o-')
        ax.annotate(item.label, xy=(np.datetime64(item.start), y + 0.025))

    # ticks
    current = get_month_start(min_start)
    ticks = [current]
    while current < max_end:
        current = get_month_incremented(current)
        ticks.append(current)
    ax.set_xticks(ticks)

    plt.show()


def parse(line) -> np.datetime64:
    tokens = line.split('.')
    return np.datetime64(f'{tokens[2]}-{tokens[1]}-{tokens[0]}')


def get_coverage():
    process = subprocess.run(['dotnet', 'run', '--project', '.\\CLI\\CLI.csproj', 'coverage', '--movements', 'D:\\finances\\movements\\'],
                             capture_output=True, text=True)
    items = []
    for line in process.stdout.split('\n'):
        if len(line) == 0:
            continue
        parts = line.split(',')
        label = parts[0]
        range = parts[1]
        range_part = range.split('-')
        start = parse(range_part[0])
        end = parse(range_part[1])
        items.append(item(start, end, label))
    return items


def get_expences():
    process = subprocess.run(['dotnet', 'run', '--project', '.\\CLI\\CLI.csproj', 'sum', '--operations-path', 'D:\\finances\\log.json'],
                             capture_output=True, text=True)
    expences = []
    for line in process.stdout.split('\n'):
        if len(line) == 0:
            continue
        parts = line.split(':')
        month = parts[0].split('.')[0]
        year = parts[0].split('.')[1]
        m = np.datetime64(f'{year}-{month}')
        sums = {}
        for c in parts[1].split(','):
            category = c.split('=')[0]
            sum = float(c.split('=')[1])
            sums[category] = sum
        expences.append((m, sums))
    return expences


def plot_expences(expences):
    for expence in expences:
        categories = []
        sums = []
        for category in expence[1]:
            categories.append(category)
            sum = expence[1][category]
            sums.append(-sum)
        _, ax = plt.subplots()
        ax.pie(sums, labels=categories, autopct='%1.1f%%')
        plt.title(expence[0])
        plt.show()


@dataclass
class expences_table:
    categories: tuple
    periods: tuple
    data: list


def get_expences_table_test() -> expences_table:
    # external call
    data = [[1, 2, 3],
            [3, 4, 5]]
    periods = ['January', 'February']
    categories = ['food', 'transport', 'housing']
    return expences_table(categories, periods, data)


def get_expences_table() -> expences_table:
    process = subprocess.run(['dotnet', 'run', '--project', '.\\CLI\\CLI.csproj', 'sum', '--operations-path', 'D:\\finances\\log.json'],
                             capture_output=True, text=True)

    periods = []
    categories = ['housing', '?']
    data = []
    for period in json.loads(process.stdout):
        period_id = period['ID']
        periods.append(period_id)
        present_categories = {}
        for c in categories:
            present_categories[c] = 0
        for category, value in period['Categories'].items():
            if category in categories:
                key = category
            else:
                key = '?'
            present_categories[key] = present_categories[key] + value
        period_data = []
        for c in categories:
            period_data.append(-present_categories[c])
        data.append(period_data)

    return expences_table(categories, periods, data)


def plot_expences_bars(expences_table: expences_table):

    categories = expences_table.categories
    periods = expences_table.periods
    data = expences_table.data

    _, ax = plt.subplots()
    y_offset = np.zeros(len(periods))
    index = np.arange(len(periods))
    for cat in range(len(categories)):
        column = []
        for row in range(len(data)):
            column.append(data[row][cat])

        rects = ax.bar(index, column, bottom=y_offset, label=categories[cat])
        ax.bar_label(rects, fmt='%d')
        y_offset = y_offset + column

    ax.legend(ncol=len(categories))
    ax.set_xticks(index, periods)
    plt.show()


# plot_coverage(get_coverage())
# plot_expences(get_expences())
# plot_expences_bars(get_expences_table_test())
plot_expences_bars(get_expences_table())
