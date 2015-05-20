

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ws.winx.editor.extensions{
public class HandlesEx
{
	// internal state for HandlesEx()
	static int HANDLESEX_HASH = "HandlesEx".GetHashCode();
	static Vector2 MOUSE_POS_AT_START;
	static Vector2 MOUSE_POS_CURRENT;
	static Vector3 HANDLE_POS_WORLD;
	//static bool s_DragHandleHasMoved;

	




	
	public static Vector3 DragHandle<K>(Vector3 position, float handleSize, Handles.DrawCapFunction capFunc, Color colorSelected,string label,K userData,
		                                    EditorGUILayoutEx.EventCallback<K> onEvent,GUIContent[] displayOptions=null, IList<K> values=null,EditorGUILayoutEx.MenuCallback<K> onSelection=null)
	{
		int id = GUIUtility.GetControlID(HANDLESEX_HASH, FocusType.Passive);
		


		
		Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
		Matrix4x4 cachedMatrix = Handles.matrix;
		
		
		
		switch (Event.current.GetTypeForControl(id))
		{
			case EventType.KeyDown:
				if(GUIUtility.hotControl==id){
					if(onEvent!=null)
						onEvent(id,Event.current,userData);
				}
			break;


		case EventType.MouseDown:


			if (HandleUtility.nearestControl == id && (Event.current.button == 0 || Event.current.button == 1))
			{
				GUIUtility.hotControl = id;
				MOUSE_POS_CURRENT = MOUSE_POS_AT_START = Event.current.mousePosition;
				HANDLE_POS_WORLD = position;
				//s_DragHandleHasMoved = false;
				
				//Event.current.Use();
				EditorGUIUtility.SetWantsMouseJumping(1);


					if(Event.current.type==EventType.ContextClick && displayOptions!=null && values!=null){

						// Now create the menu, add items and show it
						GenericMenu menu = new GenericMenu ();

						GUIContent content;
						int len = displayOptions.Length;
						
						for (int i=0; i<len; i++) {
							
							content = displayOptions [i];
							
							// null mean AddSeparator
							if (content == null)
								menu.AddSeparator ("");
							// "*" at the end => mean AddDisabledItem
							else if (content.text.LastIndexOf ('*', content.text.Length - 1) > -1) {
								content.text = content.text.Remove (content.text.Length - 1);
								menu.AddDisabledItem (content);
							} else
								menu.AddItem (content, false, (obj) => {
									int inx = (int)obj;


									
									//dispatch selected
									if (onSelection != null)
										onSelection (inx, values [inx], id);
									
								}, i);
						}
						
						
						
						menu.ShowAsContext ();


					}

					if(onEvent!=null)
						onEvent(id,Event.current,userData);
			}
			break;
			
		case EventType.MouseUp:
			if (GUIUtility.hotControl == id && (Event.current.button == 0 || Event.current.button == 1))
			{
				//GUIUtility.hotControl = 0;
				//Event.current.Use();
				EditorGUIUtility.SetWantsMouseJumping(0);
				
					if(onEvent!=null)
						onEvent(id,Event.current,userData);
			}
			break;
			
		case EventType.MouseDrag:
			if (GUIUtility.hotControl == id)
			{
				MOUSE_POS_CURRENT += new Vector2(Event.current.delta.x, -Event.current.delta.y);
				Vector3 position2 = Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(HANDLE_POS_WORLD))
					+ (Vector3)(MOUSE_POS_CURRENT - MOUSE_POS_AT_START);
				position = Handles.matrix.inverse.MultiplyPoint(Camera.current.ScreenToWorldPoint(position2));
				
				if (Camera.current.transform.forward == Vector3.forward || Camera.current.transform.forward == -Vector3.forward)
					position.z = HANDLE_POS_WORLD.z;
				if (Camera.current.transform.forward == Vector3.up || Camera.current.transform.forward == -Vector3.up)
					position.y = HANDLE_POS_WORLD.y;
				if (Camera.current.transform.forward == Vector3.right || Camera.current.transform.forward == -Vector3.right)
					position.x = HANDLE_POS_WORLD.x;
				
					if(onEvent!=null)
						onEvent(id,Event.current,userData);
				
				//s_DragHandleHasMoved = true;
				
				GUI.changed = true;
				//Event.current.Use();
			}
			break;
			
		case EventType.Repaint:
			Color currentColour = Handles.color;
			//if (id == GUIUtility.hotControl && s_DragHandleHasMoved)
			if (id == GUIUtility.hotControl)
				Handles.color = colorSelected;
			
			Handles.matrix = Matrix4x4.identity;
			capFunc(id, screenPosition, Quaternion.identity, handleSize);
			Handles.matrix = cachedMatrix;
			
			Handles.color = currentColour;

			Handles.Label(position+Vector3.up,label);
			break;
			
		case EventType.Layout:
			Handles.matrix = Matrix4x4.identity;

			HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(screenPosition, handleSize));
			Handles.matrix = cachedMatrix;
			break;
		}
		
		return position;
	}
}

}


//
//void OnSceneGui()
//{
//	MyHandles.DragHandleResult dhResult;
//	Vector3 newPosition = MyHandles.DragHandle(position, size, Handles.SphereCap, Color.red, out dhResult);
//	
//	switch (dhResult)
//	{
//	case MyHandles.DragHandleResult.LMBDoubleClick:
//		// do something
//		break;
//	}
//}