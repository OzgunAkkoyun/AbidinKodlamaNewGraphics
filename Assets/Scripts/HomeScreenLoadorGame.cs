using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenLoadorGame : MonoBehaviour
{
    public void SetLoadOrGame(int index)
    {
        var gameDataString = PlayerPrefs.GetString("gameDatas");
        if (gameDataString != "")
        {
            PlayerPrefs.SetInt("isGameOrLoad", index);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Game()
    {
        PlayerPrefs.SetInt("isGameOrLoad", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}