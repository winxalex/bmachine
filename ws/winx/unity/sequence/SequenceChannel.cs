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
					}else{

						Debug.LogWarning("Target of channel \""+name+"\" is missing from the scene ");
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

		//[HideInInspector]
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


		public SequenceNode getActiveNodeAt(ref double time){


			SequenceNode node = null;
			
			IAnimatedValues animatedValues = null;
			
			//find node in time (node in which time is in range between nodeStartTime and nodeEndTime)
			foreach (SequenceNode n in this.nodes) {
				
				//find first node which has n.startTime >= then current time (lower boundary)
				if (time - n.timeStart >= 0) {
					//check if time comply to upper boundary
					if (time <= n.timeStart + n.duration) {
						node = n;
						break;
					} else { 
						//if channel is of animation type 
						//and this is only node
						//or there is next node, so time is between prev and next node => snap time to prev node endTime
						if (this.type == SequenceChannel.SequenceChannelType.Animation 
							&& (this.nodes.Count == 1
							|| (n.index + 1 < this.nodes.Count && time < this.nodes [n.index + 1].timeStart))
						    
						    ) {
							
							node = n;
							time = n.timeStart + n.duration;//snap to node time end
							break;
							
						}
						
					}
				}else {//time is left from the first most left node => return node and snap time to that node start time
					
					if (this.type == SequenceChannel.SequenceChannelType.Animation){
//						if((animatedValues = this.target.GetComponent<IAnimatedValues> ()) != null)
//							
//							//reset need cos you might have click in one node with ikAnimatedValues then in another => first node should be reseted
//							animatedValues.ResetValues ();
//						else 
							time=n.timeStart;//snap to node time start
							
						node=n;
						
						break;
					}
					
				}
				
				
			}
			
			return node;
			
			
		}
		
		
		#endregion

		public void Reset ()
		{
			if (this.target != null) {
				this.target.transform.position = this.targetPositionOriginal;
				this.target.transform.rotation = this.targetRotationOriginal;

				Debug.Log("Channel reset. Target position and rotation returned to target starting pos and rot");
			}

		
		}


	}

				
}