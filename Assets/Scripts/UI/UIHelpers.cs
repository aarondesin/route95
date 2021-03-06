﻿// UIHelpers.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to hold UI-related helper functions.
    /// </summary>
    public class UIHelpers : MonoBehaviour {

        /// <summary>
        /// Creates a button GameObject with default properties.
        /// </summary>
        /// <param name="buttonName"></param>
        public static GameObject MakeButton(string buttonName) {
            GameObject button = new GameObject(buttonName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Button),
                //typeof(ShowHide),
                typeof(Image)
            );
            RectTransform tr = button.GetComponent<RectTransform>();
            tr.localScale = Vector3.one;
            tr.localRotation = Quaternion.Euler(Vector3.zero);

            return button;
        }

		/// <summary>
		/// Makes a button with the given graphic.
		/// </summary>
        public static GameObject MakeButton(string buttonName, Sprite graphic) {
            GameObject button = MakeButton(buttonName);
            button.GetComponent<Image>().sprite = graphic;
            return button;
        }

		/// <summary>
		/// Makes a button with the given properties.
		/// </summary>
        public static GameObject MakeButton(string buttonName, Sprite image, RectTransform parent, Vector2 sizeD, Vector2 pos) {
            GameObject button = MakeButton(buttonName);

            RectTransform tr = button.GetComponent<RectTransform>();
            tr.SetParent(parent);
            tr.sizeDelta = sizeD;
            tr.anchoredPosition = pos;

            button.GetComponent<Image>().sprite = image;

            return button;
        }

        /// <summary>
        /// Creates a button GameObject with default properties
        /// and a child text GameObject.
        /// </summary>
        public static GameObject MakeTextButton(string buttonText) {
            GameObject button = MakeButton(buttonText);
            RectTransform button_tr = button.GetComponent<RectTransform>();
            GameObject text = MakeText(buttonText + "text");
            RectTransform text_tr = text.GetComponent<RectTransform>();
            Text text_text = text.GetComponent<Text>();
            text_tr.SetParent(button_tr);
            text_tr.sizeDelta = button_tr.sizeDelta;
            text_tr.localScale = button_tr.localScale;
            text_text.alignment = TextAnchor.MiddleCenter;
            text_text.text = buttonText;

            return button;
        }

        /// <summary>
        /// Creates a text GameObject with default properties.
        /// </summary>
        /// <param name="textName"></param>
        /// <returns></returns>
        public static GameObject MakeText(string textName) {
            GameObject text = new GameObject(textName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text)
            );
            RectTransform tr = text.GetComponent<RectTransform>();
            tr.localScale = Vector3.one;
            tr.localRotation = Quaternion.Euler(Vector3.zero);
            Text txt = text.GetComponent<Text>();
            txt.text = textName;
            txt.font = UIManager.Instance.Font;
            txt.resizeTextForBestFit = false;
            txt.fontStyle = FontStyle.Normal;
            return text;
        }

		/// <summary>
		/// Makes a text object with the given properties.
		/// </summary>
        public static GameObject MakeText(string textName, RectTransform parent, Vector2 sizeD, Vector2 pos) {
            GameObject text = MakeText(textName);

            RectTransform tr = text.GetComponent<RectTransform>();
            tr.SetParent(parent);
            tr.sizeDelta = sizeD;
            tr.anchoredPosition = pos;

            return text;
        }

        /// <summary>
        /// Creates an image GameObject with default properties.
        /// </summary>
        public static GameObject MakeImage(string imageName) {
            GameObject image = new GameObject(imageName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );
            RectTransform tr = image.GetComponent<RectTransform>();
            tr.ResetScaleRot();
            return image;
        }

		/// <summary>
		/// Makes an image object with the given graphic.
		/// </summary>
        public static GameObject MakeImage(string imageName, Sprite graphic) {
            GameObject image = MakeImage(imageName);
            image.GetComponent<Image>().sprite = graphic;
            return image;
        }

		/// <summary>
		/// Makes an image object with the given properties.
		/// </summary>
        public static GameObject MakeImage(string imageName, Sprite graphic, RectTransform parent, Vector2 sizeD, Vector2 pos) {
            GameObject image = MakeImage(imageName);

            RectTransform tr = image.GetComponent<RectTransform>();
            tr.SetParent(parent);
            tr.sizeDelta = sizeD;
            tr.anchoredPosition = pos;

            image.GetComponent<Image>().sprite = graphic;

            return image;
        }
    }
}