using System.Collections;
using System.Collections.Generic;
using Utility.Service;
using UtilityWpf.Demo.Common.ViewModel;
using UtilityWpf.Demo.Data.Model;
using mdvm = Utility.ViewModel.MasterDetailViewModel;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class MasterDetailViewModel : mdvm
    {
        private IEnumerator<Fields> build;

        private static FieldsFactory Factory() => new();

        private static readonly CollectionService CollectionService = new();

        public MasterDetailViewModel() : base(CollectionService, new MockDatabaseService())
        {
        }

        public override IEnumerator NewItem => build ??= Factory().Build();
    }
}