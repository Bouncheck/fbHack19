from django.urls import path
from django.contrib.auth.decorators import login_required

from . import views

app_name = 'activities'

urlpatterns = [
    path('table/', login_required(views.TableView.as_view()), name='activities_table_view'),
    path('table/<str:day>', login_required(views.TableView.as_view()), name='activities_table_view'),
    path('upload', views.upload_view, name='activities_json_upload')
]