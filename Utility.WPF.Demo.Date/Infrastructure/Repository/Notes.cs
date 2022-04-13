using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DateWork.Helpers;
using DateWork.Models;
using Utility.WPF.Demo.Date.Infrastructure.Entity;

namespace Utility.WPF.Demo.Date.Infrastructure {
   //   [XmlRoot("Notes")]
   //   public class Notes : BaseViewModel {
   //      //private static readonly string _Path = AppDomain.CurrentDomain.BaseDirectory + "Notes.xml";
   //      private static string _Path {
   //         get {
   //#if DEBUG
   //            {
   //               var xx = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName + "\\Notes.sqlite";
   //               return xx;
   //            }

   //#endif
   //#if RELEASE
   //                {
   //                var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Utility Journal";
   //                var directory = Directory.CreateDirectory(directoryPath);
   //                var path = Path.Combine(directoryPath, "Notes.sqlite");
   //                return path;
   //                }
   //#endif
   //         }
   //      }


   internal class NotesHelper {

      public static async Task<NoteEntity[]> SelectNotes(DateTime day) {

         var dayNotes = await NoteEntity
            .Where(a => a.Date == day)
            .ToListAsync();

         return dayNotes.ToArray();
      }

   }

}
