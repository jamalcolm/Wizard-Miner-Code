using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * General maths functions to be used throughout the code
 * 
*/
public class Functions{

    //Modulo with negatives
    //So -3%3=0, -2%3=1, -1%3=2, 0%3=0, 1%3=1, 2%3=2 etc.
    public static float nfmod(float a,float b)
    {
        return (a >= 0f ? a % b : b + (a+1f) % b - 1f);
    }

    public static int nfmod(int a,int b)
    {
        return (a >= 0 ? a % b : b + (a+1) % b - 1);
    }

    //Snap to specific snap size
    public static float Snap(float n, float snap)
    {
        return (Mathf.Round(n / snap) * snap);
    }

    //Convert int to string with specfic no. of digits
    public static string StringDigit(int n, int digits)
    {
        string s = n.ToString();
        while (s.Length < digits)
            s = "0" + s;
        return s;
    }
}
