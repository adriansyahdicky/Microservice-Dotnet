using Catalog.API.Model;
using Catalog.API.Model.Request;
using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.Events
{
    public class CatalogUpdatedEvent : IntegrationEvent
    {
        public CatalogRequest _catalogRequest  { get; set; }

        public CatalogUpdatedEvent(CatalogRequest catalogRequest)
        {
            _catalogRequest = catalogRequest;
        }
    }
}
