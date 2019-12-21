using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawData : MonoBehaviour
{
    public RawImage m_rawImage;
    public Texture2D m_texture;
    public int dataPoint;
    public int dataSelect;
    private int graphWidth = 300;
    private int graphHeight = 300;
    private int[] dataArray;
    private Color[] pixels;
    //private Color GraphBackground = new Color(0.3f, 0.4f, 0.6f, 0.8f);
    private Color GraphBackground = new Color(0, 1, 0, 1);
    private Color LineColor = new Color(1.0f, 0.0f, 0.0f, 1f);
    

    // Start is called before the first frame update
    void Start()
    {
        m_texture = new Texture2D(graphWidth, graphHeight);
        m_rawImage.texture = m_texture;
        m_rawImage.SetNativeSize();

        pixels = new Color[graphWidth * graphHeight];
        dataArray = new int[graphWidth];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = GraphBackground;
        }
        m_texture.SetPixels(pixels);
        m_texture.Apply();
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(dataSelect)
        {
            case 1: dataPoint = (int)(500*BlendShapesController.leftEyeShape[1]);break; // LeftEye data (Kizuna AI model only)
            case 2: dataPoint = (int)(500*BlendShapesController.rightEyeShape[1]);break; // RightEye data (Kizuna AI model only)
            case 3: dataPoint = (int)(1000*HeadController.headRot[1]+150);break;// Head rotation Quaternion y
            case 4: dataPoint = (int)(1000*HeadController.headRot[3]);break;// Head rotation Quaternion w
            default: Debug.Log("Please Select a proper data number.");break;
        }

        dataArray[0] = dataPoint;
        pixels[dataArray[0] * graphWidth+graphWidth-1] = LineColor;
        for(int i = graphWidth-1; i>0; i--)
        {
            dataArray[i] = dataArray[i-1];
            pixels[dataArray[i] * graphWidth + graphWidth-1-i] = LineColor;
        }
        m_texture.SetPixels(pixels);
        m_texture.Apply();
        for(int i = graphWidth-1; i>=0; i--)
        {
            pixels[dataArray[i] * graphWidth + graphWidth-1-i] = GraphBackground;
        }
    }
}
