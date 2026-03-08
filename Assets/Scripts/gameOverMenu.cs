using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dubinci
{
    public class gameOverMenu : MonoBehaviour
    {
        public void RestartLevel()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        public void QuitToMainMenu()
        {
            SceneManager.LoadScene("VojtaMenuTest");
        }

        public void LoadEnd()
        {
            SceneManager.LoadScene(12);
        }

        public void LoadNextLevel()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No more levels! Returning to Main Menu.");
                SceneManager.LoadScene("VojtaMenuTest");
            }
        }
    }
}