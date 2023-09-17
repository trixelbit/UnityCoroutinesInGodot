# UnityCoroutinesInGodot

## About 

Hi. I'm a Unity developer who jumped ship to Godot. 
I'm used to Unity's coroutines, so I decided to make something that recreates 
how coroutines work in Godot. 


https://github.com/trixelbit/UnityCoroutinesInGodot/assets/38009610/40353607-fc0a-4955-8300-243eb7e848c8


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
CoroutineRunner.Run(Node owningNode, IEnumerator coroutine)

CoroutineRunner.StopAllCoroutinesForNode(Node owningNode)
```

StopCoroutine is next and Pause/Resume methods is currently under consideration.


## Example Usage

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

## Installation

- Download the repo. 
- Throw it into your project. 
- Goto Project > Project Settings > AutoLoad
- Add CoroutineRunner.tscn to your AutoLoad. 

You should be good to go. 
