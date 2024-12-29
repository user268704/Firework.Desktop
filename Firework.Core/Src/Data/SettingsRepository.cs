using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Firework.Abstraction.Data;
using Firework.Core.Settings;
using Firework.Models.Data;

namespace Firework.Core.Data;

public class SettingsRepository : IDataRepository<SettingsItem>
{
    private readonly DbRepository _dbRepository;

    public SettingsRepository(DbRepository dbRepository)
    {
        _dbRepository = dbRepository;

        MigrateSettings();
    }

    public void Insert(SettingsItem item)
    {
        string insertSql =
                    $"""
                     INSERT INTO Settings ({nameof(item.UniqueKey)}, {nameof(item.DisplayName)}, {nameof(item.Value)}) VALUES
                         ('{item.UniqueKey}', '{item.DisplayName}', '{item.Value}');
                     """;

        _dbRepository.Execute(insertSql);
    }

    public SettingsItem GetById(string id)
    {
        string selectSql = $"SELECT UniqueKey, DisplayName, Value FROM Settings WHERE UniqueKey = '{id}'";

        return _dbRepository.ExecuteGetSingle<SettingsItem>(selectSql);
    }

    public SettingsItem FindBy(Predicate<SettingsItem> predicate)
    {
        var result = GetAll().FirstOrDefault(x => predicate(x));

        return result;
    }

    public List<SettingsItem> FindAll(Predicate<SettingsItem> predicate)
    {
        var result = GetAll().Where(x => predicate(x));

        return result.ToList();
    }

    public void Change(SettingsItem oldItem, SettingsItem newItem)
    {
        if (oldItem.Equals(newItem))
            return;

        string updateSql = $"""
                           UPDATE Settings
                           SET Value = '{newItem.Value}', 
                           DisplayName = '{newItem.DisplayName}'
                           WHERE UniqueKey = '{newItem.UniqueKey}';
                           """;

        _dbRepository.Execute(updateSql);
    }

    public List<SettingsItem> GetAll()
    {
        string getAllSql = "SELECT * FROM Settings";

        return _dbRepository.ExecuteGetCollection<SettingsItem>(getAllSql);
    }

    public void Delete(SettingsItem item)
    {
        throw new NotImplementedException();
    }

    public void DeleteById(string id)
    {
        throw new NotImplementedException();
    }

    private void MigrateSettings()
    {
        string createTableSql = """
                                CREATE TABLE IF NOT EXISTS Settings (
                                    UniqueKey varchar(30) PRIMARY KEY,
                                    DisplayName varchar(100) NOT NULL,
                                    Value varchar(100) NOT NULL
                                    )
                                """;

        _dbRepository.Execute(createTableSql);

        CreateDefaultValues();
    }

    private void CreateDefaultValues()
    {
        var names = typeof(SettingsDefault.Fields).GetProperties().Select(x =>
        {
            var defaultValue = x.GetValue(null).ToString();
            var displayName = x.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

            return new
            {
                x.Name,
                DefaultValue = defaultValue,
                DisplayName = displayName.Name
            };
        }).ToList();

        foreach (var item in names)
        {
            var oldValue = GetById(item.Name);

            var newItem = new SettingsItem
            {
                UniqueKey = item.Name,
                Value = item.DefaultValue,
                DisplayName = item.DisplayName
            };

            if (oldValue == null)
            {
                Insert(newItem);
            }
        }
    }
}