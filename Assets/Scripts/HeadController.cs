using UnityEngine;

public class HeadController : MonoBehaviour
{
    // Model selector
    public int modelSelect = 1;

    public GameObject theHead;
    public GameObject theLeftEye;
    public GameObject theRightEye;
    public GameObject theMouth;

    public static Vector3 headPos;
    public static Quaternion headRot;
    public static Vector3 leftEyeShape;
    public static Vector3 rightEyeShape;
    public static Vector3 mouthShape;


    // Start is called before the first frame update
    void Start()
    {
        theHead.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        switch(modelSelect)
        {
            case 1:// Model 1
            {
                //Apply position and rotation changes 位姿变化
                theHead.transform.SetPositionAndRotation(headPos, headRot);
                // For Debug only
                //Debug.Log(headRot);

                //Apply facial expression shapes changes 形变
                theLeftEye.transform.localScale = leftEyeShape;
                theRightEye.transform.localScale = rightEyeShape;
                theMouth.transform.localScale = mouthShape;
                break;
            }
            case 2:// Model 2
            {
                //Apply rotation changes 位姿变化
                theHead.transform.rotation = headRot;
                break;
            }
            default:
            {
                Debug.Log("Please Select a proper Model number.");
                break;
            }
        }
        
    }
}
