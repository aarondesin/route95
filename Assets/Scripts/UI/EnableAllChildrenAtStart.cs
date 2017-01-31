using UnityEngine;
using System.Collections;

public class EnableAllChildrenAtStart : MonoBehaviour {

	void Awake () {
        foreach (Transform t in GetComponentInChildren<Transform>()) {
            t.gameObject.SetActive(true);
        }
    }
}
