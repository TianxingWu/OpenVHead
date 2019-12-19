using UnityEngine;

using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

public class SocketServer : MonoBehaviour
{
    public static string data;

    System.Threading.Thread SocketThread;
    volatile bool keepReading = false;

    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;      
    }

    public static void Execute(string command)// Execute cmd command
    {
        var processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/S /C " + command)
        {
            CreateNoWindow = true,
            UseShellExecute = true,
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
        };

        System.Diagnostics.Process.Start(processInfo);
    }

    public void StartServer()
    {
        SocketThread = new System.Threading.Thread(NetworkCode);
        SocketThread.IsBackground = true;
        SocketThread.Start();
        
        Execute("cd PythonScript & python visual_measurement.py");// Execute python client script
    }



    private string GetIPAddress()
    {
        string localIP = "127.0.0.1";
        return localIP;
    }


    Socket listener;
    Socket handler;

    void NetworkCode()
    {
        // Data buffer for incoming data.
        byte[] bytes = new Byte[1024];

        // Host running the application.
        Debug.Log("Ip " + GetIPAddress().ToString());
        IPAddress[] ipArray = Dns.GetHostAddresses(GetIPAddress());
        IPEndPoint localEndPoint = new IPEndPoint(ipArray[0], 1755);//端口为1755

        // Create a TCP/IP socket.
        listener = new Socket(ipArray[0].AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and 
        // listen for incoming connections.

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.
            while (true)
            {
                keepReading = true;

                // Program is suspended while waiting for an incoming connection.
                Debug.Log("Waiting for Connection");

                handler = listener.Accept();
                Debug.Log("Client Connected");
                data = null;

                // An incoming connection needs to be processed.
                while (keepReading)
                {
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    Debug.Log("Received from Server");

                    if (bytesRec <= 0)
                    {
                        keepReading = false;
                        handler.Disconnect(true);
                        break;
                    }

                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    
                    string[] tempdata = data.Split(':');

                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(1);
                }

                System.Threading.Thread.Sleep(1);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void StopServer()
    {
        keepReading = false;

        //stop thread
        if (SocketThread != null)
        {
            //listener.Shutdown(SocketShutdown.Both);
            //listener.Close();
            SocketThread.Abort();
        }

        if (handler != null && handler.Connected)
        {
            handler.Disconnect(false);
            Debug.Log("Disconnected!");
        }
    }

    public void OnDisable()
    {
        StopServer();
    }


}

