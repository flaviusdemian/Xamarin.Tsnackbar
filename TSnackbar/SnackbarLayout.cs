using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Orientation = Android.Widget.Orientation;

namespace com.deventure.topsnackbar
{
    [Register("com.deventure.topsnackbar.SnackbarLayout")]
    public class SnackbarLayout : LinearLayout
    {
        private readonly int mMaxInlineActionWidth;
        private readonly int mMaxWidth;
        private Button mActionView;
        private TextView mMessageView;

        private OnLayoutChangeListener mOnLayoutChangeListener;

        public SnackbarLayout(Context context)
            : base(context, null)
        {
        }

        public SnackbarLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.SnackbarLayout);
            mMaxWidth = a.GetDimensionPixelSize(Resource.Styleable.SnackbarLayout_android_maxWidth, -1);
            mMaxInlineActionWidth = a.GetDimensionPixelSize(Resource.Styleable.SnackbarLayout_maxActionInlineWidth, -1);
            if (a.HasValue(Resource.Styleable.SnackbarLayout_elevation))
            {
                ViewCompat.SetElevation(this, a.GetDimensionPixelSize(Resource.Styleable.SnackbarLayout_elevation, 0));
            }
            a.Recycle();
            Clickable = true;

            LayoutInflater.From(context).Inflate(Resource.Layout.tsnackbar_layout_include, this);
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            mMessageView = FindViewById<TextView>(Resource.Id.snackbar_text);
            mActionView = FindViewById<Button>(Resource.Id.snackbar_action);
        }

        public TextView MessageView()
        {
            return mMessageView;
        }

        public Button ActionView()
        {
            return mActionView;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            if (mMaxWidth > 0 && MeasuredWidth > mMaxWidth)
            {
                widthMeasureSpec = MeasureSpec.MakeMeasureSpec(mMaxWidth, MeasureSpecMode.Exactly);
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            }
            int multiLineVPadding = Resources.GetDimensionPixelSize(Resource.Dimension.design_snackbar_padding_vertical_2lines);
            int singleLineVPadding = Resources.GetDimensionPixelSize(Resource.Dimension.design_snackbar_padding_vertical);

            bool isMultiLine = mMessageView.Layout.LineCount > 1;
            bool remeasure = false;
            if (isMultiLine && mMaxInlineActionWidth > 0 && mActionView.MeasuredWidth > mMaxInlineActionWidth)
            {
                if (UpdateViewsWithinLayout(TSnackbar.VERTICAL, multiLineVPadding, multiLineVPadding - singleLineVPadding))
                {
                    remeasure = true;
                }
            }
            else
            {
                int messagePadding = isMultiLine ? multiLineVPadding : singleLineVPadding;
                if (UpdateViewsWithinLayout(TSnackbar.HORIZONTAL, messagePadding, messagePadding))
                {
                    remeasure = true;
                }
            }
            if (remeasure)
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (changed && mOnLayoutChangeListener != null)
            {
                mOnLayoutChangeListener.OnLayoutChange(this, l, t, r, b);
            }
        }

        public void SetOnLayoutChangeListener(OnLayoutChangeListener onLayoutChangeListener)
        {
            mOnLayoutChangeListener = onLayoutChangeListener;
        }

        private bool UpdateViewsWithinLayout(int orientation, int messagePadTop, int messagePadBottom)
        {
            bool changed = false;
            if (!orientation.Equals(Orientation))
            {
                Orientation = (Orientation) orientation;
                changed = true;
            }
            if (mMessageView.PaddingTop != messagePadTop
                || mMessageView.PaddingBottom != messagePadBottom)
            {
                UpdateTopBottomPadding(mMessageView, messagePadTop, messagePadBottom);
                changed = true;
            }
            return changed;
        }

        private static void UpdateTopBottomPadding(View view, int topPadding, int bottomPadding)
        {
            if (ViewCompat.IsPaddingRelative(view))
            {
                ViewCompat.SetPaddingRelative(view, ViewCompat.GetPaddingStart(view), topPadding,
                    ViewCompat.GetPaddingEnd(view), bottomPadding);
            }
            else
            {
                view.SetPadding(view.PaddingLeft, topPadding, view.PaddingRight, bottomPadding);
            }
        }
    }
}