using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public Transform interActionPoint;


    public void OpenDoor() {

        gameObject.SetActive(false);
    }
}
