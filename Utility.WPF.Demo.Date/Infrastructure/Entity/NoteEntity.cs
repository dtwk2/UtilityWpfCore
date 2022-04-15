using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeSql;

namespace Utility.WPF.Demo.Date.Infrastructure.Entity {
   public class NoteEntity : BaseEntity<NoteEntity, Guid> {

      public DateTime Date { get; set; }
      public string Text { get; set; }
   }
}
