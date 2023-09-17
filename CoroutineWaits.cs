using Godot;
using System;
using System.Collections;

namespace U2GCoroutines
{
    /// <summary>
    /// Waits for the provided number of seconds to pass regardless of time scale.
    /// </summary>
    public class WaitForSecondsRealtime : IEnumerator
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

    /// <summary>
    /// Waits until the provided function returns true.
    /// </summary>
    public class WaitUntil : IEnumerator
    {
        private readonly Func<bool> _function;
        
        public object Current { get; }
        
        public WaitUntil(Func<bool> function)
        {
            _function = function;
        }
        
        public bool MoveNext()
        {
            return !_function();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

    }
    
    /// <summary>
    /// Waits while the provided function returns true.
    /// </summary>
    public class WaitWhile : IEnumerator
    {
        private readonly Func<bool> _function;
        
        public object Current { get; }
        
        public WaitWhile(Func<bool> function)
        {
            _function = function;
        }
        
        public bool MoveNext()
        {
            return _function();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

    }
    
}
