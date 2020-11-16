using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Catalog.API.Infrastructure.Repositories;
using Catalog.API.Infrastructure.Services;
using Catalog.API.Model.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogController(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateOrUpdateUserCatalogAsync([FromBody] CatalogRequest newCatReq)
        {
            var result = await _catalogRepository.AddAsync(newCatReq);

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
