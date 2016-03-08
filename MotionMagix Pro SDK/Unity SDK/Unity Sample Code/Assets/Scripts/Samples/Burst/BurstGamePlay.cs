using UnityEngine;
using System.Collections;
using System.Timers;
using UnityEngine.UI;
using System.Collections.Generic;

public class BurstGamePlay : MonoBehaviour 
{
	public float MinX;
	public float MaxX;
	
	public int Delay = 1000;
	
	Timer timer = new Timer ();
	Timer gameTimer = new Timer (1000);

	Timer restartTimer = new Timer (2000);
	bool gameRestart = false;

	bool isGameobjectCreated = false;
	bool gameOver = false;

	[HideInInspector]
	public int Score= 0;
	int Time = 30;

	public int MaxPoints = 25;
	public Text ScoreText;
	public Text TimeText;
	public Text StatusText;
	public GameObject Balloon;
	public GameObject GameOverPanel;
	public GameObject Blob;
	public GameObject Canvas;

	GameObject[] Blobs;
	List<MotionMagixSimulator.Model.Coordinate> coordinates;
	RectTransform CanvasRect;

	Color[] colors = new Color[]{Color.cyan,Color.red,Color.green,Color.yellow, Color.blue};

	// Use this for initialization
	void Start () 
	{
		Initialize ();
		CanvasRect = Canvas.GetComponent<RectTransform> ();
	}

	void HandleElapsed (object sender, ElapsedEventArgs e)
	{
		isGameobjectCreated = false;
	}

	void HandleElapsed1 (object sender, ElapsedEventArgs e)
	{
		Time--;
		if (Time == 0) 
		{
			GameOverStep1();
		}
	}

	void GameOverStep1()
	{
		gameTimer.Stop ();
		timer.Stop();
		timer.Elapsed -= HandleElapsed;
		gameTimer.Elapsed -= HandleElapsed1;
		MMData.PointDataReceived -= HandlePointDataReceived;
		MMClient.ConnectionClosed -= HandleConnectionClosed;
		restartTimer.Elapsed += HandleElapsed2;
		restartTimer.Start ();
		gameOver = true;
		coordinates = null;
	}

	void HandleElapsed2 (object sender, ElapsedEventArgs e)
	{
		restartTimer.Stop ();
		gameRestart = true;
	}

	void GameOverStep2()
	{
		ToggleGameobjectVisibility (GameOverPanel);
		for (int i = 0; i < MaxPoints; i++) 
		{
			GameObject.Destroy(Blobs[i]);
		}
		gameOver = false;
	}

	void HandlePointDataReceived (object sender, MotionMagixSimulator.Model.PointEventArgs e)
	{
		if (e.MultiPoint != null && e.MultiPoint.MultiPointCoordinates.Count > 0) 
		{
			coordinates = e.MultiPoint.MultiPointCoordinates;
		} 
		else 
		{
			coordinates = null;
		}
	}

	public void Initialize()
	{
		isGameobjectCreated = false;

		Blobs = new GameObject[MaxPoints];

		for (int i = 0; i < MaxPoints; i++) 
		{
			Blobs[i] = GameObject.Instantiate(Blob) as GameObject;
			Blobs[i].transform.SetParent(Canvas.transform);
		}

		Score = 0;
		Time = 30;

		timer.Interval = Delay;
		
		timer.Elapsed += HandleElapsed;
		timer.Start ();
		
		gameTimer.Elapsed += HandleElapsed1;
		gameTimer.Start ();

		MMData.PointDataReceived += HandlePointDataReceived;

		MMClient.ConnectionClosed += HandleConnectionClosed;
	}

	void HandleConnectionClosed (object sender, System.EventArgs e)
	{
	}

	// Update is called once per frame
	void Update () 
	{

		
		if (gameOver) 
		{
			GameOverStep2();
		}
		if (gameRestart) 
		{
			ToggleGameobjectVisibility(GameOverPanel);
			Initialize();
			restartTimer.Elapsed -= HandleElapsed2;
			gameRestart = false;
		}
	}

	void FixedUpdate () 
	{
		ScoreText.text = "Score : " + Score.ToString ();
		TimeText.text = "Remaining Time : " + Time.ToString () + " seconds";
		StatusText.text = MMData.Status;

		if (!isGameobjectCreated) 
		{
			GameObject balloon = GameObject.Instantiate (Balloon, new Vector3 (Balloon.transform.position.x, Balloon.transform.position.y, Random.Range (MinX, MaxX)), Quaternion.Euler(270,0,0)) as GameObject;
			balloon.renderer.material.color = colors[Random.Range(0,colors.Length - 1)];
			isGameobjectCreated = true;
		}
		if (Time > 0 && coordinates != null) 
		{
			for (int i = 0; i < coordinates.Count; i++) 
			{
				Blobs[i].transform.position = new Vector3(CanvasRect.rect.width * coordinates[i].XCoordinate/MMData.Width,CanvasRect.rect.height - CanvasRect.rect.height * coordinates[i].YCoordinate/MMData.Height);
			}
			for (int i = coordinates.Count; i < MaxPoints; i++) 
			{
				Blobs[i].transform.position = new Vector3(-20,0,0);
			}
		}
	}

	public void ToggleGameobjectVisibility(GameObject Go)
	{
		Go.SetActive(!Go.activeSelf);
	}
}
