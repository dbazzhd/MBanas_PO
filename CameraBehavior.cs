using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {
    [SerializeField] private Camera m_camera = null;
    private Transform m_transform_cue = null;
    private Transform m_transform_whiteBall = null;

    public bool Play = false;

    public void Initialize(Transform pTransformCue, Transform pTransformWhiteBall) {
        m_transform_cue = pTransformCue;
        m_transform_whiteBall = pTransformWhiteBall;
    }

    void Update() {
        if (Play) {
            transform.position = m_transform_cue.position + (Vector3.up * 0.1f) - (m_transform_cue.forward * 0.25f);
            transform.LookAt(m_transform_whiteBall);
        }
    }

    public void ReadyUp() {
        transform.position = m_transform_cue.position - m_transform_cue.forward * 0.2f + Vector3.up * 0.2f;
        Play = true;
    }
}
