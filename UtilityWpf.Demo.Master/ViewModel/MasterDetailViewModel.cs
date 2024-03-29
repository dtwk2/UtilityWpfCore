﻿using System.Collections;
using System.Collections.Generic;
using Utility.Service;
using UtilityWpf.Demo.Common.ViewModel;
using UtilityWpf.Demo.Data.Factory;
using UtilityWpf.Demo.Master.Infrastructure;
using mdvm = Utility.ViewModel.MasterDetailViewModel;

namespace UtilityWpf.Demo.Master.ViewModel
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