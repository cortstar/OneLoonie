using System.Collections.Generic;

/// <summary>
/// A gesture recognizer which allows for -normalization and serialization of gestures
/// </summary>
public interface IGestureRecognizer
{
    /// <summary>
    /// Returns the gesture which is the closest match for the given gesture.
    /// </summary>
    /// <param name="g">The gesture to recognize</param>
    /// <param name="candidates">The candidate templates to test against</param>
    /// <returns>The gesture in the database which is the closest match.</returns>
    GestureTemplate Recognize(Gesture g, List<GestureTemplate> candidates);
    

    /// <summary>
    /// Normalize the gesture for use with this recognizer.
    /// </summary>
    /// <returns>The normalized gesture.</returns>
    Gesture Normalize(Gesture g);
    
    /// <summary>
    /// Recognize a gesture, returning a template matching the best guess.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="score">The gesture's score.</param>
    /// <param name="candidates">The candidate templates to test against</param>
    /// <returns></returns>
    GestureTemplate Recognize(Gesture g, List<GestureTemplate> candidates, out float score);

}
