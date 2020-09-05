using UtilityInterface.NonGeneric;

namespace UtilityWpf.Interactive.Demo.ViewModel
{
    public class TestViewModel : IName
    {
        public string Name => "Test";
    }

    public class Test1ViewModel : IName
    {
        public string Name => "Test";
    }

    public class Test2ViewModel : IName
    {
        public string Name => "A";
    }

    public class Test3ViewModel : IName
    {
        public string Name => "B";
    }

    public class Test4ViewModel : IName
    {
        public string Name => "C";
    }
}