using UnityEngine;

public class Cursor : MonoBehaviour 
{
    [SerializeField] Texture2D crosshair;

    private void Start()
    {
         SetCrosshair();
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
    }
    
    private void SetCrosshair()
    {
        //UnityEngine.Cursor.SetCursor(crosshair, new Vector2(.5f, .5f), CursorMode.ForceSoftware);
    }
}
