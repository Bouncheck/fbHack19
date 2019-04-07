from django.db import models
from django.contrib.auth.models import  User


class Note(models.Model):
    image = models.ImageField(upload_to='screenshots/%Y/%m/%d/')
    timestamp = models.DateTimeField()
    note = models.TextField()
    user = models.ForeignKey(User, on_delete=models.CASCADE)
