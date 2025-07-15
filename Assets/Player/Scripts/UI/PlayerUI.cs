using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player _player;
    private LevelManager _manager;

    [SerializeField] private TextMeshProUGUI _playerState;
    [SerializeField] private Slider _stamineSlider;
    [SerializeField] private Image _tracker;
    public Image Tracker => _tracker;

    [SerializeField] private PlayerMiddleDot _dot;

    public void InitUI(Player thisPlayer)
    {
        _player = thisPlayer;
        _manager = FindObjectOfType<LevelManager>();
        _dot.Init(_player);
    }

    private void Start()
    {
        _player.OnPlayerStateChanged += UpdatePlayerStateText;
        _player.MyStamina.OnStaminaChanged += UpdateStaminaSlider;
    }

    private void UpdatePlayerStateText(IPlayerState.PlayerState newState)
    {
        _playerState.text = newState.ToString();
    }

    private void UpdateStaminaSlider(float value)
    {
        _stamineSlider.value = value;
    }
}
