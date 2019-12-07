using System;
using System.Collections.Generic;

namespace UtilityWpf.ViewModel
{
    public sealed class SortContainer<T> : IEquatable<SortContainer<T>>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SortContainer(string description, IComparer<T> comparer)
        {
            Description = description;
            Comparer = comparer;
        }

        public IComparer<T> Comparer { get; }
        public string Description { get; }

        #region Equality members

        public bool Equals(SortContainer<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SortContainer<T>)obj);
        }

        public override int GetHashCode()
        {
            return (Description != null ? Description.GetHashCode() : 0);
        }

        public static bool operator ==(SortContainer<T> left, SortContainer<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SortContainer<T> left, SortContainer<T> right)
        {
            return !Equals(left, right);
        }

        #endregion Equality members
    }
}