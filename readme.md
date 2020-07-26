# OneLoonie

OneLoonie is a gesture recognizer for the Unity game engine based on the [OneDollar algorithim written by the University of Washington and Microsoft](http://faculty.washington.edu/wobbrock/pubs/uist-07.01.pdf). OneLoonie also handles basic saving and loading of gestures via text file via [JSON.NET for Unity.](https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347)

## Installation
1.  Clone the repository into your workspace
2.  In `Serialization/GestureTemplateLoader.cs`, configure the filepath for the gesture save file. *Later, this will be done via config file.*
3.  *Optional* In `Recognizers/OneLoonieRecognizer.cs` , edit the values for "resample size" to whichever fidelity is required.

## Saving Templates
1. Add new templates directly to `GestureTemplateLoader.TemplateLoader.templates` and call `GestureTemplateLoader.TemplateLoader.SaveTemplates()` to store all templates currently loaded. The templates should be normalized before recognition with `IGestureRecognizer.Normalize()` which will "get the gesture ready" for recognition.

## Basic Recognition
1. Templates are automatically loaded at runtime and can be found in `GestureTemplateLoader.TemplateLoader.templates`.
2. Create a new gesture via constructor: `new Gesture()` or `Gesture(IEnumerable<Vector2> points)`.  Gesture implements List, so you can `.Add()` points or even filter if you want.
3. Create an instance of `IGestureRecognizer`. You can either write your own, but if you want to use the built-in $1 recognizer, creat an instance of OneLoonieRecognizer with `new OneLoonieRecognizer()`. Then you can call OneLoonieRecognizer.Recognize(), passing in the gesture you wish to recognize and the list of templates to recognize against (probably TemplateLoader.templates if you want to use what you just loaded).

## Usage notes
1. You don't need to explicitly call `Normalize()` before `Recognize()`, but you want to ensure your templates are `Normalize()`d before you test something against them. OneLoonie doesn't dictate how this should be done; you may want to normalize before you save the gestures or after if you plan to implement and use multiple recognizers. For standard use, `Normalize()` before saving, but don't do it again before using `Recognize()`.

