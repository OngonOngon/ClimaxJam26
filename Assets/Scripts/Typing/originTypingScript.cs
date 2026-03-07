using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

// Defines whether the user is typing story text or entering commands
public enum TypingMode { Story, Command }

// Static helper for input detection
public static class CInput
{
    public static string GetCapturedInput() => Input.inputString;
    public static bool IsActionTriggered() => Input.GetKeyDown(KeyCode.Tab);
    public static bool IsSubmitTriggered() => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
    public static bool IsToggleModeTriggered() => Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
}

// Interface for objects that want to react to typing events (e.g., PlayerResources)
public interface ITypingHandler
{
    void OnLineCompleted(string completedLine);
    void OnCommandExecuted(string command);
    void OnAllLinesCompleted();
}

// Interface for modular sound handling
public interface ITypingSoundProvider
{
    void PlayCharSound(char c);
    void PlaySuccess();
    void PlayFail();
}

public class originTypingScript : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private TypingMode mode = TypingMode.Story;
    [SerializeField] private TextMeshProUGUI displayLabel;

    [Header("Story Mode Settings")]
    [TextArea(3, 5)]
    [SerializeField] private string referenceText;

    [Header("Command Mode Settings")]
    [SerializeField] private List<string> validCommands = new List<string> { "build", "upgrade", "shoot" };
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
    public ITypingHandler Handler { get; set; }

    void Awake()
    {
        // First, try to find the provider on the same GameObject
        _soundProvider = GetComponent<ITypingSoundProvider>();

        // If not found, search the scene for any MonoBehaviour that implements the interface
        if (_soundProvider == null)
        {
            // We search for all MonoBehaviours because FindFirstObjectByType cannot find interfaces directly
            var providers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var p in providers)
            {
                if (p is ITypingSoundProvider found)
                {
                    _soundProvider = found;
                    break;
                }
            }
        }
    }

    void Start()
    {
        PrepareStoryText();
        if (displayLabel != null) displayLabel.gameObject.SetActive(false);
    }

    void Update()
    {
        // Toggle the whole typing system
        if (CInput.IsActionTriggered()) ToggleSystem();

        if (!_isActive) return;

        // Switch between Story and Command modes
        if (CInput.IsToggleModeTriggered())
        {
            mode = (mode == TypingMode.Story) ? TypingMode.Command : TypingMode.Story;
            _playerInput = "";
            _failedInput = "";
            UpdateVisuals();
            Debug.Log($"[TypingSystem] Mode switched to: {mode}");
        }

        // Do not process input while the screen is flashing red
        if (_isFlashingError) return;

        HandleTyping();
    }

    private void ToggleSystem()
    {
        _isActive = !_isActive;
        _playerInput = "";
        _failedInput = "";
        if (displayLabel != null)
        {
            displayLabel.gameObject.SetActive(_isActive);
            UpdateVisuals();
        }
    }

    private void PrepareStoryText()
    {
        if (string.IsNullOrEmpty(referenceText)) return;
        // Split text by newlines and store in a list
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
        if (_currentLineIndex >= _lines.Count) return;
        string target = _lines[_currentLineIndex];

        foreach (char c in input)
        {
            if (c == '\r' || c == '\n') continue;

            // Handle backspace
            if (c == '\b')
            {
                if (_playerInput.Length > 0) _playerInput = _playerInput.Substring(0, _playerInput.Length - 1);
                continue;
            }

            // Compare character with target text
            int idx = _playerInput.Length;
            if (idx < target.Length && c == target[idx])
            {
                _playerInput += c;
                _soundProvider?.PlayCharSound(c);
            }
            else
            {
                // Mismatch triggers immediate error state
                TriggerError(_playerInput + c);
                break;
            }
        }

        // Completion check
        if (_playerInput == target && CInput.IsSubmitTriggered())
        {
            CompleteStoryLine();
        }
    }

    private void HandleCommandInput(string input)
    {
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

        // Submit command on Enter
        if (CInput.IsSubmitTriggered() && !string.IsNullOrEmpty(_playerInput))
        {
            string cmd = _playerInput.Trim().ToLower();
            if (validCommands.Contains(cmd))
            {
                _soundProvider?.PlaySuccess();
                Handler?.OnCommandExecuted(cmd);
                _playerInput = "";
            }
            else
            {
                TriggerError(_playerInput);
            }
        }
    }

    private void TriggerError(string textToFlash)
    {
        _failedInput = textToFlash;
        _playerInput = "";
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

        // Notify external handlers like PlayerResources
        Handler?.OnLineCompleted(finishedLine);

        if (_currentLineIndex >= _lines.Count)
        {
            Handler?.OnAllLinesCompleted();
            ToggleSystem();
        }
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
                // Render the failed attempt in red
                string errorPart = target.Substring(0, Mathf.Min(_failedInput.Length, target.Length));
                string rest = target.Substring(Mathf.Min(_failedInput.Length, target.Length));
                displayLabel.text = $"<color=#{errorHex}>{errorPart}</color><color=#{idleHex}>{rest}</color>";
            }
            else
            {
                // Render typed part (green) and remaining part (gray)
                string typed = target.Substring(0, _playerInput.Length);
                string untyped = target.Substring(_playerInput.Length);
                displayLabel.text = $"<color=#{correctHex}>{typed}</color><color=#{idleHex}>{untyped}</color>";
            }
        }
        else
        {
            // Standard console-like command prompt
            string content = _isFlashingError ? _failedInput : _playerInput;
            string color = _isFlashingError ? errorHex : correctHex;
            displayLabel.text = $"{commandPrefix}<color=#{color}>{content}</color>";
        }
    }
}