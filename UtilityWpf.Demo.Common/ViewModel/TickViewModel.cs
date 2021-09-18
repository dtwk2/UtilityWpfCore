using System;
using System.Reactive.Subjects;
using System.Windows.Input;
using ReactiveUI;

namespace UtilityWpf.Demo.Common.ViewModel
{
    public record struct TickChange(string Text, bool NewValue);

    public class TickViewModel : ReactiveObject, IObservable<TickChange>
    {
        readonly ReplaySubject<TickChange> tickChanges = new();
        private bool isChecked;

        public TickViewModel(bool isChecked, string text = "Tick")
        {
            IsChecked = isChecked;
            Text = text;
            Command = ReactiveCommand.Create(() =>
            {
                IsChecked = !IsChecked;
                tickChanges.OnNext(new TickChange(text, IsChecked));

            });
        }

        public bool IsChecked { get => isChecked; set => this.RaiseAndSetIfChanged(ref isChecked, value); }
        public string Text { get; }

        public ICommand Command { get; }

        public IDisposable Subscribe(IObserver<TickChange> observer)
        {
            return tickChanges.Subscribe(observer);
        }
    }
}
