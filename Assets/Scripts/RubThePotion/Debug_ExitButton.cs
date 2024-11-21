using UnityEngine;
using UnityEngine.SceneManagement;

public class Debug_ExitButton : MonoBehaviour
{
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
