using System.Collections.Generic;

public class ControladorDelChat : IControlDeLoQueSeMuestraEnChat
{
    public ControladorDelChat(int maxDeLineasDelChat)
    {
        this.maximoDeLineasDelChat = maxDeLineasDelChat;
        this.manejadorDeArchivo = new ManejadorDeArchivosLogs();
    }
    Queue<string> colaDelChat = new Queue<string>();
    private int maximoDeLineasDelChat;
    private IManejadorDeArchivosLogs manejadorDeArchivo;

    public string AgregarComandoCola(string comando,string quien)
    {
        //podemos guardar el comando formateado
        manejadorDeArchivo.GuardarLog(comando, quien);
        colaDelChat.Enqueue($"{comando.ToUpper()} -> {quien}");
        if(colaDelChat.Count >= maximoDeLineasDelChat)
        {
            colaDelChat.Dequeue();
        }
        string result = "";
        foreach(string line in colaDelChat)
        {
            result += $"{line}\n";
        }
        return result;
    }
}