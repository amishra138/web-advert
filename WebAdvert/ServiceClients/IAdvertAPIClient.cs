using AdvertApi.Models;
using System.Threading.Tasks;

namespace WebAdvert.ServiceClients
{
    public interface IAdvertAPIClient
    {
        Task<AdvertResponse> Create(CreateAdvertModel model);

        Task<bool> Confirm(ConfirmAdvertRequest model);
    }
}
