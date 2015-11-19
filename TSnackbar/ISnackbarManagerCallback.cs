namespace com.deventure.topsnackbar
{
    public interface ISnackbarManagerCallback
    {
        void Show();

        void Dismiss(int ev);
    }
}