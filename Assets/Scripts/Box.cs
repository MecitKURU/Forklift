using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            GameManager.Instance.Fail();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ForkPlatform")
        {
            GameObject.FindObjectOfType<ForkliftController>().myBox = transform.parent.gameObject;
        }

        if(other.tag == "Death")
        {
            GameManager.Instance.Fail();
        }
    }
}
