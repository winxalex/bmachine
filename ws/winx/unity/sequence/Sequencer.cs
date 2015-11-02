using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using System;

namespace ws.winx.unity.sequence
{
		public class Sequencer : MonoBehaviour
		{
					public void Play (double time)
					{
						if (sequence != null)
						sequence.Play (time);
					}
				
				public float timeCurrent;
				public ws.winx.unity.sequence.SequenceEvent
						eventSelected;

				public event UnityAction<SequenceNode> SequenceNodeStart {
						add {
								//sequence.channels.ForEach (chn => chn.nodes.ForEach (nd => nd.onStart.AddListener(value)));
					
								foreach (SequenceChannel channel in sequence.channels)
										foreach (SequenceNode node in channel.nodes)
												node.onStart.AddListener (value);
						}
						remove {
								foreach (SequenceChannel channel in sequence.channels)
										foreach (SequenceNode node in channel.nodes)
												node.onStart.RemoveListener (value);
						}
				}
				//
				public event UnityAction<SequenceNode> SequenceNodeStop {
						add {
								foreach (SequenceChannel channel in sequence.channels)
										foreach (SequenceNode node in channel.nodes)
												node.onStop.AddListener (value);
				
				
						}
						remove {
								foreach (SequenceChannel channel in sequence.channels)
										foreach (SequenceNode node in channel.nodes)
												node.onStop.RemoveListener (value);
						}
				}

			

				

				[NonSerialized]
				public SequenceNode
						nodeSelected;
				[NonSerialized]
				public SequenceChannel
						channelSelected;

				public Sequence.SequenceWrap wrap =Sequence.SequenceWrap.ClampForever;
				public bool playOnStart = true;
				public int frameRate = 30;
				public Vector2 scale;
				public Sequence sequence;
				int _eventCurrentIndex;

				
				void Update(){

						if (sequence != null && Application.isPlaying && sequence.isPlaying)
								sequence.UpdateSequence (Time.time);


				}


				void LateUpdate(){
					if (sequence != null && Application.isPlaying)
						sequence.LateUpdate ();

				}

				
	}
				
}