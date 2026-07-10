using System.Collections.Generic;
using ResurcesLoading;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Utils
{
    public class FXPool
    {
        private readonly List<GameObject> _items = new();
        private IObjectResolver _objectResolver;
        private GameObject _fxPrefab;
        private GameResurcesLoader _gameResurcesLoader;

        public FXPool(GameResurcesLoader gameResurcesLoader,
            IObjectResolver objectResolver)
        {
            _gameResurcesLoader = gameResurcesLoader;
            _objectResolver = objectResolver;
        }

        public GameObject GetFX(Vector3 position, Transform parent)
        {
            for(int i=0; i < _items.Count; i++)
            {
                if (_items[i].activeInHierarchy) continue;

                _items[i].transform.position = position;
                _items[i].SetActive(true);
                return _items[i];
            }
            
            return CreateNewFX(position, parent);
        }

        private GameObject CreateNewFX(Vector3 position,  Transform parent)
        {
            var FX = _objectResolver.Instantiate(_gameResurcesLoader.FXRemoveTilePrefab, 
                position, quaternion.identity, parent);
            _items.Add(FX);
            return FX;
        }
    }
}