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
using ws.winx.input.states;
using ws.winx.input;

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
					GetInputDown
				}
				
				public float
						multiplier;
			
				public float
						storeAxis;
				
				public InputType
						inputType;

			
				public int //this is enum from Status.cs generated
					inputState;


				public UnityVariable variable;
	
				//
				// Methods
				//
				public override void Reset ()
				{
					variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
				}
	
				public override Status Update ()
				{
						if (inputType == InputType.GetInput)
								variable.Value = InputManager.GetInput (this.inputState);
//						this.storeAxis.Value = Input.GetAxis ((!this.axisName.isNone) ? this.axisName.Value : "Horizontal");
//						this.storeAxis.Value *= this.multiplier.Value;


						return Status.Success;
				}
		}
}