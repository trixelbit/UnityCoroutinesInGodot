using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

/// Stop For Specific Node
/// Stop Specific coroutine
/// ExtendTime waits (WaitForSecondsScaled, WaitForFrames, WaitForPhysicsFrames)

/// <summary>
/// Responsible for running of coroutines.
/// </summary>
public class CoroutineRunner
{
    private static CoroutineRunner s_instance;
    
    private static List<Node> s_owner = new List<Node>();
    
    private static List<Stack<IEnumerator>> s_enumerators = new List<Stack<IEnumerator>>();
    
    public CoroutineRunner()
    {
        if (s_instance != null)
        {
            throw new InvalidOperationException();
        }

        s_instance = this;
    }
    
    public static void RunCoroutine(Node owningNode, IEnumerator coroutine)
    {
        s_owner.Add(owningNode);
        s_enumerators.Add(new Stack<IEnumerator>(new[] { coroutine }));
    }

    public void ProcessCoroutines()
    {
        List<int> indiciesToRemove = new List<int>();

        for (var i = 0; i < s_enumerators.Count; i++)
        {
            
            if (s_enumerators[i].Count == 0)
            {
                indiciesToRemove.Add(i);
            }

           
            IEnumerator instruction = s_enumerators[i].Peek();
            
            if (!instruction.MoveNext())
            {
                s_enumerators[i].Pop();
            }

            if (instruction.Current is IEnumerator next && instruction != next)
            {
                s_enumerators[i].Push(next);
            }
        }
        
        for (var i = indiciesToRemove.Count -1; i >= 0; i--)
        {
            s_enumerators.RemoveAt(indiciesToRemove[i]);
            s_owner.RemoveAt(indiciesToRemove[i]);
        }
    }
}

/// <summary>
/// Waits for the provided number of seconds to pass regardless of time scale.
/// </summary>
internal class WaitForSecondsRealtime : IEnumerator
{
    private readonly float _seconds;

    private readonly TimeSpan _endTime;
    
    public object Current { get; }
  
    
    public WaitForSecondsRealtime(float seconds)
    {
        _seconds = seconds;
        _endTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(_seconds));
    }

    public bool MoveNext()
    {
        return DateTime.Now.TimeOfDay < _endTime;
    }

    public void Reset()
    {
        
    }
}
    
/// <summary>
/// Wait for the provided number of seconds to pass. This is affected by time scale.
/// </summary>
public class WaitForSeconds : IEnumerator
{
    private readonly float _unscaledWaitTime;

    private readonly TimeSpan _startTime;
    
    public object Current { get; }
   
    
    public WaitForSeconds(float seconds)
    {
        _unscaledWaitTime = seconds;
        _startTime = DateTime.Now.TimeOfDay;
    }
    
    public bool MoveNext()
    {
        if (Engine.TimeScale == 0)
        {
            return true;
        }
        
        var endTime = _startTime.Add(TimeSpan.FromSeconds(_unscaledWaitTime / Engine.TimeScale));
       
        return DateTime.Now.TimeOfDay < endTime;
    }

    public void Reset()
    {
        
    }
}

//WaitForEndOfFrame
//WaitForFixedUpdate

/// <summary>
///  
/// </summary>
public class WaitForFixedUpdate : IEnumerator
{
    public bool MoveNext()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public object Current { get; }
}

//WaitUntil

//WaitWhile
