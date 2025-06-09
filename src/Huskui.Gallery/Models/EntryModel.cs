using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Huskui.Gallery.Models;

public class EntryModel : ObservableObject
{
    public required Type Page { get; set; }
    public required string Display { get; set; }
}