using System;
using System.Threading;
using System.Threading.Tasks;
using ChessEngine.Engine;
using Microsoft.FSharp.Core;
using Types;
using UnityEngine;

public class OnlineManager
{
    private static ChessConnection.ServerConnection _serverConnection;
    private static Task _connectedTask;
    private static Task _moveTask;
    private static bool _matched;
    private static bool _matchFailed;
    private static CancellationTokenSource _connectionCts;

    public static void Connect()
    {
        var handler = ChessConnection.notificationHandlerAdapter(ChatNotification, MoveNotification, EndGameNotification, StartGameNotification, SessionCloseNotification);
        var ip = Settings.ServerIp.TrimEnd('/');
        _serverConnection =
            new ChessConnection.ServerConnection($"{ip}/command", handler);

        _connectionCts = new CancellationTokenSource();
        _connectedTask = ConnectWorkflow();
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

    private static async Task ConnectWorkflow()
    {
        await _serverConnection.Connect(_connectionCts.Token);
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
        _matched = true;
        PlayerColor = EngineMappers.toEngineColor.Invoke(notification.Color);
        Settings.InitialCameraSide = PlayerColor;
    }

    private static void EndGameNotification(Command.EndGameNotify notification)
    {
        OnEndGame?.Invoke(notification);
    }

    private static void SessionCloseNotification(Command.SessionCloseReason reason)
    {
        OnSessionClosed?.Invoke(reason);
    }
}
