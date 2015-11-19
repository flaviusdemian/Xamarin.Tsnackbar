using System;

namespace com.deventure.topsnackbar
{
    public class SnackbarRecord : Java.Lang.Object
    {
        public WeakReference<ISnackbarManagerCallback> mCallback;
        public int mDuration;

        public SnackbarRecord(int mDuration, ISnackbarManagerCallback callback)
        {
            this.mCallback = new WeakReference<ISnackbarManagerCallback>(callback);
            this.mDuration = mDuration;
        }

        public bool isSnackbar(ISnackbarManagerCallback callback)
        {
            ISnackbarManagerCallback callbackResult;
            if (this.mCallback.TryGetTarget(out callbackResult))
            {
                bool result = callback != null && callbackResult.Equals(callback);
                return result;
            }
            return false;
        }
    }
}