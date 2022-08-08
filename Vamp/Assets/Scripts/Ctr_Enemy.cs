using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctr_Enemy : Character
{
    public Ctr_Player Target;
    /// <summary>
    /// 기본 초기화
    /// </summary>
    public override void Init(int idx, int level)
    {
        // 스텟 세팅
        m_Stat = new Stat();
        m_Stat.State = State_Type.Idle;
        m_Stat.Idx = idx;
        m_Stat.Level = level;
        m_Stat.Hp = 3 * m_Stat.Level;
        m_Stat.Damage = 1;
        m_Stat.Move_Speed = 0.5f;
        m_Stat.Spell_List.Add(new Spell());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.m_Stage_Info.Game_State == GameManager.Game_State.Play) // 게임 시작인 경우
        {
            if (Vector2.Distance(this.transform.position, Target.transform.position) > 0.1f) // 거리가 0.5보다 크면
                this.transform.position = Vector3.MoveTowards(this.transform.position, Target.transform.position, Time.deltaTime * m_Stat.Move_Speed);

            if(this.transform.position.x < Target.transform.position.x) // 각 계산 후 방향
                this.transform.right = Vector3.left;
            else
                this.transform.right = Vector3.right;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Spell") // 총알과 충돌
        {
            if (other.GetComponent<Ctr_Spell>().Spawn_Character.tag == "Player") // 소환한 캐릭터가 플레이어인 경우
            {
                Get_Damage(other.GetComponent<Ctr_Spell>().Spawn_Character.m_Stat.Damage);
                other.gameObject.SetActive(!other.GetComponent<Ctr_Spell>().m_Spell.Destroy_Hitted); // 파괴되는 거면 파괴
            }
        }
    }

    public override void Death()
    {
        base.Death();
        GameManager.Instance.m_Stage_Info.Monster_Death_Count += 1; // 몬스터 죽었을 때 숫자 업
        Drop_Item();
    }

    /// <summary>
    /// 몬스터 드랍 아이템
    /// </summary>
    public void Drop_Item()
    {
        if(UnityEngine.Random.Range(0, 11) < 3f) // 30% 이하일 경우 드랍
        {
            
        }
    }
}
