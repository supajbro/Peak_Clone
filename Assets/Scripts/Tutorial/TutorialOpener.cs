using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOpened : MonoBehaviour
{
    private const float RayDistance = 5f;

    [SerializeField] private LayerMask _tutorialLayer;

    private Player _player;
    private PlayerUI _ui;

    private void Start()
    {
        _player = GetComponent<Player>();
        _ui = _player?.UI;
    }

    private void Update()
    {
        RaycastChecker();
    }

    private void RaycastChecker()
    {
        // Raycast against everything
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, RayDistance))
        {
            // Check if the hit object is on the tutorial layer
            if (((1 << hit.collider.gameObject.layer) & _tutorialLayer) != 0)
            {
                var dialogueSetter = hit.collider.GetComponent<DialogueSetter>();
                if (dialogueSetter != null)
                {
                    var dialogues = dialogueSetter.Dialogue;
                    _ui.OpenTutorial(dialogues);
                    return;
                }
            }
        }

        // If no hit, or hit is not on correct layer, close tutorial
        _ui.CloseTutorial();
    }
}
