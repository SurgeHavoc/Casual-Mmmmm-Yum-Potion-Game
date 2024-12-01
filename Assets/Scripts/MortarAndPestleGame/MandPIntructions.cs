using UnityEngine;

public class MandPIntruction : MonoBehaviour
{
   public GameObject instruct;
    public PauseScript pause;
    public Mortar mortar;

    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
        mortar.IsTimerRunning = true;
    }
}
