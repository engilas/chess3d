using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Models;
using UnityEngine;
using ChessEngine.Engine;

public class ChessManager : MonoBehaviour
{
    private Piece _selectedPiece;
    public static Engine _engine = new Engine();
    
    private PlatformManager _platformManager;
    private ChessState _chessState;
    private Piece[] _allPieces;
    private IChessStrategy _chessStrategy;

    [SerializeField] private UIManager uiManager;

    void Start()
    {
        _chessState = new ChessState { Engine = _engine, MonoBehaviour = this };
        _chessStrategy = ChessStrategyFactory.ResolveStrategy();
        _chessStrategy.Init(_chessState);

        StartGame();
        _platformManager = new PlatformManager(_engine);
        uiManager.OnRestartClick += () => Restart();

        FindObjectOfType<CameraManager>().InitCamera();
    }

    void OnDestroy()
    {
        _chessStrategy.OnDestroy();
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
        //if (uiManager.IsMenuActive) return;

        if (!PlayerLock.IsLocked && Input.GetMouseButtonDown(0))
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
                if (piece.Color == _chessState.PlayerColor)
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
                    var col = enemyHit?.Position.Col ?? Mathf.RoundToInt(7 - hitInfo.point.x);
                    var row = enemyHit?.Position.Row ?? Mathf.RoundToInt(hitInfo.point.z);

                    var piecePos = _selectedPiece.Position;

                    if (_engine.IsValidMove((byte) piecePos.Col, (byte) piecePos.Row, (byte) col,
                            (byte) row)
                        &&
                        _engine.MovePiece((byte) piecePos.Col, (byte) piecePos.Row, (byte) col,
                            (byte) row))
                    {
                        _chessState.Board.Move(_engine.LastMove);
                        if (!CheckEndGame())
                        {
                            _chessStrategy.Move();
                        }
                    }
                    else
                    {
                        Debug.Log("Invalid move");
                    }
                    CheckEndGame();//todo нужно ли?
                }
            }
            DeselectPiece();
        }
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
            //_chessState.GameOver = true;
            PlayerLock.GameLock = true;
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
        _chessState.Board = new Board(_allPieces);
    }

    private void Restart()
    {
        DeselectPiece();
        _chessStrategy.StopGame();
        foreach (var p in _allPieces)
            Destroy(p.gameObject);
        PlayerLock.GameLock = false;
        StartGame();
    }
}
