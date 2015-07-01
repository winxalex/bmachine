using UnityEngine;
using System.Collections;

namespace VisualTween.Action.UnityRenderer{
	[RequireComponent (typeof (Renderer))]
	[System.Serializable]
	[Category("Renderer")]
	public class TweenColor : TweenAction {
		public Color from=Color.white;
		public Color to=Color.black;
		public string property="_Color";
		
		private Renderer cachedRenderer;
		
		public override void OnEnter (GameObject target)
		{
			cachedRenderer = target.GetComponent<Renderer>();
		}
		
		public override void OnUpdate (GameObject target,float percentage)
		{
			if (this.cachedRenderer)
			{
				Color value = GetValue(from,to,percentage);
				if (this.cachedRenderer is SpriteRenderer)
				{
					((SpriteRenderer)this.cachedRenderer).color = value;
				}else{
					cachedRenderer.sharedMaterial.SetColor(property, value);
				}
			}
		}

		private Color recColor;
		public override void RecordAction (GameObject target)
		{
			if (target.GetComponent<Renderer>() is SpriteRenderer)
			{
				recColor=((SpriteRenderer)target.GetComponent<Renderer>()).color;
			}else{
				recColor=target.GetComponent<Renderer>().sharedMaterial.GetColor( property);
			}
		}
		
		public override void UndoAction (GameObject target)
		{
			if (target.GetComponent<Renderer>() is SpriteRenderer)
			{
				((SpriteRenderer)target.GetComponent<Renderer>()).color = recColor;
			}else{
				target.GetComponent<Renderer>().sharedMaterial.SetColor(property, recColor);
			}
		}
	}
}