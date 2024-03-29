﻿using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using Utility.Persist;
using Utility.Service;
using UtilityInterface.NonGeneric.Data;
using UtilityWpf.Demo.Common.ViewModel;

namespace UtilityWpf.Demo.Hybrid.ViewModel
{
    public class ResourceDictionariesViewModel : Common.ViewModel.ResourceDictionariesViewModel
    {
        private readonly CollectionService collectionService = new();

        public ResourceDictionariesViewModel()
        {
            var dictionaries = typeof(UtilityWpf.Demo.Common.ViewModel.ResourceDictionariesViewModel)
                .Assembly
                .SelectResourceDictionaries(a => a.Key.ToString().EndsWith("themes.baml", System.StringComparison.CurrentCultureIgnoreCase))
                .Single()
                .resourceDictionary
                .MergedDictionaries;

            var themes = ThemesViewModelFactory
                .CreateViewModels(dictionaries)
                .ToArray();

            ResourceDictionaryService service = new(dictionaries);

            //foreach(var item in collectionService.Items)
            //{
            //    service.OnNext(item as TickViewModel);
            //}

            collectionService
                .Select(a => a.change)
                .Subscribe(a =>
              {
                  if (a.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                  {
                      foreach (var item in a.Items)
                      {
                          service.OnNext(item as TickViewModel);
                      }
                      return;
                  }
                  throw new Exception("gd77fdfwe");
              });

            collectionService.OnNext(new(Repository()));

            foreach (var item in themes)
            {
                if (collectionService.Items.Contains(item) == false)
                    collectionService.Items.Add(item);
                //else
                //    collectionService.Items.Remove(item);
            }
        }

        public override IEnumerable Collection => collectionService.Items;

        private IRepository Repository() => new LiteDbRepository(new(typeof(TickViewModel), nameof(TickViewModel.Id)));
    }
}