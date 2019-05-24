using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using ChessEngine;
using ChessEngine.Engine;

public class ChessManager : MonoBehaviour {

	// Use this for initialization
    private Board _board = new Board();

    private PieceComponent _selectedPiece;
    private Engine _engine = new Engine();
    private bool _playerLock = false;
    private bool _gameOver = false;
    private PieceColor _playerColor = PieceColor.White;

	void Start ()
    {
        //_engine.
        _engine.NewGame();
        _engine.PlyDepthSearched = 1;
        var pieces = BoardLoader.FillBoard(_board);
        foreach (var piece in pieces)
        {
            var path = string.Format("Assets/Prefabs/{0}{1}.prefab", piece.Type.ToString(),
                piece.Color == PieceColor.Black ? "Dark" : "Light");
            var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(PieceComponent));
            var pieceComp = Instantiate(prefab) as PieceComponent;
            pieceComp.SetPiece(piece);
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (!_gameOver && !_playerLock && Input.GetMouseButtonDown(0))
        {
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo)) return;

            Piece enemyHit = null;

            if (hitInfo.transform.tag == "Piece")
            {
                var piece = hitInfo.transform.gameObject.GetComponent<PieceComponent>();
                if (piece.Piece.Color == _playerColor)
                {
                    _selectedPiece = piece;
                    return;
                }
                if (_selectedPiece != null)
                    enemyHit = piece.Piece;
            }
            if (enemyHit != null || hitInfo.transform.tag == "Board")
            {
                if (_selectedPiece != null)
                {
                    var col = enemyHit?.Position.Col ?? Mathf.RoundToInt(hitInfo.point.x);
                    var row = enemyHit?.Position.Row ?? Mathf.RoundToInt(hitInfo.point.z);

                    var piecePos = _selectedPiece.Piece.Position;

                    if (_engine.IsValidMove((byte) piecePos.Col, (byte) (7 - piecePos.Row), (byte) col,
                        (byte) (7 - row))
                        &&
                        _engine.MovePiece((byte) piecePos.Col, (byte) (7 - piecePos.Row), (byte) col,
                        (byte) (7 - row)))
                    {
                        _playerLock = true;
                        ProcessMove();
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
                    _selectedPiece = null;
                    
                    //CheckEndGame();
                }
            }
            else
            {
                _selectedPiece = null;
            }


            //var board =
            //if (hitInfo.transform.gameObject.GetComponent<PieceComponent>() )
        } 
	}

    void ProcessMove()
    {
        var lastMove = _engine.GetMoveHistory().ToArray()[0];
        if (lastMove.TakenPiece.PieceType != ChessPieceType.None)
        {
            _board.Destroy(GetPosition(lastMove.TakenPiece.Position));
        }
        _board.Move(GetPosition(lastMove.MovingPiecePrimary.SrcPosition), GetPosition(lastMove.MovingPiecePrimary.DstPosition));
        if (lastMove.MovingPieceSecondary.PieceType != ChessPieceType.None)
        {
            _board.Move(GetPosition(lastMove.MovingPieceSecondary.SrcPosition), GetPosition(lastMove.MovingPieceSecondary.DstPosition));
        }
    }

    IEnumerator EngineMove()
    {
        //DateTime start = DateTime.Now;

        yield return null;
        _engine.AiPonderMove();
        if (_engine.Thinking)
            throw new Exception("Still thinking");

        ProcessMove();

        if (CheckEndGame())
        {
            _gameOver = true;
        }
        else
        {
            _playerLock = false;
        }
    }

    Position GetPosition(byte enginePos)
    {
        return new Position(enginePos % 8, 7 - enginePos / 8);
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
