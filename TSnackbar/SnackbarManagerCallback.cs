using System;
using System.Threading.Tasks;
using System.Timers;
using Android.OS;

namespace com.deventure.topsnackbar
{
    public class SnackbarManagerCallback : ISnackbarManagerCallback
    {
        public void Show()
        {
            Message message = TSnackbar.sHandler.ObtainMessage(TSnackbar.MSG_SHOW, TSnackbar.getInstace());
            if (message.Obj is TSnackbar)
            {
                TSnackbar tSnackbar = (TSnackbar)message.Obj;
                TSnackbar.sHandler.Post(tSnackbar.ShowCallback);
            }
        }

        public void Dismiss(int ev)
        {
            Message message = TSnackbar.sHandler.ObtainMessage(TSnackbar.MSG_DISMISS, ev, 0, TSnackbar.getInstace());
            if (message.Obj is TSnackbar)
            {
                TSnackbar tSnackbar = ((TSnackbar)message.Obj);
                TSnackbar.sHandler.Post(() => tSnackbar.HideCallback(ev));
            }
        }


    }
}