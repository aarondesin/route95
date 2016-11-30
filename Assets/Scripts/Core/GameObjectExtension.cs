// GameObjectExtension.cs
// ©2016 Team 95

using Route95.UI;

using System;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.Core {

    /// <summary>
    /// Class with extension methods for GameObjects and Components.
    /// </summary>
    public static class GameObjectExtension {

        #region GameObject Extensions

		public static void ToggleActive (this GameObject obj) {
			obj.SetActive (!obj.activeSelf);
		}

        /// <summary>
        /// Sets a UI GameObject's parent.
        /// </summary>
        public static void SetParent(this GameObject obj, RectTransform parent) {
            RectTransform tr = obj.GetComponent<RectTransform>();
            Vector3 scale = tr.localScale;
            tr.SetParent(parent);
            tr.localScale = scale;
        }

		public static T GetComponentInSiblings<T> (this GameObject obj) 
			where T : Component 
		{
			var parent = obj.transform.parent;
			for (int i = 0; i < parent.childCount; i++) {
				var component = parent.GetChild(i).GetComponent<T>();
				if (component != default(T)) return component;
			}

			return default(T);
		}

        /// <summary>
        /// Anchors a UI GameObject at a point.
        /// </summary>
        public static void AnchorAtPoint(this GameObject obj, float x, float y) {
            obj.GetComponent<RectTransform>().AnchorAtPoint(new Vector2(x, y));
        }

        /// <summary>
        /// Sets the size (not scale) of a UI GameObject.
        /// </summary>
        public static void SetSize2D(this GameObject obj, float x, float y) {
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        }

        /// <summary>
        /// Sets the size (not scale) of a UI GameObject.
        /// </summary>
        public static void SetSize2D(this GameObject obj, Vector2 v) {
            obj.GetComponent<RectTransform>().sizeDelta = v;
        }

        /// <summary>
        /// Sets the width (not scale) of a square UI GameObject.
        /// </summary>
        public static void SetSideWidth(this GameObject obj, float width) {
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, width);
        }

        /// <summary>
        /// Sets the 2D position of a UI GameObject.
        /// </summary>
        public static void SetPosition2D(this GameObject obj, float x, float y) {
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }

        /// <summary>
        /// Sets the text alignment of a text GameObject.
        /// </summary>
        public static void SetTextAlignment(this GameObject obj, TextAnchor align) {
            obj.GetComponent<Text>().alignment = align;
        }

        /// <summary>
        /// Sets the font size of a text GameObject.
        /// </summary>
        public static void SetFontSize(this GameObject obj, int size) {
            obj.GetComponent<Text>().fontSize = size;
        }

        #endregion
        #region RectTransform Extensions

        /// <summary>
        /// Sets the side width (not scale) of a square RectTransform.
        /// </summary>
        public static void SetSideWidth(this RectTransform tr, float width) {
            tr.sizeDelta = new Vector2(width, width);
        }

        #endregion
        #region ParticleSystem Extensions

        /// <summary>
        /// Sets the emission rate of a particle system.
        /// </summary>
        public static void SetRate(this ParticleSystem sys, float newRate) {
            ParticleSystem.EmissionModule temp = sys.emission;
            ParticleSystem.MinMaxCurve curve = temp.rate;
            curve = new ParticleSystem.MinMaxCurve(newRate);
            temp.rate = curve;
        }

        #endregion
    }
}
