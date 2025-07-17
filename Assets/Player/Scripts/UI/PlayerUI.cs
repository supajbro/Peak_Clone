using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Managers")]
    private Player _player;
    private LevelManager _manager;

    [SerializeField] private TextMeshProUGUI _playerState;
    [SerializeField] private Slider _stamineSlider;

    [Header("Tracker")]
    [SerializeField] private Image _tracker;
    public Image Tracker => _tracker;

    [Header("Middle Dot")]
    [SerializeField] private PlayerMiddleDot _dot;

    [Header("Tutorial")]
    [SerializeField] private RectTransform _tutorialOpenPopup;
    [SerializeField] private RectTransform _tutorialPopup;
    [SerializeField] private CanvasGroup _tutorialBackPanel;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private Ease _openEase = Ease.OutBack;
    [SerializeField] private Ease _closeEase = Ease.InBack;
    [SerializeField] private Image _gameTitle;
    [SerializeField] private Button _nextPage;
    [SerializeField] private Button _previousPage;
    private bool _canOpenTutorial = false;
    private bool _tutorialOpen = false;
    private const float TutorialScaleTime = .5f;

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
        _tutorialBackPanel.GetComponent<Button>().onClick.AddListener(CloseTutorialPopup);
        _nextPage.onClick.AddListener(NextPage);
        _previousPage.onClick.AddListener(PreviousPage);
    }

    private void Update()
    {
        TutorialOpenUpdate();
    }

    #region - Stamina -
    private void UpdateStaminaSlider(float value)
    {
        _stamineSlider.value = value;
    }
    #endregion

    #region - TUTORIAL -
    private List<string> _dialogues = new();
    private int _tutorialIdx = -1;
    public void OpenTutorial(List<string> strings)
    {
        if (_canOpenTutorial)
        {
            return;
        }

        _canOpenTutorial = true;
        _tutorialOpenPopup.DOScale(1f, TutorialScaleTime).SetEase(_openEase);
        _dialogues = strings;
    }
    public void CloseTutorial()
    {
        if (!_canOpenTutorial)
        {
            return;
        }

        _canOpenTutorial = false;
        _tutorialOpenPopup.DOScale(0f, TutorialScaleTime).SetEase(_closeEase);
    }

    private void TutorialOpenUpdate()
    {
        if (_canOpenTutorial && Input.GetKeyDown(KeyCode.E))
        {
            if (_tutorialOpen)
                CloseTutorialPopup();
            else
                SetTutorialPopup();
        }

        if (_tutorialOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTutorialPopup();
        }
    }

    private void SetTutorialPopup()
    {
        _tutorialOpen = true;
        _player.SetMovement(false);
        _tutorialPopup.DOScale(1f, TutorialScaleTime).SetEase(_openEase);
        _tutorialBackPanel.DOFade(1f, TutorialScaleTime);
        _tutorialBackPanel.interactable = true;
        _tutorialBackPanel.blocksRaycasts = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _tutorialIdx = 0;
        _dialogue.text = _dialogues[_tutorialIdx];
        _gameTitle.gameObject.SetActive(true);
    }

    private void CloseTutorialPopup()
    {
        _tutorialOpen = false;
        _player.SetMovement(true);
        _tutorialPopup.DOScale(0f, TutorialScaleTime).SetEase(_closeEase);
        _tutorialBackPanel.DOFade(0f, TutorialScaleTime);
        _tutorialBackPanel.interactable = false;
        _tutorialBackPanel.blocksRaycasts = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void NextPage()
    {
        if (_tutorialIdx < _dialogues.Count - 1)
        {
            _tutorialIdx++;
        }

        _gameTitle.gameObject.SetActive(_tutorialIdx == 0);
        _dialogue.text = _dialogues[_tutorialIdx].Replace(@"\n", "\n");
    }


    private void PreviousPage()
    {
        if (_tutorialIdx > 0)
        {
            _tutorialIdx--;
        }

        _gameTitle.gameObject.SetActive(_tutorialIdx == 0);
        _dialogue.text = _dialogues[_tutorialIdx].Replace(@"\n", "\n");
    }
    #endregion

    #region - DEBUG -
    private void UpdatePlayerStateText(IPlayerState.PlayerState newState)
    {
        _playerState.text = newState.ToString();
    }
    #endregion
}
