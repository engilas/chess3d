using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Models;
using ChessEngine.Engine;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public class PlatformManager
    {
        private readonly Engine _engine;
        private readonly List<GameObject> _platforms = new List<GameObject>();
        private readonly Object _platformPrefab;

        public PlatformManager(Engine engine)
        {
            _engine = engine;

            _platformPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/platform.prefab", typeof(GameObject));
        }

        public void DrawValidMoves(Position pos)
        {
            DrawValidMoves((byte) pos.Col, (byte) pos.Row);
        }

        public void DrawValidMoves(byte col, byte row)
        {
            Clear();

            var validMoves = _engine.GetValidMoves(col, row);

            var path = $"Assets/Prefabs/platform.prefab";

            for (int i = 0; i < validMoves.Length; i++)
            {
                var moveCol = validMoves[i][0];
                var moveRow = validMoves[i][1];

                var platform = (GameObject) Object.Instantiate(_platformPrefab);
                platform.transform.position += new Vector3(moveCol, 0, moveRow);
                _platforms.Add(platform);
            }
        }

        public void Clear()
        {
            foreach (var gameObject in _platforms)
            {
                Object.Destroy(gameObject);
            }
            _platforms.Clear();
        }
    }
}
