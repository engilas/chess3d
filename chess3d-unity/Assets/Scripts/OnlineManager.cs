using System;
using System.Threading;
using System.Threading.Tasks;
using ChessEngine.Engine;
using ChessServer.Client;
using ChessServer.Common;
using ChessServer.Common.Types;
using Microsoft.FSharp.Core;
using UnityEngine;

namespace Assets.Scripts
{
    public class OnlineManager
    {
        private static ChessConnection.ServerConnection _serverConnection;
        private static Task _connectedTask;
        private static Task _moveTask;
        private static bool _matched;
        private static bool _matchFailed;
        private static CancellationTokenSource _connectionCts;

        private static string LastConnectionId
        {
            get => PlayerPrefs.GetString(nameof(LastConnectionId));
            set => PlayerPrefs.SetString(nameof(LastConnectionId), value);
        }

        public static void Connect()
        {
            IsClosed = false;
            _matched = false;
            _connectedTask = null;
            var handler = ChessConnection.notificationHandlerAdapter(ChatNotification, MoveNotification, EndGameNotification, StartGameNotification, SessionCloseNotification);
            var ip = Settings.ServerIp.TrimEnd('/');
            _serverConnection =
                new ChessConnection.ServerConnection($"{ip}/command", handler);

            _connectionCts = new CancellationTokenSource();
            _connectedTask = ConnectWorkflow();

            _serverConnection.Reconnecting += e => 
            {
                Debug.Log("Reconnecting");
                IsReconnecting = true;
                Reconnecting?.Invoke();
                return Task.CompletedTask;
            };

            _serverConnection.Reconnected += e =>
            {
                Debug.Log("Reconnected");
                IsReconnecting = false;
                return TryRestoreConnection();
            };

            _serverConnection.Closed += e =>
            {
                Debug.Log("Closed");
                IsClosed = true;
                Closed?.Invoke();
                return Task.CompletedTask;
            };
        }

        public static async void CloseConnection()
        {
            await _serverConnection.Close();
            await _serverConnection.DisposeAsync();
        }

        public static void StopConnection()
        {
            _connectionCts.Cancel();
            _serverConnection.DisposeAsync();
        }

        public static bool IsConnected() => _matched && !_matchFailed;
        public static bool IsConnectionFailed() => _matchFailed || _connectedTask.IsFaulted;
        public static ChessPieceColor PlayerColor { get; private set; }

        public static Exception GetConnectionError()
        {
            if (!_connectedTask.IsFaulted) return null;
            return _connectedTask.Exception?.InnerException;
        }

        public static void Move(byte src, byte dst, ChessPieceType pawnPromotion)
        {
            var serverType = EngineMappers.fromEngineType(pawnPromotion);
            var command = new Command.MoveCommand(src, dst, serverType);
            _moveTask = MoveWorkflow(command);
        }

        private static async Task MoveWorkflow(Command.MoveCommand command)
        {
            try
            {
                var response = await _serverConnection.Move(command);
                if (response.IsErrorResponse)
                {
                    Debug.LogError("Error response");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Move error: " + e.Message);
                throw;
            }
        }

        public static event Action<Domain.MoveDescription> OnOpponentMove;
        public static event Action<Command.EndGameNotify> OnEndGame;
        public static event Action<Command.SessionCloseReason> OnSessionClosed;
        public static event Action Reconnecting;
        public static event Action Reconnected;
        public static event Action Closed;

        public static bool IsReconnecting;
        public static bool IsClosed;

        private static async Task ConnectWorkflow()
        {
            await _serverConnection.Connect(_connectionCts.Token);
            LastConnectionId = _serverConnection.GetConnectionId();
            var matchResponse = await _serverConnection.Match(new Command.MatchOptions(FSharpOption<string>.None));
            _matchFailed = matchResponse.IsErrorResponse;
        }

        private static void ChatNotification(string message)
        {
        }

        private static void MoveNotification(Domain.MoveDescription notification)
        {
            OnOpponentMove?.Invoke(notification);
        }

        private static void StartGameNotification(Command.SessionStartNotify notification)
        {
            PlayerColor = EngineMappers.toEngineColor.Invoke(notification.Color);
            Settings.InitialCameraSide = PlayerColor;
            _matched = true;
        }

        private static void EndGameNotification(Command.EndGameNotify notification)
        {
            OnEndGame?.Invoke(notification);
        }

        private static void SessionCloseNotification(Command.SessionCloseReason reason)
        {
            OnSessionClosed?.Invoke(reason);
        }

        private static async Task TryRestoreConnection()
        {
            if (LastConnectionId == null) return;
            var response = await _serverConnection.Restore(LastConnectionId);
            if (response.IsOkResponse)
            {
                Reconnected?.Invoke();
            }
            else
            {
                Debug.LogError("Restore connection failed");
                await _serverConnection.Close();
            }
        }
    }
}
