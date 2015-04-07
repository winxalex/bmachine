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
using ws.winx.input.components;

namespace ws.winx.bmachine.extensions
{
		[NodeInfo (category = "Extensions/Input/", icon = "Axis", description = "Stores the value of the virtual axis identified by \"Axis Name\" in \"Store Axis\"", url = "")]
		public class GetInputNode : ActionNode
		{
				//
				// Fields
				//

				public InputPlayer.Player player = InputPlayer.Player.Player0;

				public new BlackboardCustom blackboard {
						get{ return (BlackboardCustom)base.blackboard; }
				}

				public enum InputType
				{
						GetInput,
						GetInputUp,
						GetInputDown,
						GetInputHold
				}
				
				
				
				public InputType
						inputType;

				//[EnumAttribute("ws.winx.input.states.States")]//?????
				[EnumAttribute(typeof(ws.winx.input.states.States))]
				public int //this is enum from Status.cs generated
						inputStatePos;
				[EnumAttribute(typeof(ws.winx.input.states.States))]
				public int //this is enum from Status.cs generated
						inputStateNeg;

				
				public bool fullAxis = true;

				[UnityVariablePropertyAttribute(typeof(float),"Input Value")]
				public UnityVariable
						variable;
				
				
				public float
						sensitivity = 0.25f;
				
				public float
						dreadzone = 0.1f;
				
				public float
						gravity = 0.3f;
				
				public float
						multiplier = 1f;
		
		
				//
				// Methods
				//

				public override void Awake ()
				{
				
						base.Awake ();
				}

				public override void Reset ()
				{
					
						variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
						variable.Value = 0f;//make it float type

						multiplier = 1f;

						sensitivity = 0.25f;
						gravity = 0.3f;
						dreadzone = 0.1f;
						fullAxis = true;
					


				}

		public void onTick(){

			Debug.Log ("tick");
				}
	
				public override Status Update ()
				{


				
						

						if (inputType == InputType.GetInput) {


								if (fullAxis) {
										variable.Value = multiplier * 
												(Math.Abs (InputManager.GetInput (this.inputStatePos, player, sensitivity, dreadzone, gravity)) -
												Math.Abs (InputManager.GetInput (this.inputStateNeg, player, sensitivity, dreadzone, gravity)));	
								} else

										variable.Value = multiplier * 
												(Math.Abs (InputManager.GetInput (this.inputStatePos, player, sensitivity, dreadzone, gravity)));
								

								return Status.Success;
						} else if (inputType == InputType.GetInputDown) {
								if (InputManager.GetInputDown (this.inputStatePos, player))
										return Status.Success;

								return Status.Failure;
						} else if (inputType == InputType.GetInputUp) {
								if (InputManager.GetInputUp (this.inputStatePos, player))
										return Status.Success;
				
								return Status.Failure;
						} else if (inputType == InputType.GetInputHold) {
								if (InputManager.GetInputHold (this.inputStatePos, player))
										return Status.Success;
				
								return Status.Failure;
						}
			

						return Status.Success;
				}
		}
}