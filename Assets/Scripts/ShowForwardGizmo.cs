using UnityEngine;

/// <summary>
/// 在 Scene 视图用可选颜色的线段显示物体的 forward（Z 正）方向。
/// </summary>
[ExecuteAlways]
[AddComponentMenu("Debug/Show Forward Gizmo (Color)")]
public class ShowForwardGizmo : MonoBehaviour
{
    [Header("线段长度（世界单位）")]
    [Min(0.01f)]
    public float length = 1.0f;

    [Header("线段颜色")]
    public Color gizmoColor = Color.yellow;

    /// <summary>
    /// 若只想在选中时显示，请改为 OnDrawGizmosSelected。
    /// </summary>
    private void OnDrawGizmos()
    {
        // 设置颜色
        Gizmos.color = gizmoColor;

        // 计算起点 & 终点
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * length;

        // 绘制线段和终点小球
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, length * 0.05f);
    }
}
