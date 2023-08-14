using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> _attackTargetsPanels;

    int currentPanelIndex = 0;
    int prevPanelIndex = 0;

    private void OnEnable()
    {
        foreach (GameObject go in _attackTargetsPanels)
        {
            go.SetActive(false);
        }

        _attackTargetsPanels[0].SetActive(true);
    }

    public void NextTarget()
    {
        prevPanelIndex = currentPanelIndex;

        _attackTargetsPanels[currentPanelIndex].SetActive(false);

        currentPanelIndex++;
        if (currentPanelIndex > _attackTargetsPanels.Count - 1)
        {
            currentPanelIndex = 0;
        }

        _attackTargetsPanels[currentPanelIndex].SetActive(true);
    }

    public void PreviousTarget()
    {
        prevPanelIndex = currentPanelIndex;

        _attackTargetsPanels[currentPanelIndex].SetActive(false);

        currentPanelIndex--;
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = _attackTargetsPanels.Count - 1;
        }

        _attackTargetsPanels[currentPanelIndex].SetActive(true);
    }
}
