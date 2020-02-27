using UnityEngine;

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class SocketServer
{
    private string clientName = "client";
    private string clientPath;
    private string paramsPath;

    private Thread socketThread;
    private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
    private bool _running
    {
        get { return !_stopEvent.WaitOne(0); }
    }
    
    private string GetIPAddress()
    {
        return "127.0.0.1";
    }
    
    public static string data;

    // Initialization
    public SocketServer()
    {
        // Find Python client's working directory
        clientPath = string.Join(Path.DirectorySeparatorChar.ToString(), new string[] {
            Environment.CurrentDirectory, "PythonScript", "dist", "client", clientName + ".exe"
        });
        paramsPath = string.Join(Path.DirectorySeparatorChar.ToString(), new string[] {
            Environment.CurrentDirectory, "PythonScript", "data", "shape_predictor_68_face_landmarks.dat"
        });
    }


    // Start the thread
    public void Start()
    {
        // Reset the flag to start receiving data from the python client
        _stopEvent.Reset();
        
        // Create a new thread
        socketThread = new Thread(ReceiveData);
        socketThread.IsBackground = true;
        socketThread.Start();
    }


    // Block the thread
    public void Stop()
    {
        // Set the flag to prevent receiving data from the python client
        _stopEvent.Set();
    }


    // Start Python client
    private void StartClient(string path, string param)
    {
        Debug.Log(path);
        System.Diagnostics.Process.Start(path, param);
    }


    // Stop Python client
    private void StopClient(string name)
    {
        System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcesses();
        foreach (System.Diagnostics.Process myProcess in myProcesses)
        {
            if (name == myProcess.ProcessName) myProcess.Kill();
        }
    }
    

    // Receive data from client
    private void ReceiveData()
    {
        // Host running the application. Port: 1755.
        IPAddress[] ipArray = Dns.GetHostAddresses(GetIPAddress());
        IPEndPoint localEndPoint = new IPEndPoint(ipArray[0], 1755);

        // Create a TCP/IP socket.
        Socket listener = new Socket(ipArray[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        listener.Bind(localEndPoint);
        listener.Listen(10);

        // Start the python client
        StartClient(clientPath, paramsPath);
        
        // Start listening for connections
        while (_running)
        {
            try {
                // Data buffer for incoming data
                byte[] bytes = new Byte[1024];

                // Program is suspended while waiting for an incoming connection
                Debug.Log("Waiting for Connection");

                Socket handler = listener.Accept();
                Debug.Log("Client Connected");
                data = null;

                // An incoming connection needs to be processed
                while (_running)
                {
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    // Debug.Log("Received from Server");

                    if (bytesRec <= 0)
                    {
                        handler.Disconnect(true);
                        break;
                    }

                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    string[] tempdata = data.Split(':');
                    if (data.IndexOf("<EOF>") > -1) break;
                    
                    // System.Threading.Thread.Sleep(1);
                }
                // System.Threading.Thread.Sleep(1);
                handler.Close();
            }
            catch (Exception e) {  
                Debug.Log(e.ToString());  
            }
        }

        // Shut down the client
        StopClient(clientName);

        // Shut down the server
        listener.Close();
        Debug.Log("Disconnected!");
    }

}

