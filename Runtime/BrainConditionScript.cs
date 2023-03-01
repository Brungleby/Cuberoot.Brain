
/** BrainConditionScript.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.

*/

#region Includes

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot.Brain
{
	/// <summary>
	/// This is a script that a <see cref="BrainCondition"/> employs to determine whether or not its child processes should continue running.
	///</summary>

	public abstract class BrainConditionScript : ScriptableObject
	{
		/// <summary>
		/// This function is called each time a child Process node is running.
		///</summary>
		/// <returns>
		/// TRUE to continue running child process(es). FALSE to stop all child processes.
		///</returns>
		public abstract bool Update(BrainData data);

		// #if UNITY_EDITOR

		// 		[MenuItem("Assets/Create/Brain/New Condition")]
		// 		private static void CreateTemplateScript()
		// 		{
		// 			Cuberoot.Editor.EditorExtensions.CreateNewScriptFromTemplate("Templates/BrainConditionScriptTemplate", "NewConditionScript");
		// 		}

		// #endif
	}
}
