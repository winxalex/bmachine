using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using System.Linq.Expressions;

namespace ws.winx.unity
{

	public class EditorClipBinding:ScriptableObject{


		public AnimationClip clip;

		/// <summary>
		/// GameObject root container of all (need to have Animator Component with RuntimeContoller and one state)
		/// </summary>
		public GameObject gameObject;

	}
}