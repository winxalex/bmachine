using UnityEngine;
using System;
using System.Reflection;


namespace ws.winx.editor
{






	#region BlendTreeExtension
		public static class BlendTreeExtension
		{
	
				public static int GetRecursiveBlendParamCount (this UnityEditor.Animations.BlendTree bt)
				{
						object val = bt.GetType ().GetProperty ("recursiveBlendParameterCount", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public).GetValue (bt, new object[]{});
						return (int)val;
				}

				public static string GetRecursiveBlendParam (this UnityEditor.Animations.BlendTree bt, int index)
				{
						object val = bt.GetType ().GetMethod ("GetRecursiveBlendParameter", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke (bt, new object[]{index});
						return (string)val;
				}

				public static float GetRecursiveBlendParamMax (this UnityEditor.Animations.BlendTree bt, int index)
				{
						object val = bt.GetType ().GetMethod ("GetRecursiveBlendParameterMax", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke (bt, new object[]{index});
						return (float)val;
				}

				public static float GetRecursiveBlendParamMin (this UnityEditor.Animations.BlendTree bt, int index)
				{
						object val = bt.GetType ().GetMethod ("GetRecursiveBlendParameterMin", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke (bt, new object[]{index});
						return (float)val;
				}
	
		}
	#endregion

}
