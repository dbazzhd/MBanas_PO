using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour {
    [SerializeField] private Rigidbody m_rigidBody = null;
    [SerializeField] private MeshRenderer m_meshRenderer = null;
    private Material m_material = null;
    private Vector3 m_startPosition = Vector3.zero;
    private int m_kind = -1;

    public bool Play = false;

    public void Initialize(Vector3 pPosition, Color pColor, int pFull, float pSleepThreshold) {
        Initialize_Rigidbody(pSleepThreshold);
        Initialize_Material(pColor, pFull);
        m_startPosition = pPosition;
    }

    // Update is called once per frame
    void Update() {

    }

    private void Initialize_Rigidbody(float pSleepThreshold) {
        m_rigidBody.mass = 2f;
        m_rigidBody.drag = 0.125f;
        m_rigidBody.angularDrag = 0.25f;
        m_rigidBody.sleepThreshold = pSleepThreshold;
        m_rigidBody.useGravity = true;
        m_rigidBody.isKinematic = false;
        m_rigidBody.interpolation = RigidbodyInterpolation.None;
        m_rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        m_rigidBody.constraints = RigidbodyConstraints.None;
    }

    private void Initialize_Material(Color pColor, int pFull) {
        m_material = m_meshRenderer.material;
        m_material.SetColor("_Color", pColor);
        m_material.SetFloat("_Full", pFull);
        m_kind = pFull;
    }

    public bool IsSleeping() => m_rigidBody.IsSleeping();
    public void StopVelocity() {
        m_rigidBody.velocity = Vector3.zero;
        m_rigidBody.angularVelocity = Vector3.zero;
        m_rigidBody.Sleep();
    }

    public int GetKind() => m_kind;

    public void ReadyUp() {
        StopVelocity();
        transform.position = m_startPosition;
        Play = true;
    }

    public void Shoot(Vector3 pDirection) {
        if (Play) {
            m_rigidBody.AddForce(pDirection, ForceMode.Force);
        }
    }
}
