using System.Collections;
using ChessEngine.Engine;
using UnityEngine;

namespace Assets.Scripts
{
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
            _side = Settings.InitialCameraSide;
            _camera = GetComponent<Camera>();
        }

        public void InitCamera()
        {
            SetDefault(smooth: false);
            if (Settings.FrontView)
                ToggleFrontView(false);
        }

        public void MoveOtherSide()
        {
            ToggleSide();
            if (!_frontView)
            {
                SetDefault(smooth: true);
            }
            else
            {
                ChangeCameraSide(new Vector3(BoardCenter, 0, BoardCenter), smooth: true);
            }
        }

        public void ToggleFrontView()
        {
            ToggleFrontView(true);
        }

        private void ToggleFrontView(bool smooth)
        {
            _frontView = !_frontView;
            Settings.FrontView = _frontView;

            if (_frontView)
            {
                var offset = _side == ChessPieceColor.White ? 1e-1f : -1e-1f;
                var pos = new Vector3(BoardCenter, transform.position.y, BoardCenter + offset);
                var lookAt = new Vector3(BoardCenter, 0, BoardCenter);

                MoveCamera(pos, lookAt, smooth, slerp: false, fov: 70);
            }
            else
            {
                SetDefault(smooth, slerp: false, startRotateAfter: 0.15f);
            }
        }

        private void SetDefault(bool smooth = false, bool slerp = true, float speed = 1, float startRotateAfter = 0)
        {
            var pos = new Vector3(BoardCenter, StartYPos, GetZ());
            var lookAt = new Vector3(BoardCenter, 0, Mathf.Abs(pos.z - 3f));
            MoveCamera(pos, lookAt, smooth, slerp, speed, startRotateAfter: startRotateAfter, fov: 60);
        }

        //mirror z pos
        private void ChangeCameraSide(Vector3 lookAt, bool smooth = false, float speed = 1)
        {
            var pos = new Vector3(BoardCenter, StartYPos, - transform.position.z + 2 * BoardCenter);
            MoveCamera(pos, lookAt, smooth, slerp: true, speed: speed);
        }

        private void MoveCamera(Vector3 pos, Vector3 lookAt, bool smooth = false, bool slerp = false, float speed = 1, float startRotateAfter = 0, float? fov = null)
        {
            if (smooth)
            {
                StartCoroutine(CameraTransition(speed, pos, lookAt, slerp, fov ?? _camera.fieldOfView, startRotateAfter));
            }
            else
            {
                transform.position = pos;
                transform.LookAt(lookAt);
                if (fov != null)
                    _camera.fieldOfView = fov.Value;
            }
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

        private IEnumerator CameraTransition(float lerpSpeed, Vector3 newPosition, Vector3 lookAt, bool slerpPosition = true, float fov = 60, float startRotateAfter = 0)
        {
            if (PlayerLock.CameraLock) yield break;

            PlayerLock.CameraLock = true;

            var t = 0.0f;
            var startingPos = transform.position;
            var startingFov = _camera.fieldOfView;

            Vector3 center = (startingPos + newPosition) * 0.5F;
            // move the center a bit downwards to make the arc vertical
            //center -= new Vector3(0, 1, 0);
            Vector3 riseRelCenter = startingPos - center;
            Vector3 setRelCenter = newPosition - center;

            while (t < 1.0f)
            {
                t += Time.deltaTime * (Time.timeScale / lerpSpeed);
                if (slerpPosition)
                {
                    transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, t);
                    transform.position += center;
                }
                else
                {
                    transform.position = Vector3.Lerp(startingPos, newPosition, t);
                }
            
                var relativePos = lookAt - transform.position;
                if (t >= startRotateAfter)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos), t);
                _camera.fieldOfView = Mathf.Lerp(startingFov, fov, t);
                yield return 0;
            }

            PlayerLock.CameraLock = false;
        }
    }
}
