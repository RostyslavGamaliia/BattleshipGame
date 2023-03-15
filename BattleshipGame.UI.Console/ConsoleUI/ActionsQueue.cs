using System.Collections.Concurrent;

namespace BattleshipGame.UI.Console.ConsoleUI
{
    internal class ActionsQueue : IActionsQueue
    {
        private readonly ConcurrentQueue<Action> _actionsQueue = new();
        private readonly Task _actionsInvoker;
        private TaskCompletionSource _pausedActionsInvokerSource = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public ActionsQueue()
        {
            _actionsInvoker = Task.Run(
                () => InvokeActionsFromQueue(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            await _actionsInvoker.ContinueWith(_ => _cancellationTokenSource.Dispose(), TaskContinuationOptions.OnlyOnCanceled);
        }

        public Task InvokeAvailableActionsAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                while (!_actionsQueue.IsEmpty)
                {
                    await Task.Delay(100, cancellationToken);
                }
            }, cancellationToken);
        }

        public void Enqueue(Action action)
        {
            _actionsQueue.Enqueue(action);
            _pausedActionsInvokerSource.TrySetResult();
        }

        private async Task InvokeActionsFromQueue(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_actionsQueue.TryDequeue(out var action))
                {
                    action.Invoke();
                }
                else
                {
                    await _pausedActionsInvokerSource.Task;
                    _pausedActionsInvokerSource = new TaskCompletionSource();
                }
            }
        }
    }
}