using System;
using System.Security.Cryptography.X509Certificates;
using Android.OS;
using TSnackbar = com.deventure.topsnackbar;

namespace com.deventure.topsnackbar
{
    public class SnackbarManager
    {
        public static int MSG_TIMEOUT = 0;
        public static int SHORT_DURATION_MS = 1500;
        public static int LONG_DURATION_MS = 3000;
        private static SnackbarManager sSnackbarManager;

        private Handler mHandler;
        private Object mLock;
        private SnackbarRecord mCurrentSnackbar;
        private SnackbarRecord mNextSnackbar;

        private SnackbarManager()
        {
            mLock = new Object();
            mHandler = new Handler(Looper.MainLooper);
        }

        public static SnackbarManager Instance()
        {
            if (sSnackbarManager == null)
            {
                sSnackbarManager = new SnackbarManager();
            }
            return sSnackbarManager;
        }

        public void HandleTimeout(SnackbarRecord record)
        {
            lock (mLock)
            {
                if (mCurrentSnackbar == record || mNextSnackbar == record)
                {
                    CancelSnackbarLocked(record, Callback.DISMISS_EVENT_TIMEOUT);
                }
            }
        }

        public void Show(int duration, ISnackbarManagerCallback callback)
        {
            lock (mLock)
            {
                if (IsCurrentSnackbar(callback))
                {
                    // Means that the callback is already in the queue. We'll just update the duration
                    mCurrentSnackbar.mDuration = duration;
                    // If this is the TSnackbar currently being shown, call re-schedule it's
                    // timeout
                    mHandler.RemoveCallbacksAndMessages(mCurrentSnackbar);
                    ScheduleTimeoutLocked(mCurrentSnackbar);
                    return;
                }
                if (IsNextSnackbar(callback))
                {
                    // We'll just update the duration
                    mNextSnackbar.mDuration = duration;
                }
                else
                {
                    // Else, we need to create a new record and queue it
                    mNextSnackbar = new SnackbarRecord(duration, callback);
                }
                if (mCurrentSnackbar != null && CancelSnackbarLocked(mCurrentSnackbar,
                    Callback.DISMISS_EVENT_CONSECUTIVE))
                {
                    // If we currently have a TSnackbar, try and cancel it and wait in line
                }
                // Clear out the current snackbar
                mCurrentSnackbar = null;
                // Otherwise, just show it now
                ShowNextSnackbarLocked();
            }
        }

        public void Dismiss(ISnackbarManagerCallback callback, int ev)
        {
            lock (mLock)
            {
                if (IsCurrentSnackbar(callback))
                {
                    CancelSnackbarLocked(mCurrentSnackbar, ev);
                }
                else if (IsNextSnackbar(callback))
                {
                    CancelSnackbarLocked(mNextSnackbar, ev);
                }
            }
        }

        public void OnDismissed(ISnackbarManagerCallback callback)
        {
            lock (mLock)
            {
                if (IsCurrentSnackbar(callback))
                {
                    // If the callback is from a TSnackbar currently show, remove it and show a new one
                    mCurrentSnackbar = null;
                    if (mNextSnackbar != null)
                    {
                        ShowNextSnackbarLocked();
                    }
                }
            }
        }

        public void OnShown(ISnackbarManagerCallback callback)
        {
            lock (mLock)
            {
                if (IsCurrentSnackbar(callback))
                {
                    ScheduleTimeoutLocked(mCurrentSnackbar);
                }
            }
        }

        public void CancelTimeout(ISnackbarManagerCallback callback)
        {
            lock (mLock)
            {
                if (IsCurrentSnackbar(callback))
                {
                    mHandler.RemoveCallbacksAndMessages(mCurrentSnackbar);
                }
            }
        }

        public void RestoreTimeout(ISnackbarManagerCallback callback)
        {
            lock (mLock)
            {
                if (IsCurrentSnackbar(callback))
                {
                    ScheduleTimeoutLocked(mCurrentSnackbar);
                }
            }
        }

        private void ShowNextSnackbarLocked()
        {
            if (mNextSnackbar != null)
            {
                mCurrentSnackbar = mNextSnackbar;
                mNextSnackbar = null;
                if (mCurrentSnackbar.mCallback != null)
                {
                    ISnackbarManagerCallback callbackResult;
                    if (mCurrentSnackbar.mCallback.TryGetTarget(out callbackResult))
                    {
                        callbackResult.Show();
                    }
                }
                else
                {
                    // The callback doesn't exist any more, clear out the TSnackbar
                    mCurrentSnackbar = null;
                }
            }
        }

        private bool CancelSnackbarLocked(SnackbarRecord record, int ev)
        {
            if (record.mCallback != null)
            {
                ISnackbarManagerCallback callbackResult;
                if (record.mCallback.TryGetTarget(out callbackResult))
                {
                    callbackResult.Dismiss(ev);
                    return true;
                }
            }
            return false;
        }

        private bool IsCurrentSnackbar(ISnackbarManagerCallback callback)
        {
            var result = mCurrentSnackbar != null && mCurrentSnackbar.isSnackbar(callback);
            return result;
        }

        private bool IsNextSnackbar(ISnackbarManagerCallback callback)
        {
            var result = mNextSnackbar != null && mNextSnackbar.isSnackbar(callback);
            return result;
        }

        private void ScheduleTimeoutLocked(SnackbarRecord record)
        {
            if (record.mDuration == TSnackbar.LENGTH_INDEFINITE)
            {
                // If we're set to indefinite, we don't want to set a timeout
                return;
            }
            int durationMs = LONG_DURATION_MS;
            if (record.mDuration > 0)
            {
                durationMs = record.mDuration;
            }
            else if (record.mDuration == TSnackbar.LENGTH_SHORT)
            {
                durationMs = SHORT_DURATION_MS;
            }
            mHandler.RemoveCallbacksAndMessages(record);

            mHandler.PostDelayed(() => HandleTimeout(record), durationMs);
        }
    }
}