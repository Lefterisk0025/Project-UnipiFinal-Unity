using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMenu : MonoBehaviour
{
    int currentPanelIndex = 0;
    int prevPanelIndex = 0;
    List<GameObject> _menusGOsList = new List<GameObject>();

    private void Update()
    {
        if (_menusGOsList.Count <= 0)
            InitializeScroll();
    }

    private void InitializeScroll()
    {
        foreach (Transform child in transform)
        {
            _menusGOsList.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        _menusGOsList[currentPanelIndex].SetActive(true);
    }

    public void NextTarget()
    {
        prevPanelIndex = currentPanelIndex;

        _menusGOsList[currentPanelIndex].SetActive(false);

        currentPanelIndex++;
        if (currentPanelIndex > _menusGOsList.Count - 1)
        {
            currentPanelIndex = 0;
        }

        _menusGOsList[currentPanelIndex].SetActive(true);
    }

    public void PreviousTarget()
    {
        prevPanelIndex = currentPanelIndex;

        _menusGOsList[currentPanelIndex].SetActive(false);

        currentPanelIndex--;
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = _menusGOsList.Count - 1;
        }

        _menusGOsList[currentPanelIndex].SetActive(true);
    }
}
