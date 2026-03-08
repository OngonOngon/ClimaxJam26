using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Required for loading scenes
using TMPro;
using System;
using Dubinci;
using System.Collections;
using System.Collections.Generic;

public enum TypingMode { Story, Command }

public static class CInput
{
    public static string GetCapturedInput() => Input.inputString;
    // Tab is now dedicated to Story mode
    public static bool IsStoryTriggered() => Input.GetKeyDown(KeyCode.Tab);
    public static bool IsSubmitTriggered() => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
}

public interface ITypingHandler
{
    bool OnLineCompleted(string completedLine);
    bool OnCommandExecuted(string command);
    void OnAllLinesCompleted();
}

public interface ITypingSoundProvider
{
    void PlayCharSound(char c);
    void PlaySuccess();
    void PlayFail();
}

public class typingScript : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private TypingMode mode = TypingMode.Story;
    [SerializeField] private TextMeshProUGUI displayLabel;
    [SerializeField] private Image availableBGL;
    [SerializeField] private Image availableBGR;
    [SerializeField] private TextMeshProUGUI availableTextsL;
    [SerializeField] private TextMeshProUGUI availableTextsR;

    [Header("Story Mode Settings")]
    [TextArea(3, 5)]
    [SerializeField] private string referenceText;

    [Header("Command Mode Settings")]
    [SerializeField] private List<CommandSO> validCommands;
    [SerializeField] private string commandPrefix = "> ";

    [Header("Colors")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color idleColor = Color.gray;
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private float flashDuration = 0.25f;

    private List<string> _lines = new List<string>();
    private int _currentLineIndex = 0;
    private string _playerInput = "";
    private string _failedInput = "";
    private bool _isActive = false;
    private bool _isFlashingError = false;

    private ITypingSoundProvider _soundProvider;
    private Dubinci.Cursor _activeCursor;

    public ITypingHandler Handler { get; set; }
    public bool IsActive => _isActive;

    void Awake()
    {
        // Interface-compliant search for sound provider
        _soundProvider = GetComponent<ITypingSoundProvider>();
        if (_soundProvider == null)
        {
            var providers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var p in providers)
            {
                if (p is ITypingSoundProvider found) { _soundProvider = found; break; }
            }
        }
    }

    void Start()
    {
        PrepareStoryText();
        if (displayLabel != null) displayLabel.gameObject.SetActive(false);

        // HACK: Hardcoded check for the main menu scene to start automatically
        if (SceneManager.GetActiveScene().name == "VojtaMenuTest")
        {
            ActivateStoryMode();
        }
    }

    void Update()
    {
        if (availableBGL != null) availableBGL.enabled = false;
        if (availableBGR != null) availableBGR.enabled = false;
        if (availableTextsL != null) availableTextsL.text = "";
        if (availableTextsR != null) availableTextsR.text = "";

        // Handle dedicated Story Mode activation (Tab)
        if (CInput.IsStoryTriggered())
        {
            // HACK: Disable Tab toggling in the main menu so the player doesn't accidentally close it
            if (SceneManager.GetActiveScene().name != "VojtaMenuTest")
            {
                if (!_isActive) ActivateStoryMode();
                else DeactivateSystem(); // Toggle off if already active
            }
        }

        if (!_isActive || _isFlashingError) return;

        HandleTyping();
    }

    // Called internally via Tab
    public void ActivateStoryMode()
    {
        _isActive = true;
        mode = TypingMode.Story;
        ResetInputState();
        ShowUI(true);
    }

    // Called externally by the Cursor via Enter
    public void ActivateCommandMode(Dubinci.Cursor caller)
    {
        _activeCursor = caller;
        _isActive = true;
        mode = TypingMode.Command;
        ResetInputState();
        ShowUI(true);
    }

    // Completely shuts down the typing interface
    public void DeactivateSystem()
    {
        _isActive = false;
        ResetInputState();
        ShowUI(false);

        // Notify cursor to unlock if it was a command session
        if (_activeCursor != null)
        {
            _activeCursor.ExitSelection();
            _activeCursor = null;
        }
    }

    private void ResetInputState()
    {
        _playerInput = "";
        _failedInput = "";
    }

    private void ShowUI(bool state)
    {
        if (displayLabel != null)
        {
            displayLabel.gameObject.SetActive(state);
            if (state) UpdateVisuals();
        }
    }

    private void PrepareStoryText()
    {
        if (string.IsNullOrEmpty(referenceText)) return;
        string[] split = referenceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        _lines.Clear();
        _lines.AddRange(split);
    }

    private void HandleTyping()
    {
        string input = CInput.GetCapturedInput();

        if (mode == TypingMode.Story)
            HandleStoryInput(input);
        else
            HandleCommandInput(input);

        UpdateVisuals();
    }

    private void HandleStoryInput(string input)
    {
        string target = _lines[_currentLineIndex];

        foreach (char c in input)
        {
            if (c == '\r' || c == '\n') continue;
            if (c == '\b')
            {
                if (_playerInput.Length > 0) _playerInput = _playerInput.Substring(0, _playerInput.Length - 1);
                continue;
            }

            // allow only letters and space
            if (!char.IsLetter(c) && c != ' ') continue;

            int idx = _playerInput.Length;

            if (idx >= target.Length) break;

            char inputCharLower = char.ToLower(c);
            char targetLower = char.ToLower(target[idx]);

            if (idx < target.Length && inputCharLower == targetLower)
            {
                _playerInput += inputCharLower;
                _soundProvider?.PlayCharSound(inputCharLower);
            }
            else
            {
                TriggerError(_playerInput + inputCharLower);
                break;
            }
        }

        if (_playerInput.Equals(target, StringComparison.OrdinalIgnoreCase))
        {
            CompleteStoryLine();
        }
    }

    private void HandleCommandInput(string input)
    {
        if (availableTextsL != null)
        {
            availableBGL.enabled = true;
            foreach (var c in validCommands)
            {
                if (c.ValidCommand())
                    availableTextsL.text += c.text + "\n";
            }
        }

        if (availableTextsR != null)
        {
            availableBGR.enabled = true;
            foreach (var c in validCommands)
            {
                if (c.ValidCommand())
                    availableTextsR.text += c.text + "\n";
            }
        }

        foreach (char c in input)
        {
            if (c == '\r' || c == '\n') continue;
            if (c == '\b')
            {
                if (_playerInput.Length > 0) _playerInput = _playerInput.Substring(0, _playerInput.Length - 1);
                continue;
            }
            _playerInput += c;
            _soundProvider?.PlayCharSound(c);
        }

        if (CInput.IsSubmitTriggered() && !string.IsNullOrEmpty(_playerInput))
        {
            string cmd = _playerInput.Trim().ToLower();
            bool commandFound = false;

            foreach (var c in validCommands)
            {
                if (c != null && c.ValidCommand() && c.TryCommand(cmd))
                {
                    commandFound = true;

                    // 1. NEJDŘÍV se zeptáme na peníze
                    bool canAfford = (Handler == null) || Handler.OnCommandExecuted(cmd);

                    if (canAfford)
                    {
                        // 2. AŽ TEĎ reálně stavíme (pokud je to build příkaz)
                        if (c is BuildCommandSO buildCmd)
                        {
                            buildCmd.Execute();
                        }

                        _soundProvider?.PlaySuccess();
                        DeactivateSystem(); // SUCCESS: Zavřeme terminál
                    }
                    else
                    {
                        // FAILURE: Málo peněz -> červené bliknutí, terminál zůstane
                        TriggerError(_playerInput);
                    }
                    break;
                }
            }

            if (!commandFound) TriggerError(_playerInput);
        }
    }


    private void TriggerError(string textToFlash)
    {
        _failedInput = textToFlash;
        // _playerInput = "";
        _soundProvider?.PlayFail();
        StartCoroutine(FlashErrorRoutine());
    }

    private IEnumerator FlashErrorRoutine()
    {
        _isFlashingError = true;
        UpdateVisuals();
        yield return new WaitForSeconds(flashDuration);
        _isFlashingError = false;
        _failedInput = "";
        UpdateVisuals();
    }

    private void CompleteStoryLine()
    {
        string finishedLine = _lines[_currentLineIndex];
        _soundProvider?.PlaySuccess();
        _playerInput = "";
        _currentLineIndex++;

        Handler?.OnLineCompleted(finishedLine);

        // HACK: If we are in the main menu and finished typing, load Scene 4
        if (_currentLineIndex >= _lines.Count && SceneManager.GetActiveScene().name == "VojtaMenuTest")
        {
            Debug.Log("[TypingSystem] Main Menu text completed. Loading Scene 4...");
            SceneManager.LoadScene(4);
            return; // Důležité: zastavíme kód tady, aby nešel dál
        }

        // Colleague's logic: loop the text endlessly for other scenes
        if (_currentLineIndex >= _lines.Count)
        {
            Handler?.OnAllLinesCompleted();
        }

        _currentLineIndex %= _lines.Count;
    }

    private void UpdateVisuals()
    {
        if (displayLabel == null) return;

        string correctHex = ColorUtility.ToHtmlStringRGB(correctColor);
        string idleHex = ColorUtility.ToHtmlStringRGB(idleColor);
        string errorHex = ColorUtility.ToHtmlStringRGB(errorColor);

        if (mode == TypingMode.Story)
        {
            if (_currentLineIndex >= _lines.Count) return;
            string target = _lines[_currentLineIndex];

            if (_isFlashingError)
            {
                string correctPart = target.Substring(0, _playerInput.Length);

                string errorChar = _failedInput.Substring(_playerInput.Length, 1);

                string rest = "";
                if (_playerInput.Length + 1 < target.Length)
                {
                    rest = target.Substring(_playerInput.Length + 1);
                }

                if (errorChar == " ")
                {
                    errorChar = "-"; // Visuální náhrada pro mezery, aby bylo vidět, co bylo špatně
                }
                displayLabel.text = $"<color=#{correctHex}>{correctPart}</color><color=#{errorHex}><s>{errorChar}</s></color><color=#{idleHex}>{rest}</color>";
            }
            else
            {
                string typed = target.Substring(0, _playerInput.Length);

                if (_playerInput.Length < target.Length)
                {
                    string currentChar = target.Substring(_playerInput.Length, 1);
                    string untyped = target.Substring(_playerInput.Length + 1);
                    displayLabel.text = $"<color=#{correctHex}>{typed}</color><mark=#FFFFFF7D><color=#000000><b>{currentChar}</b></color></mark><color=#{idleHex}>{untyped}</color>";
                }
                else
                {
                    string untyped = target.Substring(_playerInput.Length);
                    displayLabel.text = $"<color=#{correctHex}>{typed}</color><color=#{idleHex}>{untyped}</color>";
                }
            }
        }
        else
        {
            string content = _isFlashingError ? _failedInput : _playerInput;
            string color = _isFlashingError ? errorHex : correctHex;
            displayLabel.text = $"{commandPrefix}<color=#{color}>{content}</color>";
        }
    }
}