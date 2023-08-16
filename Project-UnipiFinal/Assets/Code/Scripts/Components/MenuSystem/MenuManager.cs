using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField] private List<MenuEntity> _menus;

    protected IDictionary<Menu, MenuEntity> _menusDict;

    private Stack<MenuEntity> _menusStack;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _menusStack = new Stack<MenuEntity>();

        _menusDict = new Dictionary<Menu, MenuEntity>();

        foreach (MenuEntity menuEntity in _menus)
        {
            _menusDict[menuEntity.MenuType] = menuEntity;

            menuEntity.gameObject.SetActive(false);
        }
    }

    public void ToggleMenu(Menu menu)
    {
        if (_menusStack.Count > 0)
        {
            _menusStack.Peek().gameObject.SetActive(false);
        }

        _menusStack.Push(_menusDict[menu]);

        _menusStack.Peek().gameObject.SetActive(true);
    }

    public void GoBack()
    {
        _menusStack.Peek().gameObject.SetActive(false);

        _menusStack.Pop();

        _menusStack.Peek().gameObject.SetActive(true);
    }
}
