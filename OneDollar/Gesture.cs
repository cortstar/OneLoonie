using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A collection of points representing a drawn gesture.
/// </summary>
[Serializable]
public class Gesture : List<Vector2>
{

    public Gesture(){}
    public Gesture(IEnumerable<Vector2> points) : base(points) { }
    
    /// <summary>
    /// The bounding box of the gesture.
    /// </summary>
    public Rect BoundingBox
    {
        get { return GetBoundingBox(); }
    }

    /// <summary>
    /// The centroid of the gesture.
    /// </summary>
    public Vector2 Centroid => GetCentroid();

    /// <summary>
    /// The gesture's indicative angle.
    /// </summary>
    public float IndicativeAngle => GetIndicativeAngle();

    /// <summary>
    /// Resamples the gesture into a given number of points.
    /// </summary>
    /// <param name="n">The number of points to end with.</param>
    public Gesture Resample(int n)
    {
        var I = PathLength() / (n - 1); // interval length
        var D = 0.0f;
        var srcPts = new Gesture(this);
        var dstPts = new Gesture {srcPts[0]};
        
        for (var i = 1; i < srcPts.Count; i++)
        {
            var pt1 = srcPts[i - 1];
            var pt2 = srcPts[i];

            var d = Vector2.Distance(pt1, pt2);
            if ((D + d) >= I)
            {
                var qx = pt1.x + ((I - D) / d) * (pt2.x - pt1.x);
                var qy = pt1.y + ((I - D) / d) * (pt2.y - pt1.y);
                var q = new Vector2(qx, qy);
                dstPts.Add(q); // append new point 'q'
                srcPts.Insert(i, q); // insert 'q' at position i in points s.t. 'q' will be the next i
                D = 0.0f;
            }
            else
            {
                D += d;
            }
        }
        // sometimes we fall a rounding-error short of adding the last point, so add it if so
        if (dstPts.Count == n - 1)
        {
            dstPts.Add(srcPts[srcPts.Count - 1]);
        }

        return dstPts;

    }
    
    /// <summary>
    /// Returns the total path length of the gesture.
    /// </summary>
    /// <returns></returns>
    public float PathLength()
    {
        if (Count < 2)
        {
            Debug.LogWarning("Tried to recognize a gesture with 0 or 1 points.");
            return 0f;
        }
        //Calculate Path Length
        var pathLength = 0f;

        for (var i = 1; i < Count; i++)
        {
            pathLength += Vector2.Distance(this[i], this[i - 1]);
        }

        return pathLength;
    }
    
    /// <summary>
    /// Rotate the gesture by "theta" radians about it's centroid.
    /// </summary>
    /// <param name="theta">The amount of rotation in rads</param>
    public Gesture Rotate(float theta)
    {
        var result = new Gesture();
        var centroid = Centroid;
        
        foreach (var point in this)
        {      
            var qX = (point.x - centroid.x) * Mathf.Cos(theta) - (point.y - centroid.y) * Mathf.Sin(theta);
            var qY = (point.x - centroid.x) * Mathf.Sin(theta) + (point.y - centroid.y) * Mathf.Cos(theta);
            //translate back to origin and add to result
            result.Add(new Vector2(qX, qY) + centroid);
        }

        return result;
    }

    /// <summary>
    /// Translate the gesture so the center rests at the provided destination.
    /// </summary>
    /// <param name="destination">The destination of the translation.</param>
    public Gesture Translate(Vector2 destination)
    {
        var c = Centroid;
        var result = new Gesture();

        foreach (var p in this)
        {
            var qX = p.x - c.x + destination.x;
            var qY = p.y - c.y + destination.y;
            
            result.Add(new Vector2(qX, qY));
        }

        return result;
    }

    /// <summary>
    /// Scale the gesture so it's square bounding box is the given size.
    /// </summary>
    /// <param name="size">The size of the resultant bounding box.</param>
    public Gesture Scale(float size)
    {
        var bbox = BoundingBox;
        var result = new Gesture();
        
        foreach (var p in this)
        {
            var qX = p.x * size / bbox.width;
            var qY = p.y * size / bbox.height;
            result.Add(new Vector2(qX, qY));
        }

        return result;
    }
    
    /// <summary>
    /// Find the centroid (center point) of the gesture.
    /// </summary>
    /// <returns>The coordinate of the centroid.</returns>
    private Vector2 GetCentroid()
    {
        var x_bar = this.Sum(v => v.x) / Count;
        var y_bar = this.Sum(v => v.y) / Count;
    
        return new Vector2(x_bar, y_bar);
    }

    /// <summary>
    /// Find the bounding box of the gesture.
    /// </summary>
    /// <returns></returns>
    private Rect GetBoundingBox()
    {
        return Rect.MinMaxRect(
            this.Min(v => v.x),
            this.Min(v => v.y),
            this.Max(v => v.x),
            this.Max(v => v.y)
        );
    }

    /// <summary>
    /// Find the "indicative angle" of the gesture; http://faculty.washington.edu/wobbrock/pubs/uist-07.01.pdf
    /// </summary>
    /// <returns>The indicative angle as measured from Vector2.Right in radians</returns>
    private float GetIndicativeAngle()
    {
        var centroid = Centroid;
        return Mathf.Atan2(centroid.y - this[0].y, centroid.x - this[0].x);
    }
}
