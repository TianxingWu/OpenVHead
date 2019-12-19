using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapesController : MonoBehaviour
{
    public int leftEyeNum;
    public int rightEyeNum;
    public int mouthWidNum;
    public int mouthLenNum;
    public int shockEyeNum;

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
        // Set blend shape for left eye
        if(leftEyeShape[1]<0.05f)//闭眼 //参数匹配：ParameterServer -> 参数控制!闭眼
        {
            skinnedMeshRenderer.SetBlendShapeWeight (leftEyeNum, 100);
        }
        else if(leftEyeShape[1]<0.1f)
        {
            skinnedMeshRenderer.SetBlendShapeWeight (leftEyeNum, 5/leftEyeShape[1]);
            
        }
        else if(leftEyeShape[1]<0.2f)
        {
            skinnedMeshRenderer.SetBlendShapeWeight (leftEyeNum, -500*leftEyeShape[1]+100);  
            //Debug.Log(leftEyeShape[1]);
        }
        else
        {
            skinnedMeshRenderer.SetBlendShapeWeight (leftEyeNum, 0);
        }
        
        // Set blend shape for right eye
        if(rightEyeShape[1]<0.05f)
        {
            skinnedMeshRenderer.SetBlendShapeWeight (rightEyeNum, 100);
        }
        else if(rightEyeShape[1]<0.1f)
        {
            skinnedMeshRenderer.SetBlendShapeWeight (rightEyeNum, 5/rightEyeShape[1]);
            
        }
        else if(rightEyeShape[1]<0.2f)
        {
            
            skinnedMeshRenderer.SetBlendShapeWeight (rightEyeNum, -500*rightEyeShape[1]+100);
        }
        else
        {
            skinnedMeshRenderer.SetBlendShapeWeight (rightEyeNum, 0);
        }

        // Shocked
        if(rightEyeShape[1]>0.25f)//惊愕 0.25-0.35线性
        {
            skinnedMeshRenderer.SetBlendShapeWeight (shockEyeNum, 500*rightEyeShape[1]-125);
        }
        else
        {
            skinnedMeshRenderer.SetBlendShapeWeight (shockEyeNum, 0);
        }

        // Set blend shape for mouth
        if (500*mouthShape[1] < 100f)
        {
            skinnedMeshRenderer.SetBlendShapeWeight (mouthWidNum, 500*mouthShape[1]);
        }

        if(mouthShape[0] <0.1f)
        {
            skinnedMeshRenderer.SetBlendShapeWeight (mouthLenNum, 50);
        }
        else if(mouthShape[0] < 0.4f)
        {
            skinnedMeshRenderer.SetBlendShapeWeight (mouthLenNum, 120f-400*mouthShape[0]);
        }
        else
        {
            skinnedMeshRenderer.SetBlendShapeWeight (mouthLenNum, 0);
        }
    }
}