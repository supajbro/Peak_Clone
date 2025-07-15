using UnityEngine;
using UnityEngine.UI;

public class PlayerMiddleDot : MonoBehaviour
{
    private Player _player;

    [SerializeField] private Image _image;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _climbSprite;
    [SerializeField] private Sprite _climbingSprite;

    public void Init(Player player)
    {
        _player = player;
    }

    private void Update()
    {
        if(_player == null)
        {
            return;
        }

        if (_player.CanClimb() && _player.CurrentState != IPlayerState.PlayerState.Climbing)
        {
            _image.sprite = _climbSprite;
        }
        else if (_player.CurrentState == IPlayerState.PlayerState.Climbing)
        {
            _image.sprite = _climbingSprite;
        }
        else
        {
            _image.sprite = _defaultSprite;
        }
    }
}
