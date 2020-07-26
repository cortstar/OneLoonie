using System.Collections.Generic;

/// <summary>
/// A GestureTemplate is a gesture made up of a set of examples of gestures.
/// The examples are tested in 
/// </summary>
public class GestureTemplate
{
    public readonly List<Gesture> Examples = new List<Gesture>();
    public string Name { get; private set; }

    public GestureTemplate(string Name)
    {
        this.Name = Name;
    }
}
