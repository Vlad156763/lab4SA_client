using System.Collections.Generic;
using System;
using System.IO;
using Npgsql;
using DotNetEnv;
using System.Threading.Tasks;

namespace lab4.Models {
    public static class DB {
        private static string? connString;
        static DB() {
            Env.Load( Path.GetFullPath( Path.Combine( AppContext.BaseDirectory, "..", "..", "..", ".env.DB" ) ) );
            connString = $"Host={Env.GetString("PGHOST")};Port={Env.GetString("PGPORT")};Username={Env.GetString("PGUSER")};Password={Env.GetString("PGPASSWORD")};Database={Env.GetString("PGDATABASE")}";
        }

        public static void ExecuteQuery(string sql, Dictionary<string, object>? parameters = null) {
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
                Logger.info($"Запит виконано:\t{sql}");
            } catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
            }
        }

        public static bool ExecuteBoolQuery(string sql, Dictionary<string, object>? parameters = null) {
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
                Logger.info($"Запит виконано:\t{sql}");
                Logger.info($"Результат виконання: {result}");
                return result != null && result is bool b && b;
            } catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
                return false;
            }
        }
        public static async Task<bool> ExecuteBoolQueryAsync(string sql, Dictionary<string, object>? parameters = null) {
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
                Logger.info($"Запит виконано:\t{sql}");
                Logger.info($"Результат виконання: {result}");
                return result != null && result is bool b && b;
            }
            catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
                return false;
            }
        }
        public static async Task ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null) {
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
                Logger.info($"Запит виконано:\t{sql}");
            } catch (Exception ex) {
                Logger.error($"Помилка: {ex.Message}");
            }
        }

    }
}