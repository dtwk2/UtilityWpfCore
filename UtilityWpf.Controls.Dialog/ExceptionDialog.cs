using System;
using UtilityWpf.Controls.Objects;

namespace UtilityWpf.Controls.Dialog
{
    public class ExceptionDialog : ConfirmationDialog
    {
        private Exception exception;

        public ExceptionDialog(Exception exception, string message = null) : this()
        {
            Content = new ObjectFlowControl(message, exception);
            Exception = exception;
        }

        public Exception Exception
        {
            get => exception; set
            {
                exception = value;
                Content = new ObjectFlowControl("Exception", exception);
            }
        }

        public ExceptionDialog() : base()
        {
            //MaxHeight = 400;
        }
    }
}