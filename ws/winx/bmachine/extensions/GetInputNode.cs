//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BehaviourMachine;
using Motion=UnityEngine.Motion;
using ws.winx.unity;
using ws.winx.input;
using ws.winx.unity.attributes;
using System;

namespace ws.winx.bmachine.extensions
{
	[NodeInfo (category = "Extensions/Input/", icon = "Axis", description = "Stores the value of the virtual axis identified by \"Axis Name\" in \"Store Axis\"", url = "http://docs.unity3d.com/Documentation/ScriptReference/Input.GetAxis.html")]
		public class GetInputNode : ActionNode
		{
				//
				// Fields
				//

				public enum InputType {
					GetInput,
			        GetInputUp,
					GetInputDown,
			        GetInputHold
				}
				
				public float
						multiplier;
			
				public float
						storeAxis;
				
				public InputType
						inputType;

		//[EnumAttribute("ws.winx.input.states.States")]//?????
				[EnumAttribute(typeof(ws.winx.input.states.States))]
				public int //this is enum from Status.cs generated
					inputState;


				public UnityVariable variable;
	
				//
				// Methods
				//
				public override void Reset ()
				{
					
					variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
					variable.Value = 0f;//make it float type
				}
	
				public override Status Update ()
				{
						if (inputType == InputType.GetInput)
								variable.Value = InputManager.GetInput (this.inputState);
						else if(inputType==InputType.GetInputDown)
								variable.Value=InputManager.GetInputUp(this.inputState);
			            else if(inputType==InputType.GetInputUp)
								variable.Value=InputManager.GetInputUp(this.inputState);

			Debug.Log (variable.Value);
//						this.storeAxis.Value = Input.GetAxis ((!this.axisName.isNone) ? this.axisName.Value : "Horizontal");
//						this.storeAxis.Value *= this.multiplier.Value;
		//	Type t = Type.GetType ("ws.winx.input.states.States");

						return Status.Success;
				}
		}
}