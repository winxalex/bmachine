using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using ws.winx.unity.surrogates;
using ws.winx.csharp.utilities;
using ws.winx.csharp.extensions;



namespace ws.winx.unity.utilities{

	public static class AnimatorUtilityEx{
		
		static RuntimeAnimatorController _dummyController;
		
		/// <summary>
		/// Gets the DUMMy Controller at runtime (one layer, one motion="Override")
		/// </summary>
		/// <value>The DUMM y_ CONTROLLE.</value>
		public static RuntimeAnimatorController DUMMY_CONTROLLER {
			get {
				if (_dummyController == null)
					_dummyController = Resources.Load<RuntimeAnimatorController> ("DummyController");
				
				return _dummyController;
			}
		}
		
		
		
		
		
	}



}

