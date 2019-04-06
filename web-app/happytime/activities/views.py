import json
import datetime

from django.shortcuts import render
from django.utils.dateparse import parse_datetime
from django.contrib.auth.models import User

from . import models


def upload_view(request):
    data = request.body
    data = json.loads(data)

    username = data['Username']
    start_time = data['StartTime']
    data = data['Data']

    start_time = parse_datetime(start_time)

    user = User.object.filter(username=username)
    if not user:
        return None

    for item in data:
        app = models.Application.objects.filter(name=item['Key'])
        if not app:
            app = models.Application(name=item['Key'])
            app.save()

        end = start_time + datetime.timedelta(milliseconds=int(item['Value']))
        models.Activity(beginning=start_time, end=end, app=app, user=user).save()

    return None


def table_view(request):
    return None
