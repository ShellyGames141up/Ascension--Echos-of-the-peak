using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    private Canvas canvasComponent;
    private FPSPlayerController fpsController;
    private float escapePressTime;
    private bool firstEscapePress = false;
    private const float doublePressTime = 0.5f;

    void Start()
    {
        canvasComponent = GetComponent<Canvas>();
        
        if (canvasComponent != null)
        {
            canvasComponent.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (firstEscapePress && Time.time - escapePressTime < doublePressTime)
            {
                ExitGame();
            }
            else
            {
                firstEscapePress = true;
                escapePressTime = Time.time;
                ToggleMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && canvasComponent.enabled)
        {
            ResumeGame();
        }

        if (firstEscapePress && Time.time - escapePressTime >= doublePressTime)
        {
            firstEscapePress = false;
        }
    }

    public void ToggleMenu()
    {
        if (canvasComponent != null)
        {
            bool newState = !canvasComponent.enabled;
            canvasComponent.enabled = newState;
            
            if (fpsController != null)
            {
                fpsController.SetInputEnabled(!newState);
            }
        }
    }

    public void ResumeGame()
    {
        if (canvasComponent != null && canvasComponent.enabled)
        {
            canvasComponent.enabled = false;
            
            if (fpsController != null)
            {
                fpsController.SetInputEnabled(true);
            }
        }
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}