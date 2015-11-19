using Android.Support.Design.Widget;
using Android.Views;
using Java.Lang;

namespace com.deventure.topsnackbar
{
    public class Behavior : SwipeDismissBehavior
        //public class Behavior<T> : SwipeDismissBehavior where T : SnackbarLayout
    {
        public override bool OnInterceptTouchEvent(CoordinatorLayout parent, Object child, MotionEvent ev)
        {
            if (parent.IsPointInChildBounds(child as SnackbarLayout, (int) ev.GetX(), (int) ev.GetY()))
            {
                switch (ev.ActionMasked)
                {
                    case MotionEventActions.Down:
                        SnackbarManager.Instance().CancelTimeout(TSnackbar.getInstace().mManagerCallback);
                        break;
                    case MotionEventActions.Up:
                    case MotionEventActions.Cancel:
                        SnackbarManager.Instance().RestoreTimeout(TSnackbar.getInstace().mManagerCallback);
                        break;
                }
            }
            return base.OnInterceptTouchEvent(parent, child, ev);
        }
    }
}