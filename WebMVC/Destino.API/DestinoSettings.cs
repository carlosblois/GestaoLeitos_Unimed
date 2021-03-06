﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destino.API
{
    public class DestinoSettings
    {
        public string PicBaseUrl { get; set; }

        public string EventBusConnection { get; set; }

        public bool UseCustomizationData { get; set; }
        public bool AzureStorageEnabled { get; set; }
    }
}
