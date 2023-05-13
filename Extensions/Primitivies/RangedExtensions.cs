using DamnLibrary.Types;

namespace DamnLibrary.Extensions
{
	public static class RangedExtensions
	{
		public static T GetValue<T>(this IRanged<T> ranged, int index) => index == 0 
			? ranged.MinimalValue 
			: ranged.MaximalValue; 
	}
}