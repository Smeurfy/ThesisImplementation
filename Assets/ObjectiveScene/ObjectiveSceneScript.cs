using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveSceneScript : MonoBehaviour
{
    private bool _clickedBn = false;
    public GameObject loadingPrefab;
    private bool animationDone = false;

    public void AnimationDone()
    {
        animationDone = true;
    }

    private void Update()
    {
        if (animationDone)
        {
            if (Input.anyKeyDown && !_clickedBn)
            {
                _clickedBn = true;
                Instantiate(loadingPrefab, transform.position, Quaternion.identity, transform);
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
