using Dapper;
using MySqlConnector;
using System.Data;

namespace TMCC.Db_Helper
{
    public class DapperHelper
    {
        private readonly string _connectionString;

        public DapperHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBConnection");
        }

        private IDbConnection GetConnection() => new MySqlConnection(_connectionString);

        // ------------------- Async SQL Queries -------------------
        public async Task<int> ExecuteAsync(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using var db = GetConnection();
            return await db.ExecuteAsync(sql, param, commandType: commandType);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using var db = GetConnection();
            return await db.QueryAsync<T>(sql, param, commandType: commandType);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using var db = GetConnection();
            return await db.QueryFirstOrDefaultAsync<T>(sql, param, commandType: commandType);
        }

        // ------------------- Stored Procedure Methods -------------------
        public async Task<int> ExecuteNonQueryAsync(string procName, DynamicParameters parameters)
        {
            using var db = GetConnection();
            return await db.ExecuteAsync(procName, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<T> QueryFirstOrDefaultSPAsync<T>(string procName, DynamicParameters parameters)
        {
            using var db = GetConnection();
            return await db.QueryFirstOrDefaultAsync<T>(procName, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
