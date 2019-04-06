from django.db import models


# Create your models here.
class TimeFrame(models.Model):
    beginning = models.DateTimeField()
    end = models.DateTimeField()


class Activity(models.Model):
    name = models.CharField(max_length=300)
