using UnityEngine;

public class playerResources : MonoBehaviour, ITypingHandler
{
    [Header("Stats")]
    [SerializeField] private int lives = 3;
    [SerializeField] private int money = 0;

    [Header("Rewards & Costs")]
    [SerializeField] private int moneyPerStoryLine = 50;
    [SerializeField] private int buildCost = 100;
    [SerializeField] private int upgradeCost = 250;

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

        PrintStats();
    }

    // --- ITypingHandler Implementation ---

    public void OnLineCompleted(string completedLine)
    {
        if (_isGameOver) return;

        money += moneyPerStoryLine;
        Debug.Log($"[Resources] Story line finished! Earned: {moneyPerStoryLine}. Total Money: {money}");
    }

    public void OnCommandExecuted(string command)
    {
        if (_isGameOver) return;

        switch (command.ToLower())
        {
            case "build":
                TrySpendMoney(buildCost, "Building structure");
                break;

            case "upgrade":
                TrySpendMoney(upgradeCost, "Upgrading system");
                break;

            case "shoot":
                Debug.Log("[Resources] PEW PEW! Shooting costs nothing but skill.");
                break;

            default:
                Debug.Log($"[Resources] Command '{command}' executed but has no resource cost.");
                break;
        }
    }

    public void OnAllLinesCompleted()
    {
        Debug.Log("[Resources] Story finished! Bonus 500 money awarded.");
        money += 500;
        PrintStats();
    }

    // --- Logic ---

    private void TrySpendMoney(int cost, string actionName)
    {
        if (money >= cost)
        {
            money -= cost;
            Debug.Log($"[Resources] {actionName} success! Spent: {cost}. Remaining: {money}");
        }
        else
        {
            Debug.LogWarning($"[Resources] Not enough money for {actionName}! Need: {cost}, Have: {money}");
            // Optional: trigger some "No Money" sound or effect here
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isGameOver) return;

        lives -= amount;
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