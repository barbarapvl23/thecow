using UnityEngine;

public class TouchController : MonoBehaviour
{
    public bool moveLeft = false;
    public bool moveRight = false;
    public bool jump = false;

    public void SetMoveLeft(bool state)
    {
         moveLeft = state;
    }

    public void SetMoveRight(bool state)
    {
         moveRight = state;
    }

    public void SetJump(bool state)
    {
         jump = state;
    }
}
