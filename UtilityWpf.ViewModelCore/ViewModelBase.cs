//using log4net;
//using MvvmDialogs;
//using MvvmDialogs.FrameworkDialogs.OpenFile;
//using MvvmDialogs.FrameworkDialogs.SaveFile;
//using System.Reflection;
//using System.Windows.Input;
//using UtilityWpf;

//namespace UtilityWpf.ViewModel
//{
//    /// <summary>
//    /// Implements INotifyPropertyChanged for all ViewModel
//    /// </summary>
//    public abstract class ViewModelBase : NPC
//    {
//        #region Parameters

//        private readonly IDialogService DialogService;

//        /// <summary>
//        /// Title of the application, as displayed in the top bar of the window
//        /// </summary>
//        public string Title
//        {
//            get { return "WebDataSystem"; }
//        }

//        #endregion Parameters

//        #region Constructors

//        public ViewModelBase()
//        {
//            // DialogService is used to handle dialogs
//            this.DialogService = new MvvmDialogs.DialogService();
//        }

//        #endregion Constructors



//        #region Commands
//        public RelayCommand<object> SampleCmdWithArgument { get { return new RelayCommand<object>(OnSampleCmdWithArgument); } }

//        public ICommand SaveAsCmd { get { return new RelayCommand(OnSaveAsTest, AlwaysFalse); } }
//        public ICommand SaveCmd { get { return new RelayCommand(OnSaveTest, AlwaysFalse); } }
//        public ICommand NewCmd { get { return new RelayCommand(OnNewTest, AlwaysFalse); } }
//        public ICommand OpenCmd { get { return new RelayCommand(OnOpenTest, AlwaysFalse); } }

//        public ICommand ExitCmd { get { return new RelayCommand(OnExitApp, AlwaysTrue); } }

//        private bool AlwaysTrue()
//        {
//            return true;
//        }

//        private bool AlwaysFalse()
//        {
//            return false;
//        }

//        private void OnSampleCmdWithArgument(object obj)
//        {
//            // TODO
//        }

//        private void OnSaveAsTest()
//        {
//            var settings = new SaveFileDialogSettings
//            {
//                Title = "Save As",
//                Filter = "Sample (.xml)|*.xml",
//                CheckFileExists = false,
//                OverwritePrompt = true
//            };

//            bool? success = DialogService.ShowSaveFileDialog(this, settings);
//            if (success == true)
//            {
//                // Do something
//                Log.Info("Saving file: " + settings.FileName);
//            }
//        }

//        private void OnSaveTest()
//        {
//            // TODO
//        }

//        private void OnNewTest()
//        {
//            // TODO
//        }

//        private void OnOpenTest()
//        {
//            var settings = new OpenFileDialogSettings
//            {
//                Title = "Open",
//                Filter = "Sample (.xml)|*.xml",
//                CheckFileExists = false
//            };

//            bool? success = DialogService.ShowOpenFileDialog(this, settings);
//            if (success == true)
//            {
//                // Do something
//                Log.Info("Opening file: " + settings.FileName);
//            }
//        }

//        private void OnExitApp()
//        {
//            System.Windows.Application.Current.MainWindow.Close();
//        }

//        #endregion Commands

//        protected static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//    }
//}