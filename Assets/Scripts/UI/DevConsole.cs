using System.Collections.Generic;
using UnityEngine;

public class CommandNode : MonoBehaviour
{
    public Dictionary<string, CommandNode> CommandMap = new Dictionary<string, CommandNode>();

    public CommandNode AddChild(string name)
    {
        //here
    }
}

public class DevConsole : MonoBehaviour
{
    public void InputChanged(string inputshit)
    {

    }
}
