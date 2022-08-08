using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctr_Player : Character
{
    public Transform Camera;
    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// 기본 초기화
    /// </summary>
    public override void Init(int level)
    {
        m_Stat = new Stat();
        m_Stat.State = State_Type.Idle;
        m_Stat.Level = level;
        m_Stat.Hp = 30 + 10 * level;
        m_Stat.Damage = 10 + 5 * level;
        m_Stat.Move_Speed = 1.0f;
        Spell basic_Spell = new Spell();
        basic_Spell.Set(Spell_Type.Normal_Bullet, m_Stat.Damage, true, 1.0f);
        m_Stat.Spell_List.Add(basic_Spell);
        basic_Spell = new Spell();
        basic_Spell.Set(Spell_Type.Guided_Bullet, m_Stat.Damage, true, 0.5f);
        m_Stat.Spell_List.Add(basic_Spell);        
        this.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    Vector3 mov_Pos = new Vector3();
    void Update()
    {
        if (GameManager.Instance.m_Stage_Info.Game_State == GameManager.Game_State.Play) // 게임 시작인 경우
        {
            // 카메라 위치 고정
            Camera.transform.position = this.transform.position;
            // 키 입력 이동
            mov_Pos = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow)) // 왼쪽
                mov_Pos += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow)) // 오른쪽
                mov_Pos += Vector3.right;
            if (Input.GetKey(KeyCode.UpArrow)) // 위
                mov_Pos += Vector3.up;
            if (Input.GetKey(KeyCode.DownArrow)) // 아래
                mov_Pos += Vector3.down;

            this.transform.position += mov_Pos * Time.deltaTime * m_Stat.Move_Speed; // 이동

            foreach (Spell t in m_Stat.Spell_List) // 스펠 체크
            {
                if (t.Speed.Current >= t.Speed.Destiantion)
                {
                    Spawn_Spell(t.Type);
                    t.Speed.Current = 0f;
                }
                else
                    t.Speed.Current += Time.deltaTime;
            }

            if (mov_Pos.x != 0f)
                this.transform.right = mov_Pos.x > 0 ? Vector3.left : Vector3.right;
        }
    }

    /// <summary>
    /// 스펠 소환
    /// </summary>
    /// <param name="idx"></param>
    public override void Spawn_Spell(Spell_Type type)
    {
        base.Spawn_Spell(type);
        Ctr_Spell t = null;

        switch (type) // 타입에 따라 분류
        {
            case Spell_Type.Normal_Attack:
                t = Get_Spell_Obj(0, this, type); // 풀링에서 스펠 획득
                t.m_Spell.Damage = m_Stat.Damage;
                t.m_Spell.Direction = Vector3.zero;
                t.m_Spell.Destroy_Hitted = false;
                t.m_Spell.Destroy_Time = true;
                t.m_Spell.Move_Speed = 0.0f;
                break;
            case Spell_Type.Normal_Bullet:
                t = Get_Spell_Obj(0, this, type);
                t.m_Spell.Damage = m_Stat.Damage;
                t.m_Spell.Direction = this.transform.up;
                t.m_Spell.Destroy_Hitted = true;
                t.m_Spell.Destroy_Time = true;
                t.m_Spell.Move_Speed = 1.0f;
                break;
            case Spell_Type.Guided_Bullet:
                t = Get_Spell_Obj(1, this, type);
                t.m_Spell.Destroy_Hitted = true;
                t.m_Spell.Destroy_Time = true;
                t.m_Spell.Damage = m_Stat.Damage;
                Vector2 t_Pos = Random.insideUnitCircle;
                t.m_Spell.Direction = new Vector3(t_Pos.x, t_Pos.y, 0);
                t.m_Spell.Move_Speed = 0.3f;
                //t.Find_Target();
                break;
            case Spell_Type.Shield_Damage:
                t = Get_Spell_Obj(2, this, type);
                t.m_Spell.Damage = m_Stat.Damage;
                t.m_Spell.Direction = Vector3.zero;
                t.m_Spell.Destroy_Hitted = false;
                t.m_Spell.Destroy_Time = false;
                t.m_Spell.Move_Speed = 0.0f;
                t.transform.parent = this.transform;
                break;
        }

        t.transform.position = this.transform.position;
        t.gameObject.SetActive(true);
    }

    /// <summary>
    /// 적 찾기 - 가까운 순, 특정 번호부터, 동시 생성 시 사용
    /// </summary>
    public Ctr_Enemy Find_Target(int idx)
    {
        Ctr_Enemy Target = null;
        for (int i = idx; i < 100 && i < GameManager.Instance.Enemy_List.Count; i++) // 특정 번호 부터 검색
        {
            if (GameManager.Instance.Enemy_List[i].gameObject.activeSelf) // 활동 중인 적들만 검색
            {
                if (Target == null)
                    Target = GameManager.Instance.Enemy_List[i];
                else
                {
                    if (Vector3.Distance(this.transform.position, Target.transform.position) > Vector3.Distance(this.transform.position, GameManager.Instance.Enemy_List[i].transform.position)) // 거리가 가까우면 등록
                        Target = GameManager.Instance.Enemy_List[i];
                }
            }
        }

        return Target;
    }

    /// <summary>
    /// 아이템 획득
    /// </summary>
    /// <param name="item"></param>
    void Get_Item(Ctr_Item.Item item)
    {
        switch (item.Specify)
        {
            case Ctr_Item.Specify.Heal:
                m_Stat.Hp += item.Value; // HP 회복
                break;
            case Ctr_Item.Specify.LevelUp:
                Init(m_Stat.Level + 1); // 레벨 업
                break;
        }
    }

    /// <summary>
    /// 게임 오버
    /// </summary>
    public override void Death()
    {
        base.Death();
        GameManager.Instance.GameOver();
    }

    /// <summary>
    /// 충돌 체크
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy") // 적과 충돌
        {
            Get_Damage(other.GetComponent<Ctr_Enemy>().m_Stat.Damage);
        }

        if (other.tag == "Spell") // 총알과 충돌
        {
            if (other.GetComponent<Ctr_Spell>().Spawn_Character.tag == "Enemy") // 소환한 캐릭터가 적인 경우
            {
                Get_Damage(other.GetComponent<Ctr_Spell>().Spawn_Character.m_Stat.Damage);
                other.gameObject.SetActive(!other.GetComponent<Ctr_Spell>().m_Spell.Destroy_Hitted); // 파괴되는 거면 파괴
            }
        }

        if (other.tag == "Item") // 아이템과 충돌
        {
            Get_Item(other.GetComponent<Ctr_Item>().m_Item); // 아이템 획득
            other.gameObject.SetActive(false);
        }
    }
}
