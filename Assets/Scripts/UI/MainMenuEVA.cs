using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEVA : MonoBehaviour
{
    public void PlayGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Stage1_SectorAlpha");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}