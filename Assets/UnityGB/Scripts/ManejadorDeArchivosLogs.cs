using System;
using System.IO;

public class ManejadorDeArchivosLogs : IManejadorDeArchivosLogs
{
    private string _path = @"log.csv";
    public ManejadorDeArchivosLogs()
    {

    }
    public ManejadorDeArchivosLogs(string path)
    {
        _path = path;
    }
    public void GuardarLog(string comando, string quien)
    {
        string formateado = $"{comando};{quien};{DateTime.Now.ToString("yyyyMMddHHmmss")}";

        using (StreamWriter sw = new StreamWriter(_path, true))
        {
            sw.WriteLine(formateado);
            sw.Close();
        }
        
    }
}