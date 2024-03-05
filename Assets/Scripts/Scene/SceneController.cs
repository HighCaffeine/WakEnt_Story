using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Menu,
    Loading,
    Game,


    Count
}

public class SceneController : GenericSingleton<SceneController>
{
    

    [SerializeField] private Image loadImage;
    [SerializeField] private Image loadBar;


    private bool asyncIsDone = false;

    private new void Awake()
    {
        base.Awake();
    }
    
    public void SetLoadImage(Image loadImage)
    {
        this.loadImage = loadImage;
    }

    public void GoToScene(SceneName sceneName)
    {
        StartCoroutine(StartLoad(sceneName));
    }

    IEnumerator StartLoad(SceneName sceneName)
    {
        SceneManager.LoadSceneAsync("Loading");

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName.ToString());
        async.allowSceneActivation = false;

        while (loadImage == null)
        {
            if (loadImage != null)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        while (!async.isDone)
        {
            loadImage.fillAmount = async.progress;

            if (async.progress >= 0.9f)
            {
                asyncIsDone = true;

                async.allowSceneActivation = true;
                
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    private void GameExit()
    {
        GameManager.Instance.Save();

        Application.Quit();
    }
}
