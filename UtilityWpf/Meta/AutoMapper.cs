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
   
}
