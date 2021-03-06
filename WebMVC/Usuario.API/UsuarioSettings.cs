﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Usuario.API
{
    public class UsuarioSettings
    {
        
            
        public string cacheConnection { get; set; }
        public double cacheTime { get; set; }
        public string ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public bool UseCache { get; set; }
        public bool UseCustomizationData { get; set; }
        public bool AzureStorageEnabled { get; set; }
    }
}
