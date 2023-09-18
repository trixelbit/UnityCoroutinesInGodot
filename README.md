# UnityCoroutinesInGodot

## About 

Hi. I'm a Unity developer who jumped ship to Godot. 
I'm used to Unity's coroutines, so I decided to make something that recreates 
how coroutines work in Godot. 


This is specifically for Godot 3. 

This is started as of 9/16/2023.


## Current Supported Features

Current Supported Wait Classes: 
```cs
WaitForSeconds(float seconds)

WaitForSecondsRealtime(float seconds)

WaitUntil(Func<bool>)

WaitWhile(Func<bool>)

WaitForFixedUpdate()
```

Current Supported Methods
```cs
/// <summary>
/// Runs the provided coroutine.
/// </summary>
/// <param name="owningNode">Node that started coroutine.</param>
/// <param name="enumerator">Coroutine to be ran.</param>
/// <returns>The newly start Coroutine. Can be used for stopping 
/// this specific enumerator instance.</returns>
Coroutine CoroutineRunner.Run(Node owningNode, IEnumerator enumerator);

/// <summary>
/// Stops all coroutines of the same type as the provided <see cref="IEnumerator"/>
/// that were started by the provided Node.
/// </summary>
/// <remarks>
/// If you started multiple instances of the same coroutine, this will stop all of them.
/// </remarks>
/// <param name="owningNode">The owning node that started the coroutine.</param>
/// <param name="enumerator">The enumerator to stop.</param>
void CoroutineRunner.Stop(Node owningNode, IEnumerator enumerator);

/// <summary>
/// Stops a specific coroutine instance started by the owning node.
/// </summary>
/// <param name="coroutine">The specific coroutine instance to stop.</param>
void CoroutineRunner.Stop(Coroutine coroutine);

/// <summary>
/// Stops all coroutines started by the owning node.
/// </summary>
/// <param name="owningNode">The owning node that started the coroutines (if any)</param>
void CoroutineRunner.StopAllForNode(Node owningNode);
```

Pause/Resume methods is currently under consideration.



## Example Usage

Simple Usage:
```cs 
using U2GCoroutines;


public override void _Ready()
{
    CoroutineRunner.Run(this, PrintAfter10Seconds());
}

// Will wait 10 seconds then print a message.
private IEnumerator PrintAfter10Seconds()
{
    yield return new WaitForSecondsRealtime(10);

    GD.Print("10 Seconds have passed!");
}

```

Cached coroutine example:
```cs
using U2GCoroutines;

// Reference to a specific ran coroutine.
private Coroutine _moveLoop;



public override void _Ready()
{
    _moveLoop = CoroutineRunner.Run(this, MoveLoop());
    CoroutineRunner.Run(this, DelayedStop());
}

// moves object to the right continuesly
private IEnumerator MoveLoop()
{
    while(true)
    {
        yield return new WaitForFixedUpdate();

        Translation += Vector3.Right; 
    }
}

// will stop the move loop after 10 seconds.
private IEnumerator DelayedStop()
{
    yield return new WaitForSecondsRealtime(10);

    CoroutineRunner.Stop(_moveLoop);
}
```

## Installation

- Download the repo. 
- Throw it into your project. 
- Goto Project > Project Settings > AutoLoad
- Add CoroutineRunner.tscn to your AutoLoad. 

You should be good to go. 
