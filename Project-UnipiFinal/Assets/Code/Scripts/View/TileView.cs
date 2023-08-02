using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileView : Subject, IPointerDownHandler
{
    public Tile Tile { get; set; }

    IDictionary<TileState, Color> tileColorsOnState = new Dictionary<TileState, Color>(){
        {TileState.Default, new Color(255f, 255f, 255f, 255f)},
        {TileState.Deactivated, new Color(255f, 0f, 0f, 255f)},
        {TileState.Selected, new Color(0f, 190f, 255f, 255f)},
    };
    Image image;

    private void Awake()
    {

    }

    private void Start()
    {
        image = GetComponent<Image>();

        UpdateView(TileState.Default);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Tile.IsActive)
            NotifyObservers(Actions.SelectTile);
    }

    public void UpdateView(TileState tileState)
    {
        switch (tileState)
        {
            case TileState.Default:
                image.color = tileColorsOnState[TileState.Default];
                break;
            case TileState.Deactivated:
                image.color = tileColorsOnState[TileState.Deactivated];
                break;
            case TileState.Selected:
                image.color = tileColorsOnState[TileState.Selected];
                break;
            default:
                break;
        }
    }
}
