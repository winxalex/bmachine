using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using UniRx;
using UniRx.InternalUtil;



using UnityEngine;

namespace ws.winx.unity{

public static class  ObservableExtensionsEx{

	
	
	
	public static IDisposable AddListener<T1,T2,T3,T4> (this IObservable<Tuple<T1,T2,T3,T4,GameObject>> source,UnityAction<T1,T2,T3,T4> call)
	{
		
		return source.Where(trg=>trg.Item5==null || trg.Item5==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,T4,GameObject>> (x => call (x.Item1,x.Item2,x.Item3,x.Item4));
		
		
	}
	
	
}


	
	/// <summary>
	/// Unity observable 4 arguments
	/// </summary>
	[Serializable]
	public abstract class UnityObservable<T1,T2,T3,T4>:UnityEvent<T1,T2,T3,T4>,IObservable<Tuple<T1,T2,T3,T4,GameObject>>
	{
		
		
		
		
		private Dictionary<int,IDisposable> __disposables;
		
		
		private Subject<Tuple<T1,T2,T3,T4,GameObject>> subject;
		private bool persistentTargetsAreSubscribed;
		private CompositeDisposable disposer = new CompositeDisposable ();
		
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<int, IDisposable> ();
			
			subject = new Subject<Tuple<T1,T2,T3,T4,GameObject>> ();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction<T1,T2,T3,T4> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			//convert UnityAction<T1,T2,T3,T4> to Action<T1,T2,T3,T4>
			__disposables [id] = subject.Where(tgt=>tgt.Item5==null || tgt.Item5==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,T4,GameObject>> (x => call (x.Item1,x.Item2,x.Item3,x.Item4)).AddTo (disposer);
		}
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction<T1,T2,T3,T4> call= System.Delegate.CreateDelegate (typeof(UnityAction<T1,T2,T3,T4>), persistentTarget, methodInfo) as UnityAction<T1,T2,T3,T4>;
					subject.Where(trg=>trg.Item5==null || trg.Item5==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,T4,GameObject>> (x => call (x.Item1,x.Item2,x.Item3,x.Item4));
					
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (Tuple.Create (arg1, arg2, arg3, arg4, target));
			}
			
		}
		
		public new void Invoke (T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext (Tuple.Create (arg1, arg2, arg3, arg4,(GameObject)null));
			}
			
		}
		
		public new void RemoveListener (UnityAction<T1,T2,T3,T4> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			if (__disposables.ContainsKey (id))
				disposer.Remove (__disposables [id]);
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			
			disposer.Clear ();
			
		}
		
		
		
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<Tuple<T1,T2,T3,T4,GameObject>> observer)
		{
			
			return subject.Subscribe<Tuple<T1,T2,T3,T4,GameObject>> (observer.OnNext).AddTo (disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}

	/// <summary>
	/// Unity observable 3 arguments
	/// </summary>
	[Serializable]
	public abstract class UnityObservable<T1,T2,T3>:UnityEvent<T1,T2,T3>,IObservable<Tuple<T1,T2,T3,GameObject>>
	{
		
		
		
		
		private Dictionary<int,IDisposable> __disposables;
		
		
		private Subject<Tuple<T1,T2,T3,GameObject>> subject;
		private bool persistentTargetsAreSubscribed;
		private CompositeDisposable disposer = new CompositeDisposable ();
		
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<int, IDisposable> ();
			
			subject = new Subject<Tuple<T1,T2,T3,GameObject>> ();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction<T1,T2,T3> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			//convert UnityAction<T1,T2,T3> to Action<T1,T2,T3>
			__disposables [id] = subject.Where(tgt=>tgt.Item4==null || tgt.Item4==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,GameObject>> (x => call (x.Item1,x.Item2,x.Item3)).AddTo (disposer);
		}
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction<T1,T2,T3> call= System.Delegate.CreateDelegate (typeof(UnityAction<T1,T2,T3>), persistentTarget, methodInfo) as UnityAction<T1,T2,T3>;
					subject.Where(trg=>trg.Item4==null || trg.Item4==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,GameObject>> (x => call (x.Item1,x.Item2,x.Item3));
					
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target, T1 arg1, T2 arg2, T3 arg3)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (Tuple.Create (arg1, arg2, arg3, target));
			}
			
		}
		
		public new void Invoke (T1 arg1, T2 arg2, T3 arg3)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext (Tuple.Create (arg1, arg2, arg3, (GameObject)null));
			}
			
		}
		
		public new void RemoveListener (UnityAction<T1,T2,T3> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			if (__disposables.ContainsKey (id))
				disposer.Remove (__disposables [id]);
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			
			disposer.Clear ();
			
		}
		
		
		
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<Tuple<T1,T2,T3,GameObject>> observer)
		{
			
			return subject.Subscribe<Tuple<T1,T2,T3,GameObject>> (observer.OnNext).AddTo (disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}


	/// <summary>
	/// Unity observable 2 args
	/// </summary>
	[Serializable]
	public abstract class UnityObservable<T1,T2>:UnityEvent<T1,T2>,IObservable<Tuple<T1,T2,GameObject>>
	{
		
		
		
		
		private Dictionary<int,IDisposable> __disposables;
		
		
		private Subject<Tuple<T1,T2,GameObject>> subject;
		private bool persistentTargetsAreSubscribed;
		private CompositeDisposable disposer = new CompositeDisposable ();
		
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<int, IDisposable> ();
			
			subject = new Subject<Tuple<T1,T2,GameObject>> ();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction<T1,T2> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			//convert UnityAction<T1,T2> to Action<T1,T2>
			__disposables [id] = subject.Where(tgt=>tgt.Item3==null || tgt.Item3==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,GameObject>> (x => call (x.Item1,x.Item2)).AddTo (disposer);
		}
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction<T1,T2> call= System.Delegate.CreateDelegate (typeof(UnityAction<T1,T2>), persistentTarget, methodInfo) as UnityAction<T1,T2>;
					subject.Where(trg=>trg.Item3==null || trg.Item3==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,GameObject>> (x => call (x.Item1,x.Item2));
					
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target, T1 arg1, T2 arg2)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (Tuple.Create (arg1, arg2, target));
			}
			
		}
		
		public new void Invoke (T1 arg1, T2 arg2)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext (Tuple.Create (arg1, arg2, (GameObject)null));
			}
			
		}
		
		public new void RemoveListener (UnityAction<T1,T2> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			if (__disposables.ContainsKey (id))
				disposer.Remove (__disposables [id]);
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			
			disposer.Clear ();
			
		}
		
		
		
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<Tuple<T1,T2,GameObject>> observer)
		{
			
			return subject.Subscribe<Tuple<T1,T2,GameObject>> (observer.OnNext).AddTo (disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}
	
	
	/// <summary>
	/// Unity observable 1 argument
	/// </summary>
	[Serializable]
	public abstract class UnityObservable<T1>:UnityEvent<T1>,IObservable<Tuple<T1,GameObject>>
	{
		
		
		
		
		private Dictionary<int,IDisposable> __disposables;
		
		
		private Subject<Tuple<T1,GameObject>> subject;
		private bool persistentTargetsAreSubscribed;
		private CompositeDisposable disposer = new CompositeDisposable ();
		
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<int, IDisposable> ();
			
			subject = new Subject<Tuple<T1,GameObject>> ();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction<T1> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			//convert UnityAction<T1,T2> to Action<T1,T2>
			__disposables [id] = subject.Where(tgt=>tgt.Item2==null || tgt.Item2==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,GameObject>> (x => call (x.Item1)).AddTo (disposer);
		}
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction<T1> call= System.Delegate.CreateDelegate (typeof(UnityAction<T1>), persistentTarget, methodInfo) as UnityAction<T1>;
					subject.Where(trg=>trg.Item2==null || trg.Item2==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,GameObject>> (x => call (x.Item1));
					
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target, T1 arg1)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (Tuple.Create (arg1, target));
			}
			
		}
		
		public new void Invoke (T1 arg1)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext (Tuple.Create (arg1,(GameObject)null));
			}
			
		}
		
		public new void RemoveListener (UnityAction<T1> call)
		{
			int id = (call.Target as UnityEngine.Object).GetInstanceID ();
			
			if (__disposables.ContainsKey (id))
				disposer.Remove (__disposables [id]);
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			
			disposer.Clear ();
			
		}
		
		
		
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<Tuple<T1,GameObject>> observer)
		{
			
			return subject.Subscribe<Tuple<T1,GameObject>> (observer.OnNext).AddTo (disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}
	
	
	
	
	
	
	







}
