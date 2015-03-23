using System;
using System.Reflection;
using System.Collections.Generic;
using ws.winx.csharp.extensions;
using UnityEngine;

namespace ws.winx.unity.attributes
{
	public class QuaternionPropertyAttribute : PropertyAttribute
	{



		public string Name;

		public QuaternionPropertyAttribute(string name=""){


			Name = name;
		}
	}

}
