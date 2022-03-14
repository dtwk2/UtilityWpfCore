using DateWork.Helpers;
using System;
using System.Linq;
using System.Windows;

namespace DateWork.Models
{
    public class DateModel : BaseViewModel
    {
        private int month = 0;
        private int year = 0;
        private DateTime[] days = null;

        public DateModel()
        {
            Current = DateTime.Now;
        }

        public void RefreshDays()
        {
            try
            {
                Days = DateHelper.VisibleDays(Month, Year).ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public int Year
        {
            get => year;
            set
            {
                year = value;
                RefreshDays();
                OnPropertyChanged();
            }
        }

        public int Month
        {
            get => month;
            set
            {
                month = value;
                RefreshDays();
                OnPropertyChanged();
            }
        }

        public DateTime Current
        {
            get
            {
                var now = DateTime.Now;
                if (now.Year == year && now.Month == month)
                {
                    return now.Date;
                }
                else
                {
                    return new DateTime(year, month, 1);
                }
            }
            set
            {
                year = value.Year;
                month = value.Month;
                RefreshDays();
            }
        }

        public DateTime[] Days
        {
            get
            {
                if (days == null)
                {
                    days = new DateTime[0];
                }
                return days;
            }
            set
            {
                days = value;
                OnPropertyChanged();
            }
        }
    }
}
