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

				public enum SequenceChannelType{
						Animation,
						Video,
						Audio
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


				[SerializeField]
				List<SequenceNode> _nodes;
				
				public List<SequenceNode> nodes {
					get {
						if(_nodes==null) _nodes=new List<SequenceNode>();
						return _nodes;
					}
				}

		public RuntimeAnimatorController runtimeAnimatorController;



	    }

				
}