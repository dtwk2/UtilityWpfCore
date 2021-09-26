using Autofac;
using Utility.Common;

namespace UtilityWpf.Demo.Common.Meta
{
    class BootStrapper : IBootStrapper
    {
        public void Register(ContainerBuilder containerBuilder)
        {
          
            containerBuilder.RegisterType<FactoryLogger>().SingleInstance();
            containerBuilder.RegisterType<Factory>().SingleInstance();
            containerBuilder.RegisterType<KeyStore>().SingleInstance();
        }
    }
}
