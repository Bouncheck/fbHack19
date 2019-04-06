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


from .models import Activity

def table_view(request):
    activities = Activity.objects.all()
    #
    # tablica 24(godziny) x 4 (przedzialy 15 minutowe)
    # w komorce (x,y) znajduje sie najczesciej uzywana aplikacja
    # w przedziale czasowym [x:y, x:(y+15)]. tab 0-indeksowana
    
    # Template dostaje tablice, numer wiersza (godzina) od ktorego zaczac,
    # numer wiersza po ktorym skonczyc
    return None
