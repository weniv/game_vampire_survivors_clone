using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctr_Spell : Character
{
    public Character Target;
    public int Idx; // 스펠 구분
    public Spell m_Spell; // 스펠 속성
    public Character Spawn_Character; // 소환한 캐릭터
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.m_Stage_Info.Game_State == GameManager.Game_State.Play)
        {
            switch (m_Spell.Type) // 스펠 종류
            {
                case Spell_Type.Normal_Attack:
                    break;
                case Spell_Type.Normal_Bullet:
                    this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + m_Spell.Direction, Time.deltaTime * m_Spell.Move_Speed);
                    break;
                case Spell_Type.Guided_Bullet:
                    if (Target != null && Target.gameObject.activeSelf) // 적이 있으면 찾아가기 - 속도 증가
                        this.transform.position = Vector3.MoveTowards(this.transform.position, Target.transform.position, Time.deltaTime * m_Spell.Move_Speed * 3f);
                    else // 없으면 방향대로
                        this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + m_Spell.Direction, Time.deltaTime * m_Spell.Move_Speed);
                    break;
                case Spell_Type.Shield_Damage:
                    break;
            }
        }
    }

    /// <summary>
    /// 적 찾기
    /// </summary>
    public void Find_Target()
    {
        for (int i = 0; i < 100 && i < GameManager.Instance.Enemy_List.Count; i++)
        {
            if (GameManager.Instance.Enemy_List[i].gameObject.activeSelf)
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
    }

    /// <summary>
    /// 활동 상태가 되면 자동 폭파 시작
    /// </summary>
    private void OnEnable()
    {
        if(m_Spell.Destroy_Time)
            StartCoroutine(Active_Off());
    }

    /// <summary>
    /// 3초 뒤 자동 폭파
    /// </summary>
    /// <returns></returns>
    IEnumerator Active_Off()
    {
        Target = null; // 타겟 초기화
        this.transform.GetChild(0).gameObject.SetActive(true); // 찾기 범위 켜기
        yield return new WaitForSeconds(3.0f);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 사라질 때 모든 코루틴 종료
    /// </summary>
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
