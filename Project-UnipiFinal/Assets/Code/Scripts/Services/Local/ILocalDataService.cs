using System.Threading.Tasks;

public interface ILocalDataService
{
    Task<bool> SaveData<T>(string RelativePath, T Data, bool Encrypted);

    Task<T> LoadData<T>(string RelativePath, bool Encrypted);

    Task<bool> DeleteData(string RelativePath);
}
