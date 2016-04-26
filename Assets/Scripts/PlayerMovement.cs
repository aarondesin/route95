using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {
	public static PlayerMovement instance;
	public GameObject lightRight;
	public GameObject lightLeft;
	//public float velocity;
	const float distPerBeat = 0.0018f;
	const float lookAhead = 0.01f;
	public bool moving;
	public bool lights;
	public float progress;
	public List<ParticleSystem> particles;
	Bezier road;
	public GameObject frontLeftWheel;
	public GameObject frontRightWheel;
	public GameObject backLeftWheel;
	public GameObject backRightWheel;

	float velocity;
	const float velocityToRotation = -20f;
	//float acceleration;
	Vector3 target;
	float offsetH = 0f;
	float velocityOffset = 0f;

	// Use this for initialization
	void Start () {
		instance = this;
		lights = false;
		moving = false;
		velocity = 0f;
		//acceleration = 0f;
		target = new Vector3 (0f, 0f, 0f);
		////if (velocity == 0f)
			//velocity = 0.2f;
		progress = 0f;
		StopMoving();
		//road = WorldManager.instance.road.GetComponent<Bezier> ();
	}

	public void StartMoving() {
		moving = true;
		foreach (ParticleSystem ps in particles) ps.Play();
		DynamicTerrain.instance.createMountain (-32, -32, 21, 43, 300f, 20f, -0.1f, 0.5f);
		DynamicTerrain.instance.createMountain (32, -32, 21, 43, 300f, 20f, -0.1f, 0.5f);
		DynamicTerrain.instance.createMountain (32, 32, 21, 43, 300f, 20f, -0.1f, 0.5f);
		DynamicTerrain.instance.createMountain (-32, 32, 21, 43, 300f, 20f, -0.1f, 0.5f);
	}

	public void StopMoving() {
		moving = false;
		foreach (ParticleSystem ps in particles) ps.Pause();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (road == null) {
			if (WorldManager.instance.road != null)
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
				/*
				progress += velocity * Time.deltaTime / road.CurveCount; 
				if (progress >= 1f)
					progress = 1f;
				this.transform.position = road.GetPoint (progress) + new Vector3 (0f, 2.27f + Bezier.instance.ROAD_HEIGHT, 0f);
				this.transform.LookAt (road.GetVelocity (progress) + this.transform.position);*/


				/*float tempo = MusicManager.tempoToFloat[MusicManager.instance.tempo];
				progress = Mathf.Clamp01 ( progress + tempo * distPerBeat * Time.deltaTime / road.CurveCount );
				Vector3 offset = new Vector3 (0f, 2.27f + road.ROAD_HEIGHT, 0f);
				target = road.GetPoint (progress + lookAhead * Time.deltaTime) + offset - road.BezRight (road.GetPoint (progress + lookAhead * Time.deltaTime)) * road.ROAD_WIDTH / 2f;

				acceleration += acceleration - Vector3.Distance (transform.position, target) * Time.deltaTime;
				velocity += acceleration;
				Vector3 velocityVector = new Vector3 (velocity * Mathf.Cos (transform.rotation.eulerAngles.y), 0f, velocity * Mathf.Sin (transform.rotation.eulerAngles.y));
				//transform.Translate (velocityVector * Time.deltaTime);
				transform.Translate (velocity * transform.forward);

				transform.rotation =  (Quaternion.LookRotation (road.GetDirection (progress), Vector3.up));
				*/
				if (Time.frameCount % 120 == 0) {
					//Debug.Log ("Progress: " + progress);
					////Debug.Log ("Velocity: " + velocityVector);
					//Debug.Log ("Target:" + target);
					//Debug.Log ("offsetH: "+offsetH);
				}
				velocityOffset += ((Mathf.PerlinNoise (Random.Range (0f, 1f), 0f) - Random.Range (0f, velocityOffset)) - 0.5f) * Time.deltaTime;
				velocity = MusicManager.tempoToFloat [MusicManager.instance.tempo] * distPerBeat + velocityOffset;
				progress += velocity * Time.deltaTime / road.CurveCount;
				if (progress >= 1f)
					progress = 1f;
				
				offsetH += (Mathf.PerlinNoise (Random.Range (0f, 1f), 0f) - Random.Range (0f, offsetH)) * Time.deltaTime;
				Vector3 offset = new Vector3 (offsetH, 2.27f + road.height, 0f);
				transform.position = road.GetPoint (progress) + offset - road.BezRight (road.GetPoint(progress)) * road.width / 3f;
				//Quaternion lookRot = Quaternion.FromToRotation (transform.position, road.GetPoint (progress + lookAhead * Time.deltaTime));
				transform.LookAt (road.GetVelocity (progress) + transform.position);
				frontLeftWheel.transform.LookAt (transform.position + road.GetVelocity (Mathf.Clamp01 (progress + lookAhead)));
				frontRightWheel.transform.LookAt (transform.position + road.GetVelocity (Mathf.Clamp01 (progress + lookAhead)));
				backRightWheel.transform.rotation = transform.rotation;
				backLeftWheel.transform.rotation = transform.rotation;

				frontLeftWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 180f, 0f));
				frontRightWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 0f, 0f));
				backLeftWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 180f, 0f));
				backRightWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 0f, 0f));
				//transform.rotation = Quaternion.Lerp (transform.rotation, lookRot, 0.9f);

			}
			//lights = (Sun.instance.getDaytime () > (Mathf.PI * (7f / 8f))
			//|| Sun.instance.getDaytime () <= Mathf.PI * (1f / 8f));
			lights = (WorldManager.instance.timeOfDay  > (Mathf.PI * (7f / 8f))
				|| WorldManager.instance.timeOfDay <= Mathf.PI * (1f / 8f));
			lightRight.GetComponent<Light> ().enabled = lights;
			lightLeft.GetComponent<Light> ().enabled = lights;
		}
	}

	void OnDrawGizmosSelected () {
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere (target, 1f);
	}
}
