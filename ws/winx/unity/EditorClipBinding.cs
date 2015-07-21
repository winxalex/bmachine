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
		/// (root) GameObject on which binding is applied
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


		public Vector3 positionOffset=Vector3.zero;
		public Quaternion rotationOffset=Quaternion.identity;
	

		/// <summary>
		/// The bone transform only of clips with Character motion
		/// </summary>
		public Transform boneTransform;

		/// <summary>
		/// The position of the root gameObject before binding is applied
		/// </summary>
		public Vector3 positionOriginalRoot;



		/// <summary>
		/// The root bone position offset.
		/// Offset between root bone position before applying animation on gameobject and 
		/// animation applied at time t=0s
		/// </summary>
		public Vector3 boneRootPositionOffset=Vector3.zero;

		/// <summary>
		/// The bone root rotation offset.
		/// </summary>
		public Quaternion boneRootRotationOffset=Quaternion.identity;


		/// <summary>
		/// The rotation of the root gameObject before binding is applied
		/// </summary>
		public Quaternion rotationOriginalRoot;


		////////////////////////////////
		///   		METHODS			///
		///////////////////////////////

		/// <summary>
		/// Resets position and rotation values before animation is applied to gameobject
		/// This is nessery cos AnimationMode on stop, doesn't reset gameobject on which animation clip sampling is done
		/// </summary>
		public void ResetRoot ()
		{
			Transform transformRoot = this._gameObject.transform.root;
			transformRoot.position = positionOriginalRoot;
			transformRoot.rotation = rotationOriginalRoot;
		}
	}
}