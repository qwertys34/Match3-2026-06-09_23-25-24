using System.Collections.Generic;
using System.Linq;
using Animations;
using Cysharp.Threading.Tasks;
using Game.Score;
using Game.Tiles;
using ResurcesLoading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.UI
{
    public class GameProgressView : MonoBehaviour
    {
        [SerializeField] private TMP_Text movesText;
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private GameObject progressUIPanel;
        
        [SerializeField] private Transform parent;
        private GameProgress _gameProgress;
        private IAnimation _animation;
        private GameResurcesLoader _gameResurcesLoader;
        private List<GameObject> _listTiles = new();
        private List<GameObject> _stashListTiles = new();
        [SerializeField] private List<Image> starsImageList;
        
        [Inject] public void Construct(GameProgress gameProgress, IAnimation animation,
            GameResurcesLoader gameResurcesLoader)
        {
            _gameProgress = gameProgress;
            _animation = animation;
            _gameResurcesLoader = gameResurcesLoader;
        }

        private async UniTask OnLoadCompleted()
        {
            foreach(var starImage in starsImageList)
                starImage.gameObject.SetActive(false);
            progressUIPanel.SetActive(true);
            await _animation.Reveal(progressUIPanel.gameObject, 0.8f);
            movesText.text = _gameProgress.Moves.ToString();
            
            await CreateGoalTiles();
        }
        
        private async UniTask ShowNewStar()
        {
            foreach (var starImage in starsImageList)
            {
                if (!starImage.isActiveAndEnabled)
                {
                    starImage.gameObject.SetActive(true);
                    await _animation.RevealUI(starImage.gameObject, 0.8f, starImage.transform.localScale);
                    break;
                }
            }
        }
        
        private async UniTask UpdateGoalTilesToRemove(TileKind tileKind)
        {
            if (tileKind == TileKind.Blank)
            {
                var tile = _listTiles.FirstOrDefault(tile
                    => tile.GetComponent<Image>().sprite.name == "BlankTileSpriteOne");
                await _animation.HideTileUI(tile);
                _listTiles.Remove(tile);
                Destroy(tile);
            }
            else if (tileKind == TileKind.Jelly)
            {
                var tile = _listTiles.FirstOrDefault(tile
                                    => tile.GetComponent<Image>().sprite.name == "JellyTileSpriteOne");
                await _animation.HideTileUI(tile);
                _listTiles.Remove(tile);
                Destroy(tile);
            }
            
            var activeTiles = _listTiles.FindAll(tile => tile.activeInHierarchy);
            var newTile = _listTiles.FirstOrDefault(newTile
                => !newTile.activeInHierarchy);
            if (newTile == true && activeTiles.Count < 5)
            {
                newTile.SetActive(true);
                await _animation.RevealUI(newTile.gameObject, 0.5f, Vector3.one);
            }
            
            var nextTile = _listTiles.FirstOrDefault(nextTile
                => !nextTile.activeInHierarchy);
            if (nextTile == false) numberText.gameObject.SetActive(false);
            else
            {
                numberText.transform.SetSiblingIndex(5);
                await _animation.RevealUI(numberText.gameObject, 0.5f, Vector3.one * 1.57f);
            }
        }

        private void DrawNumber()
        {
            numberText.transform.SetParent(parent);
            numberText.gameObject.SetActive(true);
        }
        
        private async UniTask CreateGoalTiles()
        {
            // Очищаем существующие тайлы
            foreach (var tile in _listTiles)
            {
                Destroy(tile);
            }
            foreach (var tile in _stashListTiles)
            {
                Destroy(tile);
            }
            _listTiles.Clear();
            _stashListTiles.Clear();

            var blankAmount = _gameProgress.CurrentAmountBlank;
            var jellyAmount = _gameProgress.CurrentAmountJelly;

            bool moreThanFive = false;
            for (int i = 0; i < blankAmount; i++)
            {
                var instance = new GameObject("BlankTile", typeof(RectTransform));
                instance.transform.SetParent(parent, false);
                var image = instance.AddComponent<Image>();
                image.sprite = await _gameResurcesLoader.CreateBlankSprite(1);
                image.preserveAspect = true; // Сохраняем пропорции спрайта
        
                _listTiles.Add(instance); 
                
                if (i == 4)
                {
                    DrawNumber();
                    moreThanFive = true;
                }
                else if (i > 4) instance.gameObject.SetActive(false);
            }
            for (int i = 0; i < jellyAmount; i++)
            {
                var instance = new GameObject("JellyTile", typeof(RectTransform));
                instance.transform.SetParent(parent, false);
                var image = instance.AddComponent<Image>();
                image.sprite = await _gameResurcesLoader.CreateJellySprite(1);
                image.preserveAspect = true;
        
                _listTiles.Add(instance);
                
                if (i == 4 && !numberText.gameObject.activeSelf)
                {
                    DrawNumber();
                    moreThanFive = true;
                }
                if (i > 4 || moreThanFive) instance.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _gameProgress.OnMove += UpdateMoves;
            _gameProgress.AmountTilesChanged += UpdateGoalTilesToRemove;
            _gameProgress.OnNewGoalToStar += ShowNewStar;
            _gameResurcesLoader.LoadComplete += OnLoadCompleted;
        }

        private void OnDisable()
        {
            _gameProgress.OnMove -= UpdateMoves;
            _gameProgress.AmountTilesChanged -= UpdateGoalTilesToRemove;
            _gameProgress.OnNewGoalToStar -= ShowNewStar;
            _gameResurcesLoader.LoadComplete -= OnLoadCompleted;
        }
        
        private void UpdateMoves()
        {
            movesText.text = _gameProgress.Moves.ToString();
            AnimateText(movesText.gameObject);
        }

        private void AnimateText(GameObject obj) =>
            _animation.DoPunchAnimate(obj, Vector3.one * 0.3f, 0.3f);
    }
}