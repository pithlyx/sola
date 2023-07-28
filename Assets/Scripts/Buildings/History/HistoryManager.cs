using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    private Stack<Command> history = new Stack<Command>();

    public void AddAndExecute(Command command)
    {
        command.Execute();
        history.Push(command);
    }

    public void Undo()
    {
        if (history.Count > 0)
        {
            Command command = history.Pop();
            command.Unexecute();
        }
    }
}
