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
        if (1/leftEyeShape[1] < 100f) 
        {
            if(1/leftEyeShape[1] > 20f)//闭眼 //参数匹配：ParameterServer -> 参数控制!闭眼
            {
                skinnedMeshRenderer.SetBlendShapeWeight (2, 100);
            }
            else//正常
            {
                skinnedMeshRenderer.SetBlendShapeWeight (2, 1/leftEyeShape[1]);
                Debug.Log(leftEyeShape[1]);
            }
        }
        
        //右眼形变
        if (1/rightEyeShape[1] < 100f) 
        {
            if(1/rightEyeShape[1] > 20f)
            {
                skinnedMeshRenderer.SetBlendShapeWeight (3, 100);
            }
            else
            {
                skinnedMeshRenderer.SetBlendShapeWeight (3, 1/rightEyeShape[1]);
            }
        }

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