using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="Wall"||collision.gameObject.tag == "Floor"||collision.gameObject.tag == "Player"){
            Destroy(gameObject,2);
        }
    }
}
