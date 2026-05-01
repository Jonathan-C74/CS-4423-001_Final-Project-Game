using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Play the game
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Quit Application
    public void Quit()
    {
        Application.Quit();
    }
}
