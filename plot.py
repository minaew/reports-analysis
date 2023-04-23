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


def plot_coverage(items):
    _, ax = plt.subplots()

    ax.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%b'))
    for label in ax.get_xticklabels(which='major'):
        label.set(rotation=30, horizontalalignment='right')

    y = 0
    for item in items:
        y = y + 1
        ax.plot([np.datetime64(item.start), np.datetime64(item.end)], [y, y], 'o-')
        ax.annotate(item.label, xy=(np.datetime64(item.start), y + 0.025))

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
