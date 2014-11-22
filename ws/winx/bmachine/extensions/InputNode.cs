using System;
using UnityEngine;
using ws.winx.bmachine.extensions;


namespace BehaviourMachine
{
	[NodeInfo (category = "Extensions/Mecanim/", icon = "Axis", description = "Stores the value of the virtual axis identified by \"Axis Name\" in \"Store Axis\"", url = "http://docs.unity3d.com/Documentation/ScriptReference/Input.GetAxis.html")]
	public class InputNode : ActionNode,IEventStatusNode
	{
		#region IEventStatusNode implementation

		public event StatusUpdateHandler OnUpdateStatus;

		#endregion

		//
		// Fields
		//
		[VariableInfo (tooltip = "An optional float to multiply the input before store in \"Store Axis\"")]
		public FloatVar multiplier;
		
		[VariableInfo (canBeConstant = false, tooltip = "The variable to store the axis")]
		public FloatVar storeAxis;
		
		[VariableInfo (requiredField = false, nullLabel = "Horizontal", tooltip = "The name of the axis (e.g. Horizontal, Vertical...). See axis in Edit/Project Settings/Input")]
		public StringVar axisName;

		public ObjectVar myPropCHangeObject;

		//public MecanimBlendParameterCustom cust;

		public MecanimBlendParameterCustom floatCustom;


		public override void Awake ()
		{
			base.Awake ();

			//I'm ticking myself
			//ActionNode.onNodeTick += myOwnTick;
		}


//		public 
//		public override void OnTick ()
//		{
//			//base.OnTick ();
//			UnityEngine.Debug.Log ("Did I tick myself "+this.name);
//		}
		
//		//
//		// Methods
//		//
//		public override void OnTick ()
//		{
//			if (this.storeAxis.isNone || this.multiplier.isNone)
//			{
//				base.status = Status.Error;
//				return;
//			}
//
//			UnityEngine.Debug.Log ("Did I tick myself");
//			//myPropCHangeObject.Value
//			//this.storeAxis.Value = Input.GetAxis ((!this.axisName.isNone) ? this.axisName.Value : "Horizontal");
//			//this.storeAxis.Value *= this.multiplier.Value;
//
//
//			//((MecanimBlendParameterCustom)myPropCHangeObject).floatValeu = 342.4343f;
//			//floatCustom.Value = Input.GetAxis ((!this.axisName.isNone) ? this.axisName.Value : "Horizontal");
////			UnityEngine.Debug.Log (((MecanimBlendParameterCustom)this.blackboard.GetObjectVar("New Object")).floatValeu);
//			base.status = Status.Running;
//		}
		
		public override void Reset ()
		{
			this.floatCustom = new MecanimBlendParameterCustom ();
			this.axisName = new ConcreteStringVar ();
			this.storeAxis = new ConcreteFloatVar ();
			//this.cust=new MecanimBlendParameterCustom(this.name,this.blackboard,12);
			this.multiplier = 1f;
		}
	}
}
