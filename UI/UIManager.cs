using System;
using System.Collections.Generic;
using FreeDraw;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    [SerializeField] private Button recognize_button, add_as_example_button, add_as_new_gesture_button, save_gesture_button;
    [SerializeField] private TMP_Dropdown gesturesDropdown;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Drawable drawingCanvas;
    [SerializeField] private TMP_InputField gestureNameInput;

    private Gesture currentDrawing => new Gesture(drawingCanvas.currentDrawing);

    //Change out the recognizer here.
    private IGestureRecognizer recognizer = new OneLoonieRecognizer();
    
    //TODO: Extract loaded templates to their own object
    private List<GestureTemplate> templates;

    // Start is called before the first frame update
    void Start()
    {
        templates = GestureTemplateLoader.TemplateLoader.templates;
        
        messageText.text = " ";
        
        recognize_button.onClick.AddListener(() => Recognize(currentDrawing));
        
        //Add current drawing as example to current template
        add_as_example_button.onClick.AddListener(() => TryAddExample(currentDrawing));
        
        //Add current drawing as new gesture
        add_as_new_gesture_button.onClick.AddListener(() => TryAddNew(currentDrawing));
        
        gesturesDropdown.ClearOptions(); //remove the default ABC options, too lazy to do in inspector
        templates.ForEach(t => gesturesDropdown.options.Add(new TMP_Dropdown.OptionData() {text = t.Name}));
        
        save_gesture_button.onClick.AddListener(()=>GestureTemplateLoader.TemplateLoader.SaveTemplates());

    }
    
    void Recognize(Gesture g)
    {
        var closestTemplate = recognizer.Recognize(g, templates);
        PrintToUIConsole(closestTemplate.Name);
    }

    void TryAddExample(Gesture g)
    {
        var templateIndex = templates.FindIndex(
           n => n.Name == gesturesDropdown.options[gesturesDropdown.value].text);

        try
        {
            templates[templateIndex].Examples.Add(recognizer.Normalize(g));
            PrintToUIConsole("Added example to: " + templates[templateIndex].Name);
        }
        catch (ArgumentOutOfRangeException)
        {
            PrintToUIConsole("Gesture not found. There's a mismatch between the UI and dropdown collection.");
        }
    }

    void TryAddNew(Gesture g)
    {
        var requested_name = gestureNameInput.text;

        if (templates.Exists(n => n.Name == requested_name))
        {
            PrintToUIConsole("That gesture already exists.");
            return;
        }
        
        var new_template = new GestureTemplate(requested_name);
        new_template.Examples.Add(recognizer.Normalize(g));
        
        templates.Add(new_template);
        gesturesDropdown.options.Add(new TMP_Dropdown.OptionData() {text = new_template.Name});

        PrintToUIConsole("added new template: " + new_template.Name);
    }

    void PrintToUIConsole(string s)
    {
        messageText.text = s;
    }
}
