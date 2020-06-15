using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

using UtilityHelper.NonGeneric;
using UtilityWpf;

namespace UtilityWpf.View
{
    public class CollectionEditor : ListBoxEx
    {
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(CollectionEditor));

        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons", typeof(IEnumerable<ViewModel.ButtonDefinition>), typeof(CollectionEditor));

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input", typeof(object), typeof(CollectionEditor), new PropertyMetadata(null, InputChanged));

        //public static readonly DependencyProperty ItemsPresenterProperty = DependencyProperty.Register("ItemsPresenter", typeof(object), typeof(CollectionEditor), new PropertyMetadata(null, InputChanged));

        public IEnumerable Buttons
        {
            get { return (IEnumerable)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public object Input
        {
            get { return (object)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        private static void InputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CollectionEditor).InputSubject.OnNext((DatabaseCommand)e.NewValue);
        }

        protected ISubject<DatabaseCommand> InputSubject = new Subject<DatabaseCommand>();

        static CollectionEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollectionEditor), new FrameworkPropertyMetadata(typeof(CollectionEditor)));
        }

        public CollectionEditor() : base()
        {
     

            //DeletedSubject.OnNext(new object());

            //NewItemS
            //Observable.FromEventPattern<EventHandler, EventArgs>(_ => this.Initialized += _, _ => this.Initialized -= _)
            //  .CombineLatest(NewItemsSubject, (a, b) => b)
            //     .CombineLatest(KeySubject, (item, key) => new { item, key })
            //  .Subscribe(_ => React(_.item, _.key));

            var obs = ItemsSourceSubject.Where(_ => _ != null)
                .Take(1)
                .Select(_ => (_.First()))
                 .Concat(SelectedItemSubject)
                 .DistinctUntilChanged();

            InputSubject.WithLatestFrom(obs, (input, item) => new { input, item })
              .Subscribe(_ =>
                   {
                       switch (_.input)
                       {
                           case (DatabaseCommand.Delete):
                               DeletedSubject.OnNext(_.item);
                               break;

                           case (DatabaseCommand.Update):

                               break;

                           case (DatabaseCommand.Clear):
                               ClearedSubject.OnNext(null);
                               break;
                       }
                   });

            Action<DatabaseCommand> av = (a) => this.Dispatcher.InvokeAsync(() => InputSubject.OnNext(a), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));

            var items = ButtonDefinitionHelper.GetCommandOutput<DatabaseCommand>(typeof(DatabaseCommand))
                ?.Select(meas =>
                new ViewModel.ButtonDefinition
                {
                    Command = new Command.RelayCommand(() => av(meas.Value())),
                    Content = meas.Key
                });

            if (items == null)
                Console.WriteLine("measurements-service equals null in collectionviewmodel");
            else
                this.Dispatcher.InvokeAsync(() => Buttons = items.ToList(), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        }

        private List<object> clxnitems = new List<object>();

        //protected virtual void React(object _b, string key)
        //{
        //    clxnitems.Add(_b);

        //    ItemsSourceSubject.OnNext(clxnitems);
        //}
    }
}