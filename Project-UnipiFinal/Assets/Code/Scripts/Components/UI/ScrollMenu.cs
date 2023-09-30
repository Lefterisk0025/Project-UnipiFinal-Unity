using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class ScrollMenu : MonoBehaviour
{
    int currentPanelIndex = 0;
    int prevPanelIndex = 0;
    List<GameObject> _itemGOsList = new List<GameObject>();

    [SerializeField] private List<GameObject> _navDotsList;

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

        for (int i = 0; i < _navDotsList.Count; i++)
        {
            ResetSelectedNavDot(i);
        }

        _itemGOsList[currentPanelIndex].SetActive(true);
        SetSelectedNavDot(currentPanelIndex);
    }

    public void ClearScroll()
    {
        _itemGOsList.Clear();
    }

    public void NextTarget()
    {
        prevPanelIndex = currentPanelIndex;

        _itemGOsList[currentPanelIndex].SetActive(false);
        ResetSelectedNavDot(currentPanelIndex);

        currentPanelIndex++;
        if (currentPanelIndex > _itemGOsList.Count - 1)
        {
            currentPanelIndex = 0;
        }

        _itemGOsList[currentPanelIndex].SetActive(true);
        SetSelectedNavDot(currentPanelIndex);
    }

    public void PreviousTarget()
    {
        prevPanelIndex = currentPanelIndex;

        _itemGOsList[currentPanelIndex].SetActive(false);
        ResetSelectedNavDot(currentPanelIndex);

        currentPanelIndex--;
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = _itemGOsList.Count - 1;
        }

        _itemGOsList[currentPanelIndex].SetActive(true);
        SetSelectedNavDot(currentPanelIndex);
    }

    private void SetSelectedNavDot(int index)
    {
        _navDotsList[index].GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);
        _navDotsList[index].GetComponent<Image>().color = new Color32(229, 1, 71, 255);
    }

    private void ResetSelectedNavDot(int index)
    {
        _navDotsList[index].GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
        _navDotsList[index].GetComponent<Image>().color = new Color32(255, 255, 225, 255);
    }
}
