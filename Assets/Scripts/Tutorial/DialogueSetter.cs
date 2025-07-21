using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSetter : MonoBehaviour
{
    [SerializeField] private List<string> _dialogues;
    public List<string> Dialogue => _dialogues;

    [SerializeField] private bool _showTitle = true;
    public bool ShowTitle => _showTitle;
}
