using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Huskui.Gallery.ViewModels;

public class ExceptionViewModel(Exception exception) : ObservableObject
{
    public string Message => exception.Message;
    public string StackTrace => exception.StackTrace ?? "No stack trace available";
}