using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Detection : MonoBehaviour
{
    public bool active=true;
    public float maxDistance = Mathf.Infinity; // 射线最大距离
    public LayerMask layerMask = Physics.AllLayers; // 检测层级
    public Quad lastHit = null;
    public RoundManager roundManager;
    public static Quad curHitQuad=null;
    // Update is called once per frame
    void Update()
    {
        Quad hitQuad=null;
        if (active) // 仅在激活状态检测
        {
            // 创建从摄像机到鼠标位置的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 执行射线检测
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {   
                hitQuad=hit.collider.GetComponent<Quad>();
                curHitQuad = hitQuad;
                if(lastHit != null && hitQuad!=lastHit)
                {
                    lastHit.ResetSelected();
                }    
                hitQuad.SetSelected();
                lastHit = hitQuad;
            }
            else
            {
                lastHit?.ResetSelected();
                curHitQuad =null;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            roundManager.NewRound();
        }
    }
}
