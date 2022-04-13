using Utility.Common.EventArgs;

namespace UtilityWpf.Meta
{
    public class Profile : AutoMapper.Profile
    {
        public Profile() : base(nameof(UtilityWpf.Meta))
        {
            CreateMap<Abstract.CollectionItemEventArgs, CollectionItemEventArgs>();
            CreateMap<Abstract.CollectionEventArgs, CollectionEventArgs>();
            CreateMap<Abstract.MovementEventArgs, MovementEventArgs>();
            CreateMap<Abstract.CollectionItemChangedEventArgs, CollectionChangedEventArgs>();
        }
    }
}