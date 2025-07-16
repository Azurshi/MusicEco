using System.Diagnostics;

namespace MusicEco
{
    public partial class App : Application
    {
        public App()
        {
#if WINDOWS
            InitializeComponent();
#else
            try {
                InitializeComponent();
            }
            catch (Exception e) {
                Debug.WriteLine(">>>>>>>>>");
                Debug.WriteLine(e.Message);
                Debug.WriteLine("<<<<<<<<<");
            }
#endif
            MainPage = new AppShell();
                
        }
    }
}
