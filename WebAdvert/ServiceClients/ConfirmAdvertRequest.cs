﻿using AdvertApi.Models;

namespace WebAdvert.ServiceClients
{
    public class ConfirmAdvertRequest
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public AdvertStatus Status { get; set; }
    }
}
