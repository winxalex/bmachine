
using System.Collections;
using System;

namespace ws.winx.editor.extensions
{
	public interface IEditorItem<T>:ICloneable where T:IComparable,ICloneable{


		//wrappers of orginal label and value of current object implementing it
		string EditorItemLabel{ get; set; }
		T EditorItemValue{ get; set; }

	}
}