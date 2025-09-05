using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations.Model;
using System.Reflection;
using Firework.Abstraction.Data;
using Firework.Core.Consts;
using Firework.Core.Settings;
using Firework.Models.Data;
using Firework.Models.Metadata;

namespace Firework.Core.Data;

public class MetadataRepository : IDataRepository<Metadata>
{
    private readonly DbRepository _dbRepository;

    private readonly List<Metadata> _defaultMetadata =
    [
        new()
        {
            Id = "metadata_app_name",
            Value = "Firework",
            Name = MetadataNames.APP_NAME
        },

        new()
        {
            Id = "metadata_version",
            Name = MetadataNames.VERSION,
            Value = "0.0.0"
        },
        new()
        {
            Id = "metadata_pipe_name",
            Name = SettingsDefault.Names.ServerPipeName,
            Value = SettingsDefault.Fields.ServerPipeName
        }
    ];

    public MetadataRepository(DbRepository dbRepository)
    {
        _dbRepository = dbRepository;

        Migrate();
    }

    public void Insert(Metadata item)
    {
        string insertSql = $"""
                            INSERT INTO Metadata ({nameof(item.Id)}, {nameof(item.Name)}, {nameof(item.Value)}) 
                            VALUES ('{item.Id}', '{item.Name}', '{item.Value}')
                            """;

        _dbRepository.Execute(insertSql);
    }

    public Metadata GetById(string id)
    {
        string getByIdSql = $"""
                            SELECT * FROM Metadata WHERE {nameof(Metadata.Id)} = '{id}'
                            """;

        return _dbRepository.ExecuteGetSingle<Metadata>(getByIdSql);
    }

    public Metadata FindBy(Predicate<Metadata> predicate)
    {
        var result = GetAll().FirstOrDefault(x => predicate(x));

        return result;
    }

    public List<Metadata> FindAll(Predicate<Metadata> predicate)
    {
        var result = GetAll().Where(x => predicate(x));

        return result.ToList();
    }


    public void Change(Metadata oldItem, Metadata newItem)
    {
        if (oldItem.Equals(newItem))
            return;

        string updateSql = $"""
                            UPDATE Metadata
                            SET Value = '{newItem.Value}',
                                Name = '{newItem.Name}'
                            WHERE Id = '{newItem.Id}';
                            """;

        _dbRepository.Execute(updateSql);
    }

    public List<Metadata> GetAll()
    {
        string getAllSql = "SELECT * FROM Metadata";

        return _dbRepository.ExecuteGetCollection<Metadata>(getAllSql);
    }

    public void Delete(Metadata item)
    {
        throw new NotImplementedException();
    }

    public void DeleteById(string id)
    {
        throw new NotImplementedException();
    }

    private void Migrate()
    {
        string createTableSql = """
                                CREATE TABLE IF NOT EXISTS Metadata (
                                    Id TEXT PRIMARY KEY,
                                    Name TEXT NOT NULL,
                                    Value TEXT NOT NULL
                                    )
                                """;

        _dbRepository.Execute(createTableSql);

        CreateDefaultValues();
    }

    private void CreateDefaultValues()
    {
        foreach (var item in _defaultMetadata)
        {
            var existingItem = GetById(item.Id);

            if (existingItem == null)
            {
                Insert(item);
            }
        }
    }
}