using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using ws.winx.utility;

namespace ws.winx.unity{

	public class Utility
	{
		public static void ObjectToDisplayOptionsValues<T>(UnityEngine.Object @object,out GUIContent[] displayOptions,out Property<T>[] values){
			displayOptions = null;
			values = null;

		
			if (@object == null) return;

			Type target = null;
			List<GUIContent> guiContentList = new List<GUIContent> ();
			List<Property<T>> memberInfos = new List<Property<T>> ();
			MemberInfo memberInfo;
				
					
			 target = @object.GetType ();
			
			
				string text = "No Property";


				List<string> list = new List<string> ();

				
				
			    
				
				//GET OBJECT STATIC PROPERTIES
				string text4 = target.Name + "/Static Properties/";
				MemberInfo[] publicMembers = ReflectionUtility.GetPublicMembers (target, typeof(T), true, true, true);
				if (publicMembers.Length > 0)
				{
					for (int i = 0; i < publicMembers.Length; i++)
					{
						memberInfo=publicMembers [i];
						
					guiContentList.Add(new GUIContent (text4 + memberInfo.Name));
						memberInfos.Add(new Property<T>(memberInfo,null));                    

					}
				}
				
				//GET OBJECT NON STATIC PROPERTIES
				publicMembers = ReflectionUtility.GetPublicMembers (target, typeof(T), false, true, true);
				for (int j = 0; j < publicMembers.Length; j++)
				{
					memberInfo=publicMembers [j];

				guiContentList.Add(new GUIContent (@object.GetType ().Name + "/" + memberInfo.Name));
					memberInfos.Add(new Property<T>(memberInfo,@object));

				}
				
				//GET COMPONENTS IF GAME OBJECT
				GameObject gameObject = @object as GameObject;
				if (gameObject != null)
				{
					Component currentComponent=null;
					Component[] components = gameObject.GetComponents<Component> ();
					for (int k = 0; k < components.Length; k++)
					{
						currentComponent = components [k];
						Type type = currentComponent.GetType ();
						string uniqueNameInList = StringUtility.GetUniqueNameInList (list, type.Name);
						list.Add (uniqueNameInList);
						
						
						//STATIC PROPERTIES
						text4 = uniqueNameInList + "/Static Properties/";
						publicMembers =ReflectionUtility.GetPublicMembers (currentComponent.GetType (), typeof(T), true, true, true);
						if (publicMembers.Length > 0)
						{
							for (int l = 0; l < publicMembers.Length; l++)
							{
								memberInfo=publicMembers [l];

							guiContentList.Add (new GUIContent (text4 + memberInfo.Name));
								memberInfos.Add (new Property<T>(memberInfo,null));

							}
						}
						
						//NONSTATIC PROPERTIES
						publicMembers = ReflectionUtility.GetPublicMembers (currentComponent.GetType (), typeof(T), false, true, true);
						for (int m = 0; m < publicMembers.Length; m++)
						{
							memberInfo=publicMembers [m];

							guiContentList.Add(new GUIContent (uniqueNameInList + "/" + memberInfo.Name));
							memberInfos.Add(new Property<T>(memberInfo,currentComponent));
						}
					}
				}


			displayOptions=guiContentList.ToArray();
			values=memberInfos.ToArray();
				
				
				

		}//end function

	}
}

