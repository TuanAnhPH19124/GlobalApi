using Dapper;
using GlobalApi.IRepositories;
using GlobalApi.Models;
using Npgsql;
using System.Data;

namespace GlobalApi.Repositories;

internal sealed class ProductRepository : IProductRepository
{
    private readonly IDbConnection _connection;

    public ProductRepository(IConfiguration configuration)
    {
        this._connection = new NpgsqlConnection(configuration.GetConnectionString("PostgreSql"));
    }
    public string Insert(Product product)
    {
        var sql = "INSERT INTO products (\"Name\", \"Photo\", \"State\", \"AvailableUnits\", \"Id\", \"City\", \"Laundry\", \"Wifi\") VALUES(@Name, @Photo, @State, @AvailableUnits, @Id, @City, @Laundry, @Wifi) RETURNING \"Id\";";
        _connection.Open();
        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                var id = _connection.QuerySingle<string>(sql, product, transaction);
                transaction.Commit();
                _connection.Close();
                return id;
            }
            catch (System.Exception)
            {
                transaction.Rollback();
                throw;
            }

        }

    }
    public void Remove(string Id)
    {
        var sql = "DELETE FROM products\r\nWHERE productid = @productId;\r\n";
        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                _connection.Execute(sql, new { @productId = Id }, transaction);
                transaction.Commit();
            }
            catch (System.Exception)
            {
                transaction.Rollback();
                throw;
            }

        }
    }
    public List<Product> SelectAll()
    {
        var sql = "select * from \"products\"";
        return _connection.Query<Product>(sql).ToList();

    }
    public Product SelectById(string Id)
    {
        var sql = "select * from products p where p.\"Id\" = @id;";
        return _connection.Query<Product>(sql, new
        {
            @id = Id
        }).Single();
    }
    public bool Update(Product product)
    {
        var sql = "update \"products\"  \r\nset \"Name\" = @Name,\r\n\"Photo\" = @Photo,\r\n\"State\" = @State,\r\n\"AvailableUnits\" = @AvailableUnits,\r\n\"City\" = @City,\r\n\"Laundry\" = @Laundry,\r\n\"Wifi\" = @Wifi \r\nwhere \"Id\" = @Id;";
        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                var rowEffected = _connection.Execute(sql, product, transaction);
                transaction.Commit();
                return true;
            }
            catch (System.Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}