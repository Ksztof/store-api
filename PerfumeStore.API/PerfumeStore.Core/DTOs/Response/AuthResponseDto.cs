﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.DTOs.Response
{
  public class AuthResponseDto
  {
    public bool IsAuthSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    public string Token { get; set; }
  }
}
