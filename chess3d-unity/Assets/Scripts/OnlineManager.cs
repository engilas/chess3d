using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Types;
using UnityEngine;

public class OnlineManager
{
    private static ChessConnection.ServerConnection _serverConnection;
    private static Task _connectedTask;
    private static bool _matched;

    public static void Connect()
    {
        var handler = ChessConnection.notificationHandlerAdapter(x => { }, move => { }, x => { }, x =>
            {
                _matched = true;
            }, x => { });
        _serverConnection =
            new ChessConnection.ServerConnection(new Uri("ws://localhost:1313/ws"), e => { }, () => { }, handler);

        _connectedTask = ConnectWorkflow();
    }

    private static async Task ConnectWorkflow()
    {
        await _serverConnection.Connect();
        await _serverConnection.Match(new Command.MatchOptions(FSharpOption<string>.None), CancellationToken.None);
    }

    public static bool IsConnected() => _matched;
    public static bool IsFailed() => _connectedTask.IsFaulted;

    public static Exception GetError()
    {
        if (!_connectedTask.IsFaulted) return null;
        return _connectedTask.Exception?.InnerException;
    }
}
