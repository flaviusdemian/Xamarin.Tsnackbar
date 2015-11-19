namespace com.deventure.topsnackbar
{
    public abstract class Callback : Java.Lang.Object
    {
        public static int DISMISS_EVENT_SWIPE = 0;

        public static int DISMISS_EVENT_ACTION = 1;

        public static int DISMISS_EVENT_TIMEOUT = 2;

        public static int DISMISS_EVENT_MANUAL = 3;

        public static int DISMISS_EVENT_CONSECUTIVE = 4;

        public void onDismissed(TSnackbar TSnackbar, int ev)
        {
        }

        public void onShown(TSnackbar TSnackbar)
        {
        }
    }
}