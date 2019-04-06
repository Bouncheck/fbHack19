import json
import datetime

from django.shortcuts import render
from django.utils.dateparse import parse_datetime
from django.contrib.auth.models import User
from django.views.decorators.csrf import csrf_exempt
from django.http import HttpResponse
from django.views.generic import TemplateView

from . import models
from .utils import ActivityTableRow


@csrf_exempt
def upload_view(request):
    data = request.body
    data = json.loads(data)

    username = data['Username']
    start_time = data['StartTime']
    data = data['Data']

    start_time = parse_datetime(start_time)

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

        activities = models.Activity.objects.\
            filter(beginning__range=(datetime.datetime.today(). datetime.datetime.today() + datetime.timedelta(days=1))).\
            filter(user=self.request.user).select_related('app').order_by('beginning')

        next_hour = datetime.datetime.today()
        print(next_hour)
        rows = [ActivityTableRow(next_hour)]
        next_hour = next_hour.replace(hour=1)

        for activity in activities:
            while next_hour <= activity.beginning:
                rows.append(ActivityTableRow(next_hour))
                next_hour = next_hour + datetime.timedelta(hours=1)

            rows[-1].cells.append(
                (activity.end - activity.beginning, activity.app.name))



        context['rows'] = rows
        return context
