using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dubinci
{
    public class Credits : MonoBehaviour
    {
        public void BackToMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
