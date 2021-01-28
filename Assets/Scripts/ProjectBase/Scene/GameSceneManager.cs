using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class GameSceneManager : MonoSingletonGO<GameSceneManager>
{

    [SerializeField] private int currentSceneIndex;

    private void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            LoadPreviousScene();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            LoadCurrentScene();
        }
    }



    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        currentSceneIndex += 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousScene()
    {
        if (currentSceneIndex - 1 >= 0)
        {
            currentSceneIndex -= 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        else
        {
            Debug.Log("No Previous Scene");
            LoadCurrentScene();
        }

    }

}
