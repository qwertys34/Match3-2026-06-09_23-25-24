using System.Collections.Generic;
using ResurcesLoading;
using TMPro;
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
        private GameResurcesLoader _gameResurcesLoader;

        public FXPool(GameResurcesLoader gameResurcesLoader,
            IObjectResolver objectResolver)
        {
            _gameResurcesLoader = gameResurcesLoader;
            _objectResolver = objectResolver;
        }

        public GameObject GetFX(Vector3 position, Transform parent, int amountScore)
        {
            for(int i=0; i < _items.Count; i++)
            {
                if (_items[i].activeInHierarchy) continue;

                _items[i].transform.position = position;
                _items[i].GetComponent<FXMatchTile>().amountText.text = amountScore.ToString();
                _items[i].SetActive(true);
                return _items[i];
            }
            
            return CreateNewFX(position, parent, amountScore);
        }

        private GameObject CreateNewFX(Vector3 position,  Transform parent, int amountScore)
        {
            var FX = _objectResolver.Instantiate(_gameResurcesLoader.FXRemoveTilePrefab.gameObject, 
                position, quaternion.identity, parent);
            _items.Add(FX.gameObject);
            FX.GetComponent<FXMatchTile>().amountText.text = amountScore.ToString();
            return FX.gameObject;
        }
    }
}