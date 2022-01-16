using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using _ViewModel = Utility.ViewModel.ViewModel;

namespace UtilityWpf.Demo.Forms.ViewModel
{
    public class EditViewModel : _ViewModel, IEquatable<EditViewModel>
    {
        private Guid id;

        public EditViewModel() : base("Edit")
        {
            this.WhenAnyValue(a => a.Collection)
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

        public TitleViewModel TitleViewModel { get; private set; } = new TitleViewModel();

        public NotesViewModel DescriptionViewModel { get; private set; } = new NotesViewModel("Description");

        public ImagesViewModel ImagesViewModel { get; private set; } = new ImagesViewModel("Images");

        public NotesViewModel ShippingViewModel { get; private set; } = new NotesViewModel("Shipping");

        public NotesViewModel DisclaimersViewModel { get; private set; } = new NotesViewModel("Disclaimers");

        public MeasurementsViewModel MeasurementsViewModel { get; private set; } = new MeasurementsViewModel("Measurements", new[] { new MeasurementViewModel() });

        public IReadOnlyCollection<INotifyPropertyChanged> Collection
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
                        NotesViewModel vm when vm.Header == "Description" => new Action(() => DescriptionViewModel = vm),
                        ImagesViewModel vm when vm.Header == "Images" => () => ImagesViewModel = vm,
                        NotesViewModel vm when vm.Header == "Shipping" => () => ShippingViewModel = vm,
                        NotesViewModel vm when vm.Header == "Disclaimers" => () => DisclaimersViewModel = vm,
                        MeasurementsViewModel vm => () => MeasurementsViewModel = vm,
                        _ => throw new NotImplementedException(),
                    }).Invoke();
                }
                this.RaisePropertyChanged();
            }
        }

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