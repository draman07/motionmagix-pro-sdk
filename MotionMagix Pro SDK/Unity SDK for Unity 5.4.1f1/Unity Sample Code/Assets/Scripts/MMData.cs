using System;
using MotionMagixSimulator.Model;
using UnityEngine;
using System.Net.Sockets;

public enum FeedType
{
	POINT
}

public class StateObject
{
	public Socket workSocket = null;
	public const int BUFFER_SIZE = 310000;
	public byte[] buffer = new byte[BUFFER_SIZE];
	public String sb = "";
}

public class MMData : MonoBehaviour
{
	public static string Status = "";

	private static MultiPoint _MultiPointObject = new MultiPoint();
	public static MultiPoint MultiPointObject
	{
		get
		{
			return _MultiPointObject;
		}
		set
		{
			if(_MultiPointObject != value)
			{
				_MultiPointObject = value;
				if(PointDataReceived != null)
				{
					PointDataReceived(null,new PointEventArgs(){ MultiPoint = _MultiPointObject });
				}
			}
		}
	}

	public static FeedType FeedType = FeedType.POINT;

	public static event EventHandler<PointEventArgs> PointDataReceived;

	public static int Width;
	public static int Height;

    public static void AddBlob(int xpos, int ypos)
    {
		int minDist = 10000;
		int minDistIndex = -1;
        int dx;
        int dy;
        int currDist;

		MultiPoint OBJECT_ARRAY = MultiPointObject;

        for (int i = 0; i < OBJECT_ARRAY.MultiPointCoordinates.Count; i++)
        {
			dx = OBJECT_ARRAY.MultiPointCoordinates[i].XCoordinate - xpos;
			dy = OBJECT_ARRAY.MultiPointCoordinates[i].YCoordinate - ypos;
            currDist = (dx * dx) + (dy * dy);
            if (currDist < minDist)
            {
//                OBJECT_ARRAY[i].velocityX = -dx;
//                OBJECT_ARRAY[i].velocityY = -dy;
                minDist = currDist;
                minDistIndex = i;
            }
        }

        if (minDistIndex == -1)
        { 		
			OBJECT_ARRAY.MultiPointCoordinates.Add(new Coordinate{XCoordinate = xpos, YCoordinate = ypos});
			OBJECT_ARRAY.MultiPointCoordinates[OBJECT_ARRAY.MultiPointCoordinates.Count - 1].Life = 30;
        }
        else
        {
			OBJECT_ARRAY.MultiPointCoordinates[minDistIndex].XCoordinate = xpos;
			OBJECT_ARRAY.MultiPointCoordinates[minDistIndex].YCoordinate = ypos;

            //Revive life to 20 if blob is still on stage
			OBJECT_ARRAY.MultiPointCoordinates[minDistIndex].Life = 30;
        }

		OBJECT_ARRAY = UpdateBlobs (OBJECT_ARRAY);

		MultiPointObject = OBJECT_ARRAY;
		if(PointDataReceived != null)
		{
			PointDataReceived(null,new PointEventArgs(){ MultiPoint = _MultiPointObject });
		}
	}
	
    public static MultiPoint UpdateBlobs(MultiPoint OBJECT_ARRAY)
    {
        for (int i = 0; i < OBJECT_ARRAY.MultiPointCoordinates.Count; i++)
        {
			OBJECT_ARRAY.MultiPointCoordinates[i].Life--;

			if (OBJECT_ARRAY.MultiPointCoordinates[i].Life == 0)
            {
				OBJECT_ARRAY.MultiPointCoordinates.RemoveAt(i);
                i--;
            }
        }

		return OBJECT_ARRAY;
    }
}