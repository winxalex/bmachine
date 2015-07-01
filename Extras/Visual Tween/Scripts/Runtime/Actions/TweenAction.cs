using UnityEngine;
using System.Collections;

namespace VisualTween.Action{
	[System.Serializable]
	public class TweenAction:BaseAction {
		public EasingCurve.EaseType easeType = EasingCurve.EaseType.Linear;

		public Vector4 GetValue(Vector4 from, Vector4 to, float t)
		{
			Vector4 vector4 = new Vector4();
			vector4.x = EasingCurve.GetValue(this.easeType, from.x, to.x, t);
			vector4.y = EasingCurve.GetValue(this.easeType, from.y, to.y, t);
			vector4.z = EasingCurve.GetValue(this.easeType, from.z, to.z, t);
			vector4.w = EasingCurve.GetValue(this.easeType, from.w, to.w, t);
			return vector4;
		}

		public Vector3 GetValue(Vector3 from, Vector3 to, float t)
		{
			Vector3 vector3 = new Vector3();
			vector3.x = EasingCurve.GetValue(this.easeType, from.x, to.x, t);
			vector3.y = EasingCurve.GetValue(this.easeType, from.y, to.y, t);
			vector3.z = EasingCurve.GetValue(this.easeType, from.z, to.z, t);
			return vector3;
		}

		public Vector2 GetValue(Vector2 from, Vector2 to, float t)
		{
			Vector2 vector2 = new Vector2();
			vector2.x = EasingCurve.GetValue(this.easeType, from.x, to.x, t);
			vector2.y = EasingCurve.GetValue(this.easeType, from.y, to.y, t);
			return vector2;
		}

		public Color GetValue(Color from, Color to, float t)
		{
			Color color = new Color();
			color.r = EasingCurve.GetValue(this.easeType, from.r, to.r, t);
			color.g = EasingCurve.GetValue(this.easeType, from.g, to.g, t);
			color.b = EasingCurve.GetValue(this.easeType, from.b, to.b, t);
			color.a = EasingCurve.GetValue(this.easeType, from.a, to.a, t);
			return color;
		}
		
		public float GetValue(float from, float to, float t)
		{
			float value= EasingCurve.GetValue(this.easeType, from, to, t);
			return value;
		}
	}
}