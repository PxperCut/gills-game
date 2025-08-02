using System;
using System.Collections.Generic;
using UnityEngine;

public class DevConsole : MonoBehaviour
{
    public string[][] CommandTree =
    {
        new string[] {"help"},//0 help
        new string[] {"blah","blah"},//1 test
        new string[] {"long-test","with","more","text"}//2 longer test
        //so i dont forget, this wont work because it doesnt support nested commands.
        //ie if you have a branching command like... lookup/name or lookup/word, they have to be dif
        //
    };
    
    string root = "/";

    void Awake()
    {
        foreach (var path in CommandTree)
        {
        //    print(root+string.Join(", ", path));
        }
    }

    public void InputChanged(string inputshit)
    {
        foreach (var path in CommandTree)
        {
            print(root + path[0]);
            if (root + path[0] == inputshit)
            {
                print("PATHFOUND: " + path[0]);
            }
        }
    }
}
