using System;
using Android.Support.Design.Widget;
using Android.Views;

namespace com.deventure.topsnackbar
{
    public class DismissListener : Java.Lang.Object, SwipeDismissBehavior.IOnDismissListener
    {
        public void Dispose()
        {
            //TODO:FIX
        }

        public IntPtr Handle { get; private set; }

        public void OnDismiss(View view)
        {
            TSnackbar.getInstace().DispatchDismiss(Callback.DISMISS_EVENT_SWIPE);
        }

        public void OnDragStateChanged(int state)
        {
            switch (state)
            {
                case SwipeDismissBehavior.StateDragging:
                case SwipeDismissBehavior.StateSettling:

                    SnackbarManager.Instance().CancelTimeout(TSnackbar.getInstace().mManagerCallback);
                    break;
                case SwipeDismissBehavior.StateIdle:

                    SnackbarManager.Instance().RestoreTimeout(TSnackbar.getInstace().mManagerCallback);
                    break;
            }
        }
    }
}