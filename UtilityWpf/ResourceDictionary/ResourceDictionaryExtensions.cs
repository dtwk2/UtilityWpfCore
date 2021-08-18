using System.Collections.Generic;

namespace UtilityWpf
{
    using System;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// The resource dictionary extensions.
    /// </summary>
    public static class ResourceDictionaryExtensions
    {
       public static ResourceDictionary? FirstMatch(this IEnumerable<ResourceDictionary> dictionaries, string source)
       {
          return dictionaries
                .Select(dictionary => dictionary.FindDictionary(source))
                .FirstOrDefault();
       }

      public static void ReplaceDictionary(this ResourceDictionary resourceDictionary, Uri source, ResourceDictionary destination)
        {
            resourceDictionary.BeginInit();

            resourceDictionary.MergedDictionaries.Add(destination);

            ResourceDictionary oldResourceDictionary = resourceDictionary.MergedDictionaries
                .FirstOrDefault(x => x.Source == source);
            if (oldResourceDictionary != null)
            {
                resourceDictionary.MergedDictionaries.Remove(oldResourceDictionary);
            }

            resourceDictionary.EndInit();
        }

        public static void ReplaceDictionary(this ResourceDictionary resourceDictionary, Uri source, Uri destination)
        {
            resourceDictionary.BeginInit();

            if (!resourceDictionary.MergedDictionaries.Any(x => x.Source == destination))
            {
                resourceDictionary.MergedDictionaries.Add(
                    new ResourceDictionary()
                    {
                        Source = destination
                    });
            }

            ResourceDictionary oldResourceDictionary = resourceDictionary.MergedDictionaries
                .FirstOrDefault(x => x.Source == source);
            if (oldResourceDictionary != null)
            {
                resourceDictionary.MergedDictionaries.Remove(oldResourceDictionary);
            }

            resourceDictionary.EndInit();
        }

        /// <summary>
        /// Find the resource dictionary by recursively looking in the merged dictionaries
        /// Throw an exceptionReturn if the dictionary could not be found
        /// </summary>
        public static ResourceDictionary? FindDictionary(this ResourceDictionary resourceDictionary, string source)
        {
           // If this is the resource return it
            if (resourceDictionary.Source != null && resourceDictionary.Source.OriginalString == source)
            {
                return resourceDictionary;
            }

            // Search the merges resource dictionaries
            var foundDictionary = resourceDictionary.MergedDictionaries
                .Select(mergedResource => mergedResource.FindDictionary(source))
                .FirstOrDefault();

            return foundDictionary;
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="resourceDictionary">
        /// The resource dictionary.
        public static bool ContainsDictionary(this ResourceDictionary resourceDictionary, ResourceDictionary resource)
        {
            if (resource.Source == null)
            {
                return false;
            }

            var foundDictionary = resourceDictionary.FindDictionary(resource.Source.OriginalString);
            return foundDictionary != null;
        }

        /// <summary>
        /// Determines if the specified resource dictionary (source) exists anywhere in the 
        /// resource dictionary recursively.
        /// </summary>
        public static bool ContainsDictionary(this ResourceDictionary resourceDictionary, string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            var foundDictionary = resourceDictionary.FindDictionary(source);
            return foundDictionary != null;
        }

        /// <summary>
        /// The find resource.
        /// </summary>
        public static object? FindResource(this ResourceDictionary resourceDictionary, object resourceKey)
        {
            // Try and find the resource in the root dictionary first
            var value = resourceDictionary[resourceKey];
            if (value != null)
            {
                return value;
            }

            // Then try the merged dictionaries
            var foundResource = resourceDictionary.MergedDictionaries
                                    .Select(mergedDictionary => mergedDictionary.FindResource(resourceKey))
                                    .FirstOrDefault(resource => resource != null);
            return foundResource;
        }

        #region Private Methods

        private static string GetSource(ResourceDictionary resourceDictionary)
        {
            SharedResourceDictionary sharedResourceDictionary = resourceDictionary as SharedResourceDictionary;
            if (sharedResourceDictionary != null)
            {
                return sharedResourceDictionary.Source;
            }

            return resourceDictionary.Source.ToString();
        }

        #endregion
    }
}
