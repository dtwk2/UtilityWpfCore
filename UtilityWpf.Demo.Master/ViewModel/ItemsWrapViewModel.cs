using System.Collections;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Master.ViewModel
{
    public class ItemsWrapViewModel
    {
        public ItemsWrapViewModel()
        {
        }

        public IEnumerable Collection { get; } = new FieldsFactory().BuildCollection();
    }
}
