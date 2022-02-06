using Dragablz;
using System;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls.Hybrid
{
    public class MasterBindableControl : UtilityWpf.Controls.Master.MasterControl
    {
        public static readonly DependencyProperty DisplayMemberPathProperty = ItemsControl.DisplayMemberPathProperty.AddOwner(typeof(MasterBindableControl));

        static MasterBindableControl()
        {
        }

        public MasterBindableControl()
        {
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        protected override void ExecuteAdd()
        {
            if (CommandParameter?.MoveNext() == true)
            {
                if (Content is DragablzItemsControl itemsControl)
                    try
                    {
                        itemsControl.AddToSource(CommandParameter.Current, AddLocationHint.Last);
                    }
                    catch (Exception ex)
                    {
                    }
            }
            else
            {
            }
            base.ExecuteAdd();
        }
    }
}