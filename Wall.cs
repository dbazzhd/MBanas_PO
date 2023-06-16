using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
    private void OnTriggerEnter(Collider pCollider) {
        if (pCollider.tag == "White") {
            Master.EnqueueEvent((pMaster) => {
                pMaster.ResetWhiteBall();
            });
        }
    }
}
