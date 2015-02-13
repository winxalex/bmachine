using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

namespace ws.winx.unity{
	[Serializable]
	public class Property<T>:UnityEngine.Object,IEquatable<Property<T>>
	{

		public readonly Type reflectedType;
		public readonly Type type;
		public string name;
		public UnityEngine.Object reflectedObject;
		public readonly MemberInfo memberInfo;


		//
		// Properties
		//
		public  T Value
		{
			get
			{
				if (this.memberInfo == null)
				{
					//this.Initialize ();
				}
				if (this.memberInfo is PropertyInfo)
				{
					return (T)((PropertyInfo)this.memberInfo).GetValue (this.reflectedObject, null);
				}
				if (this.memberInfo is FieldInfo)
				{
					return (T)((FieldInfo)this.memberInfo).GetValue (this.reflectedObject);
				}

				if(this.memberInfo is MethodInfo){

					return (T)((MethodInfo)this.memberInfo).Invoke(this.reflectedObject,new object[]{this.name});
				}


					Debug.LogError (string.Concat (new object[]
					                               {
						"No property with name '",
						this.name,
						"' in the component '",
						this.reflectedObject
						
					}));

				return default(T);
			}
			set
			{
				if (this.memberInfo == null)
				{
					//this.Initialize ();
				}
				if (this.memberInfo is PropertyInfo )
				{
					((PropertyInfo)this.memberInfo).SetValue (this.reflectedObject, value, null);
				}
				else
				{
					if (this.memberInfo is FieldInfo != null)
					{
						((FieldInfo)this.memberInfo).SetValue (this.reflectedObject, value);
					}
					else
					{

							Debug.LogError (string.Concat (new object[]
							                               {
								"No property with name '",
								this.name,
								"' in the component '",
								this.reflectedObject
								
							}));

					}
				}
			}
		}


		//
		// Constructor
		//
		public Property(MemberInfo memberInfo,UnityEngine.Object target){

			if(memberInfo is PropertyInfo || memberInfo is FieldInfo)
			this.name = memberInfo.Name;

			this.reflectedObject = target;

			this.memberInfo = memberInfo;
			this.reflectedType = memberInfo.ReflectedType;



		}


		//
		// Methods
		//




		#region IEquatable implementation
		public bool Equals (Property<T> other)
		{
			if (other == null)
				return false;
			



		


			
			return true;// this.reflectedObject.Equals(other.reflectedObject) && this.reflectedType.Equals(other.reflectedType)
				//&& this.reflectedType.Equals(other.reflectedType);

		}
		#endregion



	
	}
}

