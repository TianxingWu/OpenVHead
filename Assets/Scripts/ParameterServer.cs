using UnityEngine;
using System;

public class ParameterServer : MonoBehaviour
{
    // For Debug only
    //private DateTime beforeDT = System.DateTime.Now;

    // Model selector
    public int modelSelect = 1;

    // Control objects
    ControlObject LeftEyeControl = new ControlObject();
    ControlObject RightEyeControl = new ControlObject();
    ControlObject MouthWidControl = new ControlObject();
    ControlObject MouthLenControl = new ControlObject();

    // Head parameters
    public string getData;
    public Vector3 getheadPos;
    public Quaternion getheadRot;
    public Vector3 getleftEyeShape;
    public Vector3 getrightEyeShape;
    public Vector3 getmouthShape;

    private string[] tempdata;


    // Start is called before the first frame update
    void Start()
    {
        
        switch(modelSelect)
        {
            case 1:// Initialization for Model 1
            {
                // Initialize head parameters
                getheadPos.x = 0;
                getheadPos.y = 0;
                getheadPos.z = -2;
                getheadRot.w = 0;
                getheadRot.x = 1;
                getheadRot.y = 0;
                getheadRot.z = 0;
                getleftEyeShape = new Vector3(0.2f, 0.2f, 0.2f);
                getrightEyeShape = new Vector3(0.2f, 0.2f, 0.2f);
                getmouthShape = new Vector3(0.45f, 0.03f, 0.2f);

                // Initialize control objects
                RightEyeControl.M = 1;
                RightEyeControl.ALPHA = 0.8f;
                RightEyeControl.KP = 0.04f;
                RightEyeControl.KD = 1;   

                LeftEyeControl.M = 1;
                LeftEyeControl.ALPHA = 0.8f;
                LeftEyeControl.KP = 0.04f;
                LeftEyeControl.KD = 1;

                MouthWidControl.M = 1;
                MouthWidControl.ALPHA = 0.7f;
                MouthWidControl.KP = 0.04f;
                MouthWidControl.KD = 1;

                MouthLenControl.M = 1;
                MouthLenControl.ALPHA = 0.7f;
                MouthLenControl.KP = 0.04f;
                MouthLenControl.KD = 1;

                // Select control mode
                RightEyeControl.mode = 1;
                LeftEyeControl.mode = 1;
                MouthLenControl.mode = 1;
                MouthWidControl.mode = 1;
                break;
            }

            case 2:// Initialization for Model 2
            {
                // Initialize head parameters
                getheadPos.x = 0;
                getheadPos.y = 0;
                getheadPos.z = -2;
                getheadRot.w = 0;
                getheadRot.x = 1;
                getheadRot.y = 0;
                getheadRot.z = 0;
                getleftEyeShape = new Vector3(0.2f, 0.2f, 0.2f);
                getrightEyeShape = new Vector3(0.2f, 0.2f, 0.2f);
                getmouthShape = new Vector3(0.45f, 0.03f, 0.2f);

                // Initialize control objects
                RightEyeControl.M = 4;
                RightEyeControl.ALPHA = 0.8f;//test...
                RightEyeControl.KP = 0.04f;
                RightEyeControl.KD = 1;   

                LeftEyeControl.M = 4;
                LeftEyeControl.ALPHA = 0.8f;//test...
                LeftEyeControl.KP = 0.04f;
                LeftEyeControl.KD = 1;

                // Select Control Mode
                RightEyeControl.mode = 1;
                LeftEyeControl.mode = 1;
                MouthLenControl.mode = 1;
                MouthWidControl.mode = 1;

                break;
            }
            default:
            {
                Debug.Log("Please Select a proper Model number.");
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get data from socket
        try
        {
            getData = SocketServer.data;

            tempdata = getData.Split(':');

            //位置 Get Position
            getheadPos.x = -Convert.ToSingle(tempdata[0]) / 1000;
            getheadPos.y = -Convert.ToSingle(tempdata[1]) / 1000;
            getheadPos.z = -Convert.ToSingle(tempdata[2]) / 1000;
            //姿态 Get Rotation
            getheadRot.w = Convert.ToSingle(tempdata[3]);
            getheadRot.x = Convert.ToSingle(tempdata[4]);
            getheadRot.y = Convert.ToSingle(tempdata[5]);
            getheadRot.z = Convert.ToSingle(tempdata[6]);
            //眼部嘴部形状 Get Facial expression shapes
            getleftEyeShape = new Vector3(0.2f, Convert.ToSingle(tempdata[7]), 0.2f);
            getrightEyeShape = new Vector3(0.2f, Convert.ToSingle(tempdata[8]), 0.2f);
            getmouthShape = new Vector3(Convert.ToSingle(tempdata[10]), Convert.ToSingle(tempdata[9]), 0.2f);
        }
        catch(Exception)
        {
            Debug.Log("No Parameter Received.");
        }

        // Select a model
        switch(modelSelect)
        {
            case 1:// Update for Model 1
            {
                // Facial expression control!
                // 1. Right Eye
                if(getrightEyeShape[1]<0.07f)// Right eye closed
                {
                    getrightEyeShape[1] = 0.01f;
                }
                else// Right eye opened
                {
                    getrightEyeShape[1] = RightEyeControl.control(HeadController.rightEyeShape[1], getrightEyeShape[1]);
                }
                // 2. Left Eye
                if(getleftEyeShape[1]<0.07f)// Left eye closed
                {
                    getleftEyeShape[1] = 0.01f;
                }
                else// Left eye opened
                {
                    getleftEyeShape[1] = LeftEyeControl.control(HeadController.leftEyeShape[1], getleftEyeShape[1]);
                }
                // 3. Mouth Length
                getmouthShape[0] = MouthLenControl.control(HeadController.mouthShape[0], getmouthShape[0]);
                // 4. Mouth Width
                getmouthShape[1] = MouthWidControl.control(HeadController.mouthShape[1], getmouthShape[1]);

                // For Debug only
                //Debug.Log(RightEyeControl.x);
                //Debug.Log(RightEyeControl.isBlinking);


                // Update global variables 对全局变量进行更新
                // 1. Update position 更新位置 
                HeadController.headPos.x = getheadPos.x;
                HeadController.headPos.y = getheadPos.y;
                HeadController.headPos.z = getheadPos.z;
                // 2. Update rotation 更新姿态 
                HeadController.headRot.w = getheadRot.w;
                HeadController.headRot.x = getheadRot.x;
                HeadController.headRot.y = getheadRot.y;
                HeadController.headRot.z = getheadRot.z;
                // 3. Update facial expression shapes 更新形状 
                HeadController.leftEyeShape = getleftEyeShape;
                HeadController.rightEyeShape = getrightEyeShape;
                HeadController.mouthShape = getmouthShape;
                break;
            }

            case 2:// Update for Model 2
            {
                // Facial expression control!
                // 1. Right Eye
                if(1/getrightEyeShape[1]>20)// Right eye closed //注意参数匹配关系：BlendShapesController -> 眼形变 正常
                {
                    getrightEyeShape[1] = 0.01f;
                }
                else// Right eye opened
                {
                    getrightEyeShape[1] = RightEyeControl.control(BlendShapesController.rightEyeShape[1], getrightEyeShape[1]);
                }
                // 2. Left Eye
                if(1/getleftEyeShape[1]>20)// Left eye closed
                {
                    getleftEyeShape[1] = 0.01f;
                }
                else// Left eye opened
                {
                    getleftEyeShape[1] = LeftEyeControl.control(BlendShapesController.leftEyeShape[1], getleftEyeShape[1]);
                }
                // 3. Mouth Length
                getmouthShape[0] = MouthLenControl.control(BlendShapesController.mouthShape[0], getmouthShape[0]);
                // 4. Mouth Width
                getmouthShape[1] = MouthWidControl.control(BlendShapesController.mouthShape[1], getmouthShape[1]);
                

                // Update global variables 对全局变量进行更新
                // 1. Update rotation 更新姿态 
                HeadController.headRot.w = (float)(Math.Cos(1.74)*getheadRot.w -  Math.Sin(1.74)*getheadRot.x);
                HeadController.headRot.x = (float)(Math.Cos(1.74)*getheadRot.x +  Math.Sin(1.74)*getheadRot.w);
                HeadController.headRot.y = -(float)(Math.Cos(1.74)*getheadRot.y -  Math.Sin(1.74)*getheadRot.z);
                HeadController.headRot.z = -(float)(Math.Cos(1.74)*getheadRot.z +  Math.Sin(1.74)*getheadRot.y);
                // 2. Update facial expression shapes 更新形状 
                BlendShapesController.leftEyeShape = getleftEyeShape;
                BlendShapesController.rightEyeShape = getrightEyeShape;
                BlendShapesController.mouthShape = getmouthShape;

                // For Debug only
                //Debug.Log(HeadController.headRot);
                
                break;
            }

            default:
            {
                Debug.Log("Please Select a proper Model number.");
                break;
            }

        }
        

        // For Debug only
        //DateTime afterDT = System.DateTime.Now;
        //TimeSpan ts = afterDT.Subtract(beforeDT);
        //beforeDT = System.DateTime.Now;
        //Console.WriteLine("Time spend: {0}ms",ts.TotalMilliseconds);
        //Debug.Log(ts.TotalMilliseconds);

    }

   
}

class ControlObject
{
    // member variables
    public float T = 0.1f; // time interval
    public float ALPHA = 0.7f; // incomplete derivative coefficient
    public float KP = 0.04f;
    public float KD = 1;
    public float M = 1; // mass
    public float a = 0; // acceleration
    public float v = 0; // velocity
    public float x = 0; // position
    public float x_d = 0; // desired position
    public float e = 0; // error
    public float e_1 = 0; // last error
    public float de = 0; // derivative of error
    public float p_out = 0; // proportional termd_outd_out_1
    public float d_out = 0; // derivative term
    public float d_out_1 = 0; // last derivative term 
    public float F = 0; // control force

    public float THRESH = 0.05f; // control law changing threshold
    public bool isBlinking = false;
    public int mode;// control mode

    // member methods
    public float control(float X, float X_D)
    {
        x = X;
        x_d = X_D;

        // Incomplete derivative PD control
        // Control Law ==================================================
        e = x_d - x; // Update error
        de = (e - e_1)/T; // Compute the derivative of error
        p_out = KP*e;
        d_out = (1-ALPHA)*KD*de + ALPHA*d_out_1;
        
        switch(mode)
        {
            case 1:
                F = p_out + d_out; // Update control force
                break;

            case 2: // Many bugs !!!!!!!!
                if(X_D < THRESH && X > THRESH)
                {
                    isBlinking = true;
                    F = -1;
                }
                else if(isBlinking == true)
                {
                    if(X>0.001f)
                    {
                        isBlinking = true;
                        F = -1;
                    }
                    else
                    {
                        isBlinking = false;
                        F = p_out + d_out;
                    }
                }
                else
                {
                    F = p_out + d_out; // Update control force
                }
                break;
            default:
                break;

        }
        

        e_1 = e;// Update last error
        d_out_1 = d_out; // Update last derivative term

        // System Law ==================================================
        a = F/M; // Update acceleration
        v = v + a*T; // Update velocity
        x = x + v*T; // Update position
        if(x<0)
        {
            x = 0;
        }
        
        return x;
    }
}
