using Autofac;
using AutoMapper;
using AutoMapper.Configuration;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{

    public class AutoMapperSingleton
    {
        private IMapper mapper;

        private AutoMapperSingleton()
        {

            mapper = new MapperConfiguration(cfg => cfg.AddMaps(AssemblySingleton.Instance.Assemblies))
                .CreateMapper();

        }

        public static IMapper Instance { get; } = new AutoMapperSingleton().mapper;
    }



}
