using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using System;

namespace ws.winx.unity.sequence
{
		[System.Serializable]
		public class SequenceChannel:ScriptableObject
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
								return _target;
						}
						set {
								_target = value;
						}
				}

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


				public Transform targetBoneRoot;
				

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



		}

				
}