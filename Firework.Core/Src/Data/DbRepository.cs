using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Text;
using Dapper;
using Firework.Abstraction.Data;
using Firework.Models.Settings;
using Microsoft.Data.Sqlite;

namespace Firework.Core.Data;

public class DbRepository : IDisposable
{
    private string _filePath;
    private static SQLiteConnection _connection;

    public DbRepository()
    {
        _filePath = Path.Combine(Environment.CurrentDirectory, "firework.db");

        Init();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    private void Init()
    {
        if (!File.Exists(_filePath))
        {
            SQLiteConnection.CreateFile(_filePath);
        }

        _connection = new SQLiteConnection("Data Source=" + _filePath);
        _connection.Open();
    }

    public List<T> ExecuteGetCollection<T>(string sql, object param = null)
    {
        var result = _connection.Query<T>(sql, param);

        return result.ToList();
    }

    public T ExecuteGetSingle<T>(string sql, object param = null) where T : class
    {
        var result = _connection.Query<T>(sql, param);

        return result.FirstOrDefault();
    }

    public void Execute(string sql, object param = null)
    {
        _connection.Execute(sql, param);
    }
}