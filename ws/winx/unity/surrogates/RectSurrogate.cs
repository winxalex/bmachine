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
		class RectSurrogate : ISerializationSurrogate
		{
				public void GetObjectData (object obj, SerializationInfo info, StreamingContext context)
				{
						var vector = (Rect)obj;
						info.AddValue ("x", vector.x);
						info.AddValue ("y", vector.y);
						info.AddValue ("z", vector.width);
						info.AddValue ("w", vector.height);
				}
		
				public object SetObjectData (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
				{
			
						return new Rect ((float)info.GetValue ("x", typeof(float)), (float)info.GetValue ("y", typeof(float)), (float)info.GetValue ("z", typeof(float)), (float)info.GetValue ("w", typeof(float)));
				}
		}

}

