using DateWork.Helpers;
using System;
using System.IO;
using System.Xml.Serialization;

namespace DateWork.Models
{
    [XmlRoot("Notes")]
    public class Notes : BaseViewModel
    {
        //private static readonly string _Path = AppDomain.CurrentDomain.BaseDirectory + "Notes.xml";
        private static string _Path
        {
            get
            {
#if DEBUG
                {
                    var xx = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName + "\\Notes.xml";
                    return xx;
                }

#endif
#if RELEASE
                {
                var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Utility Journal";
                var directory = Directory.CreateDirectory(directoryPath);
                var path = Path.Combine(directoryPath, "Notes.xml");
                return path;
                }
#endif
            }
        }
        private CollectionBase<Note>? items = default;

        public static Notes Current { get; set; } = LoadXml() ?? throw new Exception("d66sfsd222 fdef");

        [XmlElement("Note", typeof(Note))]
        public CollectionBase<Note> Items
        {
            get => items ??= new CollectionBase<Note>();
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }

        public static Notes? LoadXml()
        {
            if (File.Exists(_Path) == false)
            {
                File.Create(_Path).Dispose();
                new Notes().ObjectToXml(_Path);
            }
            return _Path.XmlToObject<Notes>();
        }

        public void Save()
        {
            this.ObjectToXml(_Path);
            Current = LoadXml();
        }
    }

}
