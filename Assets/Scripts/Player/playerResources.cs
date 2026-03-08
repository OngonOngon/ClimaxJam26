using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Windows;

public class playerResources : MonoBehaviour, ITypingHandler
{
    [SerializeField] private bool useLettersEconomy = true;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI displayBox;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Color noLetterColor;
    [SerializeField] private Color letterGradStartColor;
    [SerializeField] private Color letterGradEndColor;
    [SerializeField] private int maxStockVis = 5;

    [Header("Stats")]
    [SerializeField] private int lives = 3;
    [SerializeField] private int money = 0;
    [SerializeField] private int debugStartingAmount = 0;
    private Dictionary<char, int> inventory;

    [Header("Rewards & Costs")]
    [SerializeField] private int moneyPerStoryLine = 50;
    [SerializeField] private int buildCost = 100;
    [SerializeField] private int upgradeCost = 50;

    private bool _isGameOver = false;

    private void Start()
    {
        typingScript ts = FindFirstObjectByType<typingScript>();
        if (ts != null)
        {
            ts.Handler = this;
            Debug.Log("[Resources] Successfully linked to TypingSystem.");
        }
        else
        {
            Debug.LogError("[Resources] TypingSystem not found in scene!");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

#if !UNITY_EDITOR
        debugStartingAmount = 0;
#endif

        inventory = Enumerable.Range('a', 26)
            .ToDictionary(c => (char)c, c => debugStartingAmount);

        PrintStats();
        UpdateUI();
    }

    private string GetGradientColor(int count)
    {
        Color displayColor;
        if (count == 0)
            displayColor = noLetterColor;
        else
            displayColor = Color.Lerp(letterGradStartColor, letterGradEndColor, (float)count / maxStockVis);
        return ColorUtility.ToHtmlStringRGB(displayColor);
    }

    private void UpdateUI()
    {
        if (livesText != null)
            livesText.text = lives.ToString();
        if (!useLettersEconomy && moneyText != null)
            moneyText.text = money.ToString();

        if (displayBox == null) return;

        if (!useLettersEconomy)
            return;

        string output = "";
        foreach (var item in inventory)
        {
            string hex = GetGradientColor(item.Value);
            output += $"<color=#{hex}>{item.Key.ToString().ToUpper()}</color>  ";
        }

        displayBox.text = output;
    }

    public bool OnLineCompleted(string completedLine)
    {
        if (_isGameOver) return false;

        foreach (char c in completedLine.ToLower())
            if (inventory.ContainsKey(c))
                inventory[c]++;

        money += moneyPerStoryLine;
        UpdateUI();
        return true;
    }

    public bool OnCommandExecuted(string command)
    {
        if (_isGameOver) return false;

        if (useLettersEconomy)
        {
            var requirements = command.ToLower().GroupBy(c => c);

            foreach (var req in requirements)
            {
                if (inventory.ContainsKey(req.Key) && inventory[req.Key] < req.Count())
                {
                    Debug.Log($"Not enough of this letter {req.Key}");
                    return false;
                }
            }

            foreach (char c in command.ToLower())
                if (inventory.ContainsKey(c))
                    inventory[c]--;
        }
        else
        {
            string cmd = command.ToLower();

            // Check if the command starts with "build" (covers "build shooter", "build wall" etc.)
            if (cmd.StartsWith("g") || cmd.StartsWith("a") || cmd.StartsWith("w") || cmd.StartsWith("s"))
            {
                return TrySpendMoney(buildCost, "Building structure");
            }

            if (cmd.StartsWith("u"))
            {
                return TrySpendMoney(upgradeCost, "Upgrading system");
            }

            if (cmd.StartsWith("e"))
            {
                return TrySpendMoney(buildCost, "Applying modifier");
            }

            if (cmd.StartsWith("f"))
            {
                Debug.Log("[Resources] PEW PEW!");
                return true;
            }

            Debug.Log($"[Resources] Command '{command}' is free or unknown.");
        }

        UpdateUI();
        return true;
    }

    public void OnAllLinesCompleted()
    {
        Debug.Log("[Resources] Story finished! Bonus 500 money awarded.");
        money += 500;
        UpdateUI();
        PrintStats();
    }

    private bool TrySpendMoney(int cost, string actionName)
    {
        if (money >= cost)
        {
            money -= cost;
            UpdateUI();
            Debug.Log($"[Resources] {actionName} success! Remaining: {money}");
            return true;
        }
        else
        {
            Debug.LogWarning($"[Resources] Not enough money for {actionName}!");
            return false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isGameOver) return;

        lives -= amount;
        UpdateUI();
        Debug.Log($"[Resources] Ouch! Took {amount} damage. Lives left: {lives}");

        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _isGameOver = true;
        lives = 0;
        UpdateUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Debug.LogError("==========================");
        Debug.LogError("GAME OVER - YOU DIED");
        Debug.LogError("==========================");
    }

    private void PrintStats()
    {
        Debug.Log($"[Resources] Current Stats -> Money: {money}, Lives: {lives}");
    }
}