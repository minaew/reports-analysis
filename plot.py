import matplotlib.pyplot as plt
import numpy as np
import matplotlib.dates as mdates
from dataclasses import dataclass
import subprocess


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
        start = parse(parts[1])
        end = parse(parts[2])
        items.append(item(start, end, label))
    return items


plot_coverage(get_coverage())
