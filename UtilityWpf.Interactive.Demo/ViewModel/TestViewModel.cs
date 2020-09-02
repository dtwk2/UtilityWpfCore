using System;
using System.Collections.Generic;
using System.Text;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.Interactive.Demo.ViewModel
{
    public class TestViewModel : IName
    {
        public string Name => "Test";
    }
}
