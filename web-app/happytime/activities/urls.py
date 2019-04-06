from django.urls import path

from . import views

app_name = 'activities'

urlpatterns = [
    path('table', views.table_view, name='activities_table_view'),
]
