using UnityEngine;
using System.Collections;
using System;

namespace ws.winx.bmachine.extensions
{
		public delegate void StatusUpdateHandler (object sender,StatusEventArgs e);

		public class StatusEventArgs : EventArgs
		{
				public BehaviourMachine.Status status { get; set; }
		
				public StatusEventArgs (BehaviourMachine.Status status)
				{
						this.status = status;
				}
		}


}
