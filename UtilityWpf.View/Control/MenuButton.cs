using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.View
{
    public static class ToolBarStyleKeys
    {
        private static ResourceKey _menuButtonStyleKey;

        static ToolBarStyleKeys()
        {
            _menuButtonStyleKey = new ComponentResourceKey(typeof(ToolBarStyleKeys), "ToolBarStyleKeys.MenuButton style key");
        }

        public static ResourceKey MenuButtonStyleKey
        {
            get { return _menuButtonStyleKey; }
        }
    }

    /// <summary>
    /// https://xstatic2.wordpress.com/2011/11/07/drop-down-button/
    /// </summary>
    public class MenuButton : System.Windows.Controls.Primitives.ToggleButton
    {
        private static ResourceKey _dropDownStyleKey;

        static MenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButton), new FrameworkPropertyMetadata(typeof(MenuButton)));
            _dropDownStyleKey = new ComponentResourceKey(typeof(MenuButton), "MenuButton.ContextMenu style key");
        }

        public static ResourceKey MenuDropDownStyleKey
        {
            get { return _dropDownStyleKey; }
        }

        /// <summary>
        /// Gets or sets he drop-down menu of the button.
        /// </summary>
        public ContextMenu DropDown
        {
            get { return (ContextMenu)GetValue(DropDownProperty); }
            set { SetValue(DropDownProperty, value); }
        }

        public static readonly DependencyProperty DropDownProperty =
            DependencyProperty.Register("DropDown", typeof(ContextMenu), typeof(MenuButton), new UIPropertyMetadata(OnDropDownProeprtyChanged));


        private static void OnDropDownProeprtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuButton sender = (MenuButton)d;
            sender.OnDropDownChanged(e.OldValue as ContextMenu, e.NewValue as ContextMenu);
        }

        protected virtual void OnDropDownChanged(ContextMenu oldMenu, ContextMenu newMenu)
        {
            if (oldMenu != null)
            {
                oldMenu.SetResourceReference(ContextMenu.StyleProperty, ContextMenu.DefaultStyleKeyProperty);
                oldMenu.SetValue(FrameworkElement.DefaultStyleKeyProperty, ContextMenu.DefaultStyleKeyProperty.DefaultMetadata.DefaultValue);
                SetParentMenuButton(oldMenu, null);
            }

            if (newMenu != null)
            {
                SetParentMenuButton(newMenu, this);

                if (DependencyPropertyHelper.GetValueSource(newMenu, FrameworkElement.StyleProperty).BaseValueSource <= BaseValueSource.ImplicitStyleReference)
                {
                    newMenu.SetResourceReference(FrameworkElement.StyleProperty, _dropDownStyleKey);
                }

                newMenu.SetValue(FrameworkElement.DefaultStyleKeyProperty, _dropDownStyleKey);
            }
        }

        /// <summary>
        /// Gets or sets the whether the drop-down menu is open or not.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, BooleanBoxes.Box(value)); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MenuButton), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// Gets or sets the placement of the drop-down menu.
        /// </summary>
        public PlacementMode DropDownPlacement
        {
            get { return (PlacementMode)GetValue(DropDownPlacementProperty); }
            set { SetValue(DropDownPlacementProperty, value); }
        }

        public static readonly DependencyProperty DropDownPlacementProperty =
            DependencyProperty.Register("DropDownPlacement", typeof(PlacementMode), typeof(MenuButton), new UIPropertyMetadata(PlacementMode.Bottom));



        internal static MenuButton GetParentMenuButton(ContextMenu obj)
        {
            return (MenuButton)obj.GetValue(ParentMenuButtonProperty);
        }

        internal static void SetParentMenuButton(ContextMenu obj, MenuButton value)
        {
            obj.SetValue(ParentMenuButtonProperty, value);
        }

        // Using a DependencyProperty as the backing store for MenuButtonParent.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentMenuButtonProperty =
            DependencyProperty.RegisterAttached("ParentMenuButton", typeof(MenuButton), typeof(MenuButton), new UIPropertyMetadata());


    }
}
