using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {
	public static Sun instance;
	// Use this for initialization

	[SerializeField]
	private float dayTime;
	private float timeScale = 1;

	private float xScale = 10;
	private float yScale = 10;
	private float zScale = 10;

	private Color NOON = Color.yellow;
	private Color DAWN = Color.red;
	private Color DUSK = Color.red;
	private Color MIDNIGHT = Color.blue;

	private Vector3 sunTarget = new Vector3 (0f, 0f, 0f); // target for the sun to point at: the car or the origin

	public float getDaytime() {
		return dayTime;
	}

	public void setPosScales(float x = 1, float y = 1, float z = 1){
		xScale = x;
		yScale = y;
		zScale = z;
	}

	public void setTimeScale(float t = 1) {
		timeScale = t;
	}

	private void updateColor() {
		//Light light = this.GetComponent<Light>();
		if ((dayTime >= 0) && (dayTime < (Mathf.PI / 2))) {
			float lerpValue = dayTime / (Mathf.PI / 2);
			this.GetComponent<Light> ().color = Color.Lerp (DAWN, NOON, lerpValue);
		} else if ((dayTime >= (Mathf.PI / 2)) && (dayTime < Mathf.PI)) {
			float lerpValue = (dayTime - Mathf.PI / 2) / (Mathf.PI / 2);
			this.GetComponent<Light> ().color = Color.Lerp (NOON, DUSK, lerpValue);
		} else if ((dayTime >= Mathf.PI) && (dayTime < ((3f/2f) * Mathf.PI))){
			float lerpValue = (dayTime - Mathf.PI) / (Mathf.PI / 2);
			this.GetComponent<Light> ().color = Color.Lerp (DUSK, MIDNIGHT, lerpValue);
		} else if ((dayTime >= ((3f/2f) * Mathf.PI)) && (dayTime < (2 * Mathf.PI))){
			float lerpValue = (dayTime - ((3f/2f) * Mathf.PI)) / (Mathf.PI / 2);
			this.GetComponent<Light> ().color = Color.Lerp (MIDNIGHT, DAWN, lerpValue);
		}
		if (Spectrum2.instance != null) {
			Color temp = new Color (this.GetComponent<Light> ().color.r, this.GetComponent<Light> ().color.g, this.GetComponent<Light> ().color.b, Spectrum2.instance.opacity);
			Spectrum2.instance.GetComponent<LineRenderer> ().SetColors (temp, temp);
			Spectrum2.instance.GetComponent<LineRenderer> ().material.color = temp;
		}
	}

	private void updateTime() {
		dayTime += timeScale * Time.deltaTime;
		while (dayTime > (2 * Mathf.PI)) { //clamp dayTime between 0 and 2PI)
			dayTime -= 2 * Mathf.PI;
		}
	}

	private void updateTransform(){
		float newX = xScale * Mathf.Cos(dayTime);
		float newY = yScale * Mathf.Sin(dayTime);
		float newZ = -zScale * Mathf.Cos(dayTime + Mathf.PI/5);
		this.transform.position = new Vector3(newX, newY, newZ);

		//float deltaDegrees = 360 * Time.deltaTime / (2 * Mathf.PI);
		//this.transform.Rotate (deltaDegrees, deltaDegrees, 0);
		this.transform.LookAt (sunTarget);
	}

	void Start() {
		instance = this;
		dayTime = Random.Range(0, 2*Mathf.PI);
		this.GetComponent<Light> ().range = 100f;
		this.GetComponent<Light> ().type = LightType.Directional;
	}
	
	// Update is called once per frame
	void Update() {
		updateTime();
		updateTransform();
		updateColor();
	}
}
