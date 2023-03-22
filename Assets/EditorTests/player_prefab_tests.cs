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
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject root = (GameObject)AssetDatabase.LoadMainAssetAtPath(path);
                if(root.name.Contains("Player") == false) { continue; }
                if(root.name.Contains("Prefab") == false) { continue; }
                yield return root;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<GameObject>)this).GetEnumerator();
    }

    [TestFixture]
    [TestFixtureSource(typeof(PlayerPrefabProvider))]
    public class player_prefab_tests
    {
        private readonly GameObject _playerRootPrefab;
        private readonly GameObject _playerPrefab;

        public player_prefab_tests(GameObject playerRootPrefab)
        {
            _playerRootPrefab = playerRootPrefab;
            _playerPrefab = playerRootPrefab.GetComponentInChildren<PlayerInputHandler>().gameObject;
        }

        #region PlayerObjectPrefab Tests

        [Test]
        public void check_if_player_root_has_DontDestroyOnLoad()
        {
            Assert.IsTrue(_playerRootPrefab.GetComponent<DontDestroyOnLoad>() != null, "Did not find dontDestroyOnLoadScript") ;
        }

        [Test]
        public void check_if_player_root_has_PlayerCamera_script()
        {
            bool hasScript = _playerRootPrefab.GetComponentInChildren<PlayerCamera>(true) != null;

            Assert.IsTrue(hasScript, "Did not find the script");
        }

        [Test]
        public void check_if_player_root_has_camera()
        {
            bool hasScript = _playerRootPrefab.GetComponentInChildren<Camera>(true) != null;

            Assert.IsTrue(hasScript, "Did not find the script");
        }

        [Test]
        public void check_if_player_root_camera_is_perspective()
        {
            Camera camera = _playerRootPrefab.GetComponentInChildren<Camera>(true);

            Assert.IsTrue(camera.orthographic == false, "Camera is not perspective");
        }

        [Test]
        public void check_if_player_root_camera_has_playerCamera_layer()
        {
            PlayerCamera v_camera = _playerRootPrefab.GetComponentInChildren<PlayerCamera>(true);

            //28 being the Player1Cam layer
            bool isCorrectLayer = v_camera.gameObject.layer == 28;

            Assert.IsTrue(isCorrectLayer, "CM vcam Player is not on correct layer");
        }

        #endregion

        #region Player Tests


        [Test]
        public void check_if_player_is_on_player_layer()
        {
            Assert.IsTrue(_playerPrefab.layer == LayerMask.NameToLayer("Player"), "Player object isn't on correct layer");
        }

        [Test]
        public void check_if_player_has_player_tag()
        {
            Assert.IsTrue(_playerPrefab.tag == "Player", "Player object doesn't have correct tag");
        }

        #endregion
    }
}

