using GlobalApi.Models;

namespace GlobalApi.IRepositories;

public interface IProductRepository
{
    string Insert(Product product);
    List<Product> SelectAll();
    Product SelectById(string Id);
    bool Update(Product product);
    void Remove(string Id);
}