﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CountType = System.Int32;
using System.Linq.Expressions;
using System.Collections;

namespace CSharpUtils.Containers.RedBlackTree
{
	public partial class RedBlackTreeWithStats<TElement>
	{
		public class Range : IEnumerable<TElement>, ICloneable/*, IOrderedQueryable<TElement>*/
		{
			internal RedBlackTreeWithStats<TElement> ParentTree;
			internal Node RangeStartNode;
			internal Node RangeEndNode;
			internal CountType RangeStartPosition;
			internal CountType RangeEndPosition;
		
			public CountType GetItemPosition(CountType LocalIndex) {
				if (this.RangeStartPosition == -1) {
					return ParentTree.GetNodePosition(RangeStartNode) + LocalIndex;
				}
				return this.RangeStartPosition + LocalIndex;
			}
		
			internal Range(RedBlackTreeWithStats<TElement> ParentTree, Node RangeStartNode, Node RangeEndNode, CountType RangeStartPosition = -1, CountType RangeEndPosition = -1) {
				this.ParentTree = ParentTree;
				if (RangeStartNode == null) RangeStartNode = ParentTree.LocateNodeAtPosition(RangeStartPosition);
				if (RangeEndNode == null) RangeEndNode = ParentTree.LocateNodeAtPosition(RangeEndPosition);
				if (RangeStartNode == null || RangeEndNode == null) {
					RangeStartNode = RangeEndNode = ParentTree.BaseRootNode;
					RangeStartPosition = -1;
					RangeEndPosition = -1;
				}
				this.RangeStartNode = RangeStartNode;
				this.RangeEndNode = RangeEndNode;
				this.RangeStartPosition = RangeStartPosition;
				this.RangeEndPosition = RangeEndPosition;
			}
		
			public Range Clone() {
				return new Range(ParentTree, RangeStartNode, RangeEndNode, RangeStartPosition, RangeEndPosition);
			}
		
			public Range Limit(CountType limitCount) {
				Assert(limitCount >= 0);
			
				if (RangeStartPosition != -1 && RangeEndPosition != -1) {
					if (RangeStartPosition + limitCount > RangeEndPosition) {
						limitCount = RangeEndPosition - RangeStartPosition; 
					}
				} else {
					// Unsecure.
				}
				return LimitUnchecked(limitCount);
			}
		
			public Range LimitUnchecked(CountType limitCount) {
				Assert(limitCount >= 0);

				return new Range(
					ParentTree,
					RangeStartNode, null,
					RangeStartPosition, GetItemPosition(limitCount)
				);
			}
		
			public Range Skip(CountType skipCount)
			{
				return SkipUnchecked(skipCount);
			}

			public Range Take(CountType skipCount)
			{
				return Limit(skipCount);
			}
		
			public Range SkipUnchecked(CountType skipCount)
			{
				return new Range(
					ParentTree,
					null, RangeEndNode,
					GetItemPosition(skipCount), RangeEndPosition
				);
			}

			bool IsEmpty
			{
				get {
					return RangeStartNode == RangeEndNode;
				}
			}

			public CountType Count
			{
				get
				{
					//writefln("Begin: %d:%s", countLesser(_begin), *_begin);
					//writefln("End: %d:%s", countLesser(_end), *_end);
					//return _begin

					if (RangeStartPosition != -1 && RangeEndPosition != -1)
					{
						return RangeEndPosition - RangeStartPosition;
					}

					return ParentTree.CountLesserThanNode(RangeEndNode) - ParentTree.CountLesserThanNode(RangeStartNode);
				}
			}

			public Range Slice()
			{
				return this.Clone();
			}

			public Range Slice(CountType start, CountType end)
			{
				return new Range(
					ParentTree,
					null,
					null,
					GetItemPosition(start),
					GetItemPosition(end)
				);
			}

			public Range Slice(CountType start)
			{
				return new Range(
					ParentTree,
					null,
					null,
					GetItemPosition(start),
					Count
				);
			}

			public Node this[CountType Index]
			{
				get
				{
					return ParentTree.LocateNodeAtPosition(GetItemPosition(Index));
				}
			}

			TElement FrontElement
			{
				get
				{
					return RangeStartNode.Value;
				}
			}

			TElement BackElement
			{
				get
				{
					return RangeEndNode.PreviousNode.Value;
				}
			}

			IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
			{
				for (var CurrentNode = RangeStartNode; CurrentNode != RangeEndNode; CurrentNode = CurrentNode.NextNode)
				{
					yield return CurrentNode.Value;
				}
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				for (Node CurrentNode = RangeStartNode; CurrentNode != RangeEndNode; CurrentNode = CurrentNode.NextNode)
				{
					yield return CurrentNode.Value;
				}
			}

			public bool Contains(TElement Item)
			{
				if (IsEmpty) return false;
				if (ParentTree.Comparer.Compare(Item, RangeStartNode.Value) < 0) return false;
				if (RangeEndNode != ParentTree.BaseRootNode && ParentTree.Comparer.Compare(Item, RangeEndNode.Value) >= 0) return false;
				return ParentTree.Contains(Item);
			}

			object ICloneable.Clone()
			{
				return new Range(ParentTree, RangeStartNode, RangeEndNode, RangeStartPosition, RangeEndPosition);
			}

			/*
			public System.Type ElementType { get { return typeof(TElement); } }
			public Expression Expression { get { return Expression.Constant(this); } }
			public IQueryProvider Provider { get { return new RedBlackTreeWithStatsQueryProvider<TElement>(this); } }
			*/
		}
	}
}
