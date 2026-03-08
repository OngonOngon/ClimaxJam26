using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dubinci
{
    public class ResetLevel : MonoBehaviour
    {
        public void ResetLevelNow()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
