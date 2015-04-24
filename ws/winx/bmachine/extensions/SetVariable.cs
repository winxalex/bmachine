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
using ws.winx.unity.attributes;
using System;

namespace ws.winx.bmachine.extensions
{
		[NodeInfo ( category = "Extensions/Blackboard/", icon = "Blackboard", description ="")]
		public class SetVariable:ActionNode
		{
			
			
			[UniUnityVariablePropertyAttribute]
			public UnityVariable variable1;

			

			//ex. [UniUnityVariablePropertyAttribute("mile",new Type[]{typeof(FsmEvent)})]
			[UniUnityVariablePropertyAttribute]
			public UnityVariable variable2;


			//operation

			public override Status Update () {
				return Status.Success;
			}


					
				public override void Reset ()
				{
					variable1=UnityVariable.CreateInstanceOf(typeof(float));
				
					variable2=UnityVariable.CreateInstanceOf(typeof(float));

				}

		}
}