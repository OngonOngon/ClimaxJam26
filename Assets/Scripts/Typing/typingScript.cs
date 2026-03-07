using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public enum TypingMode { Story, Command }

public static class CInput
{
    public static string GetCapturedInput() => Input.inputString;
    public static bool IsActionTriggered() => Input.GetKeyDown(KeyCode.Tab);
    public static bool IsSubmitTriggered() => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
    public static bool IsToggleModeTriggered() => Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
}

public interface ITypingHandler
{
    void OnLineCompleted(string completedLine);
    void OnCommandExecuted(string command);
    void OnAllLinesCompleted();
}

[RequireComponent(typeof(AudioSource))]
public class typingScript : MonoBehaviour
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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctCharClip;
    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failClip;

    [Header("Colors")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color idleColor = Color.gray;
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private float flashDuration = 0.25f;

    private List<string> _lines = new List<string>();
    private int _currentLineIndex = 0;
    private string _playerInput = "";
    private string _failedInput = ""; // Stores text to show during red flash
    private bool _isActive = false;
    private bool _isFlashingError = false;

    public ITypingHandler Handler { get; set; }

    void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PrepareStoryText();
        if (displayLabel != null) displayLabel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (CInput.IsActionTriggered()) ToggleSystem();

        if (!_isActive) return;

        // Toggle Mode logic
        if (CInput.IsToggleModeTriggered())
        {
            mode = (mode == TypingMode.Story) ? TypingMode.Command : TypingMode.Story;
            _playerInput = "";
            _failedInput = "";
            UpdateVisuals();
            Debug.Log($"[TypingSystem] Mode switched to: {mode}");
        }

        // Block typing during error flash
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
        string[] split = referenceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
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
            if (c == '\b')
            {
                if (_playerInput.Length > 0) _playerInput = _playerInput.Substring(0, _playerInput.Length - 1);
                continue;
            }

            int idx = _playerInput.Length;
            if (idx < target.Length && c == target[idx])
            {
                _playerInput += c;
                PlaySound(correctCharClip);
            }
            else
            {
                // Error: Save what we had (plus the wrong char) for the red flash
                TriggerError(_playerInput + c);
                break;
            }
        }

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
            PlaySound(correctCharClip);
        }

        if (CInput.IsSubmitTriggered() && !string.IsNullOrEmpty(_playerInput))
        {
            string cmd = _playerInput.Trim().ToLower();
            if (validCommands.Contains(cmd))
            {
                PlaySound(successClip);
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
        _playerInput = ""; // Clear working input
        PlaySound(failClip);
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
        PlaySound(successClip);
        _playerInput = "";
        _currentLineIndex++;
        if (_currentLineIndex >= _lines.Count) ToggleSystem();
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
                // During flash, show the failed part in red
                string errorPart = target.Substring(0, Mathf.Min(_failedInput.Length, target.Length));
                string rest = target.Substring(Mathf.Min(_failedInput.Length, target.Length));
                displayLabel.text = $"<color=#{errorHex}>{errorPart}</color><color=#{idleHex}>{rest}</color>";
            }
            else
            {
                string typed = target.Substring(0, _playerInput.Length);
                string untyped = target.Substring(_playerInput.Length);
                displayLabel.text = $"<color=#{correctHex}>{typed}</color><color=#{idleHex}>{untyped}</color>";
            }
        }
        else
        {
            // Command Mode visuals
            string content = _isFlashingError ? _failedInput : _playerInput;
            string color = _isFlashingError ? errorHex : correctHex;
            displayLabel.text = $"{commandPrefix}<color=#{color}>{content}</color>";
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }
}