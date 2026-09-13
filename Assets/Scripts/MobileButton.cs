using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour
{
    public enum ButtonType { Left, Right, Jump }
    
    public ButtonType buttonType;

    public TouchController inputManager;

    private void Start()
    {
        inputManager = FindObjectOfType<TouchController>();
    }
    public void Press()
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                inputManager.SetMoveLeft(true);
                break;
            case ButtonType.Right:
                inputManager.SetMoveRight(true);
                break;
            case ButtonType.Jump:
                inputManager.SetJump(true);
                break;
        }
    }
    public void Release()
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                inputManager.SetMoveLeft(false);
                break;
            case ButtonType.Right:
                inputManager.SetMoveRight(false);
                break;
            case ButtonType.Jump:
                inputManager.SetJump(false);
                break;
        }
    }
}
