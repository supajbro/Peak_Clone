using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHead : MonoBehaviour
{
    private Player _target;
    private GameObject _endGoal;
    private Image _tracker;

    [SerializeField] private float _minX = -200f;  // Leftmost UI position
    [SerializeField] private float _maxX = 200f;   // Rightmost UI position

    public void Init(Player player, Image image, GameObject endGoal)
    {
        _target = player;
        _tracker = image;
        _endGoal = endGoal;
    }

    private void Update()
    {
        if (_target == null || _tracker == null || _endGoal == null)
        {
            return;
        }

        if(_target == GameManager.Instance.LocalPlayer)
        {
            transform.SetAsLastSibling();
            // Pulsate scale with sine wave
            float pulseSpeed = 4f;
            float pulseAmount = 0.1f;
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

            transform.localScale = new Vector3(scale, scale, 1f);
        }

        float height = _target.transform.position.y;
        float highestPoint = _endGoal.transform.position.y;

        // Invert so lower Y means more left (minX)
        float t = Mathf.InverseLerp(highestPoint, 0f, height);
        float xPos = Mathf.Lerp(_maxX, _minX, t);

        RectTransform trackerRect = GetComponent<RectTransform>();
        Vector2 anchoredPos = trackerRect.anchoredPosition;
        anchoredPos.x = xPos;
        trackerRect.anchoredPosition = anchoredPos;
    }
}
