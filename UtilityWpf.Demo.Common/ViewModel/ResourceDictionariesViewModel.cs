using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections;
using UtilityHelper;
using System.Reactive.Linq;

namespace UtilityWpf.Demo.Common.ViewModel
{
    public class ResourceDictionariesViewModel
    {
        private bool isReadOnly;

        private readonly ActionCommand changeCommand;
        private readonly IReadOnlyCollection<TickViewModel> collection = new TickViewModelFactory().Collection;

        public ResourceDictionariesViewModel()
        {
            changeCommand = new ActionCommand(Change);
        }

        public bool IsReadOnly
        {
            get => isReadOnly; set => isReadOnly = value;
        }
        public string Header { get; } = "Resource-Dictionaries ViewModel";

        public virtual IEnumerable Collection => collection;

        public ICommand ChangeCommand => changeCommand;

        private void Change()
        {

        }
    }

    public class TickViewModelFactory
    {
        public TickViewModelFactory()
        {
            var coll = Source.ThemeDictionary;

            if (coll == null)
                throw new Exception("No Themes ResourceDictionary");

            Collection = CreateViewModels(coll.MergedDictionaries).ToArray();
        }

        public IReadOnlyCollection<TickViewModel> Collection { get; }

        private IEnumerable<TickViewModel> CreateViewModels(Collection<ResourceDictionary> coll)
        {
            const bool defaultValue = false;

            foreach (var dic in coll)
            {
                //if (dic.Source == null)
                //    continue;
                var tick = new TickViewModel(Guid.NewGuid(), defaultValue, GetName(dic));

                yield return tick;
            }
            static string GetName(ResourceDictionary dic)
            {
                return Converter.Convert(StringHelper.Split(dic.Source.ToString(), ";component/").Last());
            }
        }
    }

    class Source
    {
        public static ResourceDictionary? ThemeDictionary { get; } = Application.Current.Resources
                        .MergedDictionaries
                        .SingleOrDefault(d => d.Source.ToString().EndsWith("Themes.xaml"));
    }

    static class Converter
    {
        public static string Convert(string uri)
        {
            return uri.Remove(".xaml");
        }
        public static string ConvertBack(string value)
        {
            return value + ".xaml";
        }
    }

    public class ResourceDictionaryService : IObserver<TickViewModel>
    {
        private readonly Dictionary<ResourceDictionary, bool> dictionary = Source.ThemeDictionary.MergedDictionaries.ToDictionary(a => a, a => false);
        ReplaySubject<TickViewModel> tickViewModel = new();

        public ResourceDictionaryService()
        {
            ReplaySubject<(ResourceDictionary, bool)> rSubject = new(1);

            UpdateMergedDictionaries(rSubject);

            var disposable = tickViewModel.Subscribe(tick =>
            {
                tick
                .Select(ad => (MatchDictionary(ad.Text), ad.NewValue))
                .Subscribe(rSubject);

                rSubject.OnNext((MatchDictionary(tick.Text), tick.IsChecked));
            });
            rSubject.OnNext((Source.ThemeDictionary, false));
        }

        private void UpdateMergedDictionaries(ReplaySubject<(ResourceDictionary, bool)> rSubject)
        {
            rSubject
                .Subscribe(a =>
                {
                    var (dict, b) = a;
                    if (dictionary[dict] = b)
                        Application.Current.Resources.MergedDictionaries.Add(dict);
                    else
                        Application.Current.Resources.MergedDictionaries.Remove(dict);
                });
        }



        ResourceDictionary MatchDictionary(string name)
        {
            return dictionary.Keys.Single(a => a.Source.ToString().EndsWith(Converter.ConvertBack(name)));
        }



        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }


        public void OnNext(TickViewModel value)
        {
            tickViewModel.OnNext(value);
        }
    }
}
