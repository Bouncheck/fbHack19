import datetime
import pytz

from django.utils import timezone


class ActivityTableRow:
    def __init__(self, hour):
        self.hour = hour
        self.cells = []

    def group(self, delta):
        next = self.hour + delta
        cells = []
        maxi = datetime.timedelta()
        name = ''

        for cell in self.cells:
            while next <= cell.beginning:
                cells.append(name)
                next += delta
                maxi = datetime.timedelta()
                name = ''

            if maxi < cell.end - cell.beginning:
                maxi = cell.end - cell.beginning
                name = cell.app.name

        cells.append(name)
        name = ''
        next += delta

        while next <= self.hour + datetime.timedelta(hours=1):
            cells.append(name)
            next += delta

        self.cells = cells


def get_today_midnight():
    midnight = timezone.now()
    return midnight.replace(hour=0, minute=0, second=0, microsecond=0, tzinfo=pytz.timezone('Etc/GMT-2'))
