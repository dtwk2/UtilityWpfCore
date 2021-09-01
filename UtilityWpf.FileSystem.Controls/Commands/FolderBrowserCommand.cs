﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Forms;
using ReactiveUI;

namespace UtilityWpf.Controls.FileSystem
{
    public class FolderBrowserCommand : BrowserCommand
    {  
        public FolderBrowserCommand() : base(Select())
        {
        }

        private static Func<IObservable<string>> Select()
        {
            var obs =
                Observable.Create<string>(obs =>
                {
                    return Observable.Return(
                   OpenDialog(string.Empty, string.Empty))
                .Where(output => output.result ?? false)
                .ObserveOnDispatcher()
                .Select(output => output.path)
                .WhereNotNull().Subscribe(obs);
                });
            return new Func<IObservable<string>>(() => obs);

        }

        //#region properties
        //public bool IsFolderPicker
        //{
        //    get;
        //    set;
        //}
        //#endregion properties

        protected static (bool? result, string path) OpenDialog(string filter, string extension)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                return result == DialogResult.OK ? (true, dialog.SelectedPath) : (false, null);
            }
        }
    }
}