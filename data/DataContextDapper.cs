using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace dot_net_api.Data
{

    class DataContextDapper
    {
        private readonly IConfiguration _configuration;

        public DataContextDapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<T> LoadData<T> (string sql)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return connection.Query<T>(sql);
        }

         public T LoadSingleData<T> (string sql)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return connection.QuerySingleOrDefault<T>(sql);
        }

        public bool SaveData<T> (string sql)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return connection.Execute(sql) > 0;
        }

        public int SaveDataWithRowsAffected<T> (string sql)
        {
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return connection.Execute(sql);
        }

        public bool executeSqlWithParams<T> (string sql, List<SqlParameter> parameters)
        {
            SqlCommand commandWithParams = new SqlCommand(sql);

            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            foreach (var param in parameters)
            {
                commandWithParams.Parameters.Add(param);
            }
            connection.Open();

            commandWithParams.Connection = connection;
            int rowsEffected = commandWithParams.ExecuteNonQuery();

            connection.Close();
            return rowsEffected > 0;


        }

    }

}