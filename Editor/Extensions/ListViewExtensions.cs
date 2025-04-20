using UnityEngine.UIElements;

namespace Parameters.Editor.Extensions
{
    public static class ListViewExtensions
    {
        public static T ReadOnly<T>(this T element)
            where T : ListView
        {
            element.reorderable = false;
            element.allowAdd = false;
            element.allowRemove = false;
            element.enabledSelf = false;

            return element;
        }      
        
        public static T HideSize<T>(this T element)
            where T : ListView
        {
            element.showBoundCollectionSize = false;
            return element;
        }
    }
}