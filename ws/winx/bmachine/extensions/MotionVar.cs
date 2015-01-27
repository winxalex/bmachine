using UnityEngine;
using System.Collections;
using BehaviourMachine;
using System;
using Object=UnityEngine.Object;


namespace ws.winx.bmachine.extensions
{

	[Serializable]
	public class MotionVar : Variable
	{
		//
		// Fields
		//
		[UnityObjectType, HideInInspector]
		public string objectType = string.Empty;
		
		[ObjectValue ("objectType")]
		public UnityEngine.Motion value;
		
		//
		// Properties
		//
//		public override Type ObjectType
//		{
//			get
//			{
//				Type t=UnityObjectTypeAttribute.GetObjectType (this.objectType);
//				return typeof(UnityEngine.Motion);
//			}
//			set
//			{
//				this.objectType = ((value == null) ? string.Empty : value.ToString ());
//			}
//		}
//		
//		public override Object Value
//		{
//			get
//			{
//				return this.value;
//			}
//			set
//			{
//				this.value = (UnityEngine.Motion)value;
//			}
//		}
		
		//
		// Constructors
		//
		public MotionVar (string name, InternalBlackboard blackboard, int id) : base (name, blackboard, id)
		{
		}
		
		public MotionVar (UnityEngine.Motion value)
		{
			base.SetAsConstant ();
			this.value = value;
		}
		
		public MotionVar ()
		{
		}
	}
//		[System.Serializable]
//		[CustomVariable("Motion")]
//		public class MotionVar : Variable
//		{
//				[BehaviourMachine.Tooltip("The target Transform")]
//				public UnityEngine.Motion
//						motion;
//
//		//
//		// Properties
//		//
//		public override object genericValue
//		{
//			get
//			{
//				return this.Value;
//			}
//			set
//			{
//				this.Value = (UnityEngine.Motion)value;
//			}
//		}
//	
//				public UnityEngine.Motion Value {
//						get { return motion;}
//						set { motion = (UnityEngine.Motion)value;}
//				}
//	
//				public MotionVar () : base ()
//				{
//				}
//	
//				public MotionVar (UnityEngine.Motion value)
//				{
//					base.SetAsConstant ();
//					this.motion = value;
//				}
//
//				//
//				// Constructors
//				//
//				public MotionVar (string name, InternalBlackboard blackboard, int id) : base (name, blackboard, id)
//				{
//			       
//				}
//		}
}