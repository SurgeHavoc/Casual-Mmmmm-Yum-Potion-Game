using UnityEngine;

public class MandPInstructions : MonoBehaviour
{
    public GameObject instruct;
    public PauseScript pause;
    public Mortar mortar;

    public void MIntructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
        mortar.IsTimerRunning = true;
    }
}
