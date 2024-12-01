using UnityEngine;
using UnityEngine.SceneManagement;

public class MGScript : MonoBehaviour
{
    // MiniGames
    public void DancingSeed()
    {
        SceneManager.LoadScene(1);
    }

    public void PTI()
    {
        SceneManager.LoadScene(2);
    }

    public void MP()
    {
        SceneManager.LoadScene(3);
    }

    public void RubThePotion()
    {
        SceneManager.LoadScene(4);
    }

    public void HeatThePotion()
    {
        SceneManager.LoadScene(5);
    }

    
}
