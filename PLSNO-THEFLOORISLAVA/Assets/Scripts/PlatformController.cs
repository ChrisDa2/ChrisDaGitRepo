using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private float speed = 0.01f;
    private void Update()
    {
        transform.position +=new Vector3(0, -speed, 0);
    }
}
