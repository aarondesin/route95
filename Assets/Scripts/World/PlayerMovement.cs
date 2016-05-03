using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMovement : MonoBehaviour {
	public static PlayerMovement instance;
	public GameObject lightRight;
	public GameObject lightLeft;
	//public float velocity;
	const float distPerBeat = 0.0012f; // Tempo -> velocity
	const float particlesPerUnit = 100f; // Distance -> particle emission
	const float lookAhead = 0.01f;
	public bool moving;
	public bool lights;
	public float progress;
	float roadHeight;
	public List<ParticleSystem> particles;

	public GameObject frontLeftWheel;
	public GameObject frontRightWheel;
	public GameObject backLeftWheel;
	public GameObject backRightWheel;

	float minVelocity;
	float maxVelocity;
	float velocity;
	const float velocityToRotation = -10000f;
	//float acceleration;
	Vector3 target;
	float offsetH = 0f;
	float velocityOffset = 0f;

	public AudioClip engineClip;

	List<ReflectionProbe> reflectionProbes;
	Vector3 prevPosition;

	bool initialized = false;
	float dOffset;

	// Use this for initialization
	void Start () {
		instance = this;
		lights = false;
		moving = false;
		velocity = 0f;

		prevPosition = Vector3.zero;
		reflectionProbes = GetComponentsInChildren<ReflectionProbe> ().ToList<ReflectionProbe>();

		target = new Vector3 (0f, 0f, 0f);

		progress = 0f;
		StopMoving();

		minVelocity = MusicManager.tempoToFloat [Tempo.Slowest] * distPerBeat;
		maxVelocity = MusicManager.tempoToFloat [Tempo.Fastest] * distPerBeat;

		GetComponent<AudioSource>().pitch = 0.4f;

		dOffset = 0f;



	}

	public void StartMoving() {
		roadHeight = WorldManager.instance.roadHeight;
		moving = true;
		foreach (ParticleSystem ps in particles) ps.Play();
		GetComponent<AudioSource>().clip = engineClip;
		GetComponent<AudioSource>().loop = true;
		GetComponent<AudioSource>().Play();
		GetComponent<AudioSource>().volume = 1f;
	}

	public void StopMoving() {
		moving = false;
		foreach (ParticleSystem ps in particles) ps.Pause();
		GetComponent<AudioSource>().Stop();
	}

	public void DisableReflections () {
		foreach (ReflectionProbe probe in reflectionProbes)
			probe.enabled = false;
	}

	public void EnableReflections () {
		foreach (ReflectionProbe probe in reflectionProbes)
			probe.enabled = true;
	}

	void Initialize() {
		DisableReflections ();
		initialized = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!initialized)
			Initialize ();

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
				dOffset += (Mathf.PerlinNoise (Random.Range (0f, 1f), 0f) - 0.5f);
				velocityOffset = Mathf.Clamp (velocityOffset + dOffset, minVelocity, maxVelocity);

				velocity = MusicManager.tempoToFloat [MusicManager.instance.tempo] * distPerBeat + velocityOffset;
				float realVelocity = Vector3.Distance (transform.position, prevPosition) / Time.fixedDeltaTime;
				prevPosition = transform.position;
				progress += velocity * Time.deltaTime / Road.instance.CurveCount;
				if (progress >= 1f)
					progress = 1f;
				
				offsetH += (Mathf.PerlinNoise (Random.Range (0f, 1f), 0f) - Random.Range (0f, offsetH)) * Time.deltaTime;
				Vector3 offset = new Vector3 (offsetH, 2.27f + roadHeight, 0f);
				transform.position = Road.instance.GetPoint (progress) + offset - 
					Road.instance.BezRight (Road.instance.GetPoint(progress)) * Road.instance.width / 3f;
				//Quaternion lookRot = Quaternion.FromToRotation (transform.position, road.GetPoint (progress + lookAhead * Time.deltaTime));
				transform.LookAt (Road.instance.GetVelocity (progress) + transform.position);


				frontLeftWheel.transform.LookAt (transform.position + 
					Road.instance.GetVelocity (Mathf.Clamp01 (progress + lookAhead))
				);
				frontRightWheel.transform.LookAt (transform.position + 
					Road.instance.GetVelocity (Mathf.Clamp01 (progress + lookAhead))
				);
				backRightWheel.transform.rotation = transform.rotation;
				backLeftWheel.transform.rotation = transform.rotation;

				frontLeftWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 0f, 0f));
				frontRightWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 0f, 0f));
				backLeftWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 0f, 0f));
				backRightWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 0f, 0f));

				frontLeftWheel.transform.Rotate (new Vector3 (0f, 180f, 0f));
				backLeftWheel.transform.Rotate (new Vector3 (0f, 180f, 0f));

				//if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();

				foreach (ParticleSystem particle in particles) {
					var emission = particle.emission;
					var rate = emission.rate;
					rate.constantMax = velocity * particlesPerUnit;
					emission.rate = rate;
				}

				//if (!GetComponent<AudioSource>().isPlaying) Debug.Log("shit");
				GetComponent<AudioSource>().pitch = Mathf.Clamp (0.75f + (velocity - minVelocity) / (maxVelocity - minVelocity), 0.75f, 1.75f);

				//if (Random.Range(0, 100) == 0) Debug.Log(velocity * velocityToRotation);

				/*frontLeftWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 180f, 0f));
				frontRightWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 0f, 0f));
				backLeftWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 180f, 0f));
				backRightWheel.transform.Rotate (new Vector3 (velocity * velocityToRotation, 0f, 0f));*/
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
