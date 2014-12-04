using System;
using BehaviourMachine;
using UnityEngine;


namespace ws.winx.bmachine.extensions
{
	[NodeInfo (category = "Extensions/Eventor/Composite/", icon = "Sequence", description = "Has an \"and\" logic. If all children succeed, returns Success. If one child does not succeed, then the execution is stopped and the sequence returns the status of this child")]
	public class SequenceEventor : CompositeNode,IEventStatusNode
	{

		//
		// Fields
		//
		[NonSerialized]
		private int
			m_CurrentChildIndex;
		
		#region IEventStatusNode implementation
		
		StatusUpdateHandler _statusHandler;
		StatusEventArgs _statusArgs = new StatusEventArgs (Status.Error);
		
		public event StatusUpdateHandler OnChildCompleteStatus{
			
			add{
				_statusHandler+=value;
				
				//v1
				//this.tree.StartCoroutine
				
				//v2
				this.tree.update += OnTick;
			}
			
			remove{
				_statusHandler-=value;
				
				//v1
				//this.tree.StopCoroutine()
				
				//v2
				this.tree.update -= OnTick;
			}
			
		}
		
		
		
		#endregion




		public override void Awake ()
		{
			base.Awake ();

			//this.tree.update -= OnTick ();
		}
		
		
		public override void Start ()
		{
			
			this.m_CurrentChildIndex = 0;
			this.status = Status.Error;//composite without children
			
			if (this.children.Length > 0) {
				
				ActionNode child = this.children [0];
				this.status=Status.Running;
				((IEventStatusNode)child).OnChildCompleteStatus += onChildStatus;
				
				Debug.Log ("Listen to child:" + child.name);
				
				
				
				//				if(typeof(IEventStatusNode).IsAssignableFrom(child.GetType())){
				//					IEventStatusNode node=(IEventStatusNode)child;
				//					node.OnUpdateStatus+=new StatusUpdateHandler(onUpdateNodeStatus);
				//				}
			} 
			
			
		}
		
		
		public override void OnTick ()
		{
			if (this.status != Status.Running)
			{
				this.Start ();
			}
			
			
			
			//			if (this.status != Status.Running)
			//			{
			//				this.End ();
			//			}
		}
		
		
		
		void onChildStatus (object sender, StatusEventArgs args)
		{
			//Debug.Log ("onChildStatus:" + args.status);
			
			ActionNode child = (ActionNode)sender;


			if (args.status == Status.Running)//=> wait
								return;

			((IEventStatusNode)child).OnChildCompleteStatus -= onChildStatus;
			

			
			

			
			if (args.status == Status.Success)
			{
				//try next child 
				this.m_CurrentChildIndex++;

				if(this.m_CurrentChildIndex < this.children.Length) {
				
				child = this.children [this.m_CurrentChildIndex];
				
				Debug.Log ("Listen to child:" + child.name);
				
				((IEventStatusNode)child).OnChildCompleteStatus += onChildStatus;
				}else{
					this.End();
					_statusArgs.status=this.status = Status.Success;


				}
				
			} else {
				this.End ();
				_statusArgs.status=this.status = args.status;


			}


			if(_statusHandler!=null) _statusHandler.Invoke(this,_statusArgs);
		}
		
		
	}
}
