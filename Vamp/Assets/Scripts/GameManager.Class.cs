using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [Serializable]
    public enum Game_State
    {
        Ready,
        Play,
        Pause,
        GameOver,
    }
    [Serializable]
    public class Stage_Info
    {
        public Game_State Game_State = Game_State.Ready; // 게임 상태
        public int Idx = 0; // 소환하는 몬스터 인덱스
        public int Level = 1; // 레벨
        public int Monster_Count = 10; // 몬스터 숫자
        public int Monster_Death_Count = 0; // 죽은 몬스터 숫자
        public float Timer = 0f; // 현재 시간 표시
        public float Witdh = 20f; // 화면 최대 넓이
        public float Height = 20f; // 화면 최대 높이
        public Time_Checker<float> Monster_Spawn_Time = new Time_Checker<float>(); // 몬스터 스폰 시간
    }

     /// <summary>
    /// 타임 체크 할 때 사용
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Time_Checker<T>
    {
        public T Current; 
        public T Destiantion;

        public void Set(T value)
        {
            Current = default(T);
            Destiantion = value;
        }
    }
}
