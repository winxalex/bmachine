//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using BehaviourMachine;

using ws.winx.bmachine;
using BehaviourMachineEditor;

namespace ws.winx.editor.bmachine {

    /// <summary>
    /// Wrapper class for BehaviourTreeEditor.
    /// <seealso cref="BehaviourMachine.BehaviourTree" />
    /// </summary>
    [CustomEditor(typeof(BehaviourTreeCustom))]
    public class BehaviourTreeEditor : InternalBehaviourTreeEditor {}
}