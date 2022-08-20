using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PETools
{
    public static int RdInt(int min, int max, System.Random rd)
    {
        if(rd == null)
        {
            rd = new System.Random();
        }
        int val = rd.Next(min, max + 1);
        Debug.Log(val + " " + max);
        return val;
    }


}
