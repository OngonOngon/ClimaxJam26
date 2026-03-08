using UnityEngine;

namespace Dubinci
{
    public class SkipTransition : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Drag the object with the sceneTransition script here.")]
        [SerializeField] private sceneTransition transitionController;

        // Update is called once per frame
        void Update()
        {
            // Check for Enter (Return) or Space key press
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                TriggerSkip();
            }
        }

        private void TriggerSkip()
        {
            if (transitionController != null)
            {
                // Call the transition logic from your existing script
                transitionController.Transition();

                // Disable this script to prevent multiple triggers during delay
                this.enabled = false;
            }
            else
            {
                Debug.LogWarning("[SkipTransition] No transitionController assigned!");
            }
        }
    }
}