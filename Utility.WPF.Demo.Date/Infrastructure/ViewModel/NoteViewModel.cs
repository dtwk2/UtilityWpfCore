using System;
using System.Globalization;
using System.Xml.Serialization;
using ReactiveUI;

namespace Utility.WPF.Demo.Date.Infrastructure.ViewModel
{

    public class NoteViewModel : ReactiveObject
    {
        private DateTime date;
        private string text = null;

        public Guid Id { get; set; }

        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }

        public DateTime Date
        {
            get => date;
            set => this.RaiseAndSetIfChanged(ref date, value);
        }

        public DateTime CreateTime
        {
            get;
            set;
        }
    }
}
