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
    /// the coroutine it started will also be deleted alongside it.
    /// </remarks>
    public class CoroutineRunner : Node
    {
        private static CoroutineRunner s_instance;
        
        private static readonly List<Node> s_owner = new List<Node>();

        private static readonly List<Coroutine> s_coroutines = new List<Coroutine>();
        
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
        /// <returns>The newly started Coroutine. Can be used to stopping this
        /// specific enumerator instance.</returns>
        public static Coroutine Run(Node owningNode, IEnumerator coroutine)
        {
            Coroutine newCoroutine = new Coroutine(coroutine);
            
            s_owner.Add(owningNode);
            s_coroutines.Add(newCoroutine);
            s_enumeratorsStacks.Add(new Stack<IEnumerator>(new[] { coroutine }));

            return newCoroutine;
        }

        /// <summary>
        /// Stops all coroutines started by the owning node.
        /// </summary>
        /// <param name="owningNode">The owning node that started the coroutines (if any)</param>
        public static void StopAllForNode(Node owningNode)
        {
            for (int i = s_owner.Count - 1; i >= 0; i--)
            {
                if (s_owner[i] == owningNode)
                {
                    RemoveCoroutine(i);
                }
            }
        }

        /// <summary>
        /// Stops all coroutines of the same type as the provided <see cref="IEnumerator"/>
        /// that were started by the provided Node.
        /// </summary>
        /// <remarks>
        /// If you started multiple instances of the same coroutine, this will stop all of them.
        /// </remarks>
        /// <param name="owningNode">The owning node that started the coroutine.</param>
        /// <param name="enumerator">The enumerator to stop. </param>
        public static void Stop(Node owningNode, IEnumerator enumerator)
        {
            for (int i = s_owner.Count - 1; i >= 0; i--)
            {
                bool enumeratorIsSameType = s_coroutines[i].Name.Equals(enumerator.GetType().AssemblyQualifiedName);
                
                if (s_owner[i] == owningNode && 
                    enumeratorIsSameType)
                {
                    RemoveCoroutine(i);
                }
            }
        }
        
       
        /// <summary>
        /// Stops a specific coroutine instance started by the owning node.
        /// </summary>
        /// <param name="coroutine">The specific coroutine instance to stop.</param>
        public static void Stop(Coroutine coroutine)
        {
            for (int i = s_owner.Count - 1; i >= 0; i--)
            {
                if (s_coroutines[i].Id == coroutine.Id)
                {
                    RemoveCoroutine(i);
                }
            }
        }
        
        private static void RemoveCoroutine(int index)
        {
            s_enumeratorsStacks.RemoveAt(index);
            s_coroutines.RemoveAt(index); 
            s_owner.RemoveAt(index);
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
            for (int i = s_enumeratorsStacks.Count -1; i >= 0; i--)
            {
                bool noMoreInstructions  = s_enumeratorsStacks[i].Count == 0;
                bool parentIsDeleted = s_owner[i] == null;
                
                if (noMoreInstructions || parentIsDeleted)
                {
                    RemoveCoroutine(i);
                    continue;
                }
               
                IEnumerator instruction = s_enumeratorsStacks[i].Peek();

                if (instruction is ITickType tick && tick.TickType != tickType)
                {
                    continue;
                }
                
                if (!instruction.MoveNext())
                {
                    if (i == s_enumeratorsStacks.Count)
                    {
                        i -= 1;
                    }
                  
                    if(s_enumeratorsStacks[i].Count == 0)
                    {
                        RemoveCoroutine(i);
                        continue;
                    }
                    
                    s_enumeratorsStacks[i].Pop();
                }

                if (instruction.Current is IEnumerator next && instruction != next)
                {
                    s_enumeratorsStacks[i].Push(next);
                }
            }
        }
    }


    /// <summary>
    /// Is a reference to a specific coroutine.
    /// Used for stopping a specific coroutine instead of stopping all coroutines of
    /// the same <see cref="IEnumerator"/> type.
    /// </summary>
    public class Coroutine
    {
        public readonly Guid Id = Guid.NewGuid();
        
        public readonly String Name;
        
        public Coroutine(IEnumerator enumerator)
        {
            Name = enumerator.GetType().AssemblyQualifiedName;
        }
    }
}
