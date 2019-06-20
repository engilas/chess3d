using System;
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

    private Piece[] _allPieces;

    [SerializeField] private UIManager uiManager;

    void Start()
    {
        StartGame();
        _engineMoveJob = new EngineMoveJob(_engine);
        _platformManager = new PlatformManager(_engine);
        uiManager.OnRestartClick += () => Restart();

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
        if (uiManager.IsMenuActive) return;

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
                        _board.Move(_engine.LastMove);
                        if (!CheckEndGame())
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
        yield return null;
        _engineMoveJob.Start();

        while (!_engineMoveJob.IsCompleted)
            yield return null;
        
        _board.Move(_engine.LastMove);

        CheckEndGame();
        _playerLock = false;
    }

    bool CheckEndGame()
    {
        string reason = null;
        if (_engine.StaleMate)
        {
            if (_engine.InsufficientMaterial)
            {
                reason = "Draw by insufficient material";
            }
            else if (_engine.RepeatedMove)
            {
                reason = "Draw by repetition";
            }
            else if (_engine.FiftyMove)
            {
                reason = "Draw by fifty move rule";
            }
            else
            {
                reason = "Stalemate";
            }
        }
        else if (_engine.GetWhiteMate())
        {
            reason = "Black mates";
        }
        else if (_engine.GetBlackMate())
        {
            reason = "White mates";
        }

        if (_engine.GetBlackCheck())
        {
            Debug.Log("Black check");
        }

        if (_engine.GetWhiteCheck())
        {
            Debug.Log("White check");
        }

        if (reason != null)
        {
            _gameOver = true;
            uiManager.ShowGameOver(reason);
            return true;
        }
        return false;
    }

    private void StartGame()
    {
        _engine.NewGame();
        _engine.PlyDepthSearched = (byte) Settings.Difficulty;
        _allPieces = BoardLoader.FillBoard(_engine);
        _board = new Board(_allPieces);
    }

    private void Restart()
    {
        foreach (var p in _allPieces)
            Destroy(p.gameObject);
        _gameOver = false;
        _playerLock = false;
        StartGame();
    }
}
