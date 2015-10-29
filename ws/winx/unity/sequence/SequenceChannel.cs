using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using ws.winx.unity.extensions;

namespace ws.winx.unity.sequence
{
	[System.Serializable]
	public class SequenceChannel:ScriptableObject,ISerializationCallbackReceiver
	{
		public Sequence sequence;

		public enum SequenceChannelType
		{
			Animation,
			Video,
			Audio,
			Particle
		}

		public string name;
		public SequenceChannelType type;
		[SerializeField]
		GameObject
			_target;

		public GameObject target {
			get {
				if (_target == null && !String.IsNullOrEmpty (_targetPath)) {
					_target = GameObject.Find (_targetPath);
					if (_target != null) {
						this.targetPositionOriginal = target.transform.position;
						this.targetRotationOriginal = target.transform.rotation;
						this.targetBoneRoot = target.GetRootBone ();
					}
				}
				
				return _target;
			}
			set {


				_target = value;

				if (_target != null) {
					_targetPath = _target.GetPath ();
					this.targetPositionOriginal = target.transform.position;
					this.targetRotationOriginal = target.transform.rotation;
					this.targetBoneRoot = target.GetRootBone ();
				}
			}
		}

		[HideInInspector]
		public string
			_targetPath;
		[NonSerialized]
		public bool
			targetTransformHasChanged = false;

		/// <summary>
		/// The position of the target before animation is applied.
		/// </summary>
		public Vector3 targetPositionOriginal;

		/// <summary>
		/// The rotation of the target before animation is applied.
		/// </summary>
		public Quaternion targetRotationOriginal;


		/// <summary>
		/// The position of the target - current.
		/// </summary>
		public Vector3 targetPositionCurrent;
				
		/// <summary>
		/// The rotation of the target - current.
		/// </summary>
		public Quaternion targetRotationCurrent;
		Transform _targetBoneRoot;

		public Transform targetBoneRoot {
			get {
				return _targetBoneRoot;
			}
			set {
				_targetBoneRoot = value;
			}
		}

		[SerializeField]
		List<SequenceNode>
			_nodes;
				
		public List<SequenceNode> nodes {
			get {
				if (_nodes == null)
					_nodes = new List<SequenceNode> ();
				return _nodes;
			}
		}

		public RuntimeAnimatorController runtimeAnimatorController;

				#region ISerializationCallbackReceiver implementation

		public void OnBeforeSerialize ()
		{
					
		}

		public void OnAfterDeserialize ()
		{
					
		}

				#endregion

		public void Reset ()
		{
			if (this.target != null) {
				this.target.transform.position = this.targetPositionOriginal;
				this.target.transform.rotation = this.targetRotationOriginal;
			}
		}


	}

				
}