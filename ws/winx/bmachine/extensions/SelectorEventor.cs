using System;
using BehaviourMachine;

namespace ws.winx.bmachine.extensions
{
		[NodeInfo (category = "Extensions/Eventor/Composite/", icon = "Selector", description = "Has an \"or\" logic. If one child succeed, then the execution is stopped and the selector returns Success. If a child fails then sequantially runs the next child. If all child fails, returns Failure")]
		public class SelectorEventor : CompositeNode
		{
		//!!! NOT DONE
				//
				// Fields
				//
				[NonSerialized]
				private int
						m_CurrentChildIndex;
				Status _currentStatus;

				public override bool Add (ActionNode child)
				{

						//child.GetType ();


						return base.Add (child);
				}

				public override void Awake ()
				{
						base.Awake ();


				}

				public override void Start ()
				{
						base.Start ();

						_currentStatus = Status.Running;

						if (this.children.Length > 0) {
			
								IEventStatusNode child = (IEventStatusNode)this.children [0];
								child.OnChildCompleteStatus += onUpdateNodeStatus;

//				if(typeof(IEventStatusNode).IsAssignableFrom(child.GetType())){
//					IEventStatusNode node=(IEventStatusNode)child;
//					node.OnUpdateStatus+=new StatusUpdateHandler(onUpdateNodeStatus);
//				}
						}


				}

				public override void OnTick ()
				{
						//base.OnTick ();
				}

				public override void Update ()
				{
						this.status = _currentStatus;
						base.Update ();
				}

				void onUpdateNodeStatus (object sender, StatusEventArgs args)
				{

						if (args.status == Status.Success) {
								this.status = args.status;
								return;
						}

						IEventStatusNode child = (IEventStatusNode)sender;
						child.OnChildCompleteStatus -= onUpdateNodeStatus;

						//try next child or return Falure if there in no other
						this.m_CurrentChildIndex++;

						if (this.m_CurrentChildIndex < this.children.Length) {

								child = (IEventStatusNode)this.children [this.m_CurrentChildIndex];

								child.OnChildCompleteStatus += new StatusUpdateHandler (onUpdateNodeStatus);

						} else {
								this.status = Status.Failure;
						}
				}

		
		}
}
