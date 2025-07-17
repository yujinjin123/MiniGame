using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Face;

public class QuadManager : Singleton<QuadManager>
{
    Dictionary<Face, List<Quad>> quads;
    public List<Quad> allTheQuads=new List<Quad>();
    

    protected override void Awake()
    {
        allTheQuads.AddRange(this.GetComponentsInChildren<Quad>());
        quads =new Dictionary<Face, List<Quad>>();
        quads.Add(Front, new List<Quad>());
        quads.Add(Back, new List<Quad>());
        quads.Add(Left, new List<Quad>());
        quads.Add(Right, new List<Quad>());
        quads.Add(Top, new List<Quad>());
        quads.Add(Bottom, new List<Quad>());
    }

    private void Start()
    {
        //所有的quad
        for(int i=0;i<allTheQuads.Count;i++)
        {
            quads[allTheQuads[i].face].Add(allTheQuads[i]);
        }
    }

    public void SetHighLightPointQuad(List<Quad> quads)
    {
        foreach (var quad in quads)
        {
            quad.SetHighLight();
        }
    }

    public void ResetHighLightPointQuad(List<Quad> quads)
    {
        foreach (var quad in quads)
        {
            quad.ResetHighLight();
        }
    }

    public void SetFaceOutLine()
    {

        foreach (var quad in quads[RoundManager.Instance.currentSelectedFace])
        {
            quad.SetOutLine();
        }
    }

    public void SetFaceOutLine(Face face)
    {
        
        foreach(var quad in quads[face])
        {
            quad.SetOutLine();
        }
    }

    public void ResetFaceOutLine(Face face)
    {
        foreach (var quad in quads[face])
        {
            quad.ResetOutLine();
        }
    }



}
