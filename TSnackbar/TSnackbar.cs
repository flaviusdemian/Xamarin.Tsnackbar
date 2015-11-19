using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Object = Java.Lang.Object;
using String = System.String;

namespace com.deventure.topsnackbar
{
    public class TSnackbar : Object
    {
        public const int LENGTH_LONG = 0;
        private const int ANIMATION_DURATION = 250;
        public const int MSG_SHOW = 0;
        public const int MSG_DISMISS = 1;
        public static int HORIZONTAL = 0;
        public static int SHOW_DIVIDER_BEGINNING = 1;
        public static int SHOW_DIVIDER_END = 4;
        public static int SHOW_DIVIDER_MIDDLE = 2;
        public static int SHOW_DIVIDER_NONE = 0;
        public static int VERTICAL = 1;

        private static TSnackbar instace;

        public static int LENGTH_INDEFINITE = -2;

        public static int LENGTH_SHORT = -1;

        public static Handler sHandler = new Handler(Looper.MainLooper);
        private readonly Context mContext;
        private readonly ViewGroup mParent;
        private readonly SnackbarLayout mView;
        public Callback mCallback;
        private int mDuration;

        public ISnackbarManagerCallback mManagerCallback = new SnackbarManagerCallback();

        static TSnackbar()
        {
        }

        private TSnackbar(ViewGroup parent)
        {
            try
            {
                mParent = parent;
                mContext = parent.Context;
                LayoutInflater inflater = LayoutInflater.From(mContext);
                instace = this;
                var view = inflater.Inflate(com.deventure.topsnackbar.Resource.Layout.tsnackbar_layout, mParent, false);
                if (view != null)
                {
                    mView = (SnackbarLayout)view;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static TSnackbar getInstace()
        {
            return instace;
        }

        public static TSnackbar Make(View view, String text, int duration)
        {
            var TSnackbar = new TSnackbar(FindSuitableParent(view));
            TSnackbar.SetText(text);
            TSnackbar.SetDuration(duration);
            return TSnackbar;
        }

        public static TSnackbar Make(View view, int resId, int duration)
        {
            return Make(view, view.Resources.GetText(resId), duration);
        }

        private static ViewGroup FindSuitableParent(View view)
        {
            return (ViewGroup)view;
        }

        public TSnackbar AddIcon(int resource_id, int size)
        {
            TextView tv = mView.MessageView();

            tv.SetCompoundDrawablesWithIntrinsicBounds(new BitmapDrawable(Bitmap.CreateScaledBitmap(
                ((BitmapDrawable)(mContext.Resources.GetDrawable(resource_id))).Bitmap, size, size, true)), null, null,
                null);

            return this;
        }

        /**
         * This method converts dp unit to equivalent pixels, depending on device density.
         *
         * @param dp A value in dp (density independent pixels) unit. Which we need to convert into pixels
         * @param context Context to get resources and device specific display metrics
         * @return A float value to represent px equivalent to dp depending on device density
         */

        public static float ConvertDpToPixel(float dp, Context context)
        {
            Resources resources = context.Resources;
            DisplayMetrics metrics = resources.DisplayMetrics;
            float px = dp * ((float)metrics.DensityDpi / 160f);
            return px;
        }

        /**
         * This method converts device specific pixels to density independent pixels.
         *
         * @param px A value in px (pixels) unit. Which we need to convert into db
         * @param context Context to get resources and device specific display metrics
         * @return A float value to represent dp equivalent to px value
         */

        public static float ConvertPixelsToDp(float px, Context context)
        {
            Resources resources = context.Resources;
            DisplayMetrics metrics = resources.DisplayMetrics;
            float dp = px / ((float)metrics.DensityDpi / 160f);
            return dp;
        }

        public TSnackbar SetAction(int resId, Action<View> clickHandler)
        {
            var listener = new SnackbarActionClickImplementor
            {
                Handler = clickHandler
            };
            return SetAction(mContext.GetText(resId), listener);
        }

        private TSnackbar SetAction(int resId, View.IOnClickListener listener)
        {
            return SetAction(mContext.GetText(resId), listener);
        }

        private TSnackbar SetAction(String text, View.IOnClickListener listener)
        {
            TextView tv = mView.ActionView();
            if (TextUtils.IsEmpty(text) || listener == null)
            {
                tv.Visibility = ViewStates.Gone;
                tv.SetOnClickListener(null);
            }
            else
            {
                tv.Visibility = ViewStates.Visible;
                tv.Text = text;
                tv.Click += delegate(object sender, EventArgs args)
                {
                    listener.OnClick(sender as View);
                    DispatchDismiss(Callback.DISMISS_EVENT_ACTION);
                };
            }
            return this;
        }

        public TSnackbar SetAction(String text, Action<View> clickHandler)
        {
            var listener = new SnackbarActionClickImplementor
            {
                Handler = clickHandler
            };
            return SetAction(text, listener);
        }

        public TSnackbar SetActionTextColor(ColorStateList colors)
        {
            TextView tv = mView.ActionView();
            tv.SetTextColor(colors);
            return this;
        }

        public TSnackbar SetActionTextColor(Color color)
        {
            TextView tv = mView.ActionView();
            if (color == null)
            {
                color = Color.Red;
            }
            tv.SetTextColor(color);

            return this;
        }

        public TSnackbar SetText(String message)
        {
            TextView tv = mView.MessageView();
            tv.Text = message;
            return this;
        }

        public TSnackbar SetText(int resId)
        {
            return SetText(mContext.GetText(resId));
        }

        public TSnackbar SetDuration(int duration)
        {
            mDuration = duration;
            return this;
        }

        public int GetDuration()
        {
            return mDuration;
        }

        public View GetView()
        {
            return mView;
        }

        public void Show()
        {
            SnackbarManager.Instance().Show(mDuration, mManagerCallback);
        }

        public void Dismiss()
        {
            DispatchDismiss(Callback.DISMISS_EVENT_MANUAL);
        }

        public void DispatchDismiss(int ev)
        {
            SnackbarManager.Instance().Dismiss(mManagerCallback, ev);
        }

        public TSnackbar SetCallback(Callback callback)
        {
            mCallback = callback;
            return this;
        }

        public bool IsShown()
        {
            return mView.IsShown;
        }

        public void ShowView()
        {
            if (mView.Parent == null)
            {
                ViewGroup.LayoutParams lp = mView.LayoutParameters;
                try
                {
                    var result = Extensions.JavaCast<CoordinatorLayout.LayoutParams>(lp);
                    if (result != null)
                    {
                        var behavior = new Behavior();
                        behavior.SetStartAlphaSwipeDistance(0.1f);
                        behavior.SetEndAlphaSwipeDistance(0.6f);
                        behavior.SetSwipeDirection(SwipeDismissBehavior.SwipeDirectionStartToEnd);
                        behavior.SetListener(new DismissListener());
                        result.Behavior = behavior;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                mParent.AddView(mView);
            }

            if (ViewCompat.IsLaidOut(mView))
            {
                AnimateViewIn();
            }
            else
            {
                mView.LayoutChange += OnLayoutChange;
            }
        }

        private void OnLayoutChange(object sender, View.LayoutChangeEventArgs layoutChangeEventArgs)
        {
            AnimateViewIn();
            mView.LayoutChange -= OnLayoutChange;
        }

        public void AnimateViewIn()
        {
            Animation anim = Android.Views.Animations.AnimationUtils.LoadAnimation(mView.Context,
                Resource.Animation.top_in);
            anim.Interpolator = AnimationUtils.FAST_OUT_SLOW_IN_INTERPOLATOR;
            anim.Duration = ANIMATION_DURATION;
            anim.AnimationEnd += delegate
            {
                if (mCallback != null)
                {
                    mCallback.onShown(this);
                }
                SnackbarManager.Instance().OnShown(mManagerCallback);
            };
            mView.StartAnimation(anim);
        }

        public void AnimateViewOut(int ev)
        {
            Animation anim = Android.Views.Animations.AnimationUtils.LoadAnimation(mView.Context,
                Resource.Animation.top_out);
            anim.Interpolator = AnimationUtils.FAST_OUT_SLOW_IN_INTERPOLATOR;
            anim.Duration = ANIMATION_DURATION;
            anim.AnimationEnd += delegate { OnViewHidden(ev); };
            mView.StartAnimation(anim);
        }

        public void HideView(int ev)
        {
            if (mView.Visibility != ViewStates.Visible || IsBeingDragged())
            {
                OnViewHidden(ev);
            }
            else
            {
                AnimateViewOut(ev);
            }
        }

        public void OnViewHidden(int ev)
        {
            mParent.RemoveView(mView);

            if (mCallback != null)
            {
                mCallback.onDismissed(this, ev);
            }
            SnackbarManager.Instance().OnDismissed(mManagerCallback);
        }

        private bool IsBeingDragged()
        {
            ViewGroup.LayoutParams lp = mView.LayoutParameters;
            try
            {
                var result = Extensions.JavaCast<CoordinatorLayout.LayoutParams>(lp);
                if (result != null)
                {
                    CoordinatorLayout.Behavior behavior = result.Behavior;
                    if (behavior is SwipeDismissBehavior)
                    {
                        return ((SwipeDismissBehavior)behavior).DragState
                               != SwipeDismissBehavior.StateIdle;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public void ShowCallback()
        {
            ShowView();
        }

        public void HideCallback(int ev)
        {
            HideView(ev);
        }

        internal class SnackbarActionClickImplementor : Object, View.IOnClickListener
        {
            public Action<View> Handler { get; set; }

            public void OnClick(View v)
            {
                Action<View> handler = Handler;
                if (handler != null)
                {
                    handler(v);
                }
            }
        }
    }
}