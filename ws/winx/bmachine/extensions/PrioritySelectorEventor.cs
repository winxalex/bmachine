using System;
using BehaviourMachine;
using UnityEngine;

namespace ws.winx.bmachine.extensions
{
		[NodeInfo (category = "Extensions/Eventor/Composite/", icon = "PrioritySelector", description = "Similar to the Selector node, but always starts its execution from the first node")]
		public class PrioritySelectorEventor : CompositeNode
		{

				//
				// Fields
				//
				[NonSerialized]
				private int
						m_CurrentChildIndex;
				

			

				public override void Start ()
				{
						
						this.m_CurrentChildIndex = 0;
						//this.status = Status.Error;//composite without children

						if (this.children.Length > 0) {
			
								ActionNode child = this.children [0];
								

				//this.status=Status.Running;
								((IEventStatusNode)child).OnChildCompleteStatus += onChildStatus;

								//Debug.Log ("Listen to child:" + child.name);
								
								

//				if(typeof(IEventStatusNode).IsAssignableFrom(child.GetType())){
//					IEventStatusNode node=(IEventStatusNode)child;
//					node.OnUpdateStatus+=new StatusUpdateHandler(onUpdateNodeStatus);
//				}
						} 


				}

			
		public void OnTick ()
		{
//			if (this.state != Status.Running)
//			{
//				this.Start ();
//			}



//			if (this.status != Status.Running)
//			{
//				this.End ();
//			}
		}

			

				void onChildStatus (object sender, StatusEventArgs args)
				{
					//Debug.Log ("onChildStatus:" + args.status);

						ActionNode child = (ActionNode)sender;
						((IEventStatusNode)child).OnChildCompleteStatus -= onChildStatus;

						if (args.status == Status.Success) {
								//this.status = args.status;
								this.End ();
								return;
						}

					

						//try next child or return Falure if there in no other
						this.m_CurrentChildIndex++;

						if (this.m_CurrentChildIndex < this.children.Length) {

								child = this.children [this.m_CurrentChildIndex];

						//	Debug.Log ("Listen to child:" + child.name);
				                
								((IEventStatusNode)child).OnChildCompleteStatus += onChildStatus;
							

						} else {
								this.End ();
								//this.status = Status.Failure;
						}
				}

		
		}
}
