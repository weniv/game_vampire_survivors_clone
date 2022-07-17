using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject Game_Start;
    public GameObject Game_Ing;
    public Text Text_Timer;
    public GameObject Game_End;
    public Text Text_Game_Result;
    private void Awake()
    {
        Instance = this;
    }
}
