using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [SerializeField] private GameObject _screen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _screen.SetActive(false);
    }

    public void FakeOpen(int forSec)
    {
        StartCoroutine(FakeLoad(forSec));
    }

    private IEnumerator FakeLoad(int sec)
    {
        _screen.SetActive(true);

        yield return new WaitForSeconds(sec);

        _screen.SetActive(false);
    }

    public void Open()
    {
        _screen.SetActive(true);
    }

    public void Close()
    {
        _screen.SetActive(false);
    }
}
