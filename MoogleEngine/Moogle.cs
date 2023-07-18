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
        List<Archivo> Documentos = new List<Archivo>(0); 
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
            if(i<Documentos.Count-1)
            {
            items[i] = new SearchItem(Documentos[Documentos.Count-1-i].Nombre,
            Documentos[Documentos.Count-1-i].CrearSnippet(Consulta.Diccionario),
            Convert.ToSingle(Documentos[Documentos.Count-1-i].score));
            }
            else
            {
                items[i] = new SearchItem(".",
                "...",0.9f);
            }
        }
        string suggestion = query;
        if(Documentos.Count<3)
        {
            suggestion = Sugerencia(Consulta,Words);
        }
        
        return new SearchResult(items, suggestion);
    }
    static int LevenstheinDist(string a, string b)
    {
        int[,] matriz = new int[a.Length+1,b.Length+1];
        for(int i=0;i<=a.Length;i++)
        {
            matriz[i,0] = i;
        } 
        for(int j=0;j<=b.Length;j++)
        {
            matriz[0,j] = j;
        }
        for(int i= 1;i<a.Length+1;i++)
        {
            for(int j = 1;j<b.Length+1;j++)
            {
                int costo = (a[i-1]==b[j-1])?0:1;
                matriz[i,j] = Math.Min(Math.Min(matriz[i-1,j]+1,matriz[i,j-1]+1),matriz[i-1,j-1]+costo);
            }
        }
        return matriz[a.Length,b.Length];
    }
    static string Sugerencia(Query consulta, Diccinarios Words)
    {
        int x = 50;
        int y = 0;
        string sugestion = "";
        foreach(KeyValuePair<string, int> query in consulta.Diccionario)
        {
            foreach(KeyValuePair<string,int> Diccionario in Words.AllWords)
            {
                y=LevenstheinDist(query.Key,Diccionario.Key);
                if(y<x)
                {
                    x=y;
                    sugestion = sugestion + Diccionario.Key + " ";
                }
            }
        }
        return sugestion;
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
    private static char[] separador= {',','.','!','?',';',' ',':','-','*','[',']','`','“','”','_','(',')','¿','/','+','#','%'};
    //Diccionario para almacenar todas las palabras de la query junto con su TF
    public Dictionary<string,int> Diccionario = new Dictionary<string, int>();
    public double SumatoriaQuery = 0;
    public Dictionary<string,int[]> Modificador = new Dictionary<string, int[]>();
    //Constructor de la clase
    public Query(string query)
    {
        //Se lleva el texto a minúsculas
        Texto = query.ToLower();
        //Se separan las palabras
        Palabras = Texto.Split(separador,StringSplitOptions.RemoveEmptyEntries);
        for(int i=0;i<Palabras.Length;i++)
        {
            if(Palabras[i][0]=='¡')
            {
                Palabras[i] = Palabras[i].Replace("¡","");
                int[] a = {0,1};
                if(!Modificador.ContainsKey(Palabras[i]))
                {
                    Modificador.Add(Palabras[i],a);    
                }
            }
            if(Palabras[i][0]=='*')
            {
                int[] b = {1,1};
                for(int j=0;j<Palabras[i].Length;j++)
                {
                    if(Palabras[i][j]=='*')
                    {
                        Palabras[i].Replace("*","");
                        b[0] = b[0]*2;
                    }
                    else
                    {
                        break;
                    }
                }
                if(!Modificador.ContainsKey(Palabras[i]))
                {
                    Modificador.Add(Palabras[i],b);
                }
            }
            if(Palabras[i][0]=='^')
            {
                Palabras[i] = Palabras[i].Replace("^","");
                int[] a = {1,0};
                if(!Modificador.ContainsKey(Palabras[i]))
                {
                    Modificador.Add(Palabras[i],a);    
                }
            }
        }
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


