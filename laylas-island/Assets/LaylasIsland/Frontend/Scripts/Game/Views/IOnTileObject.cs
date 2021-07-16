namespace LaylasIsland.Frontend.Game.Views
{
    public interface IOnTileObject
    {
        bool HasTile { get; }

        Tile Tile { get; }
    }
}
