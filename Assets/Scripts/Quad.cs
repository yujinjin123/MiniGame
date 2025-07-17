using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Quad : MonoBehaviour
{
    /// <summary>
    /// 朝向
    /// </summary>
    public Face face;
    [Header("行")]
    public int row;
    [Header("列")]
    public int col;
    [Header("安放的数字")]
    public int num=-1;
    public Color originColor;
    public float intensity=0f;
    public bool highLight;
    private void Awake()
    {
        this.originColor = this.GetComponent<MeshRenderer>().material.color;
    }

    public void SetNum(int newNum)
    {
        if (this.num == -1)
        {
            this.num = newNum;
            this.GetComponentInChildren<Text>().text = num.ToString();
            //通知后端计算得分
            TotalPoint.Instance.GetPoint(QuadHelper.instance.GetGoals(this));
        }
        else
            return;

    }

    public void SetHighLight()
    {
        highLight = true;
        this.intensity = 2f;
        this.GetComponent<MeshRenderer>().material.SetColor("_Color", originColor * intensity);
    }

    public void ResetHighLight()
    {
        highLight=false;
        this.intensity = 1f;
        this.GetComponent<MeshRenderer>().material.SetColor("_Color", originColor *intensity);
    }

    public void SetSelected()
    {
        this.GetComponent<MeshRenderer>().material.SetColor("_Color", originColor * 2f);
    }

    public void ResetSelected()
    {
        if(highLight)
        {
            this.GetComponent<MeshRenderer>().material.SetColor("_Color", originColor * intensity);
        }
        else
        {
            this.GetComponent<MeshRenderer>().material.SetColor("_Color", originColor);
        }
    }
    public void SetOutLine()
    {
        this.GetComponent<Outline>().enabled = true;
    }

    public void ResetOutLine()
    {
        this.GetComponent<Outline>().enabled = false;
    }
}
