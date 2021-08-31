
namespace UtilityWpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;




    /// <summary>
    /// Loads singleton instance of ResourceDictionary to current scope
    /// </summary>
    /// <remarks>
    /// From Elysium
    /// </remarks>
    public class SharedResourceDictionary : ResourceDictionary
    {
        private static readonly ResourceDictionaryCollection SharedResources = new();
        private string? source;


        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public new string? Source
        {
            get => source;
            set
            {
                if (source != value)
                {
                    source = value;
                    if (value != null)
                        MergedDictionaries.Add(SharedResources.Get(value));
                }
            }
        }



        /// <summary>
        /// The is in application scope.
        /// </summary>
        private bool IsInApplicationScope(ResourceDictionary? resource)
        {
            if (resource == null || resource.Source == null)
            {
                return false;
            }
            // Try and find the resource dictionary in the application scope 
            return Application.Current != null && Application.Current.Resources.ContainsDictionary(resource.Source.OriginalString);
        }
    }



    class ResourceDictionaryCollection : DictionaryCollection<ResourceDictionary>
    {
        public ResourceDictionaryCollection() : base(
           source => new ResourceDictionary { Source = new Uri(source, UriKind.RelativeOrAbsolute) },
           (source, select) => select.FirstMatch(source))
        {
        }



        /// <summary>
        /// Find the resource dictionary by recursively looking in the merged dictionaries
        /// Throw an exceptionReturn if the dictionary could not be found
        /// </summary>
        public ResourceDictionary FindResourceDictionary(string source)
        {
            return TryFindResourceDictionary(source) ?? throw new Exception($"Could not find resource dictionary {source} in SharedResourceDictionary");
        }

        /// <summary>
        /// Find the resource dictionary by recursively looking in the merged dictionaries
        /// Return null if the dictionary could not be found
        /// </summary>
        public ResourceDictionary? TryFindResourceDictionary(string source)
        {
            return Select()
               .Select(dictionary => dictionary.FindDictionary(source))
               .FirstOrDefault();
        }

        /// <summary>
        /// The find resource.
        /// </summary>
        public object FindResource(object resourceKey)
        {
            var foundResource = TryFindResource(resourceKey);
            if (foundResource == null)
            {
                throw new Exception($"Could not find resource with resourceKey {resourceKey} in SharedResourceDictionary");
            }
            return foundResource;
        }

        /// <summary>
        /// The try find resource.
        /// </summary>
        public object? TryFindResource(object resourceKey)
        {
            var foundResource = Select()
               .Select(resourceDictionary => resourceDictionary.FindResource(resourceKey))
               .FirstOrDefault();
            return foundResource;
        }
    }

    class DictionaryCollection<T>
    {
        private readonly Func<string, T> createFunc;
        private readonly Func<string, IEnumerable<T>, T?> find;
        private readonly Dictionary<string, T> SharedResources = new Dictionary<string, T>();

        public DictionaryCollection(Func<string, T> createFunc, Func<string, IEnumerable<T>, T?> find)
        {
            this.createFunc = createFunc;
            this.find = find;
        }


        /// <summary>
        /// Return all resource dictionaries that are in memory.
        /// </summary>
        public IEnumerable<T> Select()
        {
            return SharedResources.Select(weakReference => weakReference.Value);
        }

        /// <summary>
        /// Get the resource dictionary specified by the source uri.
        /// If the dictionary is not loaded add a weak reference to the list
        /// </summary>
        public T Get(string source)
        {
            // Return the resource dictionary if it could be found
            var foundDictionary = find(source, Select());
            if (foundDictionary != null)
            {
                return foundDictionary;
            }

            // Not found so remove the weak reference
            if (SharedResources.ContainsKey(source))
            {
                SharedResources.Remove(source);
            }

            // Load the resource dictionary and hold a weak reference to it
            var newDictionary = createFunc(source);
            SharedResources.Add(source, newDictionary);
            return newDictionary;
        }


    }


}
