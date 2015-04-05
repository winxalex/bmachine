using UnityEngine;
using System.Collections;
using BehaviourMachine;
using System.Collections.Generic;
using System.Linq;
using System;
using ws.winx.unity;
using ws.winx.unity.attributes;

namespace ws.winx.bmachine
{

		/// <summary>
		/// Wrapper class for the InternalBlackboard component.
		/// <summary>
		public class BlackboardCustom:InternalBlackboard,ISerializationCallbackReceiver
		{


				[HideInInspector]
				public List<Type>
						typesCustom;
				[HideInInspector]
				public byte[]
						typesSerialized;
				[HideInInspector]
				//Comment for debug
				public List<UnityVariable>
						variablesList;

				private bool _deserialized=false;

				public void AddVariable (UnityVariable prop)
				{
					
						variablesList.Add (prop);

				}

				public List<UnityVariable> GetVariableBy (Type type)
				{
							//deserialization might not be ready
							//if (variablesList == null || (variablesList.Count >0 && variablesList[0]==null)
								if(!_deserialized)
								return new List<UnityVariable> ();

							return variablesList.Where ((item) => item.ValueType==type || item.ValueType.IsSubclassOf(type)).ToList ();

				}

				void Reset ()
				{
						variablesList = new List<UnityVariable> ();
						typesCustom = new List<Type> ();
			
				}
		
		#region ISerializationCallbackReceiver implementation
				void ISerializationCallbackReceiver.OnBeforeSerialize ()
				{
						if (typesCustom != null) {
								typesSerialized = Utility.Serialize (typesCustom);
						}
				}

				void ISerializationCallbackReceiver.OnAfterDeserialize ()
				{
						if (typesSerialized != null && typesSerialized.Length > 0) {
								typesCustom = (List<Type>)Utility.Deserialize (typesSerialized);
						}


						_deserialized = true;
				}
		#endregion
		}






}

