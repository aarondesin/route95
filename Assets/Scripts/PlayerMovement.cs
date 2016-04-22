using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {
	public static PlayerMovement instance;
	public GameObject lightRight;
	public GameObject lightLeft;
	public float velocity;
	public bool moving;
	public bool lights;
	public float progress;
	public List<ParticleSystem> particles;
	Bezier road;

	// Use this for initialization
	void Start () {
		instance = this;
		lights = false;
		moving = false;
		if (velocity == 0f)
			velocity = 0.2f;
		progress = 0f;
		StopMoving();
		//road = WorldManager.instance.road.GetComponent<Bezier> ();
	}

	public void StartMoving() {
		moving = true;
		foreach (ParticleSystem ps in particles) ps.Play();
	}

	public void StopMoving() {
		moving = false;
		foreach (ParticleSystem ps in particles) ps.Pause();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (road == null) {
			road = WorldManager.instance.road.GetComponent<Bezier> ();
		}
		if (Sun.instance != null) {
			if (moving && !GameManager.instance.paused) {
				/*
				Vector3 old = this.transform.position;
				this.transform.Translate (transform.forward * Time.deltaTime * velocity);
				Debug.Log ("Position" + this.transform.position);
				Debug.Log("Closest Point:" + WorldManager.instance.road.GetComponent<Bezier> ().ClosestPointOnBezier2 (this.transform.position));
				this.transform.position = WorldManager.instance.road.GetComponent<Bezier> ().ClosestPointOnBezier2 (this.transform.position);
				Vector3 newPos = this.transform.position;
				Vector3 target = (newPos - old) + newPos;
				Vector3 target2 = new Vector3 (target.x, this.transform.position.y, target.z);
				this.transform.LookAt (target2);
				Vector3 yLockedPos = this.transform.position;
				yLockedPos.y = 2.227f;
				this.transform.position = yLockedPos;
				*/
				progress += velocity * Time.deltaTime / road.CurveCount; 
				if (progress >= 1f)
					progress = 1f;
				this.transform.position = road.GetPoint (progress) + new Vector3 (0f, 2.27f + Bezier.instance.ROAD_HEIGHT, 0f);
				this.transform.LookAt (road.GetVelocity (progress) + this.transform.position);
			}
			//lights = (Sun.instance.getDaytime () > (Mathf.PI * (7f / 8f))
			//|| Sun.instance.getDaytime () <= Mathf.PI * (1f / 8f));
			lights = (WorldManager.instance.timeOfDay  > (Mathf.PI * (7f / 8f))
				|| WorldManager.instance.timeOfDay <= Mathf.PI * (1f / 8f));
			lightRight.GetComponent<Light> ().enabled = lights;
			lightLeft.GetComponent<Light> ().enabled = lights;
		}
	}
}
