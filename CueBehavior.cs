using UnityEngine;
using UnityEngine.UI;

public class CueBehavior : MonoBehaviour {

    private float m_cueLength = 1.6f;
    private float m_rotateSpeed = 30.0f;
    private Transform m_transform_ball = null;

    public bool Play = false;

    public void Initialize(float pRotateSpeed, Transform pTransformBall) {
        m_rotateSpeed = pRotateSpeed;
        m_transform_ball = pTransformBall;
    }

    void Update() {
        if (Play) {
            RotateAroundBall();
            transform.LookAt(m_transform_ball);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, m_transform_ball.position);
    }

    public void ReadyUp() {
        transform.position = m_transform_ball.position + Vector3.forward * m_cueLength + Vector3.up * 0.2f;
        transform.LookAt(m_transform_ball);
        Play = true;
    }

    private void RotateAroundBall() {
        if (Input.GetMouseButton(1)) {
            float axisX = Input.GetAxisRaw("Mouse X");
            if (axisX != 0.0f) {
                transform.RotateAround(m_transform_ball.position, Vector3.up, (-axisX * m_rotateSpeed).ToRadians());
            }
            float axisY = Input.GetAxisRaw("Mouse Y");
            if (axisY != 0.0f) {
                transform.RotateAround(m_transform_ball.position, transform.right, (axisY * m_rotateSpeed).ToRadians());
            }
        }
    }
}
