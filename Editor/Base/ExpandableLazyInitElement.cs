using Parameters.Editor.Extensions;
using UnityEngine.UIElements;

namespace Plugins.unity.parameters.Editor.Base
{
    public abstract class ExpandableLazyInitElement : VisualElement
    {
        protected abstract VisualElement ExpandElementsContainer { get; }
        
        private bool _isExpanded;
        private bool _isInitialized;
        
        protected void SwitchExpandState()
        {
            _isExpanded = !_isExpanded;

            if (_isExpanded == true && _isInitialized == false)
            {
                Initialize();
                _isInitialized = true;
            }

            ExpandElementsContainer.Display(_isExpanded == true ? DisplayStyle.Flex : DisplayStyle.None);
        }
        
        protected abstract void Initialize();
    }
}