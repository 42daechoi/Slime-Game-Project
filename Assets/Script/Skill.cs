using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int EleType;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Tag_Slime" ) {
            PlayerControler pc = PlayerControler.instance;
            TargetTest tt = collision.gameObject.GetComponent<TargetTest>();
            if(tt!=null){
                tt.health -= pc.skillDamage;
            }
            Destroy(this.gameObject);
        }
        if(collision.gameObject.tag =="Wall"||collision.gameObject.tag == "Floor") {
            Destroy(this.gameObject,2);
        }
    }
}
