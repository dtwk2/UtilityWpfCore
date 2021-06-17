namespace UtilityWpf.Interactive.View
{

    public readonly struct KeyBoolean
    {
        public KeyBoolean(string name, bool value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get;  }

        public bool Value { get; }

        public static KeyBoolean BuildDefault(string name) => new KeyBoolean(name, false);

    }


    public readonly struct NodePropertiesEnabledModel
    {
        public NodePropertiesEnabledModel(KeyBoolean isExpanded, KeyBoolean isSelected, KeyBoolean isChecked, KeyBoolean isVisible, KeyBoolean isEnabled) : this()
        {
            //IsExpanded = isExpanded;
            //IsSelected = isSelected;
            //IsChecked = isChecked;
            //IsVisible = isVisible;
            //IsEnabled = isEnabled;
        }


        public bool IsExpanded { get; }
        public bool IsDeleted { get; }
        public bool IsSelected { get; }
        public bool IsChecked { get; }
        public bool IsVisible { get; }
        public bool IsEnabled { get; }
    }

    public readonly struct NodeModel
    {
        public NodeModel(bool isExpanded = false, bool isSelected = false, bool isChecked = false, bool isVisible = false, bool isEnabled = false) : this()
        {
            IsExpanded = isExpanded;
            IsSelected = isSelected;
            IsChecked = isChecked;
            IsVisible = isVisible;
            IsEnabled = isEnabled;
        }


        public bool IsExpanded { get; }
        public bool IsDeleted { get; }
        public bool IsSelected { get; }
        public bool IsChecked { get; }
        public bool IsVisible { get; }
        public bool IsEnabled { get; }
        //  public bool IsExpanded { get; set; }


    }
}