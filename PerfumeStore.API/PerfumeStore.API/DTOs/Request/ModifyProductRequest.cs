﻿using PerfumeStore.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.API.DTOs.Request
{
    public class ModifyProductRequest
    {
        public ProductModificationRequest Product { get; set; }
    }
}