using UnityEngine;
using System.Collections;
using System;

namespace VisualTween{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CategoryAttribute : Attribute
	{
		public string category;
		public CategoryAttribute(string category){
			this.category = category;
		}
	}
}