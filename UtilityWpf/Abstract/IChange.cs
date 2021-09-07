using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UtilityWpf.Abstract
{


    public interface IChange
    {
        event CollectionChangedEventHandler Change;
    }
}
