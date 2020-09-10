using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace UtilityWpf.ViewModel
{
    public class FileDownloaderViewModel : ReactiveObject, IDisposable
    {
        private readonly WebClient client;
        private readonly ObservableAsPropertyHelper<(string, bool)> completed;
        private readonly ObservableAsPropertyHelper<int> progress;

        public int Progress => progress.Value;

        public (string, bool) Completed => completed.Value;

        public FileDownloaderViewModel(IObservable<Tuple<Uri, string>> files)
        {
            client = new WebClient();

            var ddd = Observable.FromEventPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(
            h => client.DownloadFileCompleted += h,
            h => client.DownloadFileCompleted -= h);

            completed = Observable.Create<(string, bool)>(observer => ddd.WithLatestFrom(files, (a, b) => new { a, b })
            .Subscribe(
                _ =>
                Task.Run(() => FileHelper.CheckFile(_.b.Item2))
                 .ToObservable()
                 .Subscribe(__ => observer.OnNext((_.b.Item1.ToString(), __)))))
           .ToProperty(this, s => s.Completed, deferSubscription: true);

            progress = Observable.FromEventPattern<DownloadProgressChangedEventHandler, DownloadProgressChangedEventArgs>(
                h => client.DownloadProgressChanged += h,
                h => client.DownloadProgressChanged -= h).Select(e => e.EventArgs.ProgressPercentage).Merge(ddd.Select(_ => 0))
                 .ToProperty(this, nameof(Progress), deferSubscription: true);

            files.Subscribe(_ =>
            {
                client.DownloadFileAsync(_.Item1, _.Item2);
            });
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }

    public static class FileHelper
    {
        public static bool CheckFile(string sink)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(sink);
            if (info.Length > 0)
            {
                return true;
                //Kaliko.Logger.Write(string.Format("File {0} downloaded to {1} @ {2}", source, sink, DateTime.Now), Kaliko.Logger.Severity.Info);
            }
            else
            {
                System.IO.File.Delete(sink);
                return false;
            }
        }
    }
}