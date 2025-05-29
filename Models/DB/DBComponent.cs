using System.Collections.Generic;
using System;
using System.IO;
using Npgsql;
using DotNetEnv;
using System.Threading.Tasks;
using lab4.Interface;

namespace lab4.Models {
    public class DBComponent : IDB {
        ILogger Logger = new LoggerComponent();
        private string? connString;
        public DBComponent () {
            Env.Load(Path.Combine(AppContext.BaseDirectory, ".env.DB"));
            connString = $"Host={Env.GetString("PGHOST")};Port={Env.GetString("PGPORT")};Username={Env.GetString("PGUSER")};Password={Env.GetString("PGPASSWORD")};Database={Env.GetString("PGDATABASE")}";
        }

        public void ExecuteQuery(string sql, Dictionary<string, object>? parameters = null) {
            try {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();
                using var cmd = new NpgsqlCommand(sql, conn);
                if (parameters != null) {
                    foreach (var p in parameters) {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
                Logger.debug($"Запит виконано:\t{sql}");
            } catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
            }
        }

        public bool ExecuteBoolQuery(string sql, Dictionary<string, object>? parameters = null) {
            try {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();
                using var cmd = new NpgsqlCommand(sql, conn);
                if (parameters != null) {
                    foreach (var p in parameters) {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }
                var result = cmd.ExecuteScalar();
                cmd.Dispose();
                conn.Close();
                Logger.debug($"Запит виконано:\t{sql}");
                Logger.debug($"Результат виконання: {result}");
                return result != null && result is bool b && b;
            } catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> ExecuteBoolQueryAsync(string sql, Dictionary<string, object>? parameters = null) {
            try {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand(sql, conn);
                if (parameters != null) {
                    foreach (var p in parameters) {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }
                var result = await cmd.ExecuteScalarAsync();
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                Logger.debug($"Запит виконано:\t{sql}");
                Logger.debug($"Результат виконання: {result}");
                return result != null && result is bool b && b;
            }
            catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
                return false;
            }
        }
        public async Task ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null) {
            try {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand(sql, conn);
                if (parameters != null) {
                    foreach (var p in parameters) {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }
                await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                Logger.debug($"Запит виконано:\t{sql}");
            } catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
            }
        }
        public async Task<List<object[]>> ExecuteQueryResultAsync(string sql, Dictionary<string, object>? parameters = null) {
            var result = new List<object[]>();
            try {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand(sql, conn);
                if (parameters != null) {
                    foreach (var p in parameters) {
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    object[] row = new object[reader.FieldCount];
                    reader.GetValues(row); // Заповнює масив усіма колонками
                    result.Add(row);
                }
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                Logger.debug($"Запит виконано:\t{sql}");
            } catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
            }
            return result;
        }

    }
}