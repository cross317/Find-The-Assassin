using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Script1Task task1;
    public Script2Task task2;
    public bool canPlay;
    public bool isTask1Complete = false;
    public bool isTask2Complete = false;
    public bool isTask3Complete = false;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

}
