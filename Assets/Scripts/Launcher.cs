using Attributes;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    #region Information

    [Foldout("Information")]
    [SerializeField] NetworkRunner gameRunner;

    #region UI
    [Foldout("Information/UI")]
    [SerializeField] Button hostBtn;
    [Foldout("Information/UI")]
    [SerializeField] Button joinBtn;
    [Foldout("Information/UI/Loader")]
    [SerializeField] GameObject loader;
    [Foldout("Information/UI/Loader")]
    [SerializeField] float loaderSpeed = 100f;
    Coroutine loaderCoroutine;
    #endregion

    Task starGameTask;

    #endregion

    public virtual void Host()
    {
        NetworkRunner gameRunner = Instantiate(this.gameRunner);

        gameRunner.name = "Player Runner [Generated]";

        starGameTask = gameRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = "Default Room",
            Scene = SceneManager.GetActiveScene().buildIndex
        });

        hostBtn.interactable = false;

        joinBtn.interactable = false;

        loaderCoroutine = StartCoroutine(RotateLoaderCoroutine());
    }

    public virtual void Join()
    {
        NetworkRunner gameRunner = Instantiate(this.gameRunner);

        gameRunner.name = "Player Runner [Generated]";

        starGameTask = gameRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = "Default Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
        });

        hostBtn.interactable = false;

        joinBtn.interactable = false;

        loaderCoroutine = StartCoroutine(RotateLoaderCoroutine());
    }

    IEnumerator RotateLoaderCoroutine()
    {
        loader.SetActive(true);

        Transform transform = loader.transform;

        while (true)
        {
            transform.eulerAngles += new Vector3(0f, 0f, loaderSpeed) * Time.deltaTime;

            if (starGameTask.IsCompleted)
                StopLoaderCoroutine();

            yield return null;
        }
    }

    public void StopLoaderCoroutine()
    {
        StopAllCoroutines();

        gameObject.SetActive(false);
    }
}
