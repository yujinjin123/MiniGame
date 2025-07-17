using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : Singleton<RoundManager>
{
    [Header("当前回合数")]
    public int currentRound = 1;
    List<Face> facesLeft=new List<Face>();
    public Face currentSelectedFace = Face.Front;
    [Header("要旋转的物体")]
    public Transform cube=null;
    [Header("主摄像机")]
    public Transform mainCamera;
    [Header("当前得分")]
    public int TotalPoint = 0;
    [Header("卡片")]
    public List<Card> cards;
    [Header("出牌数")]
    public static int cardsUsed;
    // 定义事件
    public System.Action OnRotationComplete; 
    // 初始化时建立面Tag与Transform的映射
    private Dictionary<Face, Transform> faceDict = new Dictionary<Face, Transform>();

    protected override void Awake()
    {
        OnRotationComplete += QuadManager.Instance.SetFaceOutLine;
        if (facesLeft.Count == 0)
        {
            facesLeft.Add(Face.Front);
            facesLeft.Add(Face.Back);
            facesLeft.Add(Face.Right);
            facesLeft.Add(Face.Left);
            facesLeft.Add(Face.Top);
            facesLeft.Add(Face.Bottom);
        }
        foreach (var face in facesLeft)
        {
            faceDict.Add(face, GameObject.FindGameObjectWithTag(face.ToString()).transform);
        }
    }

    private void Start()
    {
        NewRound();
    }

    public void UseCard()
    {
        cardsUsed++;
        if (cardsUsed >= 3)
        {
            NewRound();
        }
    }

    /// <summary>
    /// 随机选一面
    /// </summary>
    public void RandomSelect()
    { 
        //把上一次的outline取消掉
        QuadManager.Instance.ResetFaceOutLine(currentSelectedFace);
        int rand =Random.Range(0, facesLeft.Count);
        currentSelectedFace = facesLeft[rand];
        //Debug.Log(currentSelectedFace.ToString());
        facesLeft.Remove(currentSelectedFace);
        //开始旋转动画
        Debug.Log("我要转到" + faceDict[currentSelectedFace].ToString());
        StartCoroutine(RotateFaceToCameraOnce(2f, cube, faceDict[currentSelectedFace], Camera.main.transform.up));
    }

    /// <summary>
    /// 新一轮
    /// </summary>
    public void NewRound()
    {
        if (facesLeft.Count == 0)
        {
            facesLeft.Add(Face.Front);
            facesLeft.Add(Face.Back);
            facesLeft.Add(Face.Right);
            facesLeft.Add(Face.Left);
            facesLeft.Add(Face.Top);
            facesLeft.Add(Face.Bottom);
        }
        cardsUsed = 0;
        SetCards();
        RandomSelect();
    }

    public void SetCards()
    {
        //发三张牌
        var temp = new System.Random();
        foreach(var card in cards)
        {
            card.ShowCard();
            card.SetNum(temp.Next(1, 4));
        }
        
    }


    //private IEnumerator RotateFaceToCamera(float rotationDuration, Transform rolledTransform, Transform targetFace)
    //{
    //    // 记录初始状态
    //    Quaternion initialRolledRotation = rolledTransform.rotation;
    //    Vector3 initialFaceUp = targetFace.up; // 保存初始上方向[1](@ref)
    //    Quaternion initialFaceLocalRotation = targetFace.localRotation;

    //    // 计算目标方向：从targetFace指向摄像机
    //    Vector3 targetDirection = Camera.main.transform.position - targetFace.position;
    //    if (targetDirection == Vector3.zero) yield break; // 避免零向量错误

    //    // 计算目标旋转
    //    Quaternion targetFaceRotation = Quaternion.LookRotation(targetDirection, initialFaceUp); // 保持原始上方向[1,6](@ref)
    //    Quaternion targetRolledRotation = targetFaceRotation * Quaternion.Inverse(initialFaceLocalRotation); // 推导父物体目标旋转[1](@ref)

    //    // 平滑旋转过程
    //    float elapsedTime = 0f;
    //    while (elapsedTime < rotationDuration)
    //    {
    //        float progress = Mathf.Clamp01(elapsedTime / rotationDuration);
    //        rolledTransform.rotation = Quaternion.Slerp(
    //            initialRolledRotation,
    //            targetRolledRotation,
    //            progress
    //        );
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    // 确保最终精确朝向
    //    rolledTransform.rotation = targetRolledRotation;
    //    OnRotationComplete.Invoke();
    //}


    private IEnumerator RotateFaceToCameraOnce(
    float duration,
    Transform rolledTransform,
    Transform targetFace,
    Vector3 referenceUp         // 建议仍传 Vector3.up 或 Camera.main.transform.up
)
    {
        if (!rolledTransform || !targetFace) yield break;
        if (referenceUp.sqrMagnitude < 1e-8f) referenceUp = Vector3.up;

        /* ---------- ① 你的 dir 保持不变 ---------- */
        Vector3 dir = Camera.main.transform.forward;   // 按你现有逻辑
        if (dir.sqrMagnitude < 1e-8f) yield break;
        dir.Normalize();

        /* ---------- ② 先让 forward 对准 dir ---------- */
        Quaternion faceWorld = Quaternion.FromToRotation(targetFace.forward, dir) * targetFace.rotation;

        /* ---------- ③ 计算需要补的 roll（delta）并直接并入 faceWorld ---------- */
        {
            Vector3 newUp = faceWorld * Vector3.up;                 // 应用完 forward 后的世界 up
            Vector3 projNewUp = Vector3.ProjectOnPlane(newUp, dir);     // 投影到垂直 dir 的平面
            Vector3 projRefUp = Vector3.ProjectOnPlane(referenceUp, dir);

            float delta = Vector3.SignedAngle(projNewUp, projRefUp, dir);
            if (Mathf.Abs(delta) > 0.01f)                                // 过滤极小误差
            {
                faceWorld = Quaternion.AngleAxis(delta, dir) * faceWorld;
            }
        }

        /* ---------- ④ 推导父物体一次性目标旋转 ---------- */
        Quaternion parentTarget = faceWorld * Quaternion.Inverse(targetFace.localRotation);

        /* ---------- ⑤ 平滑插值（一次 Slerp 完成全部旋转） ---------- */
        Quaternion parentStart = rolledTransform.rotation;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            rolledTransform.rotation = Quaternion.Slerp(parentStart, parentTarget, t / duration);
            yield return null;
        }
        rolledTransform.rotation = parentTarget;

        OnRotationComplete?.Invoke();
    }

}
