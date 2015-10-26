using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Linq;
using System;


namespace ws.winx.unity.surrogates{

	public class GameObjectSurrogate : ISerializationSurrogate
	{
		GameObject[] gameObjects;

		public GameObjectSurrogate(){
			Debug.Log ("GameObjectSurrogate Constructor:"+System.Threading.Thread.CurrentThread.ManagedThreadId);
			gameObjects = GameObject.FindObjectsOfType<GameObject> ();
		}


		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var go = (GameObject)obj;

			Debug.Log ("GetObjectData:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
			//Debug.Log("GetObjectData:"+go.name+" ID:"+go.GetInstanceID());
			info.AddValue("ID", go.GetInstanceID());



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
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{

//			if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
//				return null;
			Debug.Log ("SetObjectData:" + System.Threading.Thread.CurrentThread.ManagedThreadId);

			int ID = (int)info.GetValue ("ID", typeof(int));


		//	Debug.Log ("SetObjectData " + System.Threading.Thread.CurrentThread.ManagedThreadId + "ID:" + ID);
			return gameObjects.FirstOrDefault (itm => itm.GetInstanceID () == ID);
			//return null;




		}
	}
}

