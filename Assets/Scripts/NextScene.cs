using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NextScene : MonoBehaviour
{

    private void Start()
    {
        Invoke("ProceedToNextMinigame", 3f);
    }

    void ProceedToNextMinigame()
    {
        Debug.Log("Proceeding to a random minigame.");

        int MinSceneIndex = 7;
        int MaxSceneIndex = 11;
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Check to make sure that MaxSceneIndex doesn't exceed the number of scenes in build settings.
        MaxSceneIndex = Mathf.Min(MaxSceneIndex, SceneManager.sceneCountInBuildSettings - 1);

        // Create a list of possible scene indices excluding the current scene to avoid repeating the same scene for next minigame.
        List<int> PossibleSceneIndices = new List<int>();
        for (int i = MinSceneIndex; i <= MaxSceneIndex; i++)
        {
            if (i != CurrentSceneIndex)
            {
                PossibleSceneIndices.Add(i);
            }
        }

        if (PossibleSceneIndices.Count > 0)
        {
            // Select a random index from the list of possible scene indices.
            int RandomIndex = Random.Range(0, PossibleSceneIndices.Count);
            int RandomSceneIndex = PossibleSceneIndices[RandomIndex];

            // Load the randomly selected scene other than the current scene.
            SceneManager.LoadScene(RandomSceneIndex);
        }
        else
        {
            Debug.LogWarning("No other scenes available to load, try adding scenes to the build settings.");
        }
    }
}
