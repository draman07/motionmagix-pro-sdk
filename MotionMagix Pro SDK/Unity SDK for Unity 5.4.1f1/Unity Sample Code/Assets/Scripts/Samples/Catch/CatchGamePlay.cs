using UnityEngine;
using System.Collections;
using System.Timers;
using UnityEngine.UI;

public class CatchGamePlay : MonoBehaviour 
{
	public float MinX;
	public float MaxX;

	public int Delay = 5000;

	Timer timer = new Timer ();
	Timer gameTimer = new Timer (1000);

	Timer restartTimer = new Timer (2000);
	bool gameRestart = false;

	bool isGameobjectCreated = false;
	bool gameOver = false;
	int Score= 0;
	int Time = 30;

	public Text ScoreText;
	public Text TimeText;
	public Text StatusText;
	public GameObject Ball;
	public GameObject GameOverPanel;
	private static float _Delta;
	public static float Delta
	{
		get
		{
			return _Delta;
		}
		set
		{
			if (_Delta != value) 
			{
				_Delta = value;
			}
		}
	}

	private float LastXPos = -1000;

	// Use this for initialization
	void Start () 
	{
		Initialize ();
	}

	public void Initialize()
	{
		isGameobjectCreated = false;

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
	}
	
	void HandleElapsed2 (object sender, ElapsedEventArgs e)
	{
		restartTimer.Stop ();
		gameRestart = true;
	}

	void GameOverStep2()
	{
		ToggleGameobjectVisibility (GameOverPanel);

		gameOver = false;
	}

	void HandleElapsed (object sender, ElapsedEventArgs e)
	{
		isGameobjectCreated = false;
	}

	void HandleConnectionClosed (object sender, System.EventArgs e)
	{

	}

	void HandlePointDataReceived (object sender, MotionMagixSimulator.Model.PointEventArgs e)
	{
		if (e.MultiPoint != null && e.MultiPoint.MultiPointCoordinates.Count > 0) 
		{
			if(LastXPos == -1000)
				LastXPos = MMData.MultiPointObject.MultiPointCoordinates [0].XCoordinate;
			Delta = (float)(MMData.MultiPointObject.MultiPointCoordinates [0].XCoordinate - LastXPos);
			LastXPos = MMData.MultiPointObject.MultiPointCoordinates [0].XCoordinate;
			print (MMData.MultiPointObject.MultiPointCoordinates [0].XCoordinate + " " + LastXPos);
		}
	}

	void Update()
	{
		ScoreText.text = "Score : " + Score.ToString ();
		TimeText.text = "Remaining Time : " + Time.ToString () + " seconds";
		StatusText.text = MMData.Status;

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
		if (!isGameobjectCreated) 
		{
			GameObject.Instantiate (Ball, new Vector3 (Random.Range (MinX, MaxX), Ball.transform.position.y, Ball.transform.position.z), Quaternion.identity);
			isGameobjectCreated = true;
		}
		//print (Delta);
		transform.Translate(new Vector3(Delta/100/* != 0 ?(Delta >0?0.1f:-0.1f):0*/,0,0));
		
		if (transform.position.x > MaxX)
			transform.position = new Vector3(MaxX,transform.position.y,transform.position.z);
		if (transform.position.x < MinX)
			transform.position = new Vector3(MinX,transform.position.y,transform.position.z);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag.Equals ("Target")) 
		{
			col.gameObject.GetComponentInChildren<Renderer> ().material.mainTexture = null;
			col.gameObject.GetComponent<Rigidbody>().useGravity = false;
			Destroy(col.gameObject,0.2f);
			Score++;
		}
	}

	public void ToggleGameobjectVisibility(GameObject Go)
	{
		Go.SetActive(!Go.activeSelf);
	}
}
