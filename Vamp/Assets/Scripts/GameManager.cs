using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public partial class GameManager : MonoBehaviour
{
    public Stage_Info m_Stage_Info;
    public PoolManager PoolManager;
    public SpriteAtlas Main_Atlas;
    public Ctr_Player Player; // 플레이어
    public List<Ctr_Enemy> Enemy_List; // 적 리스트
    public List<Ctr_Spell> Spell_List; // 총알 리스트
    public List<GameObject> Effect_List; // 이펙트 리스트

    public GameObject[] DataLoad_Enemy_List; // 리소스 폴더에 저장되어 있는 적 로드
    public GameObject[] DataLoad_Spell_List; // 리소스 폴더에 저장되어 있는 스펠 로드
    public GameObject[] DataLoad_Effect_List; // 리소스 폴더에 저장되어 있는 이펙트 로드

    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    /// <summary>
    /// 리소스 로드 및 초기 세팅
    /// </summary>
    void Start()
    {
        DataLoad_Enemy_List = Resources.LoadAll<GameObject>("Enemy"); // 리소스 로드
        DataLoad_Spell_List = Resources.LoadAll<GameObject>("Spell"); // 리소스 로드
        DataLoad_Effect_List = Resources.LoadAll<GameObject>("Effect"); // 리소스 로드
        m_Stage_Info = new Stage_Info();

        // 미리 풀링
        for (int i = 0; i < 100; i++)
        {
            Ctr_Enemy t_Enemy = Instantiate(DataLoad_Enemy_List[i % DataLoad_Enemy_List.Length], Vector3.zero, Quaternion.identity).GetComponent<Ctr_Enemy>(); // 새로 생성, 적 종류만큼 나누어서 생성
            t_Enemy.transform.parent = PoolManager.Enemy; // 풀 매니저에 등록
            t_Enemy.Target = Player; // 타겟 지정
            t_Enemy.Init(i % DataLoad_Enemy_List.Length, 0); // 스테이지 레벨에 몬스터 스폰 및 초기화
            t_Enemy.gameObject.SetActive(false);
            Enemy_List.Add(t_Enemy);

            Ctr_Spell t_Spell = Instantiate(DataLoad_Spell_List[0], Vector3.zero, Quaternion.identity).GetComponent<Ctr_Spell>(); // 새로 생성
            t_Spell.transform.parent = PoolManager.Spell; // 풀 매니저에 등록
            t_Spell.gameObject.SetActive(false);
            Spell_List.Add(t_Spell);

            GameObject t_Effect = Instantiate(DataLoad_Effect_List[0], Vector3.zero, Quaternion.identity);
            t_Effect.transform.parent = PoolManager.Effect;
            t_Effect.SetActive(false);
            Effect_List.Add(t_Effect);
        }

        Btn_Init(); // 첫 한번 게임 초기화
    }


    /// <summary>
    /// 게임 초기화
    /// </summary>
    public void Btn_Init()
    {
        // 스테이지 기본 세팅
        m_Stage_Info.Game_State = Game_State.Ready;
        m_Stage_Info.Level = 1;
        m_Stage_Info.Monster_Count = 3;
        m_Stage_Info.Monster_Spawn_Time.Set(10f);

        // 오브젝트 풀 초기화
        foreach (Ctr_Enemy t in Enemy_List)
            t.gameObject.SetActive(false);

        foreach (Ctr_Spell t in Spell_List)
            t.gameObject.SetActive(false);

        foreach(GameObject t in Effect_List)
            t.gameObject.SetActive(false);
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void Btn_Play()
    {
        UIManager.Instance.Game_Start.SetActive(false);
        UIManager.Instance.Game_End.SetActive(false);
        Player.Init(1); // 1레벨로 초기화
        Btn_Init();
        Player.transform.position = Vector3.zero;
        Player.gameObject.SetActive(true);
        m_Stage_Info.Monster_Spawn_Time.Current = m_Stage_Info.Monster_Spawn_Time.Destiantion - 2f; // 처음에는 2초 후 생성
        m_Stage_Info.Game_State = Game_State.Play;
    }

    public void GameOver()
    {
        m_Stage_Info.Game_State = Game_State.GameOver;
        UIManager.Instance.Game_End.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Stage_Info.Game_State == Game_State.Play)
        {
            if (m_Stage_Info.Monster_Spawn_Time.Current >= m_Stage_Info.Monster_Spawn_Time.Destiantion) // 소환 시간 체크
            {
                Monster_Spawn(); // 몬스터 스폰
                m_Stage_Info.Monster_Spawn_Time.Current = 0f; // 소환 시간 리셋
            }
            else
                m_Stage_Info.Monster_Spawn_Time.Current += Time.deltaTime; // 소환 시간 증가
        }
    }

    /// <summary>
    /// 몬스터 소환
    /// </summary>
    void Monster_Spawn()
    {
        int spawn_Type = UnityEngine.Random.Range(0, 2);
        for (int i = 0; i < m_Stage_Info.Monster_Count; i++)
        {
            Ctr_Enemy t_Spawn = null;
            bool check_Spawn = false; // 스폰 체크
            for (int idx = 0; idx < Enemy_List.Count; idx++)
            {
                if (!Enemy_List[idx].gameObject.activeSelf && Enemy_List[idx].m_Stat.Idx == m_Stage_Info.Idx) // 죽은 상태인 경우
                {
                    check_Spawn = true;
                    Enemy_List[idx].Init(m_Stage_Info.Level); // 스테이지 레벨에 몬스터 스폰 및 초기화
                    t_Spawn = Enemy_List[idx]; // 기타 세팅을 위한 임시 저장
                    break;
                }
            }

            if (!check_Spawn)
            {
                Ctr_Enemy t = Instantiate(DataLoad_Enemy_List[m_Stage_Info.Idx], Vector3.zero, Quaternion.identity).GetComponent<Ctr_Enemy>(); // 새로 생성
                t.transform.parent = PoolManager.Enemy; // 풀 매니저에 등록
                t.Target = Player; // 타겟 지정
                t.Init(m_Stage_Info.Level); // 스테이지 레벨에 몬스터 스폰 및 초기화
                Enemy_List.Add(t); // 적 리스트에 등록
                t_Spawn = t; // 기타 세팅을 위한 임시 저장
            }

            // 위치 소환 - 플레이어 화면 밖에서 소환
            switch (spawn_Type)
            {
                case 0: // 주위 일정 반경 이상 일반 무작위 소환 * 카메라 거리에 랜덤
                    t_Spawn.transform.position = Player.transform.position + Random.onUnitSphere.normalized * UnityEngine.Random.Range(4f, 6f);
                    t_Spawn.transform.position = new Vector3(t_Spawn.transform.position.x, t_Spawn.transform.position.y, 0);
                    break;
                case 1: // 동그랗게 소환 - 전체 각도 / 소환 유닛 수 * 카메라 거리
                    Vector2 t_Pos = new Vector2(Mathf.Sin(Mathf.PI * 2 * i / m_Stage_Info.Monster_Count), Mathf.Cos(Mathf.PI * 2 * i / m_Stage_Info.Monster_Count));
                    t_Spawn.transform.position = t_Pos * 5f;
                    break;
            }

            t_Spawn.gameObject.SetActive(true);
        }

        // 한 번 스폰 시 몬스터 숫자 및 레벨 업그레이드
        m_Stage_Info.Idx += 1;
        if(m_Stage_Info.Idx >= DataLoad_Enemy_List.Length)
            m_Stage_Info.Idx = 0;
        m_Stage_Info.Level += 1;
        m_Stage_Info.Monster_Count += m_Stage_Info.Level * 2;
        Debug.Log("Monster Spawn : " + spawn_Type);
    }

    public void Show_Effect(Vector3 pos)
    {
        bool find = false;
        for(int i = 0 ; i < Effect_List.Count ; i++)
        {
            if(!Effect_List[i].activeSelf)
            {
                find = true;
                Effect_List[i].transform.position = pos;
                Effect_List[i].SetActive(true);
                break;
            }
        }

        if(!find)
        {
            GameObject t_Effect = Instantiate(DataLoad_Effect_List[0], Vector3.zero, Quaternion.identity);
            t_Effect.transform.parent = PoolManager.Effect;
            t_Effect.transform.position = pos;
            t_Effect.SetActive(true);
            Effect_List.Add(t_Effect);
        }
    }
}
