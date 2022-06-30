using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // 상태
    [Serializable]
    public enum State_Type
    {
        Idle, // 기본 움직임
        Dead, // 죽음
    }
    // 캐릭터 스텟
    [Serializable]
    public class Stat
    {
        public State_Type State = State_Type.Idle; // 상태
        public int Idx = 0; // 인덱스
        public int Level = 1; // 레벨
        public int Hp = 30; // 피
        public int Damage = 1; // 대미지
        public float Move_Speed = 1.0f; // 이동속도
        public List<Spell> Spell_List = new List<Spell>(); // 스펠 종류
    }

    [Serializable]
    public enum Spell_Type // 스펠 종류
    {
        Normal_Attack, // 기본 공격
        Normal_Bullet, // 기본 총알
        Guided_Bullet, // 유도 총알
    }

    [Serializable]
    public class Spell
    {
        public Spell_Type Type = Spell_Type.Normal_Attack; // 스펠 종류
        public int Damage; // 대미지
        public Vector3 Direction; // 방향
        public bool Destroy = false; // 충돌 시 파괴 여부
        public float Move_Speed = 3.0f; // 스펠 이동 속도
        public Time_Checker<float> Speed = new Time_Checker<float>(); // 스펠 시전 간격

        public Spell() // 기본 초기화
        {
            Type = Spell_Type.Normal_Attack;
            Destroy = false;
            Speed.Set(1.0f);
        }

        public void Set(Spell_Type type, int damage, bool destroy, float speed) // 스펠 세팅
        {
            Type = type;
            Destroy = destroy;
            Speed.Set(speed);
        }
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

    public Stat m_Stat;
    public virtual void Init(int level)
    {
    }

    public virtual void Init(int idx, int level)
    {
    }

    /// <summary>
    /// 스펠 소환
    /// </summary>
    /// <param name="type"></param>
    public virtual void Spawn_Spell(Spell_Type type)
    {
        Debug.Log("Name : " + this.gameObject.name + " | Spawn Spell : " + type);
    }

    /// <summary>
    /// 사용하지 않는 스펠 오브젝트 획득
    /// </summary>
    /// <returns></returns>
    public Ctr_Spell Get_Spell_Obj()
    {
        bool spell_Check = false;
        for (int i = 0; i < GameManager.Instance.Spell_List.Count; i++) // 저장한 스펠 풀리스트
        {
            if (!GameManager.Instance.Spell_List[i].gameObject.activeSelf) // 작동하지 않는 오브젝트 체크
            {
                spell_Check = true;
                return GameManager.Instance.Spell_List[i];
            }
        }

        if(!spell_Check) // 오브젝트 못 발견했을 시 생성
        {
            Ctr_Spell t_Spell = Instantiate(GameManager.Instance.DataLoad_Spell_List[0], Vector3.zero, Quaternion.identity).GetComponent<Ctr_Spell>(); // 새로 생성
            t_Spell.transform.parent = GameManager.Instance.PoolManager.Spell; // 풀 매니저에 등록
            t_Spell.gameObject.SetActive(false);
            GameManager.Instance.Spell_List.Add(t_Spell);
            return t_Spell;
        }

        return null;
    }

    /// <summary>
    /// 대미지
    /// </summary>
    /// <param name="damage"></param>
    public virtual void Get_Damage(int damage)
    {
        Debug.Log("Name : " + this.gameObject.name + " | Get Damage : " + damage);
        m_Stat.Hp -= damage;
        GameManager.Instance.Show_Effect(this.transform.position);

        if (m_Stat.Hp <= 0)
            Death();
    }

    /// <summary>
    /// 죽음
    /// </summary>
    public virtual void Death()
    {
        Debug.Log("Name : " + this.gameObject.name + " | Dead");
        m_Stat.State = State_Type.Dead;
        this.gameObject.SetActive(false);
    }
}
