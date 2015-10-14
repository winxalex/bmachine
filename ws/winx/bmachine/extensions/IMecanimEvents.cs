#if BEHAVIOUR_MACHINE
using System;
using UnityEngine;


namespace ws.winx.bmachine.extensions
{
		public interface IMecanimEvents
		{

				AnimationEvent[] GetAnimationEvents();
				void SetAnimationEvents(AnimationEvent[] events);
		}
}
#endif
