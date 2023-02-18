using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public InputField inputField;
    public Transform Root;
    public GameObject[] s;

    public void BtnClick()
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i].gameObject.name.Contains(inputField.text))
            {
                s[i].SetActive(true);
            }
            else
                s[i].SetActive(false);
        }

    }
}
