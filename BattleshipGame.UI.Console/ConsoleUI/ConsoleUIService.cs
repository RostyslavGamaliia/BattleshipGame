using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Infrastructure.Players.Services;

namespace BattleshipGame.UI.Console.ConsoleUI
{
    internal interface IActionsQueue : IAsyncDisposable
    {
        void Enqueue(Action action);

        Task InvokeAvailableActionsAsync(CancellationToken cancellationToken = default);
    }

    internal sealed class ConsoleUIService : IUIInteractionService
    {
        private int _renderedTurn = -1;
        private readonly IActionsQueue _renderActionsQueue;

        public ConsoleUIService(IActionsQueue renderActionsQueue)
        {
            _renderActionsQueue = renderActionsQueue;
        }

        public async Task<string> ReadUserInputAsync()
        {
            await _renderActionsQueue.InvokeAvailableActionsAsync();

            System.Console.SetCursorPosition(0, 2);
            System.Console.CursorVisible = true;
            try
            {
                return System.Console.ReadLine() ?? string.Empty;
            }
            finally
            {
                System.Console.CursorVisible = false;
            }
        }

        public void DrawStaticView(int fieldSize)
        {
            _renderActionsQueue.Enqueue(() => DrawInitialView(fieldSize));
        }

        public void DrawFrame(PlayerGameViewModel playerGameViewModel)
        {
            _renderActionsQueue.Enqueue(() =>
            {
                if (_renderedTurn < playerGameViewModel.GameTurn)
                {
                    _renderedTurn = playerGameViewModel.GameTurn;
                    var fieldSize = playerGameViewModel.PlayerBattlefield.CellStates.GetLength(0);
                    DrawBattlefieldView(1, 6, playerGameViewModel.PlayerBattlefield);
                    DrawBattlefieldView(fieldSize * 2 + 7, 6, playerGameViewModel.OpponentBattlefield);
                }
            });
        }

        public void SetStatusText(string statusText)
        {
            _renderActionsQueue.Enqueue(() =>
            {
                DrawTextAtLine(1, string.Join(' ', Enumerable.Range(0, 80).Select(_ => string.Empty)));
                DrawTextAtLine(2, string.Join(' ', Enumerable.Range(0, 80).Select(_ => string.Empty)));
                DrawTextAtLine(1, statusText);
                System.Console.SetCursorPosition(0, 20);
            });
        }

        public ValueTask DisposeAsync() => _renderActionsQueue.DisposeAsync();

        private static void DrawTextAtLine(int lineIndex, string text)
        {
            System.Console.SetCursorPosition(0, lineIndex);
            System.Console.WriteLine(text);
        }

        private static void DrawInitialView(int fieldSize)
        {
            System.Console.Title = "Battleship game";
            System.Console.CursorVisible = false;
            System.Console.Clear();

            DrawBattlefieldFrames(4, fieldSize);
        }

        private static void DrawBattlefieldFrames(int offsetTop, int fieldSize)
        {
            System.Console.SetCursorPosition(0, offsetTop);

            var emptyFieldLine = string.Join(string.Empty, Enumerable.Range(0, fieldSize).Select(x => "  "));
            var borderLine = string.Join(string.Empty, Enumerable.Range(0, fieldSize).Select(x => "══"));
            var fieldCaptions = string.Join(' ', Enumerable.Range(0, fieldSize).Select(i => (char)('A' + i)));
            System.Console.WriteLine($"  {fieldCaptions}       {fieldCaptions}");
            System.Console.WriteLine($"╔{borderLine}╗    ╔{borderLine}╗");
            for (var i = 1; i <= fieldSize; i++)
            {
                System.Console.WriteLine($"║{emptyFieldLine}║ {i,2} ║{emptyFieldLine}║");
            }
            System.Console.WriteLine($"╚{borderLine}╝    ╚{borderLine}╝");
        }

        private static void DrawBattlefieldView(int offsetLeft, int offsetTop, BattlefieldViewModel battlefieldViewModel)
        {
            for (var i = 0; i < battlefieldViewModel.CellStates.GetLength(0); i++)
                for (var j = 0; j < battlefieldViewModel.CellStates.GetLength(1); j++)
                {
                    System.Console.SetCursorPosition(offsetLeft + i * 2, offsetTop + j);
                    var cellColor = battlefieldViewModel.CellStates[j, i] switch
                    {
                        BattlefieldCellState.Water => ConsoleColor.Black,
                        BattlefieldCellState.Deck => ConsoleColor.DarkGreen,
                        BattlefieldCellState.Miss => ConsoleColor.Gray,
                        BattlefieldCellState.Hit => ConsoleColor.DarkRed,
                        _ => throw new ArgumentException("Unexpected cell state")
                    };
                    System.Console.BackgroundColor = cellColor;
                    System.Console.Write("  ");
                }
            System.Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}