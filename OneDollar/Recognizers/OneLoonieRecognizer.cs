using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class OneLoonieRecognizer : IGestureRecognizer
{
    //TODO: Should come from a settings configuration rather than a 
    private const float PHI = 0.61803f;
    private const float SEARCH_DELTA = Mathf.Deg2Rad * 2;
    private const float SECTION_SIZE = Mathf.Deg2Rad * 45; //reducing this should increase performance

    private const float BOUNDING_BOX_SIZE = 100f;
    private const int NUMBER_OF_POINTS = 32; //The number of points to resample to.
    
    #region IGestureRecognizer methods
    
    public GestureTemplate Recognize(Gesture g, List<GestureTemplate> candidates, out float score)
    {
        var normalizedGesture = Normalize(g);

        var best_distance = 0f;
        var best_gesture = new GestureTemplate("No Gesture");
        score = 0f;
        
        for (var i = 0; i < candidates.Count; i++)
        {
            var examples = candidates[i].Examples;

            for (var j = 0; j < examples.Count; j++)
            {
                var distance = DistanceAtBestAngle(normalizedGesture, examples[j], -SECTION_SIZE, SECTION_SIZE, SEARCH_DELTA);
                
                if (i==0 && j==0)
                {
                    best_distance = distance;
                    best_gesture = candidates[i];
                    continue;
                }

                if (!(distance < best_distance)) continue;
                
                best_distance = distance;
                best_gesture = candidates[i];
                score = 1 - (best_distance / (0.5f * Mathf.Sqrt(Mathf.Pow(2f*BOUNDING_BOX_SIZE, 2))));
            }
        }
        
        return best_gesture;
    }
    
    public GestureTemplate Recognize(Gesture g, List<GestureTemplate> candidates)
    {
        return Recognize(g, candidates, out _);
    }

    public Gesture Normalize(Gesture g)
    {
        return g.Resample(NUMBER_OF_POINTS).Rotate(-g.IndicativeAngle).Scale(BOUNDING_BOX_SIZE).Translate(Vector2.zero);
    }
    #endregion


    /// <summary>
    /// Uses GSS (golden section search) to hill-climb for the best possible angle to score against.
    /// </summary>
    /// <param name="gesture">The gesture to test</param>
    /// <param name="template">The gesture to test against</param>
    /// <param name="theta_start">The start of the sweep. Bigger sweep=slower</param>
    /// <param name="theta_end">The end of the sweep. Bigger sweep=slower</param>
    /// <param name="d_theta">The starting section size.</param>
    /// <returns>The best possible distance as a float..</returns>
    private float DistanceAtBestAngle(Gesture gesture, Gesture template, float theta_start, float theta_end, float d_theta)
    {
        //this is a golden section search to get a "good enough" value for minimum distance.

        //Define the initial section to search
        var theta_a = theta_start;
        var theta_b = theta_end;
        
        //Define the new section endpoints
        var x1 = PHI * theta_a + (1 - PHI) * theta_b;
        var x2 = PHI * theta_b + (1 - PHI) * theta_a;
        
        //Find the distances between each section endpoints
        var f1 = DistanceAtAngle(gesture, template, x1);
        var f2 = DistanceAtAngle(gesture, template, x2);

        while (Mathf.Abs(theta_b - theta_a) > d_theta)
        {
            if (f1 > f2)
            {
                theta_b = x2; //values greater than x2 can be ignored
                x2 = x1;
                f2 = f1;
                x1 = PHI * theta_a + (1 - PHI) * theta_b;
                f1 = DistanceAtAngle(gesture, template, x1);
            }
            else
            {
                theta_a = x1; //values less than x1 can be ignored
                x1 = x2;
                f1 = f2;
                x2 = PHI * theta_b + (1 - PHI) * theta_a;
                f2 = DistanceAtAngle(gesture, template, x2);
            }
        }

        return Mathf.Min(f1, f2); //Once the interval is reduced to 2 degrees, return the smallest distance
    }
    
    /// <summary>
    /// Find the path distance at the given difference between indicative angles.
    /// </summary>
    /// <param name="test">The gesture to be rotated</param>
    /// <param name="target">The target gesture to test against</param>
    /// <param name="theta">The angle to test at.</param>
    /// <returns></returns>
    float DistanceAtAngle(Gesture test, Gesture target, float theta)
    {
        return PathDistance(test.Rotate(theta), target);
    }
    
    /// <summary>
    /// Find the path-distance between two gestures.
    /// </summary>
    /// <returns></returns>
    private float PathDistance(Gesture A, Gesture B)
    {
        var d = 0f;
        
        for (var i = 0; i < A.Count; i++)
        {
            d += Vector2.Distance(A[i], B[i]);
        }

        return d / A.Count;
    }
}
