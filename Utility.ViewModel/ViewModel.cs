using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UtilityHelperEx;
using UtilityWpf;

namespace Utility.ViewModel
{
    public class ViewModel : HeaderedNPC
    {
        private bool isReadOnly;

        public ViewModel(string header) : base(header)
        {
        }

        // ReSharper disable once IdentifierTypo
        protected void Intialise()
        {
            if (Collection is IEnumerable collection)
            {
                foreach (var item in collection.Cast<object>())
                {
                    if (item is INotifyPropertyChanged changed)
                    {
                        changed
                            .Changes()
                            .Subscribe(a =>
                            {
                                OnPropertyChanged(null);
                            });
                    }
                }

                Collection
                    .SelectNewItems<object>()
                    .Subscribe(obj =>
                {
                    if (obj is INotifyPropertyChanged changed)
                    {
                        changed.Changes().Subscribe(a =>
                        {
                            OnPropertyChanged(null);
                        });
                    }
                    OnPropertyChanged(null);
                });
            }
        }

        public bool IsReadOnly { get => isReadOnly; set => isReadOnly = value; }

        public virtual INotifyCollectionChanged Collection => throw new Exception("SDFSD 33");
    }
}