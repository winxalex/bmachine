using UnityEngine;
using System.Collections;


namespace ws.winx.unity
{

	public class TransfromX:MonoBehaviour//   Transform
		{

		    void Start(){

					Debug.Log("TranformX started..");
				
			}

//			public Quaternion localRotation
//			{
//				get
//				{
//					Quaternion result;
//					base.INTERNAL_get_localRotation (out result);
//					return result;
//				}
//				set
//				{
//					this.INTERNAL_set_localRotation (ref value);
//				}
//			}




		//public Mile oer;


		public UnityVariable var;
public			QuaternionEx _localRotation;

		public Vector3 m;
//			//Transform transform;
//				
//			void Start(){
//
//				//this.transform = this.gameObject.transform;
//
//
//			}
//
//		//[WrapperlessIcall]
//		//[DllImport("UnityEngine.dll")]
//		//[MethodImpl (4096)]
//		//private extern void INTERNAL_get_localRotation (out Quaternion value);
//
//		//private extern void INTERNAL_set_localRotation (ref Quaternion value);
//
////		public Quaternion localRotation
////		{
////			get
////			{
////				Quaternion result;
////				this.INTERNAL_get_localRotation (out result);
////				return result;
////			}
////			set
////			{
////				this.INTERNAL_set_localRotation (ref value);
////			}
////		}
//
//
//		void get_localRotation(out Quaternion q){
//			q = Quaternion.identity;
//
//				}
//
//		void set_localRotation(ref Quaternion q){
//			
//			
//		}
//		
//
//			//Properties 
//			public QuaternionEx localRotation {
//				get {
//
//					if(_localRotation==null){
//						//_localRotation=new QuaternionEx(
//					 
//					}
//					
//
//
//					return _localRotation;
//				}
//				set {
//					_localRotation = value;
//
//						set_localRotation(ref _localRotation.q);
//
//				}
//			}
		}


		
}
