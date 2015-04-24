using System;
using System.Reflection;
using System.Collections.Generic;
using ws.winx.csharp.extensions;
using UnityEngine;

namespace ws.winx.unity.attributes
{
	public class UniUnityVariablePropertyAttribute : PropertyAttribute
	{


		public Type[] typesCustom;
		public bool only;//no unity types just custom
		public String name;

		public UniUnityVariablePropertyAttribute(String name="",Type[] types=null,bool only=false){

			this.typesCustom = types;
			this.only = only;
			this.name = name;
		}
	}

}
