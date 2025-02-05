public static class ListExtensions {
	
    // algoritmo di Fisher-Yates 
	public static void Shuffle<T>(this List<T> list) {
        // SINTASSI: Il primo parametro del metodo deve avere 
        // il modificatore this seguito dal tipo che si intende estendere! 

        Random gen = new Random();
        // generatore di numeri casuali:
        // invocando il metodo Next(), si ottengono numeri pseudo-casuali
		
		int n = list.Count();
        while (n > 1) {
            int k = gen.Next(maxValue: n);
            n--;
            T tmp = list[k];
            list[k] = list[n];
            list[n] = tmp;
        }
	}
}