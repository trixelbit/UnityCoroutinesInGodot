using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

namespace U2GCoroutines 
{
    /// <summary>
    /// Responsible for running of coroutines.
    /// </summary>
    /// <remarks>
    /// Some additional behavior to be aware of.
    /// If coroutine is started by a node, and that node is deleted,
    /// the coroutine it started will also be deleted alongside it.</remarks>
    public class CoroutineRunner : Node
    {
        private static CoroutineRunner s_instance;
        
        private static readonly List<Node> s_owner = new List<Node>();
        
        private static readonly List<string> s_originalEnumerator = new List<string>();
        
        private static readonly List<Stack<IEnumerator>> s_enumeratorsStacks = new List<Stack<IEnumerator>>();
        
        public enum ERunnerTick
        {
            Process, 
            Physics
        }
       
        
        public CoroutineRunner()
        {
            if (s_instance != null)
            {
                throw new InvalidOperationException();
            }

            s_instance = this;
        }
        
        /// <summary>
        /// Runs the provided coroutine.
        /// </summary>
        /// <param name="owningNode">Node that started coroutine.</param>
        /// <param name="coroutine">Coroutine to be ran.</param>
        public static void Run(Node owningNode, IEnumerator coroutine)
        {
            s_owner.Add(owningNode);
            s_originalEnumerator.Add(coroutine.GetType().AssemblyQualifiedName);
            s_enumeratorsStacks.Add(new Stack<IEnumerator>(new[] { coroutine }));
        }


        /// <summary>
        /// Stops all coroutines started by the owning node.
        /// </summary>
        /// <param name="owningNode">The owning that started the coroutines (if any)</param>
        public static void StopAllForNode(Node owningNode)
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
        /// <remarks>
        /// If you started multiple instances of the same coroutine, this will stop all of them.
        /// A means of stopping a specific instance of a coroutine is not currently supported but under consideration.
        /// </remarks>
        /// <param name="owningNode">The owning node that started the coroutine.</param>
        /// <param name="enumerator">The enumerator to stop.</param>
        public static void Stop(Node owningNode, IEnumerator enumerator)
        {
            
            for (int i = s_owner.Count - 1; i >= 0; i--)
            {
                bool enumeratorIsSameType = s_originalEnumerator[i].Equals(enumerator.GetType().AssemblyQualifiedName);
                
                if (s_owner[i] == owningNode && 
                    enumeratorIsSameType)
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
                bool noMoreInstructions  = s_enumeratorsStacks[i].Count == 0;
                bool parentIsDeleted = s_owner[i] == null;
                
                if (noMoreInstructions || parentIsDeleted)
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
