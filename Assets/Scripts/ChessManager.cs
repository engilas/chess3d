﻿using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Models;
using UnityEngine;
using ChessEngine.Engine;

public class ChessManager : MonoBehaviour
{

    private Board _board;

    private Piece _selectedPiece;
    public static Engine _engine = new Engine();
    private bool _playerLock = false;
    private bool _gameOver = false;
    private ChessPieceColor _playerColor = ChessPieceColor.White;
    private EngineMoveJob _engineMoveJob;
    private PlatformManager _platformManager;

    void Start()
    {
        _engine.NewGame();
        _engine.PlyDepthSearched = 4;
        var pieces = BoardLoader.FillBoard(_engine);
        _board = new Board(pieces);
        _engineMoveJob = new EngineMoveJob(_engine);
        _platformManager = new PlatformManager(_engine);
    }
    
    void DeselectPiece()
    {
        _platformManager.Clear();
        _selectedPiece?.Deselect();
        _selectedPiece = null;
    }

    void SelectPiece(Piece p)
    {
        DeselectPiece();
        _selectedPiece = p;
        _selectedPiece.Select();
        _platformManager.DrawValidMoves(p.Position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameOver && !_playerLock && Input.GetMouseButtonDown(0))
        {
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo)) 
            {
                DeselectPiece();
                return;
            }

            Piece enemyHit = null;

            if (hitInfo.transform.tag == "Piece")
            {
                var piece = hitInfo.transform.gameObject.GetComponent<Piece>();
                if (piece.Color == _playerColor)
                {
                    if (_selectedPiece == piece)
                    {
                        DeselectPiece();
                    }
                    else
                    {
                        SelectPiece(piece);
                    }
                    return;
                }

                if (_selectedPiece != null)
                    enemyHit = piece;
            }

            if (enemyHit != null || hitInfo.transform.tag == "Board")
            {
                if (_selectedPiece != null)
                {
                    var col = enemyHit?.Position.Col ?? Mathf.RoundToInt(hitInfo.point.x);
                    var row = enemyHit?.Position.Row ?? Mathf.RoundToInt(hitInfo.point.z);

                    var piecePos = _selectedPiece.Position;

                    if (_engine.IsValidMove((byte) piecePos.Col, (byte) piecePos.Row, (byte) col,
                            (byte) row)
                        &&
                        _engine.MovePiece((byte) piecePos.Col, (byte) piecePos.Row, (byte) col,
                            (byte) row))
                    {
                        _playerLock = true;
                        //ProcessMove();
                        _board.Move(_engine.LastMove);
                        if (CheckEndGame())
                        {
                            _gameOver = true;
                        }
                        else
                        {
                            StartCoroutine(EngineMove());
                        }
                    }
                    else
                    {
                        _playerLock = false;
                        Debug.Log("Invalid move");
                    }
                    //CheckEndGame();//todo нужно ли?
                }
            }
            DeselectPiece();
        }
    }

    IEnumerator EngineMove()
    {
        _engineMoveJob.Start();

        while (!_engineMoveJob.IsCompleted)
            yield return null;
        
        _board.Move(_engine.LastMove);

        if (CheckEndGame())
        {
            _gameOver = true;
        }
        else
        {
            _playerLock = false;
        }
    }

    bool CheckEndGame()
    {
        if (_engine.StaleMate)
        {
            if (_engine.InsufficientMaterial)
            {
                Debug.Log("1/2-1/2 {Draw by insufficient material}");
            }
            else if (_engine.RepeatedMove)
            {
                Debug.Log("1/2-1/2 {Draw by repetition}");
            }
            else if (_engine.FiftyMove)
            {
                Debug.Log("1/2-1/2 {Draw by fifty move rule}");
            }
            else
            {
                Debug.Log("1/2-1/2 {Stalemate}");
            }

            //_engine.NewGame();
            return true;
        }
        else if (_engine.GetWhiteMate())
        {
            Debug.Log("0-1 {Black mates}");
            //_engine.NewGame();
            return true;
        }
        else if (_engine.GetBlackMate())
        {
            Debug.Log("1-0 {White mates}");
            //_engine.NewGame();
            return true;
        }

        if (_engine.GetBlackCheck())
        {
            Debug.Log("Black check");
        }

        if (_engine.GetWhiteCheck())
        {
            Debug.Log("White check");
        }

        return false;
    }
}
