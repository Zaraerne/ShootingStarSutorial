using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PresistentSingleton<SceneLoader>
{
    [SerializeField] UnityEngine.UI.Image transitionImage;
    [SerializeField] float fadeTime = 3.5f;
    Color color;
    const string GAMEPLAY = "GamePlay";
    const string MAINMENU = "MainMenu";
    const string SCORING = "Scoring";

    void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

  

    IEnumerator LoadingCoroutine(string sceneName)
    {
        var loadingOperation =  SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        transitionImage.gameObject.SetActive(true);

        // Fade out
        while(color.a < 1)
        {
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / 2);
            transitionImage.color = color;

            yield return null;
        }

        yield return new WaitUntil(() => loadingOperation.progress >= 0.89);

        loadingOperation.allowSceneActivation = true;

        // Fade in
        while (color.a > 0)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / 2);
            transitionImage.color = color;

            yield return null;
        }
        transitionImage.gameObject.SetActive(false);
    }

    public void LoadGamePlayScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(GAMEPLAY));
    }
    public void LoadMainMenuScene()
    {
        StartCoroutine(LoadingCoroutine(MAINMENU));
    }

    public void LoadScoringScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(SCORING));
    }
}
