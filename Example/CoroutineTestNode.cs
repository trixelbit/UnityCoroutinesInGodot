using System;
using Godot;
using System.Collections;
using U2GCoroutines;

public class CoroutineTestNode : Spatial
{
    [Export]
    public float IterationTimeWait;

    [Export]
    public EWaitType WaitType;

    private Button _button;

    private bool _condition; 
    
    public enum EWaitType
    {
        Wait,
        RealTimeWait,
        WaitForFixedUpdate,
        WaitUntil, 
        WaitWhile
    }

    private Coroutine _coroutine;
   
    
    public override void _Ready()
    {
        Translation = new Vector3(-10, Translation.y, 0);
            
        _coroutine = CoroutineRunner.Run(this, MoveUpdate());
    }

    public void OnStopCoroutines()
    {
        CoroutineRunner.StopAllForNode(this);
    } 
    
    
    public void OnButtonPressed()
    {
        _condition = true;
    }

    private IEnumerator DelayedStop()
    {
        yield return new WaitForSecondsRealtime(10);
        CoroutineRunner.Stop(_coroutine);    
    }
    public IEnumerator MoveUpdate()
    {
        while (true)
        {
            switch (WaitType)
            {
                case EWaitType.Wait:
                    yield return new WaitForSeconds(IterationTimeWait);
                    break;
                case EWaitType.RealTimeWait:
                    yield return new WaitForSecondsRealtime(IterationTimeWait);
                    break;
                case EWaitType.WaitForFixedUpdate: 
                    yield return new WaitForFixedUpdate();
                    break;
                case EWaitType.WaitUntil:
                    yield return new WaitUntil(() => _condition);
                    break;
                case EWaitType.WaitWhile:
                    yield return new WaitWhile(() => _condition);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Translation += new Vector3(1f, 0, 0);

            if (Translation.x > 10)
            {
                Translation = new Vector3(-10, Translation.y, 0);
            }
        }
    }
}
