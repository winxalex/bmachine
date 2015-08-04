using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace ws.winx.unity{


	/// <summary>
	/// Unity event extended no args.
	/// </summary>
	[Serializable]
	public abstract class UnityEventEx:UnityEvent {
		public delegate void PersistantCallDelegate();
		
		
		//substitute for m_Calls which is private in UnityEventBase
		private Dictionary<int,UnityAction> m_Calls;
		
		//substitute for m_PersistentCalls which is private in UnityEventBase
		private PersistantCallDelegate[] m_PersistentCallsDelegate;
		
		
		//
		// Constructors
		//
		public UnityEventEx():base(){
			m_PersistentCallsDelegate = new PersistantCallDelegate[this.GetPersistentEventCount()];
			m_Calls=new Dictionary<int, UnityAction>();
			
		}
		
		
		public new void AddListener (UnityAction call)
		{
			
			int id = (call.Target as MonoBehaviour).gameObject.GetInstanceID ();
			m_Calls [id] = call;
			
			
			base.AddListener (call);
		}
		
		
		
		
		public void InvokeOnTarget(GameObject target){
			
			UnityAction call;
			
			int id = target.GetInstanceID ();
			
			//try for runtime added listeners
			if(m_Calls.TryGetValue(id,out call)){
				
				call.Invoke();
			}else{//try persistant(editor) added listeners
				int listenerNumber = this.GetPersistentEventCount();
				
				if(listenerNumber!=m_PersistentCallsDelegate.Length)
					m_PersistentCallsDelegate=new PersistantCallDelegate[listenerNumber];
				
				MonoBehaviour listener;
				
				for (int i=0; i<listenerNumber; i++) {
					listener=this.GetPersistentTarget(i) as MonoBehaviour;
					
					
					if(listener.gameObject==target){
						
						PersistantCallDelegate callDelegate=m_PersistentCallsDelegate[i];
						
						if(callDelegate==null){
							MethodInfo methodInfo=listener.GetType().GetMethod(this.GetPersistentMethodName(i));
							
							//Call thru reflection is slow
							//methodInfo.Invoke(listener,new object[]{damage,null});
							
							m_PersistentCallsDelegate[i]=callDelegate=System.Delegate.CreateDelegate(typeof(PersistantCallDelegate),listener,methodInfo) as PersistantCallDelegate;
							
						}
						
						callDelegate.Invoke();
						
						
					}
				}
				
				
			}
		}
		
		public new void RemoveAllListeners (){
			m_Calls.Clear ();
			
			base.RemoveAllListeners ();
			
		}
		
		public new void RemoveListener (UnityAction call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			if (m_Calls.ContainsKey (id))
				m_Calls.Remove (id);
			
			base.RemoveListener (call);
		}
	}



	/// <summary>
	/// Unity event extended one argument.
	/// </summary>
	[Serializable]
	public abstract class UnityEventEx<T1>:UnityEvent<T1> {
		public delegate void PersistantCallDelegate(T1 target);
		
		
		//substitute for m_Calls which is private in UnityEventBase
		private Dictionary<int,UnityAction<T1>> m_Calls;
		
		//substitute for m_PersistentCalls which is private in UnityEventBase
		private PersistantCallDelegate[] m_PersistentCallsDelegate;
		
		
		//
		// Constructors
		//
		public UnityEventEx():base(){
			m_PersistentCallsDelegate = new PersistantCallDelegate[this.GetPersistentEventCount()];
			m_Calls=new Dictionary<int, UnityAction<T1>>();
			
		}
		
		
		public new void AddListener (UnityAction<T1> call)
		{
			
			int id = (call.Target as MonoBehaviour).gameObject.GetInstanceID ();
			m_Calls [id] = call;
			
			
			base.AddListener (call);
		}
		
		
		
		
		public void InvokeOnTarget(GameObject target,T1 arg0){
			
			UnityAction<T1> call;
			
			int id = target.GetInstanceID ();
			
			//try for runtime added listeners
			if(m_Calls.TryGetValue(id,out call)){
				
				call.Invoke(arg0);
			}else{//try persistant(editor) added listeners
				int listenerNumber = this.GetPersistentEventCount();
				
				if(listenerNumber!=m_PersistentCallsDelegate.Length)
					m_PersistentCallsDelegate=new PersistantCallDelegate[listenerNumber];
				
				MonoBehaviour listener;
				
				for (int i=0; i<listenerNumber; i++) {
					listener=this.GetPersistentTarget(i) as MonoBehaviour;
					
					
					if(listener.gameObject==target){
						
						PersistantCallDelegate callDelegate=m_PersistentCallsDelegate[i];
						
						if(callDelegate==null){
							MethodInfo methodInfo=listener.GetType().GetMethod(this.GetPersistentMethodName(i));
							
							//Call thru reflection is slow
							//methodInfo.Invoke(listener,new object[]{damage,null});
							
							m_PersistentCallsDelegate[i]=callDelegate=System.Delegate.CreateDelegate(typeof(PersistantCallDelegate),listener,methodInfo) as PersistantCallDelegate;
							
						}
						
						callDelegate.Invoke(arg0);
						
						
					}
				}
				
				
			}
		}
		
		public new void RemoveAllListeners (){
			m_Calls.Clear ();
			
			base.RemoveAllListeners ();
			
		}
		
		public new void RemoveListener (UnityAction<T1> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			if (m_Calls.ContainsKey (id))
				m_Calls.Remove (id);
			
			base.RemoveListener (call);
		}
	}


	/// <summary>
	/// Unity event exended 2 args.
	/// </summary>
	[Serializable]
	public abstract class UnityEventEx<T1,T2>:UnityEvent<T1,T2> {
		public delegate void PersistantCallDelegate(T1 arg0,T2 arg1);
		
		
		//substitute for m_Calls which is private in UnityEventBase
		private Dictionary<int,UnityAction<T1,T2>> m_Calls;
		
		//substitute for m_PersistentCalls which is private in UnityEventBase
		private PersistantCallDelegate[] m_PersistentCallsDelegate;
		
		
		//
		// Constructors
		//
		public UnityEventEx():base(){
			m_PersistentCallsDelegate = new PersistantCallDelegate[this.GetPersistentEventCount()];
			m_Calls=new Dictionary<int, UnityAction<T1, T2>>();
			
		}
		
		
		public new void AddListener (UnityAction<T1,T2> call)
		{
			
			int id = (call.Target as MonoBehaviour).gameObject.GetInstanceID ();
			m_Calls [id] = call;
			
			
			base.AddListener (call);
		}
		
		
		
		
		public void InvokeOnTarget(GameObject target,T1 arg0,T2 arg1){
			
			UnityAction<T1,T2> call;
			
			int id = target.GetInstanceID ();
			
			//try for runtime added listeners
			if(m_Calls.TryGetValue(id,out call)){
				
				call.Invoke(arg0,arg1);
			}else{//try persistant(editor) added listeners
				int listenerNumber = this.GetPersistentEventCount();
				
				if(listenerNumber!=m_PersistentCallsDelegate.Length)
					m_PersistentCallsDelegate=new PersistantCallDelegate[listenerNumber];
				
				MonoBehaviour listener;
				
				for (int i=0; i<listenerNumber; i++) {
					listener=this.GetPersistentTarget(i) as MonoBehaviour;
					
					
					if(listener.gameObject==target){
						
						PersistantCallDelegate callDelegate=m_PersistentCallsDelegate[i];
						
						if(callDelegate==null){
							MethodInfo methodInfo=listener.GetType().GetMethod(this.GetPersistentMethodName(i));
							
							//Call thru reflection is slow
							//methodInfo.Invoke(listener,new object[]{damage,null});
							
							m_PersistentCallsDelegate[i]=callDelegate=System.Delegate.CreateDelegate(typeof(PersistantCallDelegate),listener,methodInfo) as PersistantCallDelegate;
							
						}
						
						callDelegate.Invoke(arg0,arg1);
						
						
					}
				}
				
				
			}
		}
		
		public new void RemoveAllListeners (){
			m_Calls.Clear ();
			
			base.RemoveAllListeners ();
			
		}
		
		public new void RemoveListener (UnityAction<T1,T2> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			if (m_Calls.ContainsKey (id))
				m_Calls.Remove (id);
			
			base.RemoveListener (call);
		}
	}


	/// <summary>
	/// Unity event extended 3 args.
	/// </summary>
	[Serializable]
	public abstract class UnityEventEx<T1,T2,T3>:UnityEvent<T1,T2,T3> {
		public delegate void PersistantCallDelegate(T1 arg0,T2 arg1,T3 arg2);
		
		
		//substitute for m_Calls which is private in UnityEventBase
		private Dictionary<int,UnityAction<T1,T2,T3>> m_Calls;
		
		//substitute for m_PersistentCalls which is private in UnityEventBase
		private PersistantCallDelegate[] m_PersistentCallsDelegate;
		
		
		//
		// Constructors
		//
		public UnityEventEx():base(){
			m_PersistentCallsDelegate = new PersistantCallDelegate[this.GetPersistentEventCount()];
			m_Calls=new Dictionary<int, UnityAction<T1, T2, T3>>();
			
		}
		
		
		public new void AddListener (UnityAction<T1,T2,T3> call)
		{
			
			int id = (call.Target as MonoBehaviour).gameObject.GetInstanceID ();
			m_Calls [id] = call;
			
			
			base.AddListener (call);
		}
		
		
		
		
		public void InvokeOnTarget(GameObject target,T1 arg0,T2 arg1,T3 arg2){
			
			UnityAction<T1,T2,T3> call;
			
			int id = target.GetInstanceID ();
			
			//try for runtime added listeners
			if(m_Calls.TryGetValue(id,out call)){
				
				call.Invoke(arg0,arg1,arg2);
			}else{//try persistant(editor) added listeners
				int listenerNumber = this.GetPersistentEventCount();
				
				if(listenerNumber!=m_PersistentCallsDelegate.Length)
					m_PersistentCallsDelegate=new PersistantCallDelegate[listenerNumber];
				
				MonoBehaviour listener;
				
				for (int i=0; i<listenerNumber; i++) {
					listener=this.GetPersistentTarget(i) as MonoBehaviour;
					
					
					if(listener.gameObject==target){
						
						PersistantCallDelegate callDelegate=m_PersistentCallsDelegate[i];
						
						if(callDelegate==null){
							MethodInfo methodInfo=listener.GetType().GetMethod(this.GetPersistentMethodName(i));
							
							//Call thru reflection is slow
							//methodInfo.Invoke(listener,new object[]{damage,null});
							
							m_PersistentCallsDelegate[i]=callDelegate=System.Delegate.CreateDelegate(typeof(PersistantCallDelegate),listener,methodInfo) as PersistantCallDelegate;
							
						}
						
						callDelegate.Invoke(arg0,arg1,arg2);
						
						
					}
				}
				
				
			}
		}
		
		public new void RemoveAllListeners (){
			m_Calls.Clear ();
			
			base.RemoveAllListeners ();
			
		}
		
		public new void RemoveListener (UnityAction<T1,T2,T3> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			if (m_Calls.ContainsKey (id))
				m_Calls.Remove (id);
			
			base.RemoveListener (call);
		}
	}








}

