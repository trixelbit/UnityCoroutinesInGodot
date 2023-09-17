using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

namespace U2GCoroutines 
{
    /// <summary>
    /// Responsible for running of coroutines.
    /// </summary>
    public class CoroutineRunner : Node
    {
        private static CoroutineRunner s_instance;
        
        private static readonly List<Node> s_owner = new List<Node>();
        
        private static readonly List<IEnumerator> s_originalEnumerator = new List<IEnumerator>();
        
        private static readonly List<Stack<IEnumerator>> s_enumeratorsStacks = new List<Stack<IEnumerator>>();
        
        
        public CoroutineRunner()
        {
            if (s_instance != null)
            {
                throw new InvalidOperationException();
            }

            s_instance = this;
        }

        public enum ERunnerTick
        {
            Process, 
            Physics
        }
        
        /// <summary>
        /// Runs the provided coroutine.
        /// </summary>
        /// <param name="owningNode">Node that started coroutine.</param>
        /// <param name="coroutine">Coroutine to be ran.</param>
        public static void Run(Node owningNode, IEnumerator coroutine)
        {
            s_owner.Add(owningNode);
            s_originalEnumerator.Add(coroutine);
            s_enumeratorsStacks.Add(new Stack<IEnumerator>(new[] { coroutine }));
        }


        /// <summary>
        /// Stops all coroutines started by the owning node.
        /// </summary>
        /// <param name="owningNode">The owning that started the coroutines (if any)</param>
        public static void StopAllCoroutinesForNode(Node owningNode)
        {
            for (int i = s_owner.Count - 1; i >= 0; i--)
            {
                if (s_owner[i] == owningNode)
                {
                    s_enumeratorsStacks.RemoveAt(i);
                    s_originalEnumerator.RemoveAt(i);
                    s_owner.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Stops a specific coroutine started by the owning node.
        /// </summary>
        /// <param name="owningNode">The owning node that started the coroutine.</param>
        /// <param name="enumerator">The enumerator to stop.</param>
        public static void StopCoroutine(Node owningNode,IEnumerator enumerator)
        {
            for (int i = s_owner.Count - 1; i >= 0; i--)
            {
                if (s_owner[i] == owningNode && 
                    s_originalEnumerator[i] == enumerator)
                {
                    s_enumeratorsStacks.RemoveAt(i);
                    s_originalEnumerator.RemoveAt(i);
                    s_owner.RemoveAt(i);
                }
            }
        }
        
        public override void _Process(float delta)
        {
            ProcessCoroutines(ERunnerTick.Process);
        }

        public override void _PhysicsProcess(float delta)
        {
            ProcessCoroutines(ERunnerTick.Physics);
        }
        
        private void ProcessCoroutines(ERunnerTick tickType)
        {
            List<int> indiciesToRemove = new List<int>();

            for (var i = 0; i < s_enumeratorsStacks.Count; i++)
            {
                if (s_enumeratorsStacks[i].Count == 0)
                {
                    indiciesToRemove.Add(i);
                    continue;
                }
               
                IEnumerator instruction = s_enumeratorsStacks[i].Peek();

                if (instruction is ITickType tick && tick.TickType != tickType)
                {
                    continue;
                }
                
                if (!instruction.MoveNext())
                {
                    s_enumeratorsStacks[i].Pop();
                }

                if (instruction.Current is IEnumerator next && instruction != next)
                {
                    s_enumeratorsStacks[i].Push(next);
                }
            }
            
            for (var i = indiciesToRemove.Count -1; i >= 0; i--)
            {
                s_enumeratorsStacks.RemoveAt(indiciesToRemove[i]);
                s_originalEnumerator.RemoveAt(indiciesToRemove[i]);
                s_owner.RemoveAt(indiciesToRemove[i]);
            }
        }
        
    }
}
