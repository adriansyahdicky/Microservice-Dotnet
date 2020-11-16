using Catalog.API.Data;
using Catalog.API.IntegrationEvents.Events;
using Catalog.API.Model;
using Catalog.API.Model.Request;
using Catalog.API.Model.ViewModel;
using Dapper;
using EventBus.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly Database _connectionString;
        private readonly IEventBus _eventBus;
        //private readonly CatalogContext _context;

        public CatalogRepository(IConfiguration configuration, IEventBus eventBus)
        {
            _connectionString = new Database(configuration);
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            //_context = catalogContext;
        }

        public async Task<bool> AddAsync(CatalogRequest catalogRequest)
        {
            //cek di database
            using(var con = new NpgsqlConnection(_connectionString.ConnString))
            {
                await con.OpenAsync();
                var catalog = con.Query<CatalogViewModel>(@"select * from catalog where id_catalog=@idCatalog;", new { catalogRequest.idCatalog }).FirstOrDefault();
                
                if(catalog != null)
                {

                }
                else
                {

                }
            }

            return true;
        }

        public async Task<List<CatalogViewModel>> GetAsync()
        {
            using (var con = new NpgsqlConnection(_connectionString.ConnString))
            {
                await  con.OpenAsync();
                return con.Query<CatalogViewModel>(@"select * from catalog").ToList();
            }
        }

        private void PublishNewCatalogIntegrationEvent(CatalogRequest catalogRequest)
        {
            var @event = new CatalogUpdatedEvent(catalogRequest);

            //_logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

            _eventBus.Publish(@event);
        }
    }
}
