using System;
using System.Collections;
using System.Globalization;
using System.Reactive;
using System.Windows.Data;
using AutoMapper;
using ReactiveUI;
using UtilityInterface.NonGeneric.Database;
using UtilityWpf.Service;
using UtilityWpf.TestData.Model;
using static UtilityWpf.Controls.MasterControl;

namespace UtilityWpf.Demo.Sandbox.Infrastructure
{
    public class PersistList2ViewModel : ReactiveObject
    {
        private readonly FieldsFactory factory = new();

        private IDatabaseService dbS = new LiteDbRepository(new LiteDbRepository.ConnectionSettings(typeof(Fields), new System.IO.FileInfo("../../../Data/Data.litedb"), nameof(Fields.Id)));

        public PersistList2ViewModel()
        {
            ChangeCommand = ReactiveCommand.Create<object, Unit>((a) =>
            {
                switch (a)
                {
                    case MovementEventArgs eventArgs:
                        foreach (var item in eventArgs.Changes)
                        {
                            //Data.Move(item.OldIndex, item.Index);
                        }
                        break;
                    default:
                        break;
                }
                return Unit.Default;
            });

        }

        public MintPlayer.ObservableCollection.ObservableCollection<Fields> Data { get; } = new MintPlayer.ObservableCollection.ObservableCollection<Fields>();

        public IEnumerator NewItem
        {
            get
            {
                var build = factory.Build();
                return build;
            }
        }

        public ReactiveCommand<object, Unit> ChangeCommand { get; }
        public ReactiveCommand<object, Unit> ChangeRepositoryCommand { get; }
        public ReactiveCommand<object, Unit> CollectionChangedCommand { get; }

        public IDatabaseService DatabaseService { get => dbS; private set => this.RaiseAndSetIfChanged(ref dbS, value); }

        public IValueConverter ValueConverter { get; } = new ValueConverter();

    }

    public class ValueConverter : IValueConverter
    {
        class AutoMapperFactory
        {
            public static IMapper Create()
            {
                return new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ReactiveFields, Fields>();
                    cfg.CreateMap<Fields, ReactiveFields>();
                    //cfg.AddProfile(new MergeProfile());
                }).CreateMapper();
            }
        }

        private readonly Lazy<IMapper> mapper = new(() => AutoMapperFactory.Create());

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Fields fields)
            {
                return mapper.Value.Map<Fields, ReactiveFields>(fields);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReactiveFields reactiveFields)
            {
                return mapper.Value.Map<ReactiveFields, Fields>(reactiveFields);
            }
            return null;
        }
    }

}
