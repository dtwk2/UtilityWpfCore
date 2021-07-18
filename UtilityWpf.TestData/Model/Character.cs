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

    public class CharactersViewModel
    {

        public static Character[] Characters { get; } = new Character[]
             {
                new Character{First = "Bart", Last="Simpson" ,Age=10,
          Gender=Gender.Male ,Image=ToBitmapImage(Resource1.bart), Location=new Point(25,150)},

                new Character{First = "Homer", Last="Simpson" ,Age=40,
          Gender=Gender.Male ,Image = ToBitmapImage(Resource1.homer), Location=new Point(25,150)},
                 //                new Character
                 //                {
                 //                    First = "Bart",
                 //                    Last = "Simpson",
                 //                    Age = 10,
                 //                    Gender = Gender.Male,
                 //                    ImageSource = "/UtilityWpf.TestData;component/images/bart.png",
                 //                    Location = new Point(25, 150)
                 //                },
                 //new Character
                 //{
                 //    First = "Bart",
                 //    Last = "Simpson",
                 //    Age = 10,
                 //    Gender = Gender.Male,
                 //    ImageSource = "/UtilityWpf.TestData;component/images/bart.png",
                 //    Location = new Point(25, 150)
                 //},
             };


        /// <summary>
        /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
        /// https://stackoverflow.com/questions/26260654/wpf-converting-bitmap-to-imagesource
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        static BitmapImage ToBitmapImage(System.Drawing.Bitmap src)
        {
            MemoryStream ms = new MemoryStream();

            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
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
            //get { return _image; }
            set
            {
                _image = new BitmapImage(new Uri(value, UriKind.Absolute));
                RaisePropertyChanged();
            }
        }

        public BitmapImage Image { get => _image; set => _image = value; }
        //{
        //    get
        //    {
        //        //string str = Environment.CurrentDirectory;
        //        //string imagePath =
        //        //    System.IO.Directory.GetParent(str).Parent.Parent.FullName + "/" + ImageSource;

        //        return _image;
        //    }
        //    set=>
        //        {
        //        _image;
        //    }
        //}

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
}