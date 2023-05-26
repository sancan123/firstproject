using ShoppingMallSys.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace ShoppingMallSys.Services
{
    public class AuthorRepository : IAuthorRepository
    {

        public async Task<Author> GetByKey(int key)
        {
            string connectionString = "";
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT pKey, FirstName, LastName FROM Author WHERE pKey = @key"; if (connection.State != ConnectionState.Open) connection.Open();
                var result = await connection.QueryAsync<Author>(query, new { pKey = key });
                return result.FirstOrDefault();
            }
        }
    }
}
