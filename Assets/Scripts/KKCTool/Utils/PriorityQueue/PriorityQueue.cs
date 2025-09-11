// ----------------------------------------------------------------------------
// PriorityQueue class for use in Unity3D.
//
// Based on PriorityQueue implementation found in Red Blob Games' A*
// Implementation compantion:
// http://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
//
// Note: Requires EppyGames "Tuple implementation for Unity3D" by
// Michael Bartnett: https://gist.github.com/michaelbartnett/5652076
//
// Alternative implementations for Tuple usage in Unity3D should also work.
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace ESarkis
{
	/// <summary>
	/// A Queue class in which each item is associated with a Double value
	/// representing the item's priority.
	/// Dequeue and Peek functions return item with the best priority value.
	/// </summary>
	public class PriorityQueue<T>
	{

		List<Tuple<T, int>> elements = new List<Tuple<T, int>>();


		/// <summary>
		/// Return the total number of elements currently in the Queue.
		/// </summary>
		/// <returns>Total number of elements currently in Queue</returns>
		public int Count
		{
			get { return elements.Count; }
		}


		/// <summary>
		/// Add given item to Queue and assign item the given priority value.
		/// </summary>
		/// <param name="item">Item to be added.</param>
		/// <param name="priorityValue">Item priority value as Double.</param>
		public void Enqueue(T item, int priorityValue)
		{
			elements.Add(Tuple.Create(item, priorityValue));
		}


		/// <summary>
		/// Return lowest priority value item and remove item from Queue.
		/// </summary>
		/// <returns>Queue item with lowest priority value.</returns>
		public Tuple<T, int> Dequeue()
		{
			int bestPriorityIndex = 0;

			for (int i = 0; i < elements.Count; i++)
			{
				if (elements[i].Item2 < elements[bestPriorityIndex].Item2)
				{
					bestPriorityIndex = i;
				}
			}

			var bestItem = elements[bestPriorityIndex];
			elements.RemoveAt(bestPriorityIndex);
			return bestItem;
		}


		/// <summary>
		/// Return lowest priority value item without removing item from Queue.
		/// </summary>
		/// <returns>Queue item with lowest priority value.</returns>
		public T Peek()
		{
			int bestPriorityIndex = 0;

			for (int i = 0; i < elements.Count; i++)
			{
				if (elements[i].Item2 < elements[bestPriorityIndex].Item2)
				{
					bestPriorityIndex = i;
				}
			}

			T bestItem = elements[bestPriorityIndex].Item1;
			return bestItem;
		}
	}
}