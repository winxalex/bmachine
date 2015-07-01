using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;

namespace VisualTween.Action.Generic{
	[System.Serializable]
	[Category("Generic")]
	public class TweenProperty : TweenAction {
		[HideInInspector]
		public float fromFloat=0.0f;
		[HideInInspector]
		public float toFloat=1.0f;
		[HideInInspector]
		public Color fromColor=Color.white;
		[HideInInspector]
		public Color toColor=Color.black;
		[HideInInspector]
		public Vector4 fromVector4;
		[HideInInspector]
		public Vector4 toVector4;
		[HideInInspector]
		public Vector3 fromVector3;
		[HideInInspector]
		public Vector3 toVector3;
		[HideInInspector]
		public Vector2 fromVector2;
		[HideInInspector]
		public Vector2 toVector2;

		[HideInInspector]
		public string componentTypeString;
		public string propertyName;

		private Type type;
		private Component component;
		private FieldInfo fieldInfo;
		private PropertyInfo propertyInfo;
		public override void OnEnter (GameObject target)
		{
			type= TweenProperty.GetType (componentTypeString);
			if (type == null) {
				type=TweenProperty.GetType("UnityEngine."+componentTypeString);			
			}
			if (type != null) {
			 	component = target.GetComponent (type);		
				if(component != null){
					 fieldInfo = type.GetField (propertyName);
					if (fieldInfo == null) {
						propertyInfo = type.GetProperty (propertyName);
					}
				}
			}
		}
		
		public override void OnUpdate (GameObject target,float percentage)
		{
			if (this.component) {
				if (fieldInfo != null) {
					if (fieldInfo.FieldType == typeof(float)) {
						fieldInfo.SetValue (component, GetValue (fromFloat, toFloat, percentage));
					} else if (fieldInfo.FieldType == typeof(Color)) {
						fieldInfo.SetValue (component, GetValue (fromColor, toColor, percentage));
					} else if (fieldInfo.FieldType == typeof(Vector4)) {
						fieldInfo.SetValue (component, GetValue (fromVector4, toVector4, percentage));
					} else if (fieldInfo.FieldType == typeof(Vector3)) {
						fieldInfo.SetValue (component, GetValue (fromVector3, toVector3, percentage));
					} else if (fieldInfo.FieldType == typeof(Vector2)) {
						fieldInfo.SetValue (component, GetValue (fromVector2, toVector2, percentage));
					} else if (fieldInfo.FieldType == typeof(Quaternion)) {
						fieldInfo.SetValue (component, Quaternion.Euler (GetValue (fromVector3, toVector3, percentage)));
					} 
				} 
				
				if (propertyInfo != null) {
					if (propertyInfo.PropertyType == typeof(float)) {
						propertyInfo.SetValue (component, GetValue (fromFloat, toFloat, percentage), null);
					} else if (propertyInfo.PropertyType == typeof(Color)) {
						propertyInfo.SetValue (component, GetValue (fromColor, toColor, percentage), null);
					} else if (propertyInfo.PropertyType == typeof(Vector4)) {
						propertyInfo.SetValue (component, GetValue (fromVector4, toVector4, percentage), null);
					} else if (propertyInfo.PropertyType == typeof(Vector3)) {
						propertyInfo.SetValue (component, GetValue (fromVector3, toVector3, percentage), null);
					} else if (propertyInfo.PropertyType == typeof(Vector2)) {
						propertyInfo.SetValue (component, GetValue (fromVector2, toVector2, percentage), null);
					} else if (propertyInfo.PropertyType == typeof(Quaternion)) {
						propertyInfo.SetValue (component, Quaternion.Euler (GetValue (fromVector3, toVector3, percentage)), null);
					} 
				}
			}
		}

		private float recFloat;
		private Color recColor;
		private Vector4 recVector4;
		private Vector3 recVector3;
		private Vector2 recVector2;
		public override void RecordAction (GameObject target)
		{
			//OnEnter (target);
			if (this.component) {
				if (fieldInfo != null) {
					if (fieldInfo.FieldType == typeof(float)) {
						recFloat=(float)fieldInfo.GetValue (component);
					} else if (fieldInfo.FieldType == typeof(Color)) {
						recColor=(Color)fieldInfo.GetValue (component);
					} else if (fieldInfo.FieldType == typeof(Vector4)) {
						recVector4=(Vector4)fieldInfo.GetValue (component);
					} else if (fieldInfo.FieldType == typeof(Vector3)) {
						recVector3=(Vector3)fieldInfo.GetValue (component);
					} else if (fieldInfo.FieldType == typeof(Vector2)) {
						recVector2=(Vector2)fieldInfo.GetValue (component);
					} else if (fieldInfo.FieldType == typeof(Quaternion)) {
						recVector3=((Quaternion)fieldInfo.GetValue (component)).eulerAngles;
					} 
				} 
				
				if (propertyInfo != null) {
					if (propertyInfo.PropertyType == typeof(float)) {
						recFloat=(float)propertyInfo.GetValue (component, null);
					} else if (propertyInfo.PropertyType == typeof(Color)) {
						recColor= (Color)propertyInfo.GetValue (component,null);
					} else if (propertyInfo.PropertyType == typeof(Vector4)) {
						recVector4= (Vector4)propertyInfo.GetValue (component,null);
					} else if (propertyInfo.PropertyType == typeof(Vector3)) {
						recVector3= (Vector3)propertyInfo.GetValue (component,null);
					} else if (propertyInfo.PropertyType == typeof(Vector2)) {
						recVector2= (Vector2)propertyInfo.GetValue (component,null);
					} else if (propertyInfo.PropertyType == typeof(Quaternion)) {
						recVector3=((Quaternion) propertyInfo.GetValue (component,null)).eulerAngles;
					} 
				}
			}
		}
		
		public override void UndoAction (GameObject target)
		{
			//OnEnter (target);
			if (this.component) {
				if (fieldInfo != null) {
					if (fieldInfo.FieldType == typeof(float)) {
						fieldInfo.SetValue (component, recFloat);
					} else if (fieldInfo.FieldType == typeof(Color)) {
						fieldInfo.SetValue (component, recColor);
					} else if (fieldInfo.FieldType == typeof(Vector4)) {
						fieldInfo.SetValue (component, recVector4);
					} else if (fieldInfo.FieldType == typeof(Vector3)) {
						fieldInfo.SetValue (component, recVector3);
					} else if (fieldInfo.FieldType == typeof(Vector2)) {
						fieldInfo.SetValue (component, recVector2);
					} else if (fieldInfo.FieldType == typeof(Quaternion)) {
						fieldInfo.SetValue (component, Quaternion.Euler (recVector3));
					} 
				} 
				
				if (propertyInfo != null) {
					if (propertyInfo.PropertyType == typeof(float)) {
						propertyInfo.SetValue (component, recFloat, null);
					} else if (propertyInfo.PropertyType == typeof(Color)) {
						propertyInfo.SetValue (component, recColor, null);
					} else if (propertyInfo.PropertyType == typeof(Vector4)) {
						propertyInfo.SetValue (component, recVector4, null);
					} else if (propertyInfo.PropertyType == typeof(Vector3)) {
						propertyInfo.SetValue (component, recVector3, null);
					} else if (propertyInfo.PropertyType == typeof(Vector2)) {
						propertyInfo.SetValue (component, recVector2, null);
					} else if (propertyInfo.PropertyType == typeof(Quaternion)) {
						propertyInfo.SetValue (component, Quaternion.Euler (recVector3), null);
					} 
				}
			}
		}

		public static Type GetType( string TypeName )
		{
			if (string.IsNullOrEmpty (TypeName)) {
				return null;			
			}
			// Try Type.GetType() first. This will work with types defined
			// by the Mono runtime, in the same assembly as the caller, etc.
			var type = Type.GetType( TypeName );
			
			// If it worked, then we're done here
			if( type != null )
				return type;
			
			type=Type.GetType(TypeName+", UnityEngine");
			
			if( type != null )
				return type;
			
			// If the TypeName is a full name, then we can try loading the defining assembly directly
			if( TypeName.Contains( "." ) )
			{
				
				// Get the name of the assembly (Assumption is that we are using 
				// fully-qualified type names)
				var assemblyName = TypeName.Substring( 0, TypeName.IndexOf( '.' ) );
				
				// Attempt to load the indicated Assembly
				var assembly = AppDomain.CurrentDomain.GetAssemblies().ToList().Find(x=>x.FullName == assemblyName);// Assembly.Load( assemblyName );
				if( assembly == null )
					return null;
				
				// Ask that assembly to return the proper Type
				type = assembly.GetType( TypeName );
				if( type != null )
					return type;
				
			}
			
			// If we still haven't found the proper type, we can enumerate all of the 
			// loaded assemblies and see if any of them define the type
			var currentAssembly = Assembly.GetExecutingAssembly();
			var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
			foreach( var assemblyName in referencedAssemblies )
			{
				
				// Load the referenced assembly
				var assembly = AppDomain.CurrentDomain.GetAssemblies().ToList().Find(x=>x.FullName == assemblyName.FullName);
				if( assembly != null )
				{
					// See if that assembly defines the named type
					type = assembly.GetType( TypeName );
					if( type != null )
						return type;
				}
			}
			// The type just couldn't be found...
			return null;
			
		}
	}
}