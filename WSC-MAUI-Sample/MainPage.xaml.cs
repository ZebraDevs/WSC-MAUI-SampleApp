using CommunityToolkit.Mvvm.Messaging;

namespace OEMInfo_MAUI_Sample
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                    MainThread.BeginInvokeOnMainThread(() => { 
                        if(m.StartsWith("|"))
                            entryKB.Text += m.Substring(1);
                        else
                            lbDisplayDeviceId.Text = m+"\n"+lbDisplayDeviceId.Text; 
                    });
            });
        }

        public static void ModifyEntry()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.KeyPress += (sender, e) =>
                {
                    try
                    {
                        if (e.KeyCode == Android.Views.Keycode.Tab || e.KeyCode == Android.Views.Keycode.Enter)
                        {
                            e.Handled = true;

                            if ((e.KeyCode == Android.Views.Keycode.Enter) && (e.Event.Action == Android.Views.KeyEventActions.Up))
                            {
                                WeakReferenceMessenger.Default.Send("|<ENTER>");
                                ((Entry)handler.VirtualView).SendCompleted();
                            }
                        }
                        else
                            e.Handled = false;
                    }
                    catch (Exception ex)
                    {
                    }
                };
#endif
            });
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {

            WeakReferenceMessenger.Default.Send("#WSC");
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
