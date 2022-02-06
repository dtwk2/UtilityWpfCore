using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using MintPlayer.ObservableCollection;
using _ViewModel = Utility.ViewModel.ViewModel;

namespace UtilityWpf.Demo.Forms.ViewModel
{
    public interface ICollection<T>
    {
        IReadOnlyCollection<T> Collection { get; set; }
    }

    public class EditViewModel : _ViewModel, ICollection<INotifyPropertyChanged>, IEquatable<EditViewModel>
    {
        private Guid id;

        public EditViewModel() : base("Edit")
        {
            (this as ICollection<INotifyPropertyChanged>).WhenAnyValue(a => a.Collection)
                .Select(a => a.ToObservable())
                .Switch()
                .SelectMany(a => a.Changes())
                .Subscribe(a => OnPropertyChanged());
        }

        private void TitleViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public override bool Equals(object? obj)
        {
            return obj is EditViewModel model && TitleViewModel.Title == model.TitleViewModel.Title;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TitleViewModel);
        }

        public bool Equals(EditViewModel? other)
        {
            return TitleViewModel.Title == other?.TitleViewModel.Title;
        }

        public Guid Id { get => id; set => this.RaiseAndSetIfChanged(ref id, value); }

        public TitleViewModel TitleViewModel { get; private set; } = new ();

        public NotesViewModel DescriptionViewModel { get; private set; } = new ("Description");

        public ImagesViewModel ImagesViewModel { get; private set; } = new ("Images");

        public NotesViewModel ShippingViewModel { get; private set; } = new ("Shipping");

        public NotesViewModel DisclaimersViewModel { get; private set; } = new ("Disclaimers");

        public MeasurementsViewModel MeasurementsViewModel { get; private set; } = new ("Measurements", new[] { new MeasurementViewModel() });

        IReadOnlyCollection<INotifyPropertyChanged> ICollection<INotifyPropertyChanged>.Collection
        {
            get
                => new INotifyPropertyChanged[]
                {
                    TitleViewModel, DescriptionViewModel, ImagesViewModel, MeasurementsViewModel, ShippingViewModel, DisclaimersViewModel
                };
            set
            {
                foreach (var item in value)
                {
                    (item switch
                    {
                        TitleViewModel vm => () => TitleViewModel = vm,
                        NotesViewModel { Header: "Description" } vm => new Action(() => DescriptionViewModel = vm),
                        ImagesViewModel { Header: "Images" } vm => () => ImagesViewModel = vm,
                        NotesViewModel { Header: "Shipping" } vm => () => ShippingViewModel = vm,
                        NotesViewModel { Header: "Disclaimers" } vm => () => DisclaimersViewModel = vm,
                        MeasurementsViewModel vm => () => MeasurementsViewModel = vm,
                        _ => throw new NotImplementedException(),
                    }).Invoke();
                }
                this.RaisePropertyChanged();
            }
        }

        public override INotifyCollectionChanged Collection => new ObservableCollection<INotifyPropertyChanged>((this as ICollection<INotifyPropertyChanged>).Collection);

        public static bool operator ==(EditViewModel? left, EditViewModel? right)
        {
            return EqualityComparer<EditViewModel>.Default.Equals(left, right);
        }

        public static bool operator !=(EditViewModel? left, EditViewModel? right)
        {
            return !(left == right);
        }


    }
}