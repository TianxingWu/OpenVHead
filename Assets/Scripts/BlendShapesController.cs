using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapesController : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    public static Vector3 leftEyeShape;
    public static Vector3 rightEyeShape;
    public static Vector3 mouthShape;

    void Awake ()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();
        skinnedMesh = GetComponent<SkinnedMeshRenderer> ().sharedMesh;
    }

    void Start ()
    {
    }

    void Update ()
    {
        //左眼形变
        // if (1/leftEyeShape[1] < 100f) 
        // {
            if(leftEyeShape[1]<0.05f)//闭眼 //参数匹配：ParameterServer -> 参数控制!闭眼
            {
                skinnedMeshRenderer.SetBlendShapeWeight (2, 100);
            }
            else if(leftEyeShape[1]<0.1f)
            {
                skinnedMeshRenderer.SetBlendShapeWeight (2, 5/leftEyeShape[1]);
                
            }
            else if(leftEyeShape[1]<0.2f)
            {
                skinnedMeshRenderer.SetBlendShapeWeight (2, -500*leftEyeShape[1]+100);  
                //Debug.Log(leftEyeShape[1]);
            }
            else
            {
                skinnedMeshRenderer.SetBlendShapeWeight (2, 0);
            }
        // }
        
        //右眼形变
        // if (1/rightEyeShape[1] < 100f) 
        // {
            if(rightEyeShape[1]<0.05f)
            {
                skinnedMeshRenderer.SetBlendShapeWeight (3, 100);
            }
            else if(rightEyeShape[1]<0.1f)
            {
                skinnedMeshRenderer.SetBlendShapeWeight (3, 5/rightEyeShape[1]);
                
            }
            else if(rightEyeShape[1]<0.2f)
            {
                
                skinnedMeshRenderer.SetBlendShapeWeight (3, -500*rightEyeShape[1]+100);
            }
            else
            {
                skinnedMeshRenderer.SetBlendShapeWeight (3, 0);
            }
        // }

        //惊愕
        if(rightEyeShape[1]>0.25f)//惊愕 0.25-0.35线性
        {
            skinnedMeshRenderer.SetBlendShapeWeight (29, 500*rightEyeShape[1]-125);
        }
        else
        {
            skinnedMeshRenderer.SetBlendShapeWeight (29, 0);
        }


        if (500*mouthShape[1] < 100f)
        {
            skinnedMeshRenderer.SetBlendShapeWeight (9, 500*mouthShape[1]);
        }
    }
}