using UnityEngine;


public class WidgetEvents : MonoBehaviour
{

    private SocketServer server;
    
    // Click event of "Start" button 
    public void OnStartButtonClicked()
    {
        if (server == null) server = new SocketServer();
        server.Start();
    }

    // Click event of "Pause" button 
    public void OnPauseButtonClicked()
    {
        if (server == null) return;
        server.Stop();
    }

}

