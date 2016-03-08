using UnityEngine;
using System.Collections;

public class Blob : MonoBehaviour 
{

	RectTransform CanvasRect;
	BurstGamePlay burstGame;

	// Use this for initialization
	void Start () 
	{
		burstGame = Camera.main.GetComponent<BurstGamePlay> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckForRayCollision ();
	}

	void CheckForRayCollision()
	{
			Ray ray = Camera.main.ScreenPointToRay (transform.position);
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (ray, out hitInfo) && hitInfo.collider.gameObject.tag.Equals ("Target")) 
			{
			hitInfo.collider.gameObject.renderer.material.color = Color.white;

			Destroy(hitInfo.collider.gameObject,0.2f);
				burstGame.Score++;
			}
	}
}
