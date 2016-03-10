using UnityEngine;
using System.Collections;

public class TESTPlayerMovement : MonoBehaviour {
	public GameObject lightRight;
	public GameObject lightLeft;
	public float velocity;
	public static bool moving;
	public static bool lights;

	// Use this for initialization
	void Start () {
		lights = false;
		moving = false;
		if (velocity == 0)
			velocity = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Sun.instance != null) {
			if (moving && !GameManager.instance.paused && Sun.instance != null)
				this.transform.Translate (transform.forward * Time.deltaTime * velocity);
			lights = (Sun.instance.getDaytime () > (Mathf.PI * (7f / 8f))
			|| Sun.instance.getDaytime () <= Mathf.PI * (1f / 8f));
			lightRight.GetComponent<Light> ().enabled = lights;
			lightLeft.GetComponent<Light> ().enabled = lights;
		}
	}
}
