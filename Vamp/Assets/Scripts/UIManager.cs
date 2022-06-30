using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject Game_Start;
    public GameObject Game_End;
    private void Awake()
    {
        Instance = this;
    }
}
