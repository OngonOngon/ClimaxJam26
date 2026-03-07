using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public static class CInput
{
    public static string GetCapturedInput() => Input.inputString;
    public static bool IsActionTriggered() => Input.GetKeyDown(KeyCode.Tab);
    public static bool IsSubmitTriggered() => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
}

public interface ITypingHandler
{
    void OnLineCompleted(string completedLine);
    void OnAllLinesCompleted();
}

public class typingScript : MonoBehaviour
{
    [Header("Settings")]
    [TextArea(5, 10)]
    [SerializeField] private string referenceText;
    [SerializeField] private TextMeshProUGUI displayLabel;

    [Header("Colors")]
    [SerializeField] private string correctColorTag = "<color=#00FF00>";
    [SerializeField] private string remainingColorTag = "<color=#808080>";
    private const string EndTag = "</color>";

    private List<string> _lines = new List<string>();
    private int _currentLineIndex = 0;
    private string _playerInput = "";
    private bool _isActive = false;

    public ITypingHandler Handler { get; set; }

    void Start()
    {
        PrepareText();
        if (displayLabel != null) displayLabel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (CInput.IsActionTriggered())
        {
            ToggleSystem();
        }

        if (!_isActive || _currentLineIndex >= _lines.Count) return;

        HandleTyping();
    }

    private void ToggleSystem()
    {
        _isActive = !_isActive;
        Debug.Log($"[TypingSystem] System {(_isActive ? "Activated" : "Deactivated")}");

        if (displayLabel != null)
        {
            displayLabel.gameObject.SetActive(_isActive);
            if (_isActive) UpdateVisuals();
        }
    }

    private void PrepareText()
    {
        if (string.IsNullOrEmpty(referenceText)) return;

        string[] split = referenceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        _lines.Clear();
        _lines.AddRange(split);
        Debug.Log($"[TypingSystem] Loaded {_lines.Count} lines.");
    }

    private void HandleTyping()
    {
        string input = CInput.GetCapturedInput();
        string targetLine = _lines[_currentLineIndex];

        bool errorOccurred = false;

        foreach (char c in input)
        {
            // Ignore Enter/Newline characters within the character comparison loop
            if (c == '\r' || c == '\n') continue;

            if (c == '\b') // Backspace
            {
                if (_playerInput.Length > 0)
                    _playerInput = _playerInput.Substring(0, _playerInput.Length - 1);
                continue;
            }

            int nextCharIndex = _playerInput.Length;
            if (nextCharIndex < targetLine.Length && c == targetLine[nextCharIndex])
            {
                _playerInput += c;
            }
            else
            {
                // Real typing error
                _playerInput = "";
                errorOccurred = true;
            }
        }

        if (errorOccurred)
        {
            Debug.Log("[TypingSystem] Typing error! Progress reset.");
        }

        // Logic check: only proceed if input matches target exactly
        if (_playerInput == targetLine)
        {
            // Optional: Log that line is ready to be submitted
            if (CInput.IsSubmitTriggered())
            {
                Debug.Log("[TypingSystem] Success! Moving to next line.");
                CompleteLine();
            }
        }

        UpdateVisuals();
    }

    private void CompleteLine()
    {
        string finishedLine = _lines[_currentLineIndex];
        _playerInput = "";
        _currentLineIndex++;

        Handler?.OnLineCompleted(finishedLine);

        if (_currentLineIndex >= _lines.Count)
        {
            Debug.Log("[TypingSystem] ALL DONE. Final success.");
            Handler?.OnAllLinesCompleted();
            _isActive = false;
            displayLabel.gameObject.SetActive(false);
        }
    }

    private void UpdateVisuals()
    {
        if (displayLabel == null || _currentLineIndex >= _lines.Count) return;

        string targetLine = _lines[_currentLineIndex];
        string typedPart = targetLine.Substring(0, _playerInput.Length);
        string remainingPart = targetLine.Substring(_playerInput.Length);

        displayLabel.text = $"{correctColorTag}{typedPart}{EndTag}{remainingColorTag}{remainingPart}{EndTag}";
    }
}