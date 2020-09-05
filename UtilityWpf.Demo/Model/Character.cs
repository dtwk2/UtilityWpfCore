using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace UtilityWpf.DemoApp
{
    public enum Gender
    {
        Female,
        Male,
        Unknown
    }

    public class Character : INotifyPropertyChanged, IEquatable<Character>
    {
        private string _first = string.Empty;
        private string _last = string.Empty;
        private string _image = null;
        private int _age = 0;
        private Gender _gender = Gender.Unknown;
        private Point _location = new Point();
        private Color color;

        public string First
        {
            get { return _first; }
            set
            {
                _first = value;
                RaisePropertyChanged();
            }
        }

        public string Last
        {
            get { return _last; }
            set
            {
                _last = value;
                RaisePropertyChanged();
            }
        }

        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged();
            }
        }

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(TimeSpan));
            }
        }

        public Gender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                RaisePropertyChanged();
            }
        }

        public Point Location
        {
            get { return _location; }
            set
            {
                _location = value;
                RaisePropertyChanged();
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                RaisePropertyChanged();
            }
        }

        public TimeSpan TimeSpan => System.TimeSpan.FromMinutes(Math.Pow(Age, 3));

        public override string ToString()
        {
            return _first + " " + _last;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged Members

        public bool Equals([AllowNull] Character other)
        {
            return this.First == other.First && this.Last == other.Last && Age == other.Age;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.First.GetHashCode() * this.Last.GetHashCode() * Age.GetHashCode();
        }
    }
}