using UnityEngine;

public class MandPIntructionEndless : MonoBehaviour
{
   public GameObject instruct;
    public PauseScript pause;
    public MortarEndless mortar;

    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
        mortar.IsTimerRunning = true;
    }
}
