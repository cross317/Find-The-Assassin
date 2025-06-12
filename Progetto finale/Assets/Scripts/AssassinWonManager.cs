using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssassinWonManager : MonoBehaviour
{
    public float secondToWait = 0f;

    private void Update()
    {
        secondToWait += Time.deltaTime;
    }

    public void ReturnToMenu()
    {
        if (secondToWait >= 2f)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
