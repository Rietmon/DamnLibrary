using System;

namespace Rietmon.Async
{
    public class AsyncHandler
    {
        private int operationExecuting;

        public bool IsDone => operationExecuting == 0;
    
        public void AddToExecute(Action operation)
        {
            operationExecuting++;
        
            AsyncExecutor.Handle(() =>
            {
                operation?.Invoke();
                OnOperationEnd(); 
            });
        }

        private void OnOperationEnd()
        {
            operationExecuting--;
        }
    }
}
