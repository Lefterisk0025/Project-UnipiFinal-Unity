using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMenu : MonoBehaviour
{
    int currentPanelIndex = 0;
    int prevPanelIndex = 0;
    public List<GameObject> _itemGOsList = new List<GameObject>();

    private void Update()
    {
        if (_itemGOsList.Count == 0)
            InitializeScroll();
    }

    public void InitializeScroll()
    {
        currentPanelIndex = 0;
        prevPanelIndex = 0;

        foreach (Transform child in transform)
        {
            _itemGOsList.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        _itemGOsList[currentPanelIndex].SetActive(true);
    }

    public void ClearScroll()
    {
        _itemGOsList.Clear();
    }

    public void NextTarget()
    {
        prevPanelIndex = currentPanelIndex;

        _itemGOsList[currentPanelIndex].SetActive(false);

        currentPanelIndex++;
        if (currentPanelIndex > _itemGOsList.Count - 1)
        {
            currentPanelIndex = 0;
        }

        _itemGOsList[currentPanelIndex].SetActive(true);
    }

    public void PreviousTarget()
    {
        prevPanelIndex = currentPanelIndex;

        _itemGOsList[currentPanelIndex].SetActive(false);

        currentPanelIndex--;
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = _itemGOsList.Count - 1;
        }

        _itemGOsList[currentPanelIndex].SetActive(true);
    }
}
