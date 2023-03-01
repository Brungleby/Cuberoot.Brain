
/** BrainNode.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Include

using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Cuberoot.Brain
{
	#region Enumerations

	public enum NodeState
	{
		/// <summary>
		/// Indicates that the node has not started running.
		///</summary>

		Waiting,

		/// <summary>
		/// Indicates that the node is currently running.
		///</summary>

		Running,

		/// <summary>
		/// Indicates that the node is finished running and was successful.
		///</summary>

		Success,

		/// <summary>
		/// Indicates that the node is finished running and failed.
		///</summary>

		Failure,
	}

	#endregion

	#region BrainNode

	/// <summary>
	/// A BrainNode is any connection made within a <see cref="BrainTree"/>.
	///</summary>
	[System.Serializable]
	public abstract class BrainNode
	{
		public BrainNode(BrainTree _owner, string _name)
		{
			Owner = _owner;
			Name = _name;
		}
		public BrainNode(BrainTree _owner)
		{
			Owner = _owner;
			Name = string.Empty;
		}

		public BrainTree Owner;

		[SerializeField]
		public string Name;

		public abstract BrainNode Next { get; set; }

		public virtual NodeState state => NodeState.Waiting;

		private (bool, int) ValidateHierarchy(HashSet<BrainNode> nodes, int depth)
		{

			if (Next == null)
				return (true, depth);
			if (nodes.Contains(this))
				return (false, depth);

			nodes.Add(this);
			(bool, int) __result = Next.ValidateHierarchy(nodes, depth + 1);

			if (!__result.Item1 && __result.Item2 == (depth + 1))
			{
				string __msg = $"\"{Name}\" implementing \"{Next.Name}\" as Next creates an infinite behaviour branch. This is not allowed.";

				Next = null;

				throw new BrainTreeLoopException(__msg);
			}

			return __result;
		}

		public bool ValidateHierarchy()
		{
			HashSet<BrainNode> __start = new HashSet<BrainNode>();
			// start.Add(this);

			return ValidateHierarchy(__start, 0).Item1;
		}

		public override string ToString() => $"{GetType()}: {Name}";

		public override bool Equals(object other) => base.Equals(other);

		public override int GetHashCode() => base.GetHashCode();
	}

	#endregion

	/// <summary>
	/// A kind of <see cref="BrainNode"/> that contains exactly one <see cref="Next"/> node, which is processed immediately after this one. This base class does not execute any code.
	///</summary>
	[System.Serializable]
	public class BrainCell : BrainNode
	{
		#region Constructors

		public BrainCell(BrainTree _owner, string _name, BrainNode _next) : base(_owner, _name)
		{
			_Next = _next;
		}
		public BrainCell(BrainTree _owner, BrainNode _next) : base(_owner)
		{
			_Next = _next;
		}
		public BrainCell(BrainTree _owner, string _name) : base(_owner, _name)
		{
			_Next = null;
		}
		public BrainCell(BrainTree _owner) : base(_owner)
		{
			_Next = null;
		}

		#endregion

		[SerializeField]

		private BrainNode _Next;
		public sealed override BrainNode Next
		{
			get => _Next;
			set => _Next = value;
		}
	}

	/// <summary>
	/// This is a <see cref="BrainCell"/> that continues to process its code until it is finished (passed, failed, or externally aborted).
	///</summary>
	[System.Serializable]
	public class BrainProcess : BrainCell
	{
		public BrainProcess(BrainTree _owner) : base(_owner) { }

		public BrainProcessScript Script;

		private bool _isRunning;
		public bool isRunning => _state == NodeState.Running;

		private NodeState _state = NodeState.Waiting;

		public void Update(float deltaTime)
		{
			if (isRunning)
			{
				if (Script.Update(Owner.Data).HasValue)
				{
					if (_state == NodeState.Success)
						Complete(true);
					else if (_state == NodeState.Failure)
						Complete(false);
				}
			}
		}

		protected virtual void OnExecute() { }
		protected virtual void OnSuccess() { }
		protected virtual void OnFailure() { }

		public sealed override NodeState state => _state;

		public void Abort()
		{
			_state = NodeState.Failure;

			OnFailure();

			OnFinishExecution();
		}

		public void Reset()
		{
			_state = NodeState.Waiting;
		}

		protected void Execute()
		{
			_state = NodeState.Running;

			OnExecute();
		}

		protected void Complete(bool success)
		{
			_state = success ? NodeState.Success : NodeState.Failure;

			if (success)
				OnSuccess();
			else
				OnFailure();

			OnFinishExecution();
		}

		protected virtual void OnFinishExecution() { }
	}

	/// <summary>
	/// This is a simple BrainCell that does not update and whose <see cref="GetResult"/> is guaranteed to return Success or Failure. See: <see cref="OnCheckCondition"/>.
	///</summary>
	public class BrainCondition : BrainCell
	{
		public BrainCondition(BrainTree _owner) : base(_owner) { }

		public BrainConditionScript Script;

		public bool InterruptsChildren = false;

		public sealed override NodeState state => Script.Update(Owner.Data) ? Next.state : NodeState.Failure;
	}

	#region BrainSynapse

	#region BrainSynapseBase

	/// <summary>
	/// A Synapse is a node that does not execute any code but instead is a branching point for decision making. This is a multi-to-multi decision branch.
	///</summary>
	public abstract class BrainSynapseBase : BrainNode
	{
		#region Constructors

		public BrainSynapseBase(BrainTree _owner, string _name, BrainNode[] _nodeQueue) : base(_owner, _name)
		{
			NodeQueue = new List<BrainNode>();
			foreach (var node in _nodeQueue)
				NodeQueue.Add(node);
		}
		public BrainSynapseBase(BrainTree _owner, BrainNode[] _nodeQueue) : base(_owner)
		{
			NodeQueue = new List<BrainNode>();
			foreach (var node in _nodeQueue)
				NodeQueue.Add(node);
		}
		public BrainSynapseBase(BrainTree _owner, string _name) : base(_owner, _name)
		{
			NodeQueue = new List<BrainNode>();
		}
		public BrainSynapseBase(BrainTree _owner) : base(_owner)
		{
			NodeQueue = new List<BrainNode>();
		}

		#endregion

		// [HideInInspector]
		public List<BrainNode> NodeQueue;

		private int _activeIndex;

		public sealed override BrainNode Next
		{
			get => NodeQueue[_activeIndex];
			set
			{
				for (int i = 0; i < NodeQueue.Count; i++)
				{
					if (value == NodeQueue[i])
					{
						_activeIndex = i;
						return;
					}
				}

				throw new System.Exception($"\"{value.Name}\" is not a member of \"{Name}\"'s {GetType().ToString()}.NodeQueue.");
			}
		}
	}

	#endregion
	#region BrainSelector

	/// <summary>
	/// A Selector is a Synapse that Passes if ANY of its outbound Dendrites Pass. If one Dendrite succeeds, the Selector is aborted and Passes.
	///</summary>
	public sealed class BrainSelector : BrainSynapseBase
	{
		public BrainSelector(BrainTree _owner) : base(_owner)
		{
			NodeQueue = new List<BrainNode>();
		}

		public sealed override NodeState state
		{
			get
			{
				foreach (BrainNode node in NodeQueue)
				{
					if (node.state == NodeState.Failure)
						continue;
					return node.state;
				}

				return NodeState.Failure;
			}
		}
	}

	#endregion
	#region BrainSequence

	/// <summary>
	/// A Sequence is a Synapse that Passes if ALL of its outbound Dendrites Pass. If one Dendrite fails, the Sequence is aborted and fails.
	///</summary>
	public sealed class BrainSequence : BrainSynapseBase
	{
		public BrainSequence(BrainTree _owner) : base(_owner)
		{
			NodeQueue = new List<BrainNode>();
		}

		public sealed override NodeState state
		{
			get
			{
				foreach (BrainNode node in NodeQueue)
				{
					if (node.state == NodeState.Success)
						continue;
					return node.state;
				}

				return NodeState.Failure;
			}
		}
	}

	#endregion

	#endregion
}