import json
import datetime

import dateutil.parser

from django.shortcuts import render
from django.contrib.auth.models import User
from django.views.decorators.csrf import csrf_exempt
from django.http import HttpResponse
from django.views.generic import TemplateView

from . import models
from notes.models import Note
from .utils import ActivityTableRow, get_today_midnight


@csrf_exempt
def upload_view(request):
    data = request.body
    data = json.loads(data)

    username = data['Username']
    start_time = data['StartTime']
    data = data['Data']

    start_time = dateutil.parser.parse(start_time)

    user = User.objects.filter(username=username)
    if not user:
        return HttpResponse()
    user = user.first()

    for item in data:
        app = models.Application.objects.filter(name=item['Key'])
        if not app:
            app = models.Application(name=item['Key'])
            app.save()
        else:
            app = app.first()

        end = start_time + datetime.timedelta(milliseconds=int(item['Value']))
        models.Activity(beginning=start_time, end=end, app=app, user=user).save()

    return HttpResponse()


class TableView(TemplateView):
    template_name = 'activities/table.html'

    def get_context_data(self, **kwargs):
        context = super().get_context_data(**kwargs)

        today = get_today_midnight()
        activities = models.Activity.objects.\
            filter(beginning__range=(today, today + datetime.timedelta(days=1))).\
            filter(user=self.request.user).select_related('app').order_by('beginning')

        next_hour = get_today_midnight()
        rows = [ActivityTableRow(next_hour)]
        next_hour += datetime.timedelta(hours=1)

        for activity in activities:
            while next_hour <= activity.beginning:
                rows.append(ActivityTableRow(next_hour))
                next_hour = next_hour + datetime.timedelta(hours=1)

            rows[-1].cells.append(activity)

        first_hour = get_today_midnight()
        for row in rows:
            first_hour = row.hour
            if row.cells:
                break

        rows = list(filter(lambda x: x.hour >= first_hour, rows))

        for row in rows:
            row.group(datetime.timedelta(minutes=15))

        context['rows'] = rows

        today = get_today_midnight()
        notes = Note.objects.filter(timestamp__range=(today, today + datetime.timedelta(days=1))).\
            filter(user=self.request.user)

        snapshots = []
        for note in notes:
            snapshots.append(note)

        context['snapshots'] = snapshots
        return context
