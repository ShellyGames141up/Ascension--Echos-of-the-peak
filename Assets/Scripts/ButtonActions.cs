using UnityEngine;
using UnityEngine.UI;

public class SimpleExitGame : MonoBehaviour
{
    void Start()
    {
        Button exitButton = GetComponent<Button>();
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}