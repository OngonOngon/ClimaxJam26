using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading and restarting scenes

namespace Dubinci
{
    public class gameOverMenu : MonoBehaviour
    {
        // Called by the "Try Again" button
        public void RestartLevel()
        {
            // Get the currently active scene and load it again
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
            Debug.Log("[GameOverMenu] Restarting current level...");
        }

        // Called by the "Quit" button
        public void QuitToMainMenu()
        {
            // Load the specific main menu scene
            SceneManager.LoadScene(3);
            Debug.Log("[GameOverMenu] Quitting to Main Menu...");
        }
    }
}