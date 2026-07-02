using Animations;
using Game.Board;
using Game.GridSystem;
using Game.MatchTiles;
using Game.Tiles;
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
        [SerializeField] private GameResurcesLoader loader;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameBoard);    
            builder.RegisterInstance(loader);    
            builder.Register<Grid>(Lifetime.Singleton);
            builder.Register<IAnimation, AnimationManager>(Lifetime.Singleton);
            builder.Register<BlankTilesSetup>(Lifetime.Singleton);
            builder.Register<SetupCamera>(Lifetime.Singleton);
            builder.Register<TilePool>(Lifetime.Singleton);
            builder.Register<GameDebug>(Lifetime.Singleton);
            builder.Register<MatchFinder>(Lifetime.Singleton);
        }
    }
}