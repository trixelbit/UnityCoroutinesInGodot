using Godot;
using System;
using System.Collections;


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
