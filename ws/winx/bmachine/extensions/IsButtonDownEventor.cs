using System;
using UnityEngine;
using ws.winx.bmachine.extensions;


namespace BehaviourMachine
{
	[NodeInfo (category = "Extensions/Eventor/Input/", icon = "Button", description = "Returns Success during the frame the user pressed down the virtual button identified by Button Name")]
	public class IsButtonDownEventor : ConditionNode,IEventStatusNode
	{
		//
		// Fields
		//
		[VariableInfo (tooltip = "The virtual button to test")]
		public StringVar buttonName;

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


		
		//
		// Methods
		//
		public override Status Update ()
		{
			throw new NotImplementedException ();
		}

//		public  void OnTick ()
//		{
//
//		
//			if (this.buttonName.isNone)
//			{
//				//_statusArgs.status=base.status = Status.Error;
//
//
//
//			}else
//			if (Input.GetButtonDown (this.buttonName.Value))
//			{
//				if (this.onSuccess.id != 0)
//				{
//					base.owner.root.SendEvent (this.onSuccess.id);
//				}
//
//				_statusArgs.status=base.status = Status.Success;
//
//
//
//			}
//			else
//			{
//				_statusArgs.status=base.status = Status.Failure;
//
//
//			}
//
//			Debug.Log("onTick "+_statusArgs.status);
//		
//			if(_statusHandler!=null) _statusHandler.Invoke(this,_statusArgs);
//		}
		
		public override void Reset ()
		{
			base.Reset ();
			this.buttonName = "Fire1";
		}
	}
}