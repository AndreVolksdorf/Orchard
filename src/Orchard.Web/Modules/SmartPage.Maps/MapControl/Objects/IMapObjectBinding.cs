namespace SmartPage.Maps.MapControl.Objects {
    public interface IMapObjectBinding<in TMapObject>
        where TMapObject : MapObject
    {
        void ItemDataBound(TMapObject item, object value);
    }
}