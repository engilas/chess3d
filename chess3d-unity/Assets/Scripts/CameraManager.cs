using ChessEngine.Engine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //board symmetry axis
    private const float BoardCenter = 3.5f;
    private const float StartZPos = 7;
    private const float StartYPos = 7.1f;

    private ChessPieceColor _side;
    private bool _frontView;

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void InitCamera()
    {
        SetDefault();
    }

    public void MoveOtherSide()
    {
        ToggleSide();
        SetDefault();

        //re-activate front view
        ToggleFrontView();
        ToggleFrontView();
    }

    public void ToggleFrontView()
    {
        _frontView = !_frontView;
        Settings.FrontView = _frontView;

        if (_frontView)
        {
            var offset = _side == ChessPieceColor.White ? 1e-3f : -1e-3f;
            transform.position = new Vector3(BoardCenter, transform.position.y, BoardCenter + offset);
            transform.LookAt(new Vector3(BoardCenter, 0, BoardCenter));
            _camera.fieldOfView = 70;
        }
        else
        {
            _camera.fieldOfView = 60;
            SetDefault();
        }
    }

    private void SetDefault()
    {
        var pos = new Vector3(BoardCenter, StartYPos, GetZ());
        transform.position = pos;
        transform.LookAt(new Vector3(BoardCenter, 0, Mathf.Abs(pos.z - 3f)));
    }

    private void ToggleSide()
    {
        if (_side == ChessPieceColor.Black) _side = ChessPieceColor.White;
        else _side = ChessPieceColor.Black;
    }

    private float GetZ()
    {
        if (_side == ChessPieceColor.White) return StartZPos;
        else return -StartZPos + 2 * BoardCenter;
    }
}
