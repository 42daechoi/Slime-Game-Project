using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elements : MonoBehaviour
{
    public enum Type { Elements };
    public Type type;
    public int value;

    void update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
}
