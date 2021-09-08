using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

/// <summary>
/// <a href="https://wiert.me/2009/09/14/netc-exceptioncatcher-and-exceptionhelper-gems/"/></a>
/// </summary>
namespace Utility.Common
{
    public class ExceptionHelper
    {
        public static ExceptionCatcher Catch(SendOrPostCallback codeBlock)
        {
            ExceptionCatcher exceptionCatcher = new ExceptionCatcher();
            exceptionCatcher.Catch(codeBlock);
            return exceptionCatcher;

        }

        public static bool Failed(SendOrPostCallback codeBlock)
        {
            ExceptionCatcher exceptionCatcher = new ExceptionCatcher();
            bool result = exceptionCatcher.Failed(codeBlock);
            return result;
        }

        public static bool Succeeded(SendOrPostCallback codeBlock)
        {
            ExceptionCatcher exceptionCatcher = new ExceptionCatcher();
            bool result = exceptionCatcher.Succeeded(codeBlock);
            return result;
        }

    }

    public class ExceptionCatcher
    {

        private Exception _Exception;
        public Exception Exception
        {
            get { return _Exception; }
        }

        private bool _Success;
        public bool Success
        {
            get { return _Success; }
        }

        public void Catch(SendOrPostCallback codeBlock)
        {
            _Exception = null;
            try
            {
                // need 1 argument, because it is a SendOrPostCallback
                codeBlock.DynamicInvoke(1);

                _Success = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Trace.WriteLine("ExceptionCatcher.Succeeded failure", ex.ToString());
#endif
                _Exception = ex;
                _Success = false;
            }
        }

        public bool Failed(SendOrPostCallback codeBlock)
        {
            bool result = !Succeeded(codeBlock);
            return result;
        }

        public bool Failed(out string exceptionString, SendOrPostCallback codeBlock)
        {
            bool result = !Succeeded(out exceptionString, codeBlock);
            return result;
        }

        public bool Succeeded(SendOrPostCallback codeBlock)
        {
            Catch(codeBlock);
            return _Success;
        }

        public bool Succeeded(out string exceptionString, SendOrPostCallback codeBlock)
        {
            bool result = Succeeded(codeBlock);
            if (result)
                exceptionString = this.Exception.ToString();
            else
                exceptionString = string.Empty;
            return result;
        }

    }
}


