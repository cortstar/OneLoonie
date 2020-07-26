using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockRecognizer : IGestureRecognizer
{
    
    GestureTemplate mock_template = new GestureTemplate("Mock template");

    /// <summary>
    /// This is just a single line gesture.
    /// </summary>
    private Gesture mock_gesture = new Gesture()
    {
        Vector2.zero,
        Vector2.right
    };

    public MockRecognizer()
    {
        mock_template.Examples.Add(mock_gesture);
    }
    public GestureTemplate Recognize(Gesture g, List<GestureTemplate> candidates)
    {
        return mock_template;
    }

    public Gesture Normalize(Gesture g)
    {
        return mock_gesture;
    }

    public GestureTemplate Recognize(Gesture g, List<GestureTemplate> candidates, out float score)
    {
        score = 0;
        return mock_template;
    }
}
