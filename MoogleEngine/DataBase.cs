namespace MoogleEngine;
//Clase para almacenar la Base de Datos del Moogle
class DataBase
{
    //Propiedad estática que almacena una Lista de objetos de tipo Archivo y que representa los archivos de la Base de Datos 
    static List<Archivo> Archivos = new List<Archivo>();
    //Propiedad de tipo Diccionarios que almacena todas las palabras contenidas en los archivos
    static Diccinarios A = new Diccinarios();
    //Método que se invoca para cargar los elementos de la Base de Datos
    public static void Load()
    {
        //Definimos la ubicación desde donde está siendo ejecutado el programa
        string ubicacion = Directory.GetCurrentDirectory();
        //Remplazamos el string MoogleServer por Content para obtener la ruta donde se almacena la BD(Base de Datos)
        string path = ubicacion.Replace("MoogleServer","Content");
        //Obtenemos los directorios de todos los archivos, excluyendo a los que no sean de tipo .txt
        string[] Directorios = Directory.GetFiles(path,"*.txt");
        //Usando los directorios agregamos los objetos de tipo Archivo a la Lista "Archivos" 
        for(int i=0;i<Directorios.Length;i++)
        {
            Archivos.Add(new Archivo(Directorios[i],A));
        }
        //Empleamos este método para asignarle el IDF a cada palabra
        A.IDF(Archivos.Count);
    }

    internal static List<Archivo> GetArchivos()
    {
        return Archivos;
    }

    internal static Diccinarios GetDiccionarios()
    {
        return A;
    }
}
// Creo una Clase de objetos Tipo Archivo que representan cada documento de la Base de Datos
class Archivo
{
    //Constante de caracteres separadores
    private static char[] separador= {',','.','!','?',';',' ',':','-','*','[',']','`','“','”','_','(',')','¿','¡','/','+','#','%'};
    //Propiedad que almacena el texto normalizado del Doc
    public string Texto;
    //Array de Todas las Palabras del documento
    public string[] Palabras;
    //Diccionario que almacena Todas las Palabras y la cantidad de veces que aparecen
    public Dictionary<string,int> Diccionario = new Dictionary<string, int>();
    //Nombre del Doc
    public string Nombre;
    //Score que tendrá con respecto a la Query
    public double score;
    //Constructor que recibe un directorio y un objeto clase Diccionario donde se irán almacenando todas las palabras de los Doc  
    public Archivo(string Directorio , Diccinarios A)
    {
        //Se extrae del directorio el nombre del Documento
        Nombre = Directorio.Substring(Directorio.LastIndexOf("Content")+8,Directorio.Length-(Directorio.LastIndexOf("Content")+12));
        //Se lee y normaliza el Texto del Doc
        Texto=File.ReadAllText(Directorio).ToLower() + Nombre.ToLower();
        //Se divide el Texto por Palbras 
        Palabras = Texto.Split(separador,StringSplitOptions.RemoveEmptyEntries);
        //Se crea un Diccionario con todas las palabras y su TF
        foreach(string palabra in Palabras)
        {
            //Si la palabra ya está se le suma su ocurrencia al TF
            if(Diccionario.ContainsKey(palabra))
            {
                Diccionario[palabra]++;
            }
            //Si no está se agrega al Diccionario con ocurrencia 1
            else
            {
                Diccionario.Add(palabra,1);
                //Método para almacenar todas las Palabras de todos los Doc
                A.Incluir(palabra);
            }
            
        }
    }
    //Método para hallar el score de cada archivo empleando un Diccionario con el IDF y la query 
    public void CrearScore(Diccinarios A, Query consulta)
    {
        //inicializo una variable que representa la sumatoria de  los Productos Escalares de los vectores de la query y el archivo
        double ProductoEscalar=0;
        //Variable que almacenará la sumatoria de los pesos al cuadrado de cada palabra del documento
        double SumatoriaDocumento=0;
        //Variable que almacenará la sumatoria de los pesos al cuadrado de cada palabra de la query
        //Se recorre el Diccionario del documento 
        foreach(KeyValuePair<string,int> palabra in Diccionario)
        {
            //Si las palabras de la consulta y la query coinciden se halla el producto escalar(si no coinciden qiuere decir que el TF es 0 y no aporta nada a la sumatoria)
            if(consulta.Diccionario.ContainsKey(palabra.Key))
            {
                ProductoEscalar = ProductoEscalar + (palabra.Value*A.AllWordsIDF[palabra.Key]*consulta.Diccionario[palabra.Key]*A.AllWordsIDF[palabra.Key]);
            }
            //Se calcula la sumatoria de los pesos cuadrados de cada palabra
            SumatoriaDocumento = SumatoriaDocumento + Math.Pow(palabra.Value*A.AllWordsIDF[palabra.Key],2);
            
        }
        //Se calcula el score
        score = ProductoEscalar/Math.Sqrt(SumatoriaDocumento*consulta.SumatoriaQuery);
        foreach(KeyValuePair<string , int[]> palabra in consulta.Modificador)
        {
            if(Diccionario.ContainsKey(palabra.Key))
            {
                score = score*palabra.Value[0];
            }
            else
            {
                score = score * palabra.Value[1];
            }
        }
    }
    //Método que extrae un snippet con la primera ocurrencia de una palabra
    public string CrearSnippet(Dictionary<string,int> palabras)
    {
        string snippet = "";
        foreach(KeyValuePair<string,int> word in palabras)
        {
            //Condicionales para evitar pasarse de los limites del Texto
            if(Diccionario.ContainsKey(word.Key))
            {
                int x =Texto.IndexOf(word.Key);
                if(x+50>Texto.Length&&x-50>0)
                {
                    snippet = snippet + Texto.Substring(x-50,Texto.Length-x)+"...";
                }
                if(x+50<Texto.Length&&x-50<0)
                {
                    snippet = snippet + Texto.Substring(0,x+50)+"...";
                }
                if(x+50>Texto.Length&&x-50<0)
                {
                    snippet = Texto;
                }
                if(x+50<Texto.Length&&x-50>0)
                {
                    snippet= snippet + Texto.Substring(x-50,100)+"...";
                }
            }
        }
        return snippet;
    }
}

//Objeto Tipo Diccionarios
class Diccinarios
{
    //Propiedad tipo Dictionary que almacena todas las Palabras con un entero que representa la cantidad de documentos en que aparece
    public Dictionary<string,int> AllWords;
    //Propiedad tipo Dictionary que almacena cada palabra con su IDF
    public Dictionary<string,double> AllWordsIDF;
    //Constructor que crea un nuevo Diccionario vacío
    public Diccinarios()
    {
        AllWords = new Dictionary<string, int>();
        AllWordsIDF = new Dictionary<string, double>();
    }
    //Método a través del cual se van agregando las palabras al Diccionario
    public void Incluir(string palabra)
    {
        if(AllWords.ContainsKey(palabra))
        {
            AllWords[palabra]++;
        }
        else
        {
            AllWords.Add(palabra,1);
        }
    }
    //Método para calcular el IDF de cada palabra y incluirlo en AllWordsIDF
    public void IDF(int CantidadArchivos)
    {
        foreach(KeyValuePair<string,int> palabra in AllWords)
        {
            AllWordsIDF.Add(palabra.Key,Math.Log10(CantidadArchivos/palabra.Value));
        }
    }
}