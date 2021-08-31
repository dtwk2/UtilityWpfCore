using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Baml2006;

namespace UtilityWpf {
   public class AutoResourceDictionary : ResourceDictionary {
      private Type type;

      public Type Type {
         get => type;
         set {
            type = value;
            foreach (var resourceDictionary in SelectFromType(value)) {
               MergedDictionaries.Add(resourceDictionary);
            }
         }
      }

      IEnumerable<ResourceDictionary> SelectFromType(Type type) {
         return SelectResourceDictionaries(type.Assembly);
      }

      IEnumerable<ResourceDictionary> SelectResourceDictionaries(Assembly assembly) {

         // Only interested in main resource file
         return GetResourceNames().SelectMany(GetDictionaries);

         IEnumerable<ResourceDictionary> GetDictionaries(string resourceName)
         {
            Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
            using (ResourceReader reader = new ResourceReader(resourceStream))
            {
               foreach (DictionaryEntry entry in GetDictionaryEntries(reader))
               {
                  ResourceDictionary dictionary;
                  var readStream = entry.Value as Stream;
                  Baml2006Reader bamlReader = new Baml2006Reader(readStream);
                  var loadedObject = System.Windows.Markup.XamlReader.Load(bamlReader);
                  if (loadedObject is ResourceDictionary dict)
                  {
                     dictionary = dict;
                  }
                  else
                  {
                     continue;
                  }
                  yield return dictionary;
               }
            }
         }

         DictionaryEntry[] GetDictionaryEntries(ResourceReader reader) {
            var entries = reader.OfType<DictionaryEntry>()
               // only interested in baml(xaml) files not images or similar
               .Where(entry => entry.Key.ToString()?.EndsWith("baml") == true &&
                               entry.Key.ToString()?.ToLowerInvariant().Contains("generic") != true)
               .ToArray();
            return entries;
         }


         IEnumerable<string> GetResourceNames() {
            string[] resourceNames = assembly.GetManifestResourceNames().Where(a => a.EndsWith("g.resources")).ToArray();
            foreach (string resourceName in resourceNames) {
               ManifestResourceInfo info = assembly.GetManifestResourceInfo(resourceName);
               if (info?.ResourceLocation != ResourceLocation.ContainedInAnotherAssembly) {

                  yield return resourceName;

               }
            }
         }
      }

        IEnumerable<ResourceDictionary> SelectFromAddress(string dllAddress)
        {
            //string address = @"WpfCustomControlLibrary.dll";
            //List<Stream> bamlStreams = new List<Stream>();
            Assembly assembly = Assembly.LoadFrom(dllAddress);
            return SelectResourceDictionaries(assembly);
        }

   }
}
