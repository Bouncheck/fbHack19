# import json
#
# from django.shortcuts import render
# from django.contrib.auth.models import User
#
# from . import models
#
#
# def upload_view(request):
#     data = request.body
#     data = json.loads(data)
#
#     username = data['Username']
#     start_time = data['StartTime']
#     data = data['Data']
#
#     for app in data:
#         user = User.object.filter(username=username)
#         activity = models.Activity.objects.filter(name=username)
#
#
#
#     return None

def table_view(request):
    return None
