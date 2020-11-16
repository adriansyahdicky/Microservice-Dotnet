using Catalog.API.Infrastructure.Repositories;
using Catalog.API.IntegrationEvents.Events;
using Catalog.API.Model;
using Catalog.API.Model.Request;
using Catalog.API.Model.ViewModel;
using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IEventBus _eventBus;

        public CatalogService(
            ICatalogRepository catalogRepository,
            IEventBus eventBus
            )
        {
            _catalogRepository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task<bool> AddCatalogAsync(CatalogRequest catalogRequest)
        {
            await _catalogRepository.AddAsync(catalogRequest);
            PublishNewCatalogIntegrationEvent(catalogRequest);

            return true;
        }

        public Task<List<CatalogViewModel>> GetCatalogAsync()
        {
            return _catalogRepository.GetAsync();
        }

        private void PublishNewCatalogIntegrationEvent(CatalogRequest catalogRequest)
        {
            var @event = new CatalogUpdatedEvent(catalogRequest);

            //_logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            _eventBus.Publish(@event);
        }
    }
}
