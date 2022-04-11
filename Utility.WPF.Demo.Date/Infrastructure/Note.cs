using DateWork.Helpers;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace DateWork.Models
{
    public class Note : BaseViewModel
    {
        private bool isMonthDay = false;
        private string date = string.Empty;
        private string revisionDate = string.Empty;
        private string text = null;

        [XmlAttribute("Text")]
        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        public string InitialText { get; init; }

        [XmlAttribute("IsMonthDay")]
        public bool IsMonthDay
        {
            get => isMonthDay;
            set
            {
                isMonthDay = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute("Date")]
        public string Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute("RevisionDate")]
        public string RevisionDate
        {
            get => revisionDate;
            set
            {
                revisionDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateTime => SetTimeStamp(Date);
        public DateTime RevisionDateTime => SetTimeStamp(RevisionDate);

        public static string GetTimeStamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }

        public static DateTime SetTimeStamp(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;
            return DateTime.ParseExact(value, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        }
    }
}
