﻿using PerfumeStore.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Services
{
  public interface ITokenService
  {
    public Task<string> GetToken(StoreUser user);
  }
}
