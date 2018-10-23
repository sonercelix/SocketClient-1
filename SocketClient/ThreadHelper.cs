using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ThreadHelper : MonoBehaviour
{
    Queue<Action> mainThreadActions = new Queue<Action>();

    private static ThreadHelper s_instance;

    public static ThreadHelper Instance
    {
        get
        {
            return s_instance;
        }
    }
    
    public ThreadHelper()
    {
        s_instance = this;
    }

    public void InvokeOnMain(Action action)
    {
        mainThreadActions.Enqueue(action);
    }

    public void Update()
    {
        while (mainThreadActions.Count > 0)
        {
            mainThreadActions.Dequeue().Invoke();
        }
    }
}
