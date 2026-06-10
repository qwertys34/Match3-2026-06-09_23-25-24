using Game.Board;
using Game.Utils;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Grid = Game.GridSystem.Grid;

namespace DI
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private GameBoard gameBoard;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Grid>(Lifetime.Singleton);
            builder.RegisterInstance(gameBoard);
            builder.Register<SetupCamera>(Lifetime.Singleton);
        }
    }
}