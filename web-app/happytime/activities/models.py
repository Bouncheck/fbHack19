from django.db import models
from django.contrib.auth.models import User


class Application(models.Model):
    name = models.CharField(max_length=300)


class Activity(models.Model):
    beginning = models.DateTimeField()
    end = models.DateTimeField()
    app = models.ForeignKey(Application, on_delete=models.CASCADE)
    user = models.ForeignKey(User, on_delete=models.CASCADE)
