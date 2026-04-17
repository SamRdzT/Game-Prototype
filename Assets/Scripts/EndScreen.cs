using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private string titleScreenName = "TitleScreen"; 

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(titleScreenName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GoToMainMenu();
        }
    }
}
