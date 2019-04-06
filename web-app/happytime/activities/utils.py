import datetime
import pytz

from django.utils import timezone


EMPTY_APP_NAME = "None"


class ActivityTableRow:
    def __init__(self, hour):
        self.hour = hour
        self.cells = []

    def group(self, delta):
        next = self.hour + delta
        cells = []
        maxi = datetime.timedelta()
        name = EMPTY_APP_NAME

        for cell in self.cells:
            while next <= cell.beginning:
                cells.append(name)
                next += delta
                maxi = datetime.timedelta()
                name = EMPTY_APP_NAME

            if maxi < cell.end - cell.beginning:
                maxi = cell.end - cell.beginning
                name = cell.app.name

        cells.append(name)
        name = EMPTY_APP_NAME
        next += delta

        while next <= self.hour + datetime.timedelta(hours=1):
            cells.append(name)
            next += delta

        cells = list(map(lambda x: (x, cells.count(x)), cells))
        self.cells = sorted(set(cells), key=lambda x: cells.index(x))


def get_today_midnight():
    midnight = timezone.now()
    return midnight.replace(hour=0, minute=0, second=0, microsecond=0, tzinfo=pytz.timezone('Etc/GMT-2'))
