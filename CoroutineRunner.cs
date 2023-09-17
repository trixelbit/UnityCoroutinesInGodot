using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Responsible for running of coroutines.
/// </summary>
public class CoroutineRunner : Node
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

    public override void _Process(float delta)
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
