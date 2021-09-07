using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
