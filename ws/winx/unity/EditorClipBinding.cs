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


		public Color color;
		public bool visible;

		public AnimationClip clip;

		public AudioClip audioClip;

		/// <summary>
		/// GameObject root - container of all
		/// </summary>
		public GameObject gameObject;


		/// <summary>
		/// The bone transform only of clips with Character motion
		/// </summary>
		public Transform boneTransform;


		public Vector3 boneOrginalPositionOffset=Vector3.zero;

	}
}