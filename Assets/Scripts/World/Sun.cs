using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {
	public static Sun instance;

	private float xScale = 10;
	private float yScale = 10;
	private float zScale = 10;

	private Vector3 sunTarget; // target for the sun to point at: the car or the origin

	private void UpdateTransform(){
		sunTarget = PlayerMovement.instance.transform.position;
		float newX = xScale * Mathf.Cos(WorldManager.instance.timeOfDay);
		float newY = yScale * Mathf.Sin(WorldManager.instance.timeOfDay);
		float newZ = -zScale * Mathf.Cos(WorldManager.instance.timeOfDay + Mathf.PI/5);
		this.transform.position = new Vector3(newX, newY, newZ);

		this.transform.LookAt (sunTarget);
	}

	void Start() {
		instance = this;
		transform.parent = PlayerMovement.instance.transform;
		this.GetComponent<Light> ().range = 100f;
		this.GetComponent<Light> ().type = LightType.Directional;
	}
	
	// Update is called once per frame
	void Update() {
		UpdateTransform();
	}
		
}
