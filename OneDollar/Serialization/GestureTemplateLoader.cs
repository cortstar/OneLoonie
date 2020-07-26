using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// Singleton that loads and saves templates for use in the program.
/// </summary>
public class GestureTemplateLoader
{
    //TODO: should be injected/come from a settings file etc.
    private string filepath = Application.dataPath + "/data/gestures.txt";
    
    public static GestureTemplateLoader TemplateLoader = new GestureTemplateLoader();
    
    public readonly List<GestureTemplate> templates = new List<GestureTemplate>();
        
    private GestureTemplateLoader()
    {
        var data = File.ReadAllText(filepath, Encoding.UTF8);
        var loadedTemplates = JsonConvert.DeserializeObject<List<GestureTemplate>>(data);
        templates = loadedTemplates ?? new List<GestureTemplate>();
        
        templates.ForEach(t => Debug.LogFormat("Loaded gesture: {0} with {1} examples.", t.Name, t.Examples.Count));
    }
    
    /// <summary>
    /// Save all current templates.
    /// </summary>
    public void SaveTemplates()
    {
        var data = JsonConvert.SerializeObject(templates);
        File.WriteAllText(filepath, data);
    }
    
}
