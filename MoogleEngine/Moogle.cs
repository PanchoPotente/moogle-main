namespace MoogleEngine;

public static class Moogle
{
    //Método que se llama para cargar la Base de Datos
    public static void Cargar()
    {
        DataBase.Load();
    }
    public static SearchResult Query(string query) {

        List<Archivo> Files = DataBase.GetArchivos();

        Diccinarios Words = DataBase.GetDiccionarios();
        
        Query Consulta = new Query(query);
        Consulta.CalcularSumatoria(Words);
        List<Archivo> Documentos = new List<Archivo>(); 
        for(int i=0;i<Files.Count;i++)
        {
            Files[i].CrearScore(Words,Consulta);
            if(Files[i].score!=0)
            {
                Documentos.Add(Files[i]);
            }
        }
        Documentos.Sort((o1, o2) => o1.score.CompareTo(o2.score));
        SearchItem[] items = new SearchItem[3];
        for(int i=0;i<items.Length;i++)
        {
            items[i] = new SearchItem(Documentos[Documentos.Count-1-i].Nombre+Documentos[Documentos.Count-1-i].score,
            Documentos[Documentos.Count-1-i].CrearSnippet(Consulta.Diccionario),
            Convert.ToSingle(Documentos[Documentos.Count-1-i].score));

        }
        
        return new SearchResult(items, query);
    }
}


//Clase para almacenar los objetos de tipo query
class Query
{
    //Propiedad que almacena el texto normalizado de la query 
    public string Texto;
    //Array de string con todas las palabras de la query
    public string[] Palabras;
    //Array de caracteres separadores para hacer Split 
    private static char[] separador= {',','.','!','?',';',' ',':','-','*','[',']','`','“','”','_','(',')','¿','¡','/','+','#','%'};
    //Diccionario para almacenar todas las palabras de la query junto con su TF
    public Dictionary<string,int> Diccionario = new Dictionary<string, int>();
    public double SumatoriaQuery = 0;
    //Constructor de la clase
    public Query(string query)
    {
        //Se lleva el texto a minúsculas
        Texto = query.ToLower();
        //Se separan las palabras
        Palabras = Texto.Split(separador,StringSplitOptions.RemoveEmptyEntries);
        //Se crea el Diccionario con las Palabras
        foreach(string palabra in Palabras)
    {
        if(Diccionario.ContainsKey(palabra))
        {
            Diccionario[palabra]++;
        }
        else
        {
            Diccionario.Add(palabra,1);
        }
    }  
    }
    public void CalcularSumatoria(Diccinarios Words)
    {
        foreach(KeyValuePair<string,int> palabra in Diccionario)
        {
            if(Words.AllWordsIDF.ContainsKey(palabra.Key))
            {
                SumatoriaQuery += Math.Pow(Words.AllWordsIDF[palabra.Key]*palabra.Value,2);
            }
            else
            {
                SumatoriaQuery+=Math.Pow(palabra.Value,2);
            }
        }
    }
}


