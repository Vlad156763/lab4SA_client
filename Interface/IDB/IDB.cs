using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab4.Interface;
public interface IDB {
    void ExecuteQuery(string sql, Dictionary<string, object>? parameters = null);
    bool ExecuteBoolQuery(string sql, Dictionary<string, object>? parameters = null);
    Task<bool> ExecuteBoolQueryAsync(string sql, Dictionary<string, object>? parameters = null);
    Task ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null);
    Task<List<object[]>> ExecuteQueryResultAsync(string sql, Dictionary<string, object>? parameters = null);
}