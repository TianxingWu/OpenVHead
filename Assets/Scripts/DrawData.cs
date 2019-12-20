using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawData : MonoBehaviour
{
    public RawImage m_rawImage;
    public Texture2D m_texture;
    public int dataPoint;
    public int Left_Right;
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
        if(Left_Right==1)//读入左眼数据
        {
            // dataPoint = (int)(500*BlendShapesController.leftEyeShape[1]); // LeftEye data
            dataPoint = (int)(1000*HeadController.headRot[1]+150);
        }
        else if(Left_Right==2)//读入右眼数据
        {
            // dataPoint = (int)(500*BlendShapesController.rightEyeShape[1]); // RightEye data
            dataPoint = (int)(1000*HeadController.headRot[3]);
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
