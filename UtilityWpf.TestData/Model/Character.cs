using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UtilityWpf.TestData
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
        private BitmapImage _image = null;
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

        public string UriString
        {
            set
            {
                _image = new BitmapImage(new Uri(value, UriKind.Absolute));
                RaisePropertyChanged();
            }
        }

        public BitmapImage Image { get => _image; set => _image = value; }

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

        public TimeSpan TimeSpan => TimeSpan.FromMinutes(Math.Pow(Age, 3));

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
            return First == other.First && Last == other.Last && Age == other.Age;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() * Last.GetHashCode() * Age.GetHashCode();
        }
    }
    public class Person : INotifyPropertyChanged
    {
        private string _first = string.Empty;
        private string _last = string.Empty;
        private int _age = 0;
        private Gender _gender = Gender.Unknown;


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


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged Members

    }
}
