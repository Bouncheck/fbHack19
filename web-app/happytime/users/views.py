from django.shortcuts import render, redirect
from django.views.generic import CreateView

from .forms import UserRegisterForm


class RegisterView(CreateView):
    form_class = UserRegisterForm
    template_name = "users/register.html"

    def form_valid(self, form):
        form.save()
        return redirect("login")

    def get_context_data(self, **kwargs):
        context = super().get_context_data(**kwargs)
        context["title"] = "Sign up"
        return context