using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using System.Linq.Expressions;

namespace ws.winx.unity
{

	public static class ExtensionsTransform
	{

		public static void ResetTransformation(this Transform trans)
		{
			trans.position = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = new Vector3(1, 1, 1);
		}

		public static void SetPositionX(this Transform t, float newX)
		{
			t.position = new Vector3(newX, t.position.y, t.position.z);
		}
		
		public static void SetPositionY(this Transform t, float newY)
		{
			t.position = new Vector3(t.position.x, newY, t.position.z);
		}
		
		public static void SetPositionZ(this Transform t, float newZ)
		{
			t.position = new Vector3(t.position.x, t.position.y, newZ);
		}

		public static float GetPositionX(this Transform t)
		{
			return t.position.x;
		}
		
		public static float GetPositionY(this Transform t)
		{
			return t.position.y;
		}
		
		public static float GetPositionZ(this Transform t)
		{

			return t.position.z;
		}


		public static void LocalScaleSetX( this Transform t, float s ) { Vector3 v = t.localScale; v.x  = s; t.localScale = v; }
		public static void LocalScaleSetY( this Transform t, float s ) { Vector3 v = t.localScale; v.y  = s; t.localScale = v; }
		public static void LocalScaleSetZ( this Transform t, float s ) { Vector3 v = t.localScale; v.z  = s; t.localScale = v; }
		public static void LocalScaleMulX( this Transform t, float s ) { Vector3 v = t.localScale; v.x *= s; t.localScale = v; }
		public static void LocalScaleMulY( this Transform t, float s ) { Vector3 v = t.localScale; v.y *= s; t.localScale = v; }
		public static void LocalScaleMulZ( this Transform t, float s ) { Vector3 v = t.localScale; v.z *= s; t.localScale = v; }
	}
	
	public static class QuaternionExtensions
	{
		public static void set_x(this Quaternion t, float x)
		{
			Quaternion v = t; t.x  = x; t = v;

		}

		public static void set_y(this Quaternion t, float y)
		{
			t = new Quaternion(t.x, y, t.z,t.w);
		}


		public static void set_z(this Quaternion t, float z)
		{
			t = new Quaternion(t.x, t.y, z,t.w);
		}

		public static void set_w(this Quaternion t, float w)
		{
			t = new Quaternion(t.x, t.y, t.z,w);
		}


		public static Quaternion Pow(this Quaternion input, float power)
		{
			float inputMagnitude = input.Magnitude();
			Vector3 nHat = new Vector3(input.x, input.y, input.z).normalized;
			Quaternion vectorBit = new Quaternion(nHat.x, nHat.y, nHat.z, 0)
				.ScalarMultiply(power * Mathf.Acos(input.w / inputMagnitude))
					.Exp();
			return vectorBit.ScalarMultiply(Mathf.Pow(inputMagnitude, power));
		}
		
		public static Quaternion Exp(this Quaternion input)
		{
			float inputA = input.w;
			Vector3 inputV = new Vector3(input.x, input.y, input.z);
			float outputA = Mathf.Exp(inputA) * Mathf.Cos(inputV.magnitude);
			Vector3 outputV = Mathf.Exp(inputA) * (inputV.normalized * Mathf.Sin(inputV.magnitude));
			return new Quaternion(outputV.x, outputV.y, outputV.z, outputA);
		}
		
		public static float Magnitude(this Quaternion input)
		{
			return Mathf.Sqrt(input.x * input.x + input.y * input.y + input.z * input.z + input.w * input.w);
		}
		
		public static Quaternion ScalarMultiply(this Quaternion input, float scalar)
		{
			return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
		}



		/// <summary>
		/// Froms to around axis creates a FromToRotation, but makes sure it's axis remains fixed near to th Quaternion singularity point.
		/// </summary>
		/// <returns>
		/// The from to rotation around an axis.
		/// </returns>
		/// <param name='fromDirection'>
		/// From direction.
		/// </param>
		/// <param name='toDirection'>
		/// To direction.
		/// </param>
		/// <param name='axis'>
		/// Axis. Should be normalized before passing into this method.
		/// </param>
		public static Quaternion FromToAroundAxis(Vector3 fromDirection, Vector3 toDirection, Vector3 axis) {
			Quaternion fromTo = Quaternion.FromToRotation(fromDirection, toDirection);
			
			float angle = 0;
			Vector3 freeAxis = Vector3.zero;
			
			fromTo.ToAngleAxis(out angle, out freeAxis);
			
			float dot = Vector3.Dot(freeAxis, axis);
			if (dot < 0) angle = -angle;
			
			return Quaternion.AngleAxis(angle, axis);
		}
		
		/// <summary>
		/// Gets the rotation that can be used to convert a rotation from one axis space to another.
		/// </summary>
		public static Quaternion RotationToLocalSpace(Quaternion space, Quaternion rotation) {
			return Quaternion.Inverse(Quaternion.Inverse(space) * rotation);
		}
		
		/// <summary>
		/// Gets the Quaternion from rotation "from" to rotation "to".
		/// </summary>
		public static Quaternion FromToRotation(Quaternion from, Quaternion to) {
			if (to == from) return Quaternion.identity;
			
			return to * Quaternion.Inverse(from);
		}
	}

	public static class VectorExtensions {
		public static Vector3 ToGuiCoordinateSystem(this Vector3 a) {
			var copy = a;
			copy.y = Screen.height - copy.y;
			return copy;
		}
		
		public static Vector3 Mask(this Vector3 a, Vector3 mask) {
			return new Vector3(a.x * mask.x, a.y * mask.y, a.z * mask.z);
		}
		
		public static Vector3 Abs(this Vector3 a) {
			return new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
		}
		
		public static Vector3 Inverse(this Vector3 a) {
			return new Vector3(1/a.x, 1/a.y, 1/a.z);
		}
	}


	#region GameObjectExtensions
	public static class GameObjectExtensions
	{
		public static string GetPath(this GameObject obj)
		{
			string path = "/" + obj.name;
			while (obj.transform.parent != null)
			{
				obj = obj.transform.parent.gameObject;
				path = "/" + obj.name + path;
			}
			return path;
		}

		/// <summary>
		/// </summary>
		public static GameObject InstantiateChild (this GameObject parent, GameObject prototype, bool preserveScale = false)
		{
			var child = UnityEngine.Object.Instantiate (prototype) as GameObject;
			var rotCache = child.transform.rotation;
			var scaleCache = child.transform.localScale;
			child.transform.position = parent.transform.position;
			child.transform.parent = parent.transform;
			if (!preserveScale) child.transform.localScale = scaleCache;
			child.transform.localRotation = rotCache;
			return child;
		}



			public static void ForEach(this GameObject o, System.Action<GameObject> f)
			{
				f(o);
				
				int numChildren = o.transform.childCount;
				
				for (int i = 0; i < numChildren; ++i)
				{
					o.transform.GetChild(i).gameObject.ForEach(f);
				}
			}



	}
	#endregion


	
	
}
