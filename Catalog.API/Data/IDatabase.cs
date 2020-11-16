using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    internal interface IDatabase
    {
        string ConnString { get; }
    }
}
