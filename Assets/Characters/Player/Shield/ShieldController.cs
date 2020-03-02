using UnityEngine;
using UnityEngine.SceneManagement;

public class ShieldController : MonoBehaviour 
{
    private byte numberOfBulletsAbsorbed = 0;

    private void Start()
    {
        SceneManager.sceneLoaded += ResetBulletsAbsorbed;
    }

    private void ResetBulletsAbsorbed(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            numberOfBulletsAbsorbed = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        numberOfBulletsAbsorbed++;
    }

    public byte GetAmountOfBulletsAbsorbedAndReset()
    {
        byte bulletsToReturn = numberOfBulletsAbsorbed;
        numberOfBulletsAbsorbed = 0;
        return bulletsToReturn;
    }
}
