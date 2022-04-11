using System;
using System.IO;
using System.Xml.Serialization;

namespace DateWork.Helpers
{
    public static class XmlHelper
    {
        public static T XmlToObject<T>(this string fileName)
        {
            object? obj = null;
            var xs = new XmlSerializer(typeof(T));
            using (var stream = File.OpenRead(fileName))
            {
                obj = xs.Deserialize(stream);
            }
            return (T?)obj ?? throw new Exception("sdf3 sdfgdfgdfgdfgdf");
        }

        public static void ObjectToXml(this object xobj, string filename)
        {
            int index = filename.LastIndexOf('\\');
            if (index > 0)
            {
                string path = filename.Substring(0, index);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
            }
            XmlSerializer xs = new XmlSerializer(xobj.GetType());
            using (var stream = System.IO.File.Open(filename, FileMode.Create, FileAccess.Write))
            {
                xs.Serialize(stream, xobj);
            }
        }

        //public static void ObjectToXml(this object xobj, string filename)
        //{
        //    XmlSerialisation.WriteFormattedXml(filename, XmlSerialisation.Serialize(xobj));
        //}

    }
}