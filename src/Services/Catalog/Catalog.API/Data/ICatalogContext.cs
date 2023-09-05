using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public interface ICatalogContext
    {
        // interface of the CatalogContext
        IMongoCollection<Product> Products { get; }
    }
}
