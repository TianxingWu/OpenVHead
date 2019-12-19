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
    private int graphWidth = 500;
    private int graphHeight = 500;
    private List<int> data;
    private Color[] pixels;
    //private Color GraphBackground = new Color(0.3f, 0.4f, 0.6f, 0.8f);
    private Color GraphBackground = new Color(0, 1, 0, 1);
    private Color LineColor = new Color(1.0f, 0.0f, 0.0f, 0.1f);
    

    // Start is called before the first frame update
    void Start()
    {
        m_texture = new Texture2D(graphWidth, graphHeight);
        m_rawImage.texture = m_texture;
        m_rawImage.SetNativeSize();

        pixels = new Color[graphWidth * graphHeight];
        data = new List<int>();
        data.Add(0);//0 时刻接受的数据值为0

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
        if(Left_Right==1)//绘制左眼数据
        {
            dataPoint = (int)(500*BlendShapesController.leftEyeShape[1]);
        }
        else if(Left_Right==2)//绘制右眼数据
        {
            dataPoint = (int)(500*BlendShapesController.rightEyeShape[1]);
        }

        data.Add(dataPoint);
 
        for (int j = data.Count - 1; j >= Mathf.Max(0, data.Count - graphWidth); j--)
        {
            pixels[data[j] * graphWidth + graphWidth - data.Count + j] = LineColor;
        }
        m_texture.SetPixels(pixels);
        m_texture.Apply();
        for (int j = data.Count - 1; j >= Mathf.Max(0, data.Count - graphWidth); j--)
        {
            pixels[data[j] * graphWidth + graphWidth - data.Count + j] = GraphBackground;
        }
    }
}
