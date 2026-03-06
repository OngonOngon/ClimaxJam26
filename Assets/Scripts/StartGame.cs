using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dubinci
{
    public class StartGame : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene(1);
        }
    }
}
