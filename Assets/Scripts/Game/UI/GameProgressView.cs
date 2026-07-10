using System;
using Animations;
using Game.Score;
using TMPro;
using UnityEngine;
using VContainer;

namespace Game.UI
{
    public class GameProgressView : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text goalText;
        [SerializeField] private TMP_Text movesText;
        
        private GameProgress _gameProgress;
        private IAnimation _animation;

        private void Start()
        {
            scoreText.text = _gameProgress.Score.ToString();
            goalText.text = _gameProgress.GoalScore.ToString();
            movesText.text = _gameProgress.Moves.ToString();
        }

        private void OnEnable()
        {
            _gameProgress.OnScoreChanged += UpdateScore;
            _gameProgress.OnMove += UpdateMoves;
        }

        private void OnDisable()
        {
            _gameProgress.OnScoreChanged -= UpdateScore;
            _gameProgress.OnMove -= UpdateMoves;
        }

        private void UpdateScore()
        {
            scoreText.text = _gameProgress.Score.ToString();
            AnimateText(scoreText.gameObject);
        }
        
        private void UpdateMoves()
        {
            movesText.text = _gameProgress.Moves.ToString();
            AnimateText(movesText.gameObject);
        }

        private void AnimateText(GameObject obj) =>
            _animation.DoPunchAnimate(obj, Vector3.one * 0.3f, 0.3f);

        [Inject] public void Construct(GameProgress gameProgress, IAnimation animation)
        {
            _gameProgress = gameProgress;
            _animation = animation;
        }
    }
}