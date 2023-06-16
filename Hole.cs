using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour {
    void OnTriggerEnter(Collider pCollider) {
        string tag = pCollider.tag;
        if (tag == "Ball" || tag == "White") {
            Master.EnqueueEvent((pMaster) => {
                pMaster.BallPotted(pCollider.gameObject.GetComponent<BallBehavior>());
            });
        }
    }
}
