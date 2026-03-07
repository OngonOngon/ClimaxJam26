using UnityEngine;

namespace Dubinci
{
    public class Cursor : MonoBehaviour
    {
        public GridVisual grid;
        public bool moveMode = true;

        void Update()
        {
            if (!moveMode)
            {
                if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                {
                    grid.Unselect();
                    moveMode = true;
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.W))
                grid.MoveUp();
            if (Input.GetKeyDown(KeyCode.A))
                grid.MoveLeft();
            if (Input.GetKeyDown(KeyCode.S))
                grid.MoveDown();
            if (Input.GetKeyDown(KeyCode.D))
                grid.MoveRight();
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                grid.Select();
                moveMode = false;
            }
        }
    }
}
