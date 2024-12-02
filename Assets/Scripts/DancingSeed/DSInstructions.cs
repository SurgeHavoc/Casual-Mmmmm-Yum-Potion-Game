using UnityEngine;

public class DSInstructions : MonoBehaviour
{
    public GameObject instruct;
    public PauseScript pause;
    public DancingSeedGame seed;

    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
        seed.StartCountdown(); 
    }

}
