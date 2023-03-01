
/** BrainTree.cs
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

#endregion

namespace Cuberoot.Brain
{
	[CreateAssetMenu(fileName = "New Brain Tree", menuName = "Cuberoot/Brain/Behaviour Tree", order = 50)]
	[System.Serializable]
	public class BrainTree : ScriptableObject
	{
		private LinkedList<BrainProcess> _ActiveProcesses;
		public LinkedList<BrainProcess> ActiveProcesses => _ActiveProcesses;

		[SerializeField]
		public BrainData Data;

		[SerializeField]
		public BrainCell Root;

		protected virtual void OnValidate()
		{
			if (Root == null)
				Root = new BrainCell(this, "Brain Root");

			ValidateHierarchy();
			Refresh();
		}

		private void ValidateHierarchy()
		{
			if (Root.ValidateHierarchy()) return;
			else this.ValidateHierarchy();
		}

		public void Update()
		{
			foreach (var neuron in ActiveProcesses)
				neuron.Update(Time.deltaTime);
		}

		public void Execute()
		{
			Refresh();
		}

		protected virtual void OnRefresh() { }
		public void Refresh()
		{
			_ActiveProcesses = new LinkedList<BrainProcess>();

			BrainNode node = Root;
			while (true)
			{
				if (node == null) break;
				if (node.GetType() == typeof(BrainProcess))
					_ActiveProcesses.AddLast((BrainProcess)node);

				node = node.Next;
			}

			OnRefresh();
		}
	}

	[System.Serializable]
	public class BrainTreeLoopException : System.Exception
	{
		public BrainTreeLoopException() : base() { }
		public BrainTreeLoopException(string message) : base(message) { }
		public BrainTreeLoopException(string message, System.Exception inner) : base(message, inner) { }
		protected BrainTreeLoopException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
