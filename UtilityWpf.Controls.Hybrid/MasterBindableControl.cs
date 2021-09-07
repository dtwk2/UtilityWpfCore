using Dragablz;
using Evan.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility.Common.Enum;


namespace UtilityWpf.Controls.Hybrid
{
    public class MasterBindableControl : UtilityWpf.Controls.Master.MasterControl
    {

        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyHelper.Register<string>();

        static MasterBindableControl()
        {

        }

        public MasterBindableControl()
        {
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
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
