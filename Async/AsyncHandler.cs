using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rietmon.Async
{
    public class AsyncHandler
    {
        public bool IsDone => operationsQueue.Count == 0;
        
        public bool HasOperations => operationsQueue.Count > 0;

        public bool IsExecutingNow => operationsExecuting.Count > 0;

        private readonly List<Action> operationsQueue = new List<Action>();

        private readonly Dictionary<Action, Coroutine> operationsExecuting = new Dictionary<Action, Coroutine>();

        public void Add(Action operation)
        {
            operationsQueue.Add(operation);
        }

        public void AddAndExecute(Action operation)
        {
            operationsQueue.Add(operation);
            ExecuteOperation(operationsQueue.Last(), null);
        }

        public void Remove(Action operation)
        {
            if (operationsExecuting.ContainsKey(operation))
            {
                Debug.LogError("[AsyncHandler] (Remove) Cant remove operation, cuz it executing now.");
                return;
            }

            operationsQueue.Remove(operation);
        }

        public void RemoveAndShutdown(Action operation)
        {
            Shutdown(operation);
            
            operationsQueue.Remove(operation);
        }

        public void ExecuteQueue()
        {
            if (!HasOperations)
            {
                Debug.LogError("[AsyncHandler] (ExecuteQueue) Cant execute, cuz operations queue is empty.");
                return;
            }

            if (IsExecutingNow)
            {
                Debug.LogError("[AsyncHandler] (ExecuteQueue) Cant execute, cuz now some operations already executing.");
                return;
            }

            void Internal_Execute() => 
                ExecuteOperation(operationsQueue[0], Internal_Execute);
            
            Internal_Execute();
        }

        public void ExecuteAllQueue()
        {
            if (!HasOperations)
            {
                Debug.LogError("[AsyncHandler] (ExecuteAllQueue) Cant execute. Maybe operations queue is empty.");
                return;
            }

            if (IsExecutingNow)
            {
                Debug.LogError("[AsyncHandler] (ExecuteAllQueue) Cant execute, cuz now some operations already executing!");
                return;
            }
            
            for (var i = 0; i < operationsQueue.Count; i++)
                ExecuteOperation(operationsQueue[i], null);
        }

        public void Shutdown(Action operation)
        {
            if (!operationsExecuting.TryGetValue(operation, out var coroutine)) 
                return;
            
            AsyncExecutor.Shutdown(coroutine);
            operationsExecuting.Remove(operation);
        }

        public void ShutdownAll()
        {
            var operationsList = new List<Action>();
            foreach (var operation in operationsExecuting)
                operationsList.Add(operation.Key);

            foreach (var operation in operationsList)
                Shutdown(operation);
        }

        private void OnOperationEnd(Action operation)
        {
            operationsQueue.Remove(operation);
        }

        private void ExecuteOperation(Action operation, Action callback)
        {
            var coroutine = AsyncExecutor.Handle(() =>
            {
                operation?.Invoke();
                OnOperationEnd(operation);
                
                callback?.Invoke();
            });
            
            operationsExecuting.Add(operation, coroutine);
        }
    }
}
