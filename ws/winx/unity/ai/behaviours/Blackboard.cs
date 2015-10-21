using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using ws.winx.unity;
using ws.winx.unity.attributes;
using ws.winx.unity.utilities;

namespace ws.winx.unity.ai.behaviours
{

		
		public class Blackboard:MonoBehaviour,ISerializationCallbackReceiver
		{


				//[HideInInspector]
				public List<Type>
						typesCustom;
				//[HideInInspector]
				public byte[]
						typesSerialized;
				//[HideInInspector]
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
								typesSerialized = SerializationUtility.Serialize (typesCustom);
						}
				}

				void ISerializationCallbackReceiver.OnAfterDeserialize ()
				{
						if (typesSerialized != null && typesSerialized.Length > 0) {
								typesCustom = (List<Type>)SerializationUtility.Deserialize (typesSerialized);
						}


						_deserialized = true;


		
				}
		#endregion
		}






}

