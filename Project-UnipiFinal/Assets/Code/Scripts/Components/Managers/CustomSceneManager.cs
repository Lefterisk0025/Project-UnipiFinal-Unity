using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    public static CustomSceneManager Instance { get; private set; }

    public UnityEvent OnSceneUnloaded = new UnityEvent();
    public UnityEvent OnSceneLoaded = new UnityEvent();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchScene(string sceneToUnload, string sceneToLoad)
    {
        StartCoroutine(SceneTransitionCoroutine(sceneToUnload, sceneToLoad));
    }

    private IEnumerator SceneTransitionCoroutine(string sceneToUnload, string sceneToLoad)
    {
        if (SceneManager.GetSceneByName(sceneToUnload).isLoaded)
        {
            var unloadOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
            OnSceneUnloaded.Invoke();
        }

        yield return new WaitForSeconds(1);

        var loadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }
        OnSceneLoaded.Invoke();
    }
}
