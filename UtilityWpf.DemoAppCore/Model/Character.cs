using System.ComponentModel;
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

    public class Character : INotifyPropertyChanged
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
                RaisePropertyChanged("First");
            }
        }

        public string Last
        {
            get { return _last; }
            set
            {
                _last = value;
                RaisePropertyChanged("Last");
            }
        }

        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                RaisePropertyChanged("Image");
            }
        }


        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                RaisePropertyChanged("Age");
            }
        }

        public Gender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                RaisePropertyChanged("Gender");
            }
        }

        public Point Location
        {
            get { return _location; }
            set
            {
                _location = value;
                RaisePropertyChanged("Location");
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                RaisePropertyChanged("Location");
            }
        }

        public override string ToString()
        {
            return _first + " " + _last;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged Members
    }
}