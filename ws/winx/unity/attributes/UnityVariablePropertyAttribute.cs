using System;
using System.Reflection;
using System.Collections.Generic;
using ws.winx.csharp.extensions;
using UnityEngine;

namespace ws.winx.unity.attributes
{
	public class UnityVariablePropertyAttribute : PropertyAttribute
	{


		public Type VariableType;

		public UnityVariablePropertyAttribute(Type type){

			this.VariableType = type;
		}
	}

}
