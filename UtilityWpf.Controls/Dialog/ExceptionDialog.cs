using System;

namespace UtilityWpf.Controls {
   public class ExceptionDialog : ConfirmationDialog {
      public ExceptionDialog(Exception exception, string message = null) : this() {
         Content = new ObjectFlowControl(message, exception);
      }

      public ExceptionDialog() : base() {
         //MaxHeight = 400;
      }
   }
}