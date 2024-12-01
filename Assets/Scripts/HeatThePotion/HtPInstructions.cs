using UnityEngine;

public class HtPInstructions : MonoBehaviour
{
    public GameObject instruct;
    public PauseScript pause;
    public HeatThePotionGameManager heat;
    public void Intructions()
    {
        instruct.SetActive(false);
        Time.timeScale = 1f;
        pause.GameIsPaused = false;
        //heat.IsTimerRunning = true;
        heat.ShowFeedback("");
        heat.bar.SetActive(true);
    }
}
