using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;
using UtilityWpf;
using UtilityWpf.Command;
using UtilityWpf.Property;

namespace UtilityWpf.ViewModel
{
    public class SHDOObject : SHDObject<object>
    {
        public SHDOObject() : base(null)
        {
        }

        public SHDOObject(object o) : base(o)
        {
        }

        public SHDOObject(object @object, IObservable<Predicate<object>> visible = null, IObservable<Predicate<object>> enable = null, IConvertible id = null) : base(@object, visible, enable, id)
        {
        }

        public SHDOObject(object @object, bool? expanded, bool? selected, bool? @checked, bool? visible, bool? enable, bool isReadOnly, IConvertible id = null) : base(@object, expanded, selected, @checked, visible, enable, isReadOnly, id)
        {
        }
    }

    // Selectable / Hideable / Disableable
    public class SHDObject<T> : NPC, IObject<T>//,IDisposable
    {
        // set needs to be exposed for LiteDbToWork
        public T Object { get; set; }

        public IConvertible Id { get; }

        private bool? _isExpanded;
        private bool? _isSelected;
        private bool? _isChecked;
        private bool? _isVisible;
        private bool? _isEnabled;
        private bool _isDoubleClicked;
        private bool _isReadOnly;

        public ISubject<object> Deletions { get; } = new Subject<object>();

        public RelayCommand DeleteCommand { get; }

        public SHDObject(T @object, IObservable<Predicate<T>> visible = null, IObservable<Predicate<T>> enable = null, IConvertible id = null)
        {
            Object = @object;

            if (visible != null)
                visible.Select(_ => _(Object)).StartWith(true).Subscribe(_ => IsVisible = true);
            else
                IsVisible = true;

            if (enable != null)
                enable.StartWith(_ => true).Select(_ => _(Object)).Subscribe(_ => IsEnabled = true);
            else
                IsEnabled = true;

            DeleteCommand = new RelayCommand(() =>
            {
                Deletions.OnNext(null);
            });

            Id = id;
        }

        public SHDObject(T @object, bool? expanded, bool? selected, bool? @checked, bool? visible, bool? enable, bool isReadOnly, IConvertible id = null)
        {
            Object = @object;

            IsVisible = visible;

            IsEnabled = enable;

            IsExpanded = expanded;

            IsSelected = selected;

            IsChecked = @checked;

            IsReadOnly = isReadOnly;

            DeleteCommand = new RelayCommand(() =>
            {
                Deletions.OnNext(null);
            });

            Id = id;
        }

        public SHDObject()
        {
        }

        // properties don't work as reactive properties

        public bool? IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                //if (value != _isExpanded)
                //{
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
                //}
            }
        }

        public virtual bool? IsSelected
        {
            get { return _isSelected; }
            set
            {
                //if (value != _isSelected)
                //{
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
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
                OnPropertyChanged(nameof(IsChecked));
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
                OnPropertyChanged(nameof(IsVisible));
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
                OnPropertyChanged(nameof(IsEnabled));
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
                OnPropertyChanged(nameof(IsDoubleClicked));
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
                OnPropertyChanged(nameof(IsReadOnly));
                //}
            }
        }
    }
}