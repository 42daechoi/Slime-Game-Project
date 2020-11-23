using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTest : MonoBehaviour
{
    public int health = 100;

    void Update()
    {
        checkDead();
    }

    void checkDead()
    {
        if(health<=0) Destroy(this.gameObject);
    }
}
