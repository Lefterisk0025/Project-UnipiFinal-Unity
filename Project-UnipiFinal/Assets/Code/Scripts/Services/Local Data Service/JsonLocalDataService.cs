using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JsonLocalDataService : ILocalDataService
{
    public bool SaveData<T>(string RelativePath, T Data, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath;

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
            }
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            return true;
        }
        catch (Exception e)
        {
            return false;
        }

    }

    public T LoadData<T>(string RelativePath, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public bool DeleteData(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
