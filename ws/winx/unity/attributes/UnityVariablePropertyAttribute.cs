using System;
using System.Reflection;
using System.Collections.Generic;
using ws.winx.csharp.extensions;
using UnityEngine;

namespace ws.winx.unity.attributes
{
	public class UnityVariablePropertyAttribute : PropertyAttribute
	{
		[Flags]
		public enum DisplayOptions
		{
			Raw = 0x1,
			Bind = 0x2,
			Locals = 0x4,
			Globals = 0x8
		}

		public Type variableType;
		public string name;
		public DisplayOptions displayOptions;

		public UnityVariablePropertyAttribute(Type type,string name="",DisplayOptions displayOptions=DisplayOptions.Raw | DisplayOptions.Bind | DisplayOptions.Locals | DisplayOptions.Globals ){

			this.variableType = type;
			this.name = name;
			this.displayOptions = displayOptions;
		}
	}

}
