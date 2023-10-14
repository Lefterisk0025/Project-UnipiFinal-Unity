using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionTextView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void OnEnable()
    {
        _text.text = "Version: " + Application.version;
    }
}
