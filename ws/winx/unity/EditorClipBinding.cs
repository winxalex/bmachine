using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using System.Linq.Expressions;

namespace ws.winx.unity
{
	[Serializable]
	public class EditorClipBinding:ScriptableObject{




		public AnimationClip clip;

		/// <summary>
		/// GameObject root - container of all
		/// </summary>
		public GameObject gameObject;



		public Transform boneTransform;


		public Vector3 boneOrginalPositionOffset=Vector3.zero;

	}
}