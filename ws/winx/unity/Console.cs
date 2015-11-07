using UnityEngine;
using System.Collections;
using System;
using Mono.CSharp;


namespace ws.winx.unity{



/// <summary>
/// !!! find Mono.CSharp.dll and move into Pluggins folder
/// include in References in Project too
/// Console. write C# Unity Code and execute
/// </summary>
public class Console : MonoBehaviour
{
	public KeyCode[] m_ShortcutKeys = new KeyCode[]{KeyCode.LeftAlt, KeyCode.F12}; //the keys used to open Console;
	public bool m_IsConsoleOpen = false;
	
	private string m_editstr = "";
	private string m_result = "";
	
	private int m_cmdId = 0; //used to identify cmd
	
	// Use this for initialization
	void Start () {
		Mono.CSharp.Evaluator.Init(new string[] { });
		foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			//Dbg.Log("refer: {0}", assembly.FullName);
			if( assembly.FullName.Contains("Cecil") || assembly.FullName.Contains("UnityEditor") )
				continue;
			Mono.CSharp.Evaluator.ReferenceAssembly(assembly);
		}
		Evaluator.Run ("using UnityEngine;\n" + 
		               "using System;"
		               );
	}
	
	void Update() {
		//check if the console should be opened
		if (m_IsConsoleOpen)
			return;
		
		bool bAllKeysDown = true;
		foreach ( KeyCode kc in m_ShortcutKeys )
		{
			if( !Input.GetKey(kc) )
			{
				bAllKeysDown = false;
				break;
			}
		}
		
		if( bAllKeysDown )
		{
			m_IsConsoleOpen = true;
		}
	}
	
	void OnGUI()
	{
		if (!m_IsConsoleOpen)
			return;
		
		m_editstr = GUI.TextArea(new Rect(10, 10, 400, 100), m_editstr);
		
		if( GUI.Button(new Rect(420, 60, 100, 40), "Execute") )
		{
			++m_cmdId;
			bool bSuccess = Run(m_editstr);
			m_result = string.Format("{0}: {1}", m_cmdId, bSuccess ? "OK" : "Fail");
			m_editstr = ""; //clear the script
		}
		
		if( GUI.Button(new Rect(530, 60, 100, 40), "Close") )
		{
			m_IsConsoleOpen = false;
		}
		
		if (m_result.Length > 0)
		{
			GUI.TextArea(new Rect(420, 10, 200, 30), m_result);
		}
	}
	
	
	public bool Run(string cmd) {
		return Evaluator.Run(cmd);
	}
	
}
}