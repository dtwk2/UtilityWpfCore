using AutoMapper;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Common.EventArgs;

namespace UtilityWpf.Meta
{
    public class Profile : AutoMapper.Profile
    {
        public Profile() : base(nameof(UtilityWpf.Meta))
        {

            CreateMap<Abstract.CollectionEventArgs, CollectionEventArgs>();
            CreateMap<Abstract.MovementEventArgs, MovementEventArgs>();
            CreateMap<Abstract.CollectionChangedEventArgs, CollectionChangedEventArgs>();
        }
    }

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

    class AutoMapperSingleton
    {
        private readonly IMapper mapper;

        private AutoMapperSingleton()
        {
            var types = Locator.Current.GetService<AutoMapperTypeCollection>();
            if (types == null)
                throw new ArgumentNullException($"{nameof(AutoMapperTypeCollection)} is null");

            mapper = new MapperConfiguration(cfg => cfg.AddMaps(types)).CreateMapper();
        }

        public static IMapper Instance { get; } = new AutoMapperSingleton().mapper;
    }
}
