using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Linq;
using System;

using System.Threading;
using ws.winx.unity.utilities;
using System.Reflection;

namespace ws.winx.unity.surrogates
{
	public class UnityObjectSurrogate : ISerializationSurrogate
	{
		static UnityEngine.Object[] objectsUnity;

	

		public static bool CheckForMainThread ()
		{

			return Thread.CurrentThread.ManagedThreadId == 1;
//			if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA &&
//			    !Thread.CurrentThread.IsBackground && !Thread.CurrentThread.IsThreadPoolThread && Thread.CurrentThread.IsAlive &&
//			    Thread.CurrentThread.ManagedThreadId!=1
//			    )
//			{
////				MethodInfo correctEntryMethod = Assembly.GetEntryAssembly().EntryPoint;
////				StackTrace trace = new StackTrace();
////				StackFrame[] frames = trace.GetFrames();
////				for (int i = frames.Length - 1; i >= 0; i--)
////				{
////					MethodBase method = frames[i].GetMethod();
////					if (correctEntryMethod == method)
////					{
////						return true;
////					}
////				}
//
//				return true;
//
//
//			}
//
//			return false;
			
		
		}
		
		public UnityObjectSurrogate ()
		{
			UnityEngine.Debug.Log ("UnityObjectSurrogate Constructor:" + System.Threading.Thread.CurrentThread.ManagedThreadId);

			//!!! When open Unity Constructor is not called on main thread

			if (CheckForMainThread ())
				objectsUnity = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object> ();
		}

		public void GetObjectData (object obj, SerializationInfo info, StreamingContext context)
		{
			var objUnity = (UnityEngine.Object)obj;



			UnityEngine.Debug.Log ("GetObjectData:" + System.Threading.Thread.CurrentThread.ManagedThreadId+" ID:"+objUnity.GetInstanceID());
			//Debug.Log("GetObjectData:"+go.name+" ID:"+go.GetInstanceID());
			info.AddValue ("ID", objUnity.GetInstanceID ());



		}



		/// <summary>
		/// Sets the object data.
		/// 
		/// !!! Not working cos Main Thread restriction
		/// </summary>
		/// <returns>The object data.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		/// <param name="selector">Selector.</param>
		public object SetObjectData (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{


			UnityEngine.Debug.Log ("SetObjectData:" + System.Threading.Thread.CurrentThread.ManagedThreadId);

			int ID = (int)info.GetValue ("ID", typeof(int));


				Debug.Log ("SetObjectData " + System.Threading.Thread.CurrentThread.ManagedThreadId + "ID:" + ID);

			if (CheckForMainThread ()) {
				Debug.Log ("SetObjectData ReinitTry"); 
				objectsUnity = UnityEngine.Object.FindObjectsOfType (typeof(UnityEngine.Object));
			}

			//MethodInfo inf = typeof(UnityEngine.Object).GetMethod ("FindObjectsOfType", BindingFlags.Static | BindingFlags.Public, null, new Type[]{typeof(Type)}, null);

			//UnityEngine.Object[] objx=inf.Invoke(null,new object[]{typeof(UnityEngine.Object)}) as UnityEngine.Object[];	
			
			if (objectsUnity != null)
				return objectsUnity.FirstOrDefault (itm => itm.GetInstanceID () == ID);

			return null;




		}
	}
}

