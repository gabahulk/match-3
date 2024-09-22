using UnityEditor;
using UnityEngine;

namespace Code.Scripts.Match3.Editor
{
    [CustomEditor(typeof(Match3Board))]
    public class Match3BoardEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var match3Board = (Match3Board)target;
            if (GUILayout.Button("Test board"))
            {
                Debug.Log(match3Board.CheckIfBoardHasPossibleMatches());
            }
        }
    }
}