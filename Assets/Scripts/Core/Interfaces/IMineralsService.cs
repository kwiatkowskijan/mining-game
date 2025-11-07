using System;
using System.Collections.Generic;
using MiningGame.WorldGeneration;

namespace MiningGame.Core.Interfaces
{
    public interface IMineralsService
    {
        event Action<Mineral> OnMineralDiscovered;
        IReadOnlyList<Mineral> Minerals { get; }
        void DiscoverMineral(Mineral mineral);
    
    }
}