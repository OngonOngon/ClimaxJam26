using UnityEngine;

namespace Dubinci
{
    public class Cursor : MonoBehaviour
    {
        public GridVisual grid;
        public bool moveMode = true;
        [SerializeField] private typingScript terminal;
        public AudioSource audioSource;
        public AudioClip moveCursorSound;
        public AudioClip selectCursorSound;


        void Update()
        {
            // Lock cursor if ANY typing mode is active
            if (terminal != null && terminal.IsActive)
                return;

            if (!moveMode)
            {
                if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                {
                    audioSource.PlayOneShot(selectCursorSound); // play select sound
                    ExitSelection();
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.W)) { 
                grid.MoveUp();
                audioSource.PlayOneShot(moveCursorSound);// play move sound
            }
            if (Input.GetKeyDown(KeyCode.A)){
                grid.MoveLeft();
                audioSource.PlayOneShot(moveCursorSound);// play move sound
            }
            if (Input.GetKeyDown(KeyCode.S)){ 
                grid.MoveDown();
                audioSource.PlayOneShot(moveCursorSound);// play move sound
            }
            if (Input.GetKeyDown(KeyCode.D)){ 
                grid.MoveRight();
                audioSource.PlayOneShot(moveCursorSound); // play move sound
            }

if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                grid.Select();
                audioSource.PlayOneShot(selectCursorSound); // play select sound
                moveMode = false;

                if (terminal != null)
                {
                    // Specifically triggers COMMAND mode
                    terminal.ActivateCommandMode(this);
                }
            }
        }

        public void ExitSelection()
        {
            grid.Unselect();
            moveMode = true;
        }
    }
}