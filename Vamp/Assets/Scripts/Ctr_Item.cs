using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ctr_Item : MonoBehaviour
{
    [Serializable]
    public enum Specify // 아이템 종류
    {
        Heal,
        LevelUp,
    }

    [Serializable]
    public class Item
    {
        public Specify Specify = Specify.LevelUp; // 아이템 종류
        public int Value = 0; // 아이템 능력치
    }

    public Item m_Item = new Item();
}
