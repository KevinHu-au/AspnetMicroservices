using System;
using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _config;
        private const string CONNECTION_SETTINGS = "DatabaseSettings:ConnectionString";
        public DiscountRepository(IConfiguration config)
        {
            _config = config;
        }


        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_config.GetValue<string>(CONNECTION_SETTINGS));
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

            if (coupon == null)
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

            return coupon;
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_config.GetValue<string>(CONNECTION_SETTINGS));
            var result = await connection.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            return result > 0;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_config.GetValue<string>(CONNECTION_SETTINGS));
            var result = await connection.ExecuteAsync("UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id = @Id",
                new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            return result > 0;            
        }
        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_config.GetValue<string>(CONNECTION_SETTINGS));
            var result = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName=@ProductName",
                new { ProductName = productName });

            return result > 0;  
        }
    }
}
