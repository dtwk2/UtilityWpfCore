using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UtilityWpf
{
    public abstract class NPC : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Occurs when any properties are changed on this object.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  raises the PropertyChanged event for one to many properties.
        /// </summary>
        /// <param name="propertyNames">The names of the properties that changed.</param>
        public virtual void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (string name in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        ///  raises the PropertyChanged event for a single property
        ///  'propertyname' can be left null (e.g OnPropertyChanged()), if called from body of property
        /// </summary>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///  raises the PropertyChanged event for a single property
        ///  'propertyname' can be left null (e.g OnPropertyChanged()), if called from body of property
        /// </summary>
        public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Implementation
    }
}