using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(LoadNextLevelAfterDelay(3f));
    }
    IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextLevel();
    }

    void LoadNextLevel()
    {
        int NextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (NextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(NextSceneIndex);
        }
        else
        {
            Debug.Log("No more scenes to load!");
        }
    }
}
