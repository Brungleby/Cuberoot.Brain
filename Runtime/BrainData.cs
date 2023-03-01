
/** BrainData.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot.Brain
{
	/// <summary>
	/// Also known as a blackboard, this contains a set of variables for the owning BehaviourTree to use within its scripts.
	///</summary>
	public class BrainData : ScriptableObject
	{

		// #if UNITY_EDITOR

		// 		[MenuItem("Assets/Create/Brain/New Data")]
		// 		private static void CreateNewScriptFile()
		// 		{
		// 			TextAsset textFile = Resources.Load<TextAsset>("Templates/BrainDataTemplate");

		// 			UnityEngine.Debug.Log("\"NewBrainData.cs\" created. Please refocus the editor to load it.");

		// 			System.IO.File.WriteAllText($"{Cuberoot.Editor.EditorExtensions.ActiveFolderPath}/NewBrainData.cs", textFile.text);
		// 		}

		// #endif

	}
}
