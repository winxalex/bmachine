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

namespace ws.winx.bmachine.extensions
{
		[NodeInfo ( category = "Extensions/Mecanim/", icon = "Animator", description ="Use Mecanima inside BTree")]
		public class TestNode:ActionNode
		{

		public override Status Update () {
			return Status.Success;
		}

		public FloatVar[] arr;
				
			public override void Reset ()
			{
			arr =new FloatVar [0];
				base.Reset ();
			}

		}
}