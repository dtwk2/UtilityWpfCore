//using System;
//using System.Windows.Threading;

//namespace UtilityWpf
//{
//    public class DispatcherX : IContext
//    {
//        private Dispatcher dispatcher;

//        public DispatcherX(Dispatcher dispatcher) => this.dispatcher = dispatcher;

//        public void BeginInvoke(Action action) => dispatcher.BeginInvoke(action);

//        public void Invoke(Action action) => dispatcher.Invoke(action);
//    }
//}