using Microsoft.Data.SqlClient;
using System.Data;

namespace WebAPI_CRUD.Model.Data
{
    public class DapperDBContext
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionstring;
        public DapperDBContext(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionstring = _configuration.GetConnectionString("connection");
        }
        public IDbConnection CreateConnection() => new SqlConnection(connectionstring);
    }
}
