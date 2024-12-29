using System.Data.Entity.Migrations.Model;
using Firework.Abstraction.Data;
using Firework.Core.Consts;
using Firework.Models.Metadata;

namespace Firework.Core.Data;

public class MetadataRepository : IDataRepository<Metadata>
{
    private readonly DbRepository _dbRepository;

    private readonly List<Metadata> _defaultMetadata =
    [
        new()
        {
            Id = Guid.NewGuid(),
            Value = "Firework",
            Name = MetadataNames.APP_NAME
        },

        new()
        {
            Id = Guid.NewGuid(),
            Name = MetadataNames.VERSION,
            Value = "0.0.0"
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
                            VALUES ({item.Id}, {item.Name}, {item.Value})
                            """;

        _dbRepository.Execute(insertSql);
    }

    public Metadata GetById(string id)
    {
        string getByIdSql = $"""
                            SELECT * FROM Settings WHERE {nameof(Metadata.Id)} = '{id}'
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
                            UPDATE Settings
                            SET Value = '{newItem.Value}', 
                            WHERE Id = '{newItem.Id}';
                            """;

        _dbRepository.Execute(updateSql);
    }

    public List<Metadata> GetAll()
    {
        throw new NotImplementedException();
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
                                    Id varchar(30) PRIMARY KEY,
                                    Name varchar(100) NOT NULL,
                                    Value varchar(100) NOT NULL
                                    )
                                """;

        _dbRepository.Execute(createTableSql);

        CreateDefaultValues();
    }

    private void CreateDefaultValues()
    {
        var allMetadata = GetAll().ExceptBy(_defaultMetadata.Select(x => x.Name), metadata => metadata.Name);

        foreach (Metadata item in allMetadata)
        {
            Insert(item);
        }
    }
}