using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;

public class JsonLocalDataService : ILocalDataService
{
    string _persistentDataPath;

    public JsonLocalDataService()
    {
        _persistentDataPath = Application.persistentDataPath;
    }

    public async Task<bool> SaveData<T>(string RelativePath, T Data, bool Encrypted)
    {
        string path = _persistentDataPath + RelativePath;

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Debug.Log("File doesn't exists!");
            }

            using FileStream stream = File.Create(path);
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string jsonData = JsonConvert.SerializeObject(Data, Formatting.Indented);
                await writer.WriteAsync(jsonData);
            }
            stream.Close();

            return true;
        }
        catch (Exception e)
        {
            using FileStream stream = File.Create(_persistentDataPath + "/errors.txt");
            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(e.Message);
            }
            stream.Close();
            return false;
        }
    }

    public async Task<T> LoadData<T>(string RelativePath, bool Encrypted)
    {
        string path = _persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
            return data;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public async Task<bool> DeleteData(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            await Task.Run(() => File.Delete(path));
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }
}
