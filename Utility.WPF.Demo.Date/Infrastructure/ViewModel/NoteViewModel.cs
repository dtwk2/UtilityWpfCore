using System;
using ReactiveUI;
using Utility.Common.Model;

namespace Utility.WPF.Demo.Date.Infrastructure.ViewModel;

public class NoteViewModel : GuidModel<NoteViewModel>, IComparable<NoteViewModel>
{
    private DateTime date;
    private string? text;

    public string? Text
    {
        get => text;
        set => this.RaiseAndSetIfChanged(ref text, value);
    }

    public DateTime Date
    {
        get => date;
        set => this.RaiseAndSetIfChanged(ref date, value);
    }

    public DateTime CreateTime { get; set; }

    public int CompareTo(NoteViewModel? other)
    {
        return this.CreateTime.CompareTo(other.CreateTime);
    }

}