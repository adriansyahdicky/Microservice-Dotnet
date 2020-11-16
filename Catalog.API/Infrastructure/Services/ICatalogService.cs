using Catalog.API.Model;
using Catalog.API.Model.Request;
using Catalog.API.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.Services
{
    public interface ICatalogService
    {
        Task<List<CatalogViewModel>> GetCatalogAsync();
        Task<bool> AddCatalogAsync(CatalogRequest catalogRequest);
    }
}
