using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAdvert.ServiceClients;
using WebAdvert.Views.AdvertManagement;

namespace WebAdvert.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IAdvertAPIClient _advertAPIClient;
        private readonly IMapper _mapper;

        public AdvertManagementController(IAdvertAPIClient advertAPIClient, IMapper mapper)
        {
            _advertAPIClient = advertAPIClient;
            _mapper = mapper;
        }

        public IActionResult Create(CreateAdvertModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreateAdvertViewModel model)
        {
            if (ModelState.IsValid)
            {
                var advertModel = _mapper.Map<CreateAdvertModel>(model);

                var response = await _advertAPIClient.Create(advertModel);

                if (string.IsNullOrEmpty(response.Id))
                {
                    return View(model);
                }

                await _advertAPIClient.Confirm(new ConfirmAdvertRequest()
                {
                    Id = response.Id,
                    Status = AdvertApi.Models.AdvertStatus.Active
                });
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
