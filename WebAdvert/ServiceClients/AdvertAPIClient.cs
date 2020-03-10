using AdvertApi.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAdvert.ServiceClients
{
    public class AdvertAPIClient : IAdvertAPIClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public AdvertAPIClient(IConfiguration configuration, HttpClient httpClient, IMapper mapper)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _mapper = mapper;

            var baseUrl = _configuration.GetSection("AdvertAPI").GetValue<string>("BaseUrl");
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("content-type", "application/json");
        }

        public async Task<bool> Confirm(ConfirmAdvertRequest model)
        {
            var advertModel = _mapper.Map<ConfirmAdvertModel>(model);

            var jsonModel = JsonConvert.SerializeObject(advertModel);

            var response = await _httpClient.PutAsync(new Uri($"{_httpClient.BaseAddress}/confirm"), new StringContent(jsonModel)).ConfigureAwait(false);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<AdvertResponse> Create(CreateAdvertModel model)
        {
            var advertAPIModel = _mapper.Map<AdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertAPIModel);

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress, new StringContent(jsonModel));

            var responseJson = await response.Content.ReadAsStringAsync();

            var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseJson);

            var advertResponse = _mapper.Map<AdvertResponse>(createAdvertResponse);

            return advertResponse;
        }
    }
}
