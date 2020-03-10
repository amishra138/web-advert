using AutoMapper;
using WebAdvert.ServiceClients;
using WebAdvert.Views.AdvertManagement;

namespace WebAdvert.ViewProfiles
{
    public class WebsiteProfiles : Profile
    {
        public WebsiteProfiles()
        {
            CreateMap<CreateAdvertViewModel, CreateAdvertModel>().ReverseMap();
        }
    }
}
