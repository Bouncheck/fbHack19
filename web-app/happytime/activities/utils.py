import datetime
import pytz

from django.utils import timezone

from notes.models import Note


EMPTY_APP_NAME = "None"
COLORS = ["#16a085", "#27ae60", "#2980b9", "#8e44ad", "#f39c12", "#e67e22", "#e74c3c"]
EMPTY_COLOR = '#ffffff'


def name_to_color(name):
    if EMPTY_APP_NAME == name:
        return EMPTY_COLOR

    h = hash(name)
    return COLORS[h % len(COLORS)]


class ActivityTableRow:
    def __init__(self, hour):
        self.hour = hour
        self.cells = []

    def group(self, delta, user):
        next = self.hour + delta
        cells = []
        maxi = datetime.timedelta()
        name = EMPTY_APP_NAME

        notes = Note.objects.filter(timestamp__range=(self.hour, self.hour + datetime.timedelta(hours=1))).\
            filter(user=user)

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

        cells = list(map(lambda x: (x, cells.count(x), name_to_color(x)), cells))
        self.cells = sorted(set(cells), key=lambda x: cells.index(x))

        previous = self.hour
        for id, cell in enumerate(self.cells):
            if any(previous <= note.timestamp and note.timestamp <= previous + delta * cell[1] for note in notes):
                self.cells[id] = (cell[0] + ' 😊', cell[1], cell[2])

            previous += delta * cell[1]


def get_today_midnight():
    midnight = timezone.localtime()
    return midnight.replace(hour=0, minute=0, second=0, microsecond=0, tzinfo=pytz.timezone('Etc/GMT-2'))
