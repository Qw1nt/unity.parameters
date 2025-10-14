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

        public static T Editable<T>(this T element)
            where T : ListView
        {
            element.reorderable = true;
            element.allowAdd = true;
            element.allowRemove = true;
            element.enabledSelf = true;

            return element;
        }

        public static T SelectionType<T>(this T element, SelectionType value)
            where T : ListView
        {
            element.selectionType = value;
            return element;
        }

        public static T HideSizeCounter<T>(this T element)
            where T : ListView
        {
            element.showBoundCollectionSize = false;
            return element;
        }

        public static T FixedItemHeight<T>(this T element, float value)
            where T : ListView
        {
            element.fixedItemHeight = value;
            return element;
        }
        
        public static T VirtualizationMethod<T>(this T element, CollectionVirtualizationMethod value)
            where T : ListView
        {
            element.virtualizationMethod = value;
            return element;
        }
    }
}