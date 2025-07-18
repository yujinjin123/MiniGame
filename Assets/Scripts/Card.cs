using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("是否已被使用")]
    public bool used;
    [Header("数字")]
    public int num;

    // 拖拽相关私有变量
    private Vector3 _startPosition;      // 拖拽前初始位置
    private Transform _startParent;       // 拖拽前父物体
    private CanvasGroup _canvasGroup;     // 用于控制射线检测
    private RectTransform _rectTransform; // UI位置控制组件
    private Vector3 _originalScale; // 新增：保存原始缩放比例
    private Text text;
    // 声明类成员变量，用于追踪上一次的高光区域
    private Quad _lastHighlightedQuad = null;
    private List<Quad> _lastHighlightedQuads = new List<Quad>();
    [Header("缩放系数")]
    public float scaleZoom;
    public List<Quad> pointQuads;

    // 初始化组件
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        _originalScale = transform.localScale; // 记录初始缩放值
        this.text = GetComponentInChildren<Text>();
        _startParent = transform.parent;
        _startPosition = _rectTransform.position;
    }

    // 开始拖拽 - 接口实现
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 已使用的卡牌不可拖拽
        if (used) return;
        // 临时禁用射线检测和透明度
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;

        // 确保卡牌显示在最上层
        transform.SetParent(transform.root); // 移动到Canvas根节点
        this.transform.localScale = _originalScale * scaleZoom;


    }

    //private void Update()
    //{
    //    // 核心修复：当检测不到Quad时，清除之前的高光
    //    if (Detection.curHitQuad == null)
    //    {
    //        Debug.Log("检测不到Quad");
    //        // 仅当之前存在高光时才重置
    //        if (_lastHighlightedQuad != null || _lastHighlightedQuads.Count > 0)
    //        {
    //            Debug.Log("检测不到Quad,清除旧高光");
    //            ResetHightLight(_lastHighlightedQuads);
    //            _lastHighlightedQuad = null;
    //            _lastHighlightedQuads.Clear();
    //        }
    //    }
    //}


    // 拖拽中 - 接口实现
    public void OnDrag(PointerEventData eventData)
    {
        if (used) return;

        // 位置计算逻辑保持不变
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint))
        {
            _rectTransform.position = worldPoint;
        }

        // 核心修复：当检测不到Quad时，清除之前的高光
        if (Detection.curHitQuad == null)
        {
            Debug.Log("检测不到Quad");
            // 仅当之前存在高光时才重置
            if (_lastHighlightedQuad != null || _lastHighlightedQuads.Count > 0)
            {
                Debug.Log("检测不到Quad,清除旧高光");
                ResetHightLight(_lastHighlightedQuads);
                _lastHighlightedQuad = null;
                _lastHighlightedQuads.Clear();
            }
            return; // 提前返回，避免后续无效逻辑
        }

        // 仅当目标Quad变化时更新高光（避免每帧重复操作）
        if (Detection.curHitQuad != _lastHighlightedQuad)
        {
            // 清除旧高光
            if (_lastHighlightedQuad != null)
            {
                Debug.Log("Quad变化,清除旧高光");
                ResetHightLight(_lastHighlightedQuads);
            }

            // 获取并设置新高光区域
            _lastHighlightedQuads = QuadHelper.instance.GetAllGoalAreas(this.num, Detection.curHitQuad);
            _lastHighlightedQuad = Detection.curHitQuad;
            SetHighLight(_lastHighlightedQuads); // 假设SetHighLight接受List<Quad>
        }
    }

    // 结束拖拽 - 接口实现
    public void OnEndDrag(PointerEventData eventData)
    {
        if (used) return;

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        // 检查是否放置到有效区域
        Quad dropArea = Detection.curHitQuad;
        ResetHightLight(_lastHighlightedQuads);

        if (dropArea == null || dropArea.num != -1)//CardManager.Instance.cards[CardManager.Instance.curCard] != this||
        {
            ResetPosition(); // 返回原位
            return;
        }

        if (dropArea != null && dropArea.CompareTag("Quad") && RoundManager.Instance.currentSelectedFace == dropArea.face)
        {
            ExecuteCardAction(dropArea); // 执行卡牌效果
        }
        else
        {
            ResetPosition(); // 返回原位
        }


    }

    public void ResetHightLight(List<Quad> quads)
    {
        if (quads == null)
            return;
        //放置后将得分点恢复原来的颜色
        foreach (var quad in quads)
        {
            quad.ResetHighLight();
        }
    }

    public void SetHighLight()
    {
        foreach (var quad in pointQuads)
        {
            quad.SetHighLight();
        }
    }

    public void SetHighLight(List<Quad> quads)
    {
        foreach (var quad in quads)
        {
            quad.SetHighLight();
        }
    }
    public void ResetHightLight()
    {
        //放置后将得分点恢复原来的颜色
        foreach (var quad in pointQuads)
        {
            quad.ResetHighLight();
        }
    }

    /// <summary>
    /// 使用卡牌
    /// </summary>
    /// <param name="dropArea"></param>
    private void ExecuteCardAction(Quad quad)
    {
        quad.SetNum(this.num);
        //放回原位
        ResetPosition();
        //隐藏卡片
        HideCard();
        RoundManager.Instance.UseCard();
        CardManager.Instance.NextCard();
    }

    // 重置卡牌位置
    public void ResetPosition()
    {
        transform.SetParent(_startParent);
        _rectTransform.position = _startPosition;
        this.transform.localScale = _originalScale;
    }

    /// <summary>
    /// 显示卡牌（视觉恢复+启用交互）
    /// </summary>
    public void ShowCard()
    {
        // 1. 恢复原始缩放
        transform.localScale = _originalScale;
        // 2. 启用交互
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        // 3. 重置使用状态
        used = false;
    }

    /// <summary>
    /// 隐藏卡牌（视觉消失+禁用交互）
    /// </summary>
    public void HideCard()
    {
        // 1. 缩放至零（视觉消失）
        transform.localScale = Vector3.zero;

        // 2. 禁用交互（避免隐藏后仍被点击）
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        // 3. 标记为已使用（按需）
        used = true;
    }

    public void SetNum(int num)
    {
        this.num = num;
        this.text.text = num.ToString();
    }
}
