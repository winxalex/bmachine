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
		public string Name;

		public UnityVariablePropertyAttribute(Type type,string name=""){

			this.VariableType = type;
			Name = name;
		}
	}

}
