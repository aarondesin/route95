using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {
	public static Sun instance;
	// Use this for initialization


	private float xScale = 10;
	private float yScale = 10;
	private float zScale = 10;

	//private Color NOON = Color.yellow;
	//private Color DAWN = Color.red;
	//private Color DUSK = Color.red;
	//private Color MIDNIGHT = Color.blue;
	//Color NOON = WorldManager.instance.primaryDayColor;
	//Color DAWN = WorldManager.instance.primarySunriseColor;
	//Color DUSK = WorldManager.instance.primarySunsetColor;
	//Color MIDNIGHT = WorldManager.instance.primaryNightColor;

	private Vector3 sunTarget = new Vector3 (0f, 0f, 0f); // target for the sun to point at: the car or the origin

	/*public float getDaytime() {
		return dayTime;
	}*/

	public void setPosScales(float x = 1, float y = 1, float z = 1){
		xScale = x;
		yScale = y;
		zScale = z;
	}



	private void updateTransform(){
		float newX = xScale * Mathf.Cos(WorldManager.instance.timeOfDay);
		float newY = yScale * Mathf.Sin(WorldManager.instance.timeOfDay);
		float newZ = -zScale * Mathf.Cos(WorldManager.instance.timeOfDay + Mathf.PI/5);
		this.transform.position = new Vector3(newX, newY, newZ);

		//float deltaDegrees = 360 * Time.deltaTime / (2 * Mathf.PI);
		//this.transform.Rotate (deltaDegrees, deltaDegrees, 0);
		this.transform.LookAt (sunTarget);
	}

	void Start() {
		instance = this;
		this.GetComponent<Light> ().range = 100f;
		this.GetComponent<Light> ().type = LightType.Directional;
	}
	
	// Update is called once per frame
	void Update() {
		//updateTime();
		updateTransform();
		//updateColor();
	}
		
}
