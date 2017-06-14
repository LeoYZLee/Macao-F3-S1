using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class menu_close : MonoBehaviour
    {
        [SerializeField]
        private PopupMenu popupMenu = null;

        [SerializeField]
        private TestButton button = null;


        private void OnButtonPressed(TestButton source)
        {
            popupMenu.CurrentPopupState = PopupMenu.PopupState.Closed;
        }

    }
}


