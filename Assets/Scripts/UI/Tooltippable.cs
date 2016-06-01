using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Tooltippable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	public string message;

	public void OnPointerEnter (PointerEventData e) {
		GameManager.instance.Show(Tooltip.instance.gameObject);
		Tooltip.instance.SetText (message);
	}

	public void OnPointerExit (PointerEventData e) {
		GameManager.instance.Hide(Tooltip.instance.gameObject);
	}

	public void OnPointerClick (PointerEventData e) {
		GameManager.instance.Hide(Tooltip.instance.gameObject);
	}
}
