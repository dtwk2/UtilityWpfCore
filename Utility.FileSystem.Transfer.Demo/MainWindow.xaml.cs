using SevenZip;
using System;
using System.IO;
using System.Reactive.Subjects;
using System.Windows;
using Utility.FileSystem.Transfer.Common;
using Utility.FileSystem.Transfer.Service;

namespace Utility.FileSystem.Transfer.Demo
{
    public partial class MainWindow : Window
    {
        private const string destination = "../../../Data/Destination/";
        private const string source = "../../../Data/Source/";
        private readonly string dll7z = AppDomain.CurrentDomain.BaseDirectory + "Resources/7z.dll";
        private readonly Subject<bool> subject = new Subject<bool>();

        public MainWindow()
        {
            InitializeComponent();

            FileProgressView1.Source = "../../../Data/Source/huge_dummy_file";
            FileProgressView1.SourceType = PathType.File;
            FileProgressView1.DestinationType = PathType.File;
            FileProgressView1.Destination = "../../../Data/Destination/";
            FileProgressView1.Transferer = new CopyService();
            FileProgressView3.Source = "../../../Data/Source/huge_dummy_file.7z";
            FileProgressView3.SourceType = PathType.File;
            FileProgressView3.Destination = "../../../Data/Destination/Destination";
            FileProgressView3.Transferer = new ExtractService();
            ProgressView1.Transferer = new DummyService();
            FileProgressView4.Transferer = new ReactiveAsynCompress();
            FileProgressView4.DestinationType = PathType.File;
            FileProgressView4.Source = "../../../Data/Source/";
            FileProgressView4.Destination = "../../../Data/Destination/";
            FileProgressView5.Transferer = new DeleteService();
            FileProgressView5.PathType = PathType.File;
            FileProgressView5.Path = "../../../Data/Source/";
            Directory.CreateDirectory("../../../Data/Source/");
            Directory.CreateDirectory("../../../Data/Destination/");
            CreateDummyFile();
            CreateDummyZipFile();
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            subject.OnNext(true);
        }

        private void CreateDummyFile()
        {
            if (File.Exists("../../../Data/Source/huge_dummy_file"))
            {
                return;
            }

            Directory.CreateDirectory("../../../Data/Source/");
            FileStream fileStream = new FileStream("../../../Data/Source/huge_dummy_file", FileMode.CreateNew);
            fileStream.Seek(524288000L, SeekOrigin.Begin);
            fileStream.WriteByte(0);
            fileStream.Close();
        }

        private void CreateDummyZipFile()
        {
            if (File.Exists("../../../Data/Source/huge_dummy_file.7z"))
            {
                return;
            }

            SevenZipBase.SetLibraryPath(dll7z);
            new SevenZipCompressor()
            {
                CompressionMode = CompressionMode.Create,
                TempFolderPath = Path.GetTempPath(),
                ArchiveFormat = OutArchiveFormat.SevenZip
            }.CompressDirectory("../../../Data/Source/", "../../../Data/Source/huge_dummy_file.7z");
        }

        private void Show_Default_OnClick(object sender, RoutedEventArgs e)
        {
            MultiStage.Content = new MultiStageProgressView();
        }
    }
}