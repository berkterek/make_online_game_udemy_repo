using System.Collections.Generic;
using UnityEngine;

namespace MakeOnlineGame.Controllers
{
    public class SpawnPointController : MonoBehaviour
    {
        static readonly List<SpawnPointController> _spawnPoints = new List<SpawnPointController>();

        void OnEnable()
        {
            _spawnPoints.Add(this);
        }

        void OnDisable()
        {
            _spawnPoints.Remove(this);
        }

        public static Vector3 GetRandomSpawnPosition()
        {
            if (_spawnPoints.Count == 0) return default;
            
            return _spawnPoints[Random.Range(0,_spawnPoints.Count)].transform.position;
        }
    }
}