using Game.Board;
using Game.EntryPoint;
using Game.GridSystem;
using Game.MatchTiles;
using Game.Score;
using Game.Tiles;
using Game.UI;
using Game.Utils;
using ResurcesLoading;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Grid = Game.GridSystem.Grid;

namespace DI
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private GameBoard gameBoard;
        [SerializeField] private EndGamePanelView endGamePanelView;
        [SerializeField] private GameProgressView gameProgressView;
        [SerializeField] private SetupGameUI setupGameUI;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameEntryPoint>();
            
            builder.RegisterInstance(gameBoard);    
            builder.RegisterInstance(endGamePanelView);  
            builder.RegisterInstance(gameProgressView);
            builder.RegisterInstance(setupGameUI);
            
            builder.Register<Grid>(Lifetime.Singleton);
            builder.Register<SetupCamera>(Lifetime.Singleton);
            builder.Register<TilePool>(Lifetime.Singleton);
            builder.Register<GameDebug>(Lifetime.Singleton);
            builder.Register<MatchFinder>(Lifetime.Singleton);
            builder.Register<GameProgress>(Lifetime.Singleton);
            builder.Register<ScoreCalculator>(Lifetime.Singleton);
            builder.Register<EndGame>(Lifetime.Singleton);
            builder.Register<BackgroundTilesSetup>(Lifetime.Singleton);
            builder.Register<FXPool>(Lifetime.Singleton);
            builder.Register<GameResurcesLoader>(Lifetime.Singleton);
            
        }
    }
}