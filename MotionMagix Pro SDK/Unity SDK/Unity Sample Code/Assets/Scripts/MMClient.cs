using System.Collections;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using UnityEngine;
using MotionMagixSimulator.Model;
using MotionMagixSimulator.Utility;
using UnityEngine.UI;
using EncryptSocket;

public class MMClient :MonoBehaviour
{
    IPAddress[] ipAddress = Dns.GetHostAddresses("localhost");
    IPEndPoint ipEnd;
    Socket clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
    
	public FeedType FeedType = FeedType.POINT;
	public int Width;
	public int Height;
	public bool IsSinglePoint = false;

	Crypt encryption;
	public static event EventHandler<EventArgs> ConnectionClosed;

    // Use this for initialization
    public void Start()
    {
		encryption = new Crypt ();

		MMData.FeedType = FeedType;
		MMData.Width = Width;
		MMData.Height = Height;

        ipEnd = new IPEndPoint(ipAddress[0], 12345);
        try
        {
            clientSock.Connect(ipEnd);
			StateObject so2 = new StateObject();
			so2.workSocket = clientSock;
			clientSock.BeginReceive(so2.buffer, 0, StateObject.BUFFER_SIZE,0,new AsyncCallback(OnData), so2);

			MMData.Status = "Connected.";
        }
        catch (Exception e)
        {
			 MMData.Status = "Connection could not be made";
        }
    }

    public void OnData(IAsyncResult ar)
    {
        try
        {
			StateObject so = (StateObject) ar.AsyncState;
			Socket s = so.workSocket;
			
			int read = s.EndReceive(ar);
			
			if (read > 0) 
			{
				try
				{
					switch(MMData.FeedType)
					{
						case FeedType.POINT :
							if(IsSinglePoint)
							{
								so.sb = (Encoding.ASCII.GetString(so.buffer, 0, read));
								MMData.MultiPointObject = CustomSerilization.DeserializeData(encryption.getDecrypt(so.sb.ToString()),new MultiPoint()) as MultiPoint;
							}
							else
							{
								so.sb = (Encoding.ASCII.GetString(so.buffer, 0, read));
								MultiPoint multiPoint = CustomSerilization.DeserializeData(encryption.getDecrypt(so.sb.ToString()),new MultiPoint()) as MultiPoint;
								MMData.AddBlob(multiPoint.MultiPointCoordinates[0].XCoordinate,multiPoint.MultiPointCoordinates[0].YCoordinate);
							}
							break;
					}
					MMData.Status = "Receiving data";
				}
				catch(Exception ex)
				{
					//MMData.Status = "Problem receiving data";
				}

				s.BeginReceive(so.buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(OnData), so);
			}
			else
			{
				if (so.sb.Length > 1) 
				{
					if(ConnectionClosed != null)
						ConnectionClosed(null,null);
					MMData.Status = "Connection closed";
				}
				s.Close();
			}
		}
        catch (Exception e)
        {
            Debug.Log (e.Message);
        }
    }

	void OnApplicationQuit()
	{
		if (clientSock != null && clientSock.Connected) 
		{
			clientSock.Close();
		}
	}
}