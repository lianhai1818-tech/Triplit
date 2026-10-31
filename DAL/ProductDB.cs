using MySql.Data.MySqlClient;
using Model;

namespace DAL
{
    public class ProductDB : BaseDB
    {
        public ProductDB(string connectionString) : base(connectionString) { }

        public async Task<int> InsertProductAsync(Product product)
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            string query = @"INSERT INTO PRODUCTS (product_name, price, stock_quantity) 
                            VALUES (@productName, @price, @stockQuantity)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@productName", product.ProductName);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@stockQuantity", product.StockQuantity);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                product.Id = (int)cmd.LastInsertedId;
            }

            return rowsAffected;
        }
    }
}