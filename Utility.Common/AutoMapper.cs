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

    public class AutoMapperTypeCollection : Collection<Type>
    {
        public AutoMapperTypeCollection(IReadOnlyCollection<Type> types)
        {
            this.Add(typeof(AutoMapperTypeCollection));
            foreach (var type in types)
                this.Add(type);
        }

        public AutoMapperTypeCollection(params Type[] types) : this(types as IReadOnlyCollection<Type>)
        {
        }
    }

    public class AutoMapperSingleton
    {
        private IMapper mapper;

        private AutoMapperSingleton()
        {
            //var types = Locator.Current.GetService<IEnumerable<AutoMapperTypeCollection>>();
            //if (types == null)
            //    throw new ArgumentNullException($"{nameof(AutoMapperTypeCollection)} is null");

            //mapper = new MapperConfiguration(cfg => cfg.AddMaps(types.SelectMany(a=>a))).CreateMapper();


            mapper = new MapperConfiguration(cfg => cfg.AddMaps(AssemblySingleton.Instance.Assemblies))
                .CreateMapper();

        }

        public static IMapper Instance { get; } = new AutoMapperSingleton().mapper;
    }



}
