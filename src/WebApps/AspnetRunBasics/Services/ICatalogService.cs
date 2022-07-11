using System;
using AspnetRunBasics.Models;

namespace AspnetRunBasics.Services
{
  public interface ICatalogService
  {
    Task<IEnumerable<CatalogModel>> GetCatalog();
    Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category);
    Task<CatalogModel> GetCatalog(string id);
    Task<CatalogModel> CreateCatalog(CatalogModel model);
  }
}
