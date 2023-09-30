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
        Debug.Log($"<color=green>Start saving {RelativePath}</color>");
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
                string jsonData = JsonConvert.SerializeObject(Data, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                await writer.WriteAsync(jsonData);
                writer.Close();
            }
            stream.Close();

            Debug.Log($"<color=yellow>Finish saving</color>");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>An error occured while saving</color>");
            Debug.Log(e.Message);
            return false;
        }
    }

    public async Task<T> LoadData<T>(string RelativePath, bool Encrypted)
    {

        string path = _persistentDataPath + RelativePath;
        Debug.Log($"<color=green>Start loading {RelativePath}</color>");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
            Debug.Log($"<color=yellow>Finish loading</color>");
            return data;
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>An error occured while loading</color>");
            Debug.Log(e.Message);
            throw e;
        }
    }

    public async Task<bool> DeleteData(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;
        Debug.Log($"<color=green>Start deleting {RelativePath}</color>");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            await Task.Run(() => File.Delete(path));
            Debug.Log($"<color=yellow>Finish Deleting</color>");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>An error occured while Deleting</color>");
            Debug.Log(e.Message);
            return false;
        }
    }
}
