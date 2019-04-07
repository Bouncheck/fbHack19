from django.urls import path

from . import views

app_name = 'notes'

urlpatterns = [
    path('upload', views.upload_view, name='notes_json_upload')
]
