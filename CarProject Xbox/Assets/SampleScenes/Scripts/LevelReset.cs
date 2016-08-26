using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelReset :MonoBehaviour , IPointerClickHandler
{
    public void OnPointerClick(PointerEventData data)
    {
        // reload the scene
        SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
    }

    public void ResetPressBttn()
    {
        SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
    }

    private void Update()
    {
        if (Input.GetButton("LevelReset"))
        {
            ResetPressBttn();

        }

    }
}
