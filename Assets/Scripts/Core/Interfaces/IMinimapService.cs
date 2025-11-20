namespace MiningGame.Core.Interfaces
{
    public interface IMinimapService
    {
        float CurrentZoom { get; }
        float MinZoom { get; }
        float MaxZoom { get; }

        void ZoomIn(float amoount);
        void ZoomOut(float amoount);
    }
}