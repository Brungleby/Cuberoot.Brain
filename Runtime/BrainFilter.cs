
/** BrainFilter.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.

*/

#region Includes

using UnityEngine;

using Cuberoot.Brain;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// Basic AI component that runs the provided <see cref="Brain"/>.
	///</summary>
	public class BrainFilter : MonoBehaviour
	{
		public BrainTree Brain;
		public bool ExecuteOnAwake = true;

		private void Awake()
		{
			if (ExecuteOnAwake)
				Brain.Execute();
		}

		private void Update()
		{
			Brain.Update();
		}
	}
}