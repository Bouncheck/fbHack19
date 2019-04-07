import json
import base64

import dateutil.parser

from django.shortcuts import render
from django.views.decorators.csrf import csrf_exempt
from django.http import HttpResponse
from django.contrib.auth.models import User
from django.core.files.base import ContentFile

from . import models


@csrf_exempt
def upload_view(request):
    data = request.body
    data = json.loads(data)

    note = data['Note']
    username = data['User']
    timestamp = data['Time']
    image = data['Image']

    image = base64.b64decode(image)
    timestamp = dateutil.parser.parse(timestamp)

    user = User.objects.filter(username=username)
    if not user:
        return HttpResponse()
    user = user.first()

    note = models.Note(note=note, user=user, timestamp=timestamp)
    image = ContentFile(image)

    note.image.save(user.username + str(timestamp), image)
    note.save()

    return HttpResponse()
