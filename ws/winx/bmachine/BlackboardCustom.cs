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
		public class BlackboardCustom:InternalBlackboard
		{

			public List<UnityVariable> variablesList;


			public void AddProperty(UnityVariable prop){
					
					variablesList.Add (prop);

			}

			public List<UnityVariable> GetPropertyBy(Type type){
					return variablesList.Where ((item) => item.ValueType == type).ToList();

			}

		}

}

