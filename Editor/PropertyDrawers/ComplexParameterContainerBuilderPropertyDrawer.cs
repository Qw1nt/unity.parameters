using System.Collections.Generic;
using Parameters.Editor.Controls;
using Parameters.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace Parameters.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ComplexParameterContainerBuilder))]
    internal class ComplexParameterContainerBuilderPropertyDrawer : PropertyDrawer
    {
        private static Dictionary<SerializedProperty, ComplexParameterContainerBuilderControl> _cacheMap = new();
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new ComplexParameterContainerBuilderControl().Bind(property);
            if (_cacheMap.ContainsKey(property) == true)
                return _cacheMap[property];
            
            // _cacheMap[property] = new ComplexParameterContainerBuilderControl().Bind(property);
            // return _cacheMap[property];
        }
    }
}