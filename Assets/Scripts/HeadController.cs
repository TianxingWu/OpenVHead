using UnityEngine;

public class HeadController : MonoBehaviour
{
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
        //位姿变化 Apply position and rotation changes
        theHead.transform.SetPositionAndRotation(headPos, headRot);
        // For Debug only
        //Debug.Log(headRot);

        //形变  Apply facial expression shapes changes
        theLeftEye.transform.localScale = leftEyeShape;
        theRightEye.transform.localScale = rightEyeShape;
        theMouth.transform.localScale = mouthShape;
    }
}
