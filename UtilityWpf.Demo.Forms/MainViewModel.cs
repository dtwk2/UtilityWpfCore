using ReactiveUI;
using System;
using System.ComponentModel;
using Utility.Common;
using Utility.Persist;
using UtilityWpf.Demo.Forms.Infrastructure;
using UtilityWpf.Demo.Forms.Model;
using UtilityWpf.Demo.Forms.ViewModel;

namespace UtilityWpf.Demo.Forms
{
    public class MainViewModel : IReactiveObject
    {
        public MainViewModel()
        {
            MapperFactory.RegisterBsonMapper();

            var databaseService = new LiteDbRepository(new(typeof(EditModel), nameof(EditModel.Id)));
            var mapper = MapperFactory.CreateMapperConfiguration().CreateMapper();

            if (databaseService.FindBy(new FirstOrDefaultQuery()) is { } editModel)
            {
                EditViewModel = mapper.Map<EditViewModel>(editModel);
            }

            EditViewModel
                .Changes(startWithSource: true)
                .Subscribe(a =>
                {
                    EditModel = mapper.Map<EditModel>(a.source);
                    databaseService.Upsert(EditModel);
                    String = UtilityHelperEx.JsonHelper.Serialize(EditModel);
                    this.RaisePropertyChanged(new(nameof(EditModel)));
                    this.RaisePropertyChanged(new(nameof(String)));
                });
        }

        public EditViewModel EditViewModel { get; } = new() { Id = Guid.NewGuid() };
        public EditModel? EditModel { get; set; }

        public string? String { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event PropertyChangingEventHandler? PropertyChanging;

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            this.PropertyChanging?.Invoke(this, args);
        }
    }
}