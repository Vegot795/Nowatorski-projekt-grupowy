using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("BasicLevel");

    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    public void GoMainMenu()
    {
        //save
        SceneManager.LoadScene("MainMenu");

    }
    public void QuitAndSave()
    {
        //save
        Debug.Log("Quit");
        Application.Quit();

    }
}
