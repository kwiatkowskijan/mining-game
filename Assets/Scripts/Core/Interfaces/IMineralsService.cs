using System;
using System.Collections.Generic;
using MiningGame.WorldGeneration;

namespace MiningGame.Core.Interfaces
{
    public interface IMineralsService
    {
        IReadOnlyList<Mineral> Minerals { get; }
        event Action<Mineral> OnMineralDiscovered;
        void DiscoverMineral(Mineral mineral);

    }
}