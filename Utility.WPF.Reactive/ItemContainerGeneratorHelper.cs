using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UtilityWpf.Helper
{
    internal static class ItemContainerGeneratorHelper
    {
        public static IObservable<GeneratorStatus> StatusChanges(this ItemContainerGenerator generator)
        {
            var obs = Observable
            .FromEventPattern<EventHandler, EventArgs>
            (a => generator.StatusChanged += a, a => generator.StatusChanged -= a)
            .Select(a => generator.Status);

            return obs;
        }
        public static IObservable<GeneratorStatus> ContainersGeneratedChanges(this ItemContainerGenerator generator)
        {
            var obs = StatusChanges(generator).Where(a => a.Equals(GeneratorStatus.ContainersGenerated));

            return obs;
        }
    }
}