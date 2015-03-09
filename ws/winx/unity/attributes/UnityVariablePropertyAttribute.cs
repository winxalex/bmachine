using System;
using System.Reflection;
using System.Collections.Generic;
using ws.winx.csharp.extensions;
using UnityEngine;

namespace ws.winx.unity.attributes
{
	public class UnityVariablePropertyAttribute : PropertyAttribute
	{
//		List<UnityVariable> _variablesList;
//
//		string _variableListPropertyName;
//
//		MemberInfo _memberInfo;
//
//		public List<UnityVariable> variablesList {
//			get {
//
//				if(_variablesList==null && serializedObject!=null){
//					_memberInfo=serializedObject.GetType().GetProperty(_variableListPropertyName,BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//
//					if(_memberInfo==null)
//						_memberInfo=serializedObject.GetType().GetField(_variableListPropertyName,BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//
//
//					_variablesList=(List<UnityVariable>)_memberInfo.GetValue(serializedObject);
//				}
//				return _variablesList;
//			}
//		}
//
//		public object serializedObject;

		public readonly Type type;

		public UnityVariablePropertyAttribute(Type type){

			this.type = type;
		}
	}

}
