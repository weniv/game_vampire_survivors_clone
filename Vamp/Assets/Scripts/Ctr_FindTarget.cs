using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ctr_FindTarget : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy") // 적은 사용 안함    
        {
            if (this.transform.parent.GetComponent<Ctr_Spell>().Target == null) // 타겟 없으면 등록
                this.transform.parent.GetComponent<Ctr_Spell>().Target = other.GetComponent<Character>();
            this.gameObject.SetActive(false);
        }
    }
}
