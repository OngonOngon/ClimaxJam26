using UnityEngine;
using TMPro; // Added for TextMeshPro support
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Windows;

public class playerResources : MonoBehaviour, ITypingHandler
{
    [SerializeField] private bool useLettersEconomy = true;

    [Header("UI Elements")]
    [Tooltip("Drag the TextMeshPro objects for the numbers here")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI displayBox;
    [SerializeField] private Color noLetterColor;
    [SerializeField] private Color letterGradStartColor;
    [SerializeField] private Color letterGradEndColor;
    [SerializeField] private int maxStockVis = 5;

    [Header("Stats")]
    [SerializeField] private int lives = 3; // player hp Health
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
        // Automatically find the typing script in the scene and subscribe
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

#if !UNITY_EDITOR
        debugStartingAmount = 0;
#endif

        inventory = Enumerable.Range('a', 26)
            .ToDictionary(c => (char)c, c => debugStartingAmount);

        PrintStats();
        UpdateUI(); // Initial UI update
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

    // --- Method to update the texts on the screen ---
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

    // --- ITypingHandler Implementation ---

    public bool OnLineCompleted(string completedLine)
    {
        if (_isGameOver) return false;

        foreach (char c in completedLine.ToLower()) // Normalize to lowercase
            if (inventory.ContainsKey(c))
                inventory[c]++;

        money += moneyPerStoryLine;
        UpdateUI(); // Update UI after earning money
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
                    return false; // Not enough of this letter
                }
            }

            // Spend money
            foreach (char c in command.ToLower())
                if (inventory.ContainsKey(c))
                    inventory[c]--;
        }
        else
        {
            string cmd = command.ToLower();

            // Check if the command starts with "build" (covers "build shooter", "build wall" etc.)
            if (cmd.StartsWith("build"))
            {
                return TrySpendMoney(buildCost, "Building structure");
            }

            if (cmd.StartsWith("upgrade"))
            {
                return TrySpendMoney(upgradeCost, "Upgrading system");
            }

            if (cmd.StartsWith("modifier"))
            {
                return TrySpendMoney(buildCost, "Applying modifier");
            }

            if (cmd == "shoot")
            {
                Debug.Log("[Resources] PEW PEW!");
                return true;
            }

            // Command not recognized as a paid action
            Debug.Log($"[Resources] Command '{command}' is free or unknown.");
        }

        UpdateUI();
        return true;
    }

    public void OnAllLinesCompleted()
    {
        Debug.Log("[Resources] Story finished! Bonus 500 money awarded.");
        money += 500;
        UpdateUI(); // Update UI after earning bonus
        PrintStats();
    }

    // --- Logic ---

    private bool TrySpendMoney(int cost, string actionName)
    {
        if (money >= cost)
        {
            money -= cost;
            UpdateUI(); // Update UI after successfully spending money
            Debug.Log($"[Resources] {actionName} success! Remaining: {money}");
            return true; // we got money
        }
        else
        {
            Debug.LogWarning($"[Resources] Not enough money for {actionName}!");
            return false; // no money
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isGameOver) return;

        lives -= amount;
        UpdateUI(); // Update UI after taking damage
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
        UpdateUI(); // Final UI update to show 0 lives
        Debug.LogError("==========================");
        Debug.LogError("GAME OVER - YOU DIED");
        Debug.LogError("==========================");
        // Here you would normally stop the game, show a menu, etc.
    }

    private void PrintStats()
    {
        Debug.Log($"[Resources] Current Stats -> Money: {money}, Lives: {lives}");
    }
}