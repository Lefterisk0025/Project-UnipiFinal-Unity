using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuManager : MonoBehaviour
{
    [SerializeField] private List<Menu> _menus;

    protected IDictionary<Menus, Menu> _menusDict;

    private Stack<Menu> _menusStack;

    private void Awake()
    {
        _menusStack = new Stack<Menu>();

        _menusDict = new Dictionary<Menus, Menu>();

        foreach (Menu menu in _menus)
        {
            _menusDict[menu.MenuType] = menu;

            menu.gameObject.SetActive(false);
        }
    }

    protected void ToggleMenu(Menu menu)
    {
        if (_menusStack.Count > 0)
        {
            _menusStack.Peek().gameObject.SetActive(false);
        }

        _menusStack.Push(menu);

        _menusStack.Peek().gameObject.SetActive(true);
    }

    public void GoBack()
    {
        _menusStack.Peek().gameObject.SetActive(false);

        _menusStack.Pop();

        _menusStack.Peek().gameObject.SetActive(true);
    }
}
