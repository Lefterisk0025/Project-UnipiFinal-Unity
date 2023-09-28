using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using TMPro;

public class TileView : Subject, IPointerDownHandler
{
    public Tile Tile { get; set; }

    [SerializeField] private Sprite _defaultFrame;
    [SerializeField] private Sprite _selectedFrame;
    [SerializeField] private Sprite _deactivatedFrame;
    [SerializeField] private TMP_Text _valueText;

    Image image;

    private void Start()
    {
        image = GetComponent<Image>();

        UpdateView(TileState.Default);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Tile.IsActive)
            NotifyObservers(Actions.SELECT_TILE);
    }

    public void SetValue(int value)
    {
        _valueText.text = value.ToString();
    }

    public void UpdateView(TileState tileState)
    {
        switch (tileState)
        {
            case TileState.Default:
                image.sprite = _defaultFrame;
                _valueText.color = new Color32(255, 255, 255, 255);
                break;
            case TileState.Deactivated:
                image.sprite = _deactivatedFrame;
                _valueText.color = new Color32(75, 75, 75, 255);
                break;
            case TileState.Selected:
                image.sprite = _selectedFrame;
                break;
            default:
                break;
        }
    }
}
