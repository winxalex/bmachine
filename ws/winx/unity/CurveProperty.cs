// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEditor;
using UnityEngine;
using BehaviourMachine;
using System.Reflection;



namespace ws.winx.unity{

	[System.Serializable]
	public class CurveProperty:Property<float>,IEquatable<CurveProperty>
	{

		public CurveWrapperW curveWrapperW = new CurveWrapperW ();

	


		//
		// Properties
		//


		//
		// Constructor
		//
		public CurveProperty(MemberInfo memberInfo,UnityEngine.Object reflectedObject):base(memberInfo,reflectedObject)
		{



		}



		
		
		#region IEquatable implementation
		public bool Equals (CurveProperty other)
		{
			if (curveWrapperW.curve == null)
								return false;

		

			return this.curveWrapperW.id == other.curveWrapperW.id;
		}
		#endregion
	}
}

