using UnityEngine;

namespace Dubinci
{
    public class Cursor : MonoBehaviour
    {
        public GridVisual grid;
        public bool moveMode = true;
        [SerializeField] private typingScript terminal;

        void Update()
        {
            // Lock cursor if ANY typing mode is active
            if (terminal != null && terminal.IsActive)
                return;

            if (!moveMode)
            {
                if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                {
                    ExitSelection();
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.W)) grid.MoveUp();
            if (Input.GetKeyDown(KeyCode.A)) grid.MoveLeft();
            if (Input.GetKeyDown(KeyCode.S)) grid.MoveDown();
            if (Input.GetKeyDown(KeyCode.D)) grid.MoveRight();

            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                grid.Select();
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