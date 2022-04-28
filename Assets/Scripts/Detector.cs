using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pallet")
        {
            GameManager.Instance.enteringBoxCount++;

            if (GameManager.Instance.enteringBoxCount >= 3)
                GameManager.Instance.Win();

            GetComponent<Collider>().enabled = false;
        }
    }
}
