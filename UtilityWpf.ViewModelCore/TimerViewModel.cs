//using Reactive.Bindings;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace UtilityWpf.ViewModel
//{
//    public class TimerViewModel<T>
//    {
//        private TimerViewModel()
//        {
//            Time = TimerSingleton.Instance.Time.ToReadOnlyReactiveProperty();
//        }

//        public ReadOnlyReactiveProperty<DateTime> Time { get; }

//        public T Object { get; set; }
//    }

//}