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
		class BoundsSurrogate : ISerializationSurrogate
		{
				public void GetObjectData (object obj, SerializationInfo info, StreamingContext context)
				{
						var vector = (Bounds)obj;
						info.AddValue ("c", vector.center);
						info.AddValue ("s", vector.size);
					
				}
		
				public object SetObjectData (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
				{
						return new Bounds ((Vector3)info.GetValue ("c", typeof(Vector3)), (Vector3)info.GetValue ("s", typeof(Vector3)));
				}
		}

}

