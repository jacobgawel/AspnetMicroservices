using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection _connection;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connection = new NpgsqlConnection
                (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var coupon = await _connection.QueryFirstOrDefaultAsync<Coupon>
                // builds a query alongside the ProductName variable
                ("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

            if (coupon == null)
            {
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
            }

            return coupon;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var affected =
                await _connection.ExecuteAsync
                    ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var result = await _connection.ExecuteAsync
                ("UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id = @Id", 
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            if (result == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var result = await _connection.ExecuteAsync
                ("DELETE FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

            if(result == 0)
            {
                return false;
            }

            return true;
        }
    }
}
