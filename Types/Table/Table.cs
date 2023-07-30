#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace DamnLibrary.Types
{
	public class Table2D<T>
	{
		private T DefaultValue { get; }
		
		private readonly TableItem<T>[,] table;

		public T this[Vector2Int position]
		{
			get => GetValue(position);
			set
			{
				if (DefaultValue.Equals(value))
					RemoveValue(position);
				else
					SetValue(position, value);
			}
		}

		public Table2D(int width, int height, T defaultValue = default)
		{
			table = new TableItem<T>[width, height];
			DefaultValue = defaultValue;
		}

		public T GetValue(Vector2Int position)
		{
			var item = table[position.x, position.y];
			return item == null ? DefaultValue : item.Value;
		}

		public void SetValue(Vector2Int position, T value)
		{
			var item = table[position.x, position.y];
			if (item != null)
				item.Value = value;
			else
				table[position.x, position.y] = new TableItem<T>(position, value);
		}

		public void RemoveValue(Vector2Int position)
		{
			var item = table[position.x, position.y];
			if (item == null)
				return;

			item.Position = new Vector2Int(-0xfffffff, -0xfffffff);
			item.Value = DefaultValue;

			table[position.x, position.y] = null;
		}

		public bool HasValue(Vector2Int position)
		{
			var item = table[position.x, position.y];
			if (item != null)
				return !DefaultValue.Equals(item.Value);

			return false;
		}
	}
}
#endif