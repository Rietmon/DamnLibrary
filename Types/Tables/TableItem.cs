#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace DamnLibrary.Types.Tables
{
	public class TableItem<T>
	{
		public Vector2Int Position { get; internal set; }

		public T Value { get; internal set; }
		
		public TableItem(Vector2Int position, T value)
		{
			Position = position;
			Value = value;
		}
	}
}
#endif