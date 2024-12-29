namespace Firework.Abstraction.Data;

public interface IDataRepository<T>
{
    void Insert(T item);
    T GetById(string id);
    T FindBy(Predicate<T> predicate);
    List<T> FindAll(Predicate<T> predicate);
    void Change(T oldItem, T newItem);
    List<T> GetAll();
    void Delete(T item);
    void DeleteById(string id);
}