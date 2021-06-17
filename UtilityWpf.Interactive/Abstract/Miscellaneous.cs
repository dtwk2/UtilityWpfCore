using System;

namespace UtilityWpf.Interactive.Abstract
{
    public enum Interaction
    {
        Select, DoubleSelect, Include, Expand, Check
    }

    public enum UserCommand
    {
        Delete
    }

    public class InteractionArgs : EventArgs
    {
        public Interaction Interaction { get; set; }
        public object Value { get; set; }
    }

    public class UserCommandArgs : EventArgs
    {
        public UserCommand UserCommand { get; set; }
        public object Parameter { get; set; }
    }
}