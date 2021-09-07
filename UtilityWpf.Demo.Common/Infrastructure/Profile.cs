using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityWpf.Demo.Common.ViewModel;

namespace UtilityWpf.Demo.Common.Infrastructure
{
    public class Profile : AutoMapper.Profile
    {
        public Profile() : base(nameof(Common))
        {
            CreateMap<ReactiveFields, Fields>();
            CreateMap<Fields, ReactiveFields>();
        }
    }
}
