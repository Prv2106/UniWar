using System.Text.RegularExpressions;

public static class StringExtensions {
    /*
        vogliamo implementare metodi che rimuovono e aggiungono gli spazi ai nomi dei territori:
        es: 
            AmericaDelSud --> America Del Sud
            America Del Sud --> AmericaDelSud
    */

    
	public static string AddSpaces(this string text) {
        // SINTASSI extension: Il primo parametro del metodo deve avere 
        // il modificatore this seguito dal tipo che si intende estendere! 
        return Regex.Replace(text, "(?<!^)([A-Z])", " $1");

        /*
            il Replace method della classe Regex, il quale vuole tre parametri:
            - stringa di input
            - pattern --> (?<!^)([A-Z]) significa che il gruppo di cattura è l'insieme delle lettere
                                        maiuscole (quelle tra parentesi quadre), mentre la prima tonda
                                        impedisce l'inserimento dello spazio all'inizio del testo
            - replacement --> " $1" aggiungiamo uno spazio prima della lettera maiuscola "catturata"
        */

        // N.B: le stringhe sono immutabili, dobbiamo restituirne una per forza
	}

    public static string RemoveSpaces(this string text) {
        string[] words = text.Split(' '); // lo spazio è il separatore
        string newText = string.Join("", words);
        return newText;
    }
}