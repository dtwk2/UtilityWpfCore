//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Resources;
//using System.Text;
//using System.Threading.Tasks;

//namespace UtilityWpf {
//   public class AutoResourceDictionary:System.Windows.ResourceDictionary {
//      private Type type;

//      public Type Type
//      {
//         get => type;
//         set
//         {
//            type = value;
//            fsd(value);
//         }
//      }

//      IEnumerable<DictionaryEntry> fsd(Type type)
//      {
//         return SelectResourceDictionaries(type.Assembly);
//      }

//      IEnumerable<DictionaryEntry> fsd(string dllAddress) {
//         //string address = @"WpfCustomControlLibrary.dll";
//         //List<Stream> bamlStreams = new List<Stream>();
//         Assembly assembly = Assembly.LoadFrom(dllAddress);
//         return SelectResourceDictionaries(assembly);
//      }

//      IEnumerable<DictionaryEntry> SelectResourceDictionaries(Assembly assembly)
//      {
//         string[] resourceDictionaries = assembly.GetManifestResourceNames();
//         foreach (string resourceName in resourceDictionaries) {
//            ManifestResourceInfo info = assembly.GetManifestResourceInfo(resourceName);
//            if (info.ResourceLocation != ResourceLocation.ContainedInAnotherAssembly) {
//               Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
//               using (ResourceReader reader = new ResourceReader(resourceStream)) {
//                  foreach (DictionaryEntry entry in reader) {
//                     //MergedDictionaries.Add(GetResourceDictionary(newSource));
//                     yield return entry;
//                     //Here you can see all your ResourceDictionaries
//                     //entry is your ResourceDictionary from assembly
//                  }
//               }
//            }
//         }
//      }
//   }
//}
