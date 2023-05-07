using UnityEngine;

namespace DamnLibrary.Other
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