using UnityEngine;
using UnityEditor;

namespace ws.winx.editor.extensions{
	public class EditorApplicationEventDispatcher {

		// A delegate type for hooking up change notifications.
		public delegate void PlayModeChangeEventHandler(Mode oldValue,Mode newValue);

		public static event PlayModeChangeEventHandler PlayModeChanged;

		static Mode __modeCurrent=Mode.Editing;

		public enum Mode:uint{
			Playing,
			Paused,
			Compiling,
			PlayingOrWillChangePlaymod,
			Editing


		}

		static EditorApplicationEventDispatcher(){



			EditorApplication.playmodeStateChanged = () => { 
			
			   if(PlayModeChanged!=null){

					Mode mode=Mode.Editing;			

					if(EditorApplication.isPlaying){
						if(EditorApplication.isPaused)
							mode=Mode.Paused;
						else
							mode=Mode.Playing;
					}else if(EditorApplication.isCompiling){

						mode=Mode.Compiling;

					}else if(EditorApplication.isPlayingOrWillChangePlaymode){

						mode=Mode.PlayingOrWillChangePlaymod;
					}

					Mode modeOld=__modeCurrent;
					__modeCurrent=mode;

					PlayModeChanged(modeOld,mode);

				}
			
			
			
			
			};
		}

	}
}
