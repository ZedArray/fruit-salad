using System;
using UnityEngine;

public class Vector2Pol 
{
    private float _theta;
    public float theta
    {
        get => _theta;
        set => _theta = value % (Mathf.PI*2);
    }
    public float radius;
    #region constructors
    public Vector2Pol()
    {
        theta = 0f;
        radius = 0f;
    }
    public Vector2Pol(Vector2 vector2)
    {
        var t = (Vector2Pol)vector2;
        theta = t.theta;
        radius = t.radius;
    }
    public Vector2Pol(float radius, float theta)
    {
        this.theta = theta;
        this.radius = radius;
    }
    #endregion
    
    #region operators
    public static explicit operator Vector2(Vector2Pol vector2Pol)
    {
        return vector2Pol.radius * new Vector2(Mathf.Cos(vector2Pol.theta), Mathf.Sin(vector2Pol.theta));
    }
    
    public static explicit operator Vector2Pol(Vector2 vector2)
    {
        return new Vector2Pol(Mathf.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y),Mathf.Atan2(vector2.y, vector2.x) );
    }
    
    public static Vector2Pol operator +(Vector2Pol vector2Pol, Vector2Pol vector2Pol2) {
        return (Vector2Pol)((Vector2)vector2Pol + (Vector2)vector2Pol2);
    }
    
    public static Vector2Pol operator -(Vector2Pol vector2Pol, Vector2Pol vector2Pol2) {
        return (Vector2Pol)((Vector2)vector2Pol - (Vector2)vector2Pol2);
    }

    public static Vector2Pol operator *(Vector2Pol vector2Pol, float multiplier)
    {
        Vector2Pol v2p = new Vector2Pol(vector2Pol.radius*Mathf.Abs(multiplier), vector2Pol.theta);
        if (multiplier < 0)
        {
            v2p.theta = v2p.theta + Mathf.PI;
        }
        
        return v2p;
    }

    public static Vector2Pol operator *(Vector2Pol vector2Pol, Vector2Pol vector2Pol2)
    {
        return new Vector2Pol(vector2Pol.radius*vector2Pol2.radius,  vector2Pol.theta+vector2Pol2.theta);
    }
    
    public static Vector2Pol operator /(Vector2Pol vector2Pol, float divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException();
        }

        Vector2Pol v2p = new Vector2Pol(vector2Pol.radius/Mathf.Abs(divisor), vector2Pol.theta);
        if (divisor < 0)
        {
            v2p.theta = v2p.theta + Mathf.PI;
        }
        
        return v2p;
        
    }
    public static Vector2Pol operator /(Vector2Pol vector2Pol, Vector2Pol vector2Pol2)
    {
        if (vector2Pol2.radius == 0)
        {
            throw new DivideByZeroException();
        }
        return new Vector2Pol(vector2Pol.radius/vector2Pol2.radius,  vector2Pol.theta-vector2Pol2.theta);
    }
    #endregion
    
    #region methods

    public void Rotate(float delta)
    {
        theta += delta;
    }

    public void RotateByDegrees(float delta)
    {
        theta += Mathf.Deg2Rad * delta;
    }


    #endregion
}
