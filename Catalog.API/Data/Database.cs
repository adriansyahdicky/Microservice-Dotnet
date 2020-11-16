using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    public class Database : IDatabase
    {
        string _connString;

        public Database(IConfiguration configuration)
        {
            _connString = configuration.GetValue<string>("ConnectionStrings:CatalogDB");
        }

        public string ConnString
        {
            get { return _connString; }
        }
    }
}
