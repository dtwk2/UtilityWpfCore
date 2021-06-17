using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.DemoApp.View
{
    /// <summary>
    /// Interaction logic for ListUserControl.xaml
    /// </summary>
    public partial class ListUserControl : UserControl
    {
        public ListUserControl()
        {
            InitializeComponent();

            var lvm = new ListViewModel();
            MainContentControl.Content = lvm;
        }
    }

    public class ListViewModel : IObservable<Model>
    {
        readonly ReplaySubject<Model> replaySubject = new ReplaySubject<Model>(1);
        private readonly ReadOnlyObservableCollection<ReactiveObject> collection;

        public ListViewModel()
        {

            var isCheckedViewModel = new IsCheckedViewModel();
            var issueViewModel = new IssueViewModel();

            isCheckedViewModel.CombineLatest(issueViewModel, (a, b) => new Model(a.Value, b))
                .Subscribe(replaySubject.OnNext);

            new[] { (ReactiveObject)isCheckedViewModel, (ReactiveObject)issueViewModel }
            .ToObservable()
                .ToObservableChangeSet()
                .Bind(out collection)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<ReactiveObject> Collection => collection;


        public IDisposable Subscribe(IObserver<Model> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }

    public readonly struct Model
    {

        public Model(bool? isChecked = default, UtilityEnum.LogLevel? status = default) : this()
        {
            IsChecked = isChecked;
            Status = status;
        }
        public bool? IsChecked { get; }

        public UtilityEnum.LogLevel? Status { get; }

    }

    public class IsCheckedViewModel : BooleanViewModel<IsChecked>
    {
        protected override IsChecked Create(bool value)
        {
            return new IsChecked(value);
        }
    }

    public class IssueViewModel : EnumViewModel<UtilityEnum.LogLevel>
    {
        public IssueViewModel()
        {
        }
    }

    public abstract class EnumViewModel<T> : ValueViewModel<T>, IObservable<T>, IEnumViewModel where T : Enum
    {
        private readonly ReplaySubject<T> replaySubject = new ReplaySubject<T>();

        public EnumViewModel()
        {
            this.WhenAnyValue(a => a.Value).Subscribe(replaySubject.OnNext);
        }
        public Type Type => typeof(T);

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }

    public interface IEnumViewModel
    {
        public Type Type { get; }
    }

    public abstract class BooleanViewModel<T> : ValueViewModel<bool>, IObservable<T>
    {
        private readonly ReplaySubject<T> replaySubject = new ReplaySubject<T>();

        public BooleanViewModel()
        {
            this.WhenAnyValue(a => a.Value).Select(Create).Subscribe(replaySubject.OnNext);
        }
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return replaySubject.Subscribe(observer);
        }

        protected abstract T Create(bool value);

    }

    public class DeleteViewModel : CommandViewModel<Deleted>
    {

    }


    public class CommandViewModel<T> : ReactiveObject where T : struct
    {
        public ReactiveCommand<Unit, T> Command { get; } = ReactiveCommand.Create(() => new T());
    }

    public abstract class ValueViewModel<T> : ReactiveObject, IName
    {
        private T value;

        public T Value
        {
            get => value;
            set => this.RaiseAndSetIfChanged(ref this.value, value);
        }
        public virtual string Name => this.GetType().Name.Replace("ViewModel", "");
    }

    public readonly struct Deleted
    {
    }

    public readonly struct IsChecked : IValue<bool>
    {
        public IsChecked(bool value)
        {
            Value = value;
        }

        public bool Value { get; }
    }

    public interface IValue<T>
    {
        public T Value { get; }
    }

    //public interface ILabel
    //{
    //    public string Name { get; }
    //}
}