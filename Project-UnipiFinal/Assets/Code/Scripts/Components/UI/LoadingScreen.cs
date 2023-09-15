using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [SerializeField] private GameObject _screen;

    public UnityEvent OnLoadFinish;

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

        Debug.Log("Before Wait..");
        yield return new WaitForSeconds(sec);

        OnLoadFinish.Invoke();

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
