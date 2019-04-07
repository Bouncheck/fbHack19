import datetime

from django import forms


class DatePickerForm(forms.Form):
    date = forms.DateField(initial=datetime.date.today())
