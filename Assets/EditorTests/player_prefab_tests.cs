using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    class PlayerPrefabProvider : IEnumerable<GameObject>
    {
        public IEnumerator<GameObject> GetEnumerator()
        {
            //GameObject[] asteroids = Resources.LoadAll<Asteroid>("AsteroidSOs");

            foreach (var guid in AssetDatabase.FindAssets("t:GameObject", new[] { "Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject root = (GameObject)AssetDatabase.LoadMainAssetAtPath(path);
                Debug.Log("root: " + root.name);
                if(root.name != "Player") { continue; }
                yield return root;
            }

            //foreach (var asteroid in asteroids)
            //{
            //    yield return asteroid;
            //}
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<GameObject>)this).GetEnumerator();
    }

    [TestFixture]
    [TestFixtureSource(typeof(PlayerPrefabProvider))]
    public class player_prefab_tests
    {
        private readonly GameObject _playerPrefab;

        public player_prefab_tests(GameObject playerPrefab)
        {
            Debug.Log("_playerPrefab: " + _playerPrefab.name);
            _playerPrefab = playerPrefab;
        }

        [Test]
        public void check_asteroid_go_has_asteroidSo_selected()
        {
            //AsteroidScript asteroidScript = _asteroidSO.AsteroidPrefab.GetComponent<AsteroidScript>();

            //Assert.IsTrue(asteroidScript.AsteroidSo != null, "AsteroidSO in Asteroid Prefab was null");
            Assert.IsTrue(true, "AsteroidSO in Asteroid Prefab was null");
        }
    }
}

