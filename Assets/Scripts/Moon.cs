using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour {
	public static Moon instance;
	// Use this for initialization

	//private float dayTime;
	//private float timeScale = 1;

	private float xScale = 10;
	private float yScale = 10;
	private float zScale = 10;

	//private Color NOON = new Color (0.5f, 0.5f, 1f, 1f);
	//private Color DAWN = Color.blue;
	//private Color DUSK = Color.blue;
	//private Color MIDNIGHT = Color.black;

	private Vector3 sunTarget = new Vector3 (0f, 0f, 0f); // target for the sun to point at: the car or the origin

	public void setPosScales(float x = 1, float y = 1, float z = 1){
		xScale = x;
		yScale = y;
		zScale = z;
	}

	//public void setTimeScale(float t = 1) {
	//	timeScale = t;
	//}
		

	private void updateTransform(){
		float newX = -xScale * Mathf.Cos(WorldManager.instance.timeOfDay);
		float newY = -yScale * Mathf.Sin(WorldManager.instance.timeOfDay);
		float newZ = zScale * Mathf.Cos(WorldManager.instance.timeOfDay + Mathf.PI/5);
		this.transform.position = new Vector3(newX, newY, newZ);

		//float deltaDegrees = 360 * Time.deltaTime / (2 * Mathf.PI);
		//this.transform.Rotate (deltaDegrees, deltaDegrees, 0);
		this.transform.LookAt (sunTarget);
	}

	void Start() {
		instance = this;
		this.GetComponent<Light> ().range = 100f;
		this.GetComponent<Light> ().type = LightType.Directional;
		this.transform.localScale = new Vector3 (50f, 50f, 50f);
	}

	// Update is called once per frame
	void Update() {
		//updateTime();
		updateTransform();
		//updateColor();
	}
}
