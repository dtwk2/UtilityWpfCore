using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DateWork.Helpers;
using DynamicData;
using Utility.WPF.Controls.Date.Helper;

namespace Utility.WPF.Controls.Date.Model
{


    public abstract class DateModel : BaseViewModel
    {
        private int month = 0;
        private int year = 0;
        private int day;
        protected readonly ComparableModel<DateTime> comparableModel = new();

        public DateModel()
        {
            Current = DateTime.Now;
        }

        public abstract void RefreshDays();

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
                DateTime date;
                while (true)
                {
                    try
                    {
                        return new DateTime(year, month, day);
                    }
                    // some months are longer than others
                    catch (ArgumentOutOfRangeException)
                    {
                        day--;
                    }

                }
            }
            set
            {
                year = value.Year;
                month = value.Month;
                day = value.Day;
                RefreshDays();
            }
        }

        public ObservableCollection<DateTime> Days => comparableModel.Collection;
    }


    public class Date1Model : DateModel
    {

        public Date1Model()
        {
        }

        public override void RefreshDays()
        {
            try
            {
                var days = DateHelper.VisibleDays(Month, Year).ToArray();
                comparableModel.Replace(days);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }



    public class Date2Model : DateModel
    {
        private DateTime dateTime;

        public Date2Model()
        {
        }

        public override void RefreshDays()
        {
            if (dateTime == Current)
                return;


            dateTime = Current;

            //var newDays = Enumerable.Range(0, 21).Select(a => dateTime.AddDays(-10 + a)).ToArray();

            var days = dateTime.PlusMinusDateRange().ToArray();
            comparableModel.Replace(days);

        }


    }



    public class ComparableModel<T> where T : IComparable<T>
    {
        readonly List<T> removes = new();
        readonly List<T> adds = new();
        readonly ObservableCollection<T> collection = new();

        public async void Replace(ICollection<T> replacements)
        {
            removes.Clear();
            adds.Clear();
            removes.AddRange(collection.Except(replacements).OrderBy(a => a));
            adds.AddRange(replacements.Except(collection).OrderBy(a => a).ToList());

            while (removes.Count > 0)
            {
                collection.Remove(removes.First());
                removes.RemoveAt(0);
                await Task.Delay(10);
            }

            while (adds.Count > 0)
            {
                collection.InsertInOrder(adds.First());
                adds.RemoveAt(0);
                await Task.Delay(10);

            }
        }

        public ObservableCollection<T> Collection => collection;
    }


    public static class ListExt
    {
        public static void InsertInOrder<T>(this ObservableCollection<T> @this, T item) where T : IComparable<T>
        {

            if (@this.Count == 0)
            {
                @this.Add(item);
                return;
            }

            if (@this.Count > 0 && @this[^1].CompareTo(item) <= 0)
            {
                @this.Add(item);
                return;
            }

            if (@this[0].CompareTo(item) >= 0)
            {
                @this.Insert(0, item);
                return;
            }
            int index = @this.BinarySearch(item);
            if (index < 0)
                index = ~index;

            @this.Insert(index, item);
        }
    }
}
