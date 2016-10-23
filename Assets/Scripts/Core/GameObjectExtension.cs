// GameObjectExtension.cs
// ©2016 Team 95

using Route95.UI;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.Core {

    /// <summary>
    /// Class with extension methods for GameObjects and Components.
    /// </summary>
    public static class GameObjectExtension {

        #region GameObject Extensions

        /// <summary>
        /// Quick reference to the GameObject's RectTransform component.
        /// </summary>
        public static RectTransform RectTransform(this GameObject obj) {
            return obj.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Quick reference to the GameObject's Button component.
        /// </summary>
        public static Button Button(this GameObject obj) {
            return obj.GetComponent<Button>();
        }

        /// <summary>
        /// Quick reference to the GameObject's Text component.
        /// </summary>
        public static Text Text(this GameObject obj) {
            return obj.GetComponent<Text>();
        }

        /// <summary>
        /// Quick reference to the GameObject's Image component.
        /// </summary>
        public static Image Image(this GameObject obj) {
            return obj.GetComponent<Image>();
        }

        /// <summary>
        /// Quick reference to the GameObject's SpriteRenderer component.
        /// </summary>
        public static SpriteRenderer SpriteRenderer(this GameObject obj) {
            return obj.GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Quick reference to the GameObject's ShowHide component.
        /// </summary>
        public static ShowHide ShowHide(this GameObject obj) {
            return obj.GetComponent<ShowHide>();
        }

        /// <summary>
        /// Quick reference to the GameObject's Light component.
        /// </summary>
        public static Light Light(this GameObject obj) {
            return obj.GetComponent<Light>();
        }

        /// <summary>
        /// Sets a UI GameObject's parent.
        /// </summary>
        public static void SetParent(this GameObject obj, RectTransform parent) {
            RectTransform tr = obj.RectTransform();
            Vector3 scale = tr.localScale;
            tr.SetParent(parent);
            tr.localScale = scale;
        }

        /// <summary>
        /// Anchors a UI GameObject at a point.
        /// </summary>
        public static void AnchorAtPoint(this GameObject obj, float x, float y) {
            obj.RectTransform().AnchorAtPoint(new Vector2(x, y));
        }

        /// <summary>
        /// Sets the size (not scale) of a UI GameObject.
        /// </summary>
        public static void SetSize2D(this GameObject obj, float x, float y) {
            obj.RectTransform().sizeDelta = new Vector2(x, y);
        }

        /// <summary>
        /// Sets the size (not scale) of a UI GameObject.
        /// </summary>
        public static void SetSize2D(this GameObject obj, Vector2 v) {
            obj.RectTransform().sizeDelta = v;
        }

        /// <summary>
        /// Sets the width (not scale) of a square UI GameObject.
        /// </summary>
        public static void SetSideWidth(this GameObject obj, float width) {
            obj.RectTransform().sizeDelta = new Vector2(width, width);
        }

        /// <summary>
        /// Sets the 2D position of a UI GameObject.
        /// </summary>
        public static void SetPosition2D(this GameObject obj, float x, float y) {
            obj.RectTransform().anchoredPosition = new Vector2(x, y);
        }

        /// <summary>
        /// Sets the text alignment of a text GameObject.
        /// </summary>
        public static void SetTextAlignment(this GameObject obj, TextAnchor align) {
            obj.Text().alignment = align;
        }

        /// <summary>
        /// Sets the font size of a text GameObject.
        /// </summary>
        public static void SetFontSize(this GameObject obj, int size) {
            obj.Text().fontSize = size;
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
