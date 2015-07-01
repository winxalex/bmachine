using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityScreen{
	[System.Serializable]
	[Category("Screen")]
	public class TweenColor : TweenAction {
		public Color from=Color.clear;
		public Color to=Color.black;
		private float currentTime;
		private Color colorLerp;
		private Texture2D texture;

		public override void OnEnter (GameObject target)
		{
			colorLerp = from;
			texture = new Texture2D (1, 1);
			texture.SetPixel (0, 0, Color.white);
			texture.Apply ();
		}

		public override void OnUpdate (GameObject target, float percentage)
		{
			colorLerp = GetValue (from, to, percentage);
		}


		public override void OnGUI ()
		{
			if (texture) {
				var guiColor = GUI.color;
				GUI.color = colorLerp;
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), texture);
				GUI.color = guiColor;
			}
		}
	}
}