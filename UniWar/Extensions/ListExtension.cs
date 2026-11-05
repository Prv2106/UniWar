/// <summary>
/// Extension methods for <see cref="List{T}"/>.
/// </summary>
public static class ListExtensions {
	
    /// <summary>
    /// Shuffles the list in place using the Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="list">List to shuffle.</param>
	public static void Shuffle<T>(this List<T> list) {
        // The first parameter uses the ``this`` modifier to declare an
        // extension method on List<T>.

        Random gen = new();
        // Random number generator; Next() returns pseudo-random values.
		
		int n = list.Count;
        while (n > 1) {
            int k = gen.Next(maxValue: n);
            n--;
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}