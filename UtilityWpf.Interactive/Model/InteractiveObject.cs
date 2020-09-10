# nullable enable

using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;
using UtilityWpf.Command;
using UtilityWpf.Property;

namespace UtilityWpf.Model
{
    public class InteractiveObject : InteractiveObject<object>, UtilityInterface.NonGeneric.IObject
    {
        public InteractiveObject(object @object,
            IObservable<bool?>? expanded = null,
            IObservable<bool?>? selected = null,
            IObservable<bool?>? @checked = null,
            IObservable<bool?>? visible = null,
            IObservable<bool?>? enabled = null,
            IObservable<bool>? checkable = null,
            IObservable<bool>? removable = null,
            IObservable<bool>? @readonly = null,
            IObservable<bool>? doubleClickToCheck = null,
            IObservable<string?>? groupKeyName = null,
                    ReadOnlyObservableCollection<InteractiveObject<object>>? collection = null,
            IObservable<IConvertible?>? id = null)
            : base(@object, expanded, selected, @checked, visible, enabled, checkable, removable, @readonly, doubleClickToCheck, groupKeyName, collection, id)
        {
        }

        public InteractiveObject(object @object, bool? expanded = null, bool? selected = null, bool? @checked = null, bool? visible = true, bool? enabled = null, bool checkable = true, bool removable = true, bool @readonly = false, bool doubleClickToCheck = false, string? groupKeyName = null, ReadOnlyObservableCollection<InteractiveObject<object>>? collection = null, IConvertible? id = default) :
            this(@object,
                Observable.Return(expanded),
                Observable.Return(selected),
                Observable.Return(@checked),
                Observable.Return(visible),
                Observable.Return(enabled),
                Observable.Return(checkable),
                Observable.Return(removable),
                Observable.Return(@readonly),
                Observable.Return(doubleClickToCheck),
                Observable.Return(groupKeyName),
                collection,
                Observable.Return(id))
        {
        }
    }

    // Selectable / Hideable / Disableable
    public class InteractiveObject<T> : NPC, IObject<T>//,IDisposable
    {
        // set needs to be exposed for LiteDbToWork

        private bool? _isExpanded;
        private bool? _isSelected;
        private bool? _isChecked;

        //private bool? _isCheckable;
        //private bool? _isReadonly;
        private bool? _isVisible;

        private bool? _isEnabled;
        private bool _isDoubleClicked;
        private bool _isReadOnly;
        private bool isRemovable;
        private bool isCheckable;
        private object? id;
        private Subject<Unit> deletions = new Subject<Unit>();
        private bool doubleClickToCheck;
        private object? groupKey;

        public InteractiveObject(T @object, bool? expanded = null, bool? selected = null, bool? @checked = null, bool? visible = true, bool? enabled = true, bool checkable = true, bool removable = true, bool @readonly = false, bool doubleClickToCheck = false, string? groupKeyName = null, ReadOnlyObservableCollection<InteractiveObject<T>>? collection = null, object? id = null) :
    this(@object,
        Observable.Return(expanded),
        Observable.Return(selected),
        Observable.Return(@checked),
        Observable.Return(visible),
        Observable.Return(enabled),
        Observable.Return(checkable),
        Observable.Return(removable),
        Observable.Return(@readonly),
        Observable.Return(doubleClickToCheck),
        Observable.Return(groupKeyName),
         collection,
       Observable.Return(id))
        {
        }

        public InteractiveObject(T @object,
        IObservable<bool?>? expanded = null,
        IObservable<bool?>? selected = null,
        IObservable<bool?>? @checked = null,
        IObservable<bool?>? visible = null,
        IObservable<bool?>? enabled = null,
        IObservable<bool>? checkable = null,
        IObservable<bool>? removable = null,
        IObservable<bool>? @readonly = null,
        IObservable<bool>? doubleClickToCheck = null,
        IObservable<string?>? groupKeyName = null,
        ReadOnlyObservableCollection<InteractiveObject<T>>? collection = null,
        IObservable<object?>? id = null)
        {
            Object = @object;
            Collection = collection;
            expanded?.Subscribe(a => IsExpanded = a);
            selected?.Subscribe(a => IsSelected = a);
            @checked?.Subscribe(a => IsChecked = a);
            (visible ?? Observable.Return((bool?)true)).Subscribe(a => IsVisible = a);
            (enabled ?? Observable.Return((bool?)true)).Subscribe(a => IsEnabled = a);
            checkable?.Subscribe(a =>
            IsCheckable = a);
            removable?.Subscribe(a => IsRemovable = a);
            @readonly?.Subscribe(a => IsReadOnly = a);
            doubleClickToCheck?.Subscribe(a => DoubleClickToCheck = a);
            groupKeyName?.Where(a => a != null).Subscribe(a =>
                {
                    GroupKey = UtilityHelper.PropertyHelper.GetPropertyValue<object>(@object, a);
                });
            id?.Where(a => a != null).Subscribe(a => Id = a);

            DeleteCommand = new RelayCommand(() =>
            {
                deletions.OnNext(default);
            });

            DoubleClickCommand = new RelayCommand(() =>
            {
                IsDoubleClicked = true;
            });
        }

        // properties don't work as reactive properties
        public IObservable<Unit> Deletions => deletions;

        public RelayCommand DeleteCommand { get; }

        public RelayCommand DoubleClickCommand { get; }

        public T Object { get; set; }

        public ReadOnlyObservableCollection<InteractiveObject<T>>? Collection { get; }

        public object? Id
        {
            get => id;
            set
            {
                //if (value != _isExpanded)
                //{
                id = value;
                OnPropertyChanged();
                //}
            }
        }

        public bool? IsExpanded
        {
            get => _isExpanded;
            set
            {
                //if (value != _isExpanded)
                //{
                _isExpanded = value;
                OnPropertyChanged();
                //}
            }
        }

        public virtual bool? IsSelected
        {
            get => _isSelected;
            set
            {
                //if (value != _isSelected)
                //{
                _isSelected = value;
                OnPropertyChanged();
                //}
            }
        }

        public virtual bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                //if (value != _isSelected)
                //{
                _isChecked = value;
                OnPropertyChanged();
                //}
            }
        }

        public virtual bool? IsVisible
        {
            get { return _isVisible; }
            set
            {
                //if (value != _isSelected)
                //{
                _isVisible = value;
                OnPropertyChanged();
                //}
            }
        }

        public virtual bool? IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                //if (value != _isSelected)
                //{
                _isEnabled = value;
                OnPropertyChanged();
                //}
            }
        }

        public virtual bool IsDoubleClicked
        {
            get { return _isDoubleClicked; }
            set
            {
                //if (value != _isSelected)
                //{
                _isDoubleClicked = value;
                OnPropertyChanged();

                if (DoubleClickToCheck)
                {
                    IsChecked = value;
                }

                //}
            }
        }

        public virtual bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                //if (value != _isSelected)
                //{
                _isReadOnly = value;
                OnPropertyChanged();
                //}
            }
        }

        public virtual bool IsRemovable
        {
            get { return isRemovable; }
            set
            {
                //if (value != _isSelected)
                //{
                isRemovable = value;
                OnPropertyChanged();
                //}
            }
        }

        public virtual bool IsCheckable
        {
            get { return isCheckable; }
            set
            {
                //if (value != _isSelected)
                //{
                isCheckable = value;
                OnPropertyChanged();
                //}
            }
        }

        public bool DoubleClickToCheck
        {
            get => doubleClickToCheck;
            set
            {
                //if (value != _isSelected)
                //{
                doubleClickToCheck = value;
                OnPropertyChanged();
                //}
            }
        }

        public object GroupKey
        {
            get => groupKey;
            internal set
            {
                groupKey = value; OnPropertyChanged();
            }
        }
    }
}