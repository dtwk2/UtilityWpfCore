using Utility.WPF.Demo.Date.Infrastructure.Entity;
using Utility.WPF.Demo.Date.Infrastructure.ViewModel;

namespace Utility.WPF.Demo.Date.Infrastructure.Map
{

    public class Profile : AutoMapper.Profile
    {
        public Profile() : base(nameof(UtilityWpf.Meta))
        {
            CreateMap<NoteEntity, NoteViewModel>().ReverseMap();
        }
    }

}
