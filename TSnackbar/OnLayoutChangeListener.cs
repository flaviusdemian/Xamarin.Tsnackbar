using Android.Views;

namespace com.deventure.topsnackbar
{
    public interface OnLayoutChangeListener
    {
        void OnLayoutChange(View view, int left, int top, int right, int bottom);
    }
}