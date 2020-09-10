//using DynamicData.Binding;
//using PropertyTools.DataAnnotations;
//using System;
//using System.Linq;
//using System.Reactive.Linq;

//namespace UtilityWpf.ViewModel
//{
//    public class FilePickerViewModel : NPC, IOutputService<string>
//    {
//        private string file;

//        [InputFilePath(".txt")]
//        [FilterProperty("Filter")]
//        [AutoUpdateText]
//        public string File
//        {
//            get { return file; }
//            set
//            {
//                if (file != value)
//                {
//                    file = value;

//                    OnPropertiesChanged(nameof(File));
//                }
//            }
//        }

//        public IObservable<string> Output { get => this.WhenValueChanged(_ => _.File).Where(_ => _ != null); }

//    }
//}