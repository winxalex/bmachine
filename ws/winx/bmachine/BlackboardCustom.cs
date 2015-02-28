using UnityEngine;
using System.Collections;
using BehaviourMachine;
using System.Collections.Generic;
using System.Linq;
using System;
using ws.winx.unity;

namespace ws.winx.bmachine
{

		/// <summary>
		/// Wrapper class for the InternalBlackboard component.
		/// <summary>
		public class BlackboardCustom:InternalBlackboard,ISerializationCallbackReceiver
		{

		[HideInInspector]
		public List<Type> typesCustom;

		[HideInInspector]
		public byte[] typesSerialized;

	//	[HideInInspector]//Uncoment for debug
		public List<UnityVariable> variablesList;

		string _typeNameSelected="Select custom type";


			public void AddProperty(UnityVariable prop){
					
					variablesList.Add (prop);

			}

			public List<UnityVariable> GetPropertyBy(Type type){
					return variablesList.Where ((item) => item.ValueType == type).ToList();

			}

		#region ISerializationCallbackReceiver implementation
		void ISerializationCallbackReceiver.OnBeforeSerialize ()
		{
			if (typesCustom != null) {
				typesSerialized=Utility.Serialize(typesCustom);
			}
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize ()
		{
			if (typesSerialized != null && typesSerialized.Length > 0) {
				typesCustom=(List<Type>)Utility.Deserialize(typesSerialized);
			}
		}
		#endregion
		}

}

