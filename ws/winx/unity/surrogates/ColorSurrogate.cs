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

namespace ws.winx.unity.surrogates
{
	public class ColorSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var vector = (Color)obj;
			info.AddValue("x", vector.r);
			info.AddValue("y", vector.g);
			info.AddValue("z", vector.b);
			info.AddValue("w", vector.a);
		}
		
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			
			return new Color((float)info.GetValue("x", typeof(float)), (float)info.GetValue("y", typeof(float)),(float) info.GetValue("z", typeof(float)),(float) info.GetValue("w", typeof(float)));
		}
	}

}

