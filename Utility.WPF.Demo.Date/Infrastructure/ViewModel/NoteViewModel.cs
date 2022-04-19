using System;
using ReactiveUI;
using UtilityInterface.Generic.Data;

namespace Utility.WPF.Demo.Date.Infrastructure.ViewModel
{

    public class NoteViewModel : ReactiveObject, IComparable<NoteViewModel>, IEquatable<NoteViewModel>, IId<Guid>
    {
        private DateTime date;
        private string? text;

        public Guid Id
        {
            get;
            set;
        }

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

        public bool Equals(NoteViewModel? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NoteViewModel)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id + " " + Date + " " + CreateTime + " " + Text;
        }
    }
}
