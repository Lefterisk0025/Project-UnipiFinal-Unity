using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private List<Page> pages;

    Stack<GameObject> pagesStack = new Stack<GameObject>();

    private void Start()
    {
        pagesStack.Clear();

        TogglePage(Pages.MAINPAGE);
    }

    private void TogglePage(Pages page)
    {
        switch (page)
        {
            case Pages.MAINPAGE:
                break;
        }
    }
}
