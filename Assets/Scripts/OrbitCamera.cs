using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [Header("绑定目标")]
    public Transform target;          // 绑定的中心物体

    [Header("旋转参数")]
    public float rotationSpeed = 5f;   // 旋转速度
    public float minVerticalAngle = -30f; // 最小俯角
    public float maxVerticalAngle = 70f;  // 最大俯角

    [Header("距离控制")]
    public float distance = 5f;        // 初始距离
    public float minDistance = 2f;      // 最小距离
    public float maxDistance = 15f;     // 最大距离
    public float zoomSpeed = 5f;       // 缩放速度

    [Header("平滑过渡")]
    public float damping = 10f;        // 旋转/缩放的阻尼系数

    private float currentXAngle;       // 当前水平旋转角
    private float currentYAngle;       // 当前垂直旋转角
    private Vector3 offsetDirection;    // 相机相对目标的偏移方向

    void Start()
    {
        // 初始化相机角度和方向
        Vector3 angles = transform.eulerAngles;
        currentXAngle = angles.y;
        currentYAngle = angles.x;
        offsetDirection = (transform.position - target.position).normalized;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 鼠标右键旋转控制
        if (Input.GetMouseButton(1))
        {
            currentXAngle += Input.GetAxis("Mouse X") * rotationSpeed;
            currentYAngle -= Input.GetAxis("Mouse Y") * rotationSpeed;
            // 限制垂直旋转角度（防止翻转）
            currentYAngle = Mathf.Clamp(currentYAngle, minVerticalAngle, maxVerticalAngle);
        }

        // 鼠标滚轮缩放控制
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // 计算目标旋转和位置
        Quaternion targetRotation = Quaternion.Euler(currentYAngle, currentXAngle, 0);
        Vector3 targetPosition = target.position + targetRotation * offsetDirection * distance;

        // 应用平滑过渡
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, damping * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime);
    }
}