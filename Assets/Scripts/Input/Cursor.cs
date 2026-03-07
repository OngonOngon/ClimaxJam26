using UnityEngine;

namespace Dubinci
{
    public class Cursor : MonoBehaviour
    {
        public GridVisual grid;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
                grid.MoveUp();
            if (Input.GetKeyDown(KeyCode.A))
                grid.MoveLeft();
            if (Input.GetKeyDown(KeyCode.S))
                grid.MoveDown();
            if (Input.GetKeyDown(KeyCode.D))
                grid.MoveRight();
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
                grid.Select();
        }
    }
}
