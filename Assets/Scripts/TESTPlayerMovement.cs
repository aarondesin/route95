using UnityEngine;
using System.Collections;

public class TESTPlayerMovement : MonoBehaviour {
	public float velocity;
	public static bool moving;

	// Use this for initialization
	void Start () {
		moving = false;
		if (velocity == 0)
			velocity = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (moving)
			this.transform.Translate (transform.forward * Time.deltaTime * velocity);
	}
}
