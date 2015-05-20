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


		public Color color=Color.green;
		public bool visible;

		public AnimationClip clip;

		public AudioClip audioClip;

		/// <summary>
		/// GameObject on which binding is applied
		/// </summary>
		[SerializeField]
		 GameObject _gameObject;

		public GameObject gameObject {
			get {
				return _gameObject;
			}
			set {
				_gameObject = value;

				Transform rootTransform= _gameObject.transform.root;

				rotationOriginalRoot=rootTransform.rotation;
				positionOriginalRoot=rootTransform.position;

				this.boneTransform = this._gameObject.GetRootBone ();


			}
		}

	

		/// <summary>
		/// The bone transform only of clips with Character motion
		/// </summary>
		public Transform boneTransform;

		/// <summary>
		/// The position of the root gameObject before binding is applied
		/// </summary>
		public Vector3 positionOriginalRoot;



		/// <summary>
		/// The bone orginal position offset.
		/// </summary>
		public Vector3 boneOrginalPositionOffset=Vector3.zero;


		/// <summary>
		/// The rotation of the root gameObject before binding is applied
		/// </summary>
		public Quaternion rotationOriginalRoot;


		////////////////////////////////
		///   		METHODS			///
		///////////////////////////////

		/// <summary>
		/// 
		/// </summary>
		public void ResetRoot ()
		{
			Transform transformRoot = this._gameObject.transform.root;
			transformRoot.position = positionOriginalRoot;
			transformRoot.rotation = rotationOriginalRoot;
		}
	}
}