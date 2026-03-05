using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections;

public class AsyncLoader : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuObject; // The Panel
    [SerializeField] private Slider progressBar;       // The Slider
    [SerializeField] private TMP_Text promptText;       // "Press Any Key" Text

    // call for specific scene by name 
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    // Call to load next scene in build settings
    public void LoadNextScene()
    {
        // Gets the index of current scene and adds 1
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Don't try to load if there is no next scene
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadAsync(nextSceneIndex));
        }
        else
        {
            Debug.LogWarning("No more scenes in Build Settings!");
        }
    }

    // This handles the actual work for both names and indices
    IEnumerator LoadAsync(object sceneToLoad)
    {
        menuObject.SetActive(false);
        promptText.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);

        AsyncOperation operation;

        // Determine if we are loading by name (string) or index (int)
        if (sceneToLoad is string name)
            operation = SceneManager.LoadSceneAsync(name);
        else
            operation = SceneManager.LoadSceneAsync((int)sceneToLoad);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Sync the progress bar
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            // Check if load is complete (0.9 is fully loaded in background)
            if (operation.progress >= 0.9f)
            {
                promptText.gameObject.SetActive(true);

                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}