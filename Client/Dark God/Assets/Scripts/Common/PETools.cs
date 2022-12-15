using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
public class PETools
{
    public static int RdInt(int min, int max, System.Random rd = null)
    {
        if(rd == null)
        {
            rd = new System.Random();
        }
        int val = rd.Next(min, max + 1);
        return val;
    }


}
