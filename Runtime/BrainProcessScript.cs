
/** BrainProcessScript.cs
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
	/// This is a script that a <see cref="BrainProcess"/> employs when its functionality is executed.
	///</summary>

	public abstract class BrainProcessScript : ScriptableObject
	{
		/// <summary>
		/// This function is called each time a child Process node is running.
		///</summary>
		/// <returns>
		/// TRUE if the process finished successfully, FALSE if the process failed/aborted, NULL if it is still running.
		///</returns>
		public abstract bool? Update(BrainData data);

		public virtual void OnBeginExecute(BrainData data) { }
		public virtual void OnSuccessCompletion(BrainData data) { }
		public virtual void OnFailureCompletion(BrainData data) { }
		public virtual void OnInterrupted(BrainData data) { }

		// #if UNITY_EDITOR

		// 		[MenuItem("Assets/Create/Brain/New Process")]
		// 		private static void CreateTemplateScript()
		// 		{
		// 			Cuberoot.Editor.EditorExtensions.CreateNewScriptFromTemplate("Templates/BrainProcessScriptTemplate", "NewProcessScript");
		// 		}

		// #endif

	}
}
