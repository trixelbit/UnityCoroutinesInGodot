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
        WaitUntil, 
        WaitWhile
    }


    private Coroutine _coroutine;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Translation = new Vector3(-10, Translation.y, 0);
            
        _coroutine = CoroutineRunner.Run(this, MoveUpdate());
        CoroutineRunner.Run(this, DelayedStop());
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
        CoroutineRunner.Stop(this, MoveUpdate());    
    }
    public IEnumerator MoveUpdate()
    {
        for (int i = 0; i < 1000; i++)
        {
            switch (WaitType)
            {
                case EWaitType.Wait:
                yield return new WaitForSeconds(IterationTimeWait);
                    break;
               
                case EWaitType.RealTimeWait:
                yield return new WaitForSecondsRealtime(IterationTimeWait);
                    break;
                case EWaitType.WaitUntil:
                    yield return new WaitUntil(() => _condition);
                    break;
                case EWaitType.WaitWhile:
                    yield return new WaitWhile(() => _condition);
                    break;
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
