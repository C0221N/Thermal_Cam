using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ozeki.Media.IPCamera;
using Ozeki.Media.MediaHandlers;
using Ozeki.Media.MediaHandlers.Video;
using Ozeki.Media.Video.Controls;

namespace Thermal_Cam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoViewerWPF videoViewerWpf;
        
        private BitmapSourceProvider _provider;

        private IIPCamera _ipCamera;

        private WebCamera _webCamera;

        private MediaConnector _connector;
        public MainWindow()
        {
            InitializeComponent();

            _connector = new MediaConnector();

            _provider = new BitmapSourceProvider();

            SetVideoViewer();
        }

            private void SetVideoViewer()
            {
            _videoViewerWPF = new VideoViewerWPF
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = Brushes.Black
            };
            CameraBox.Children.Add(_videoViewerWpf);

            _videoViwerWPF.SetImageProvider(_provider);
            }
        #region USB Camera Connect/Disconnect
        private void ConnectUSBCamera_Click(object sender, RoutedEventArgs e)
        {
            _webcamera = WebCamera.GetDefaultDevice();
            if (_webCamera == null) 
                return;
            _connector.Connect(_webCamera, _provider);

            _webCamera.Start();
            _videoViewerWpf.Start();
        }
        private void DisconnectUSBCamera_Click(object sender, RoutedEventArgs e)
        {
            _videoViewerWpf.Stop();

            _webCamera.Stop();
            _webCamera.Dispose();

            _connector.Disconnect(_webCamera, _provider);
        }
        #endregion


        #region IP Camera Connect/Disconnect

        private void ConnectIPCamera_Click(object sender, RoutedEventArgs e)
        {
            var host = HostTextBox.Text;
            var user = UserTextBox.Text;
            var pass = Password.Password;

            _ipCamera = IPCameraFactory.GetCamera(host, user, pass);
            if (_ipCamera == null)
                return;
            _connector.Connect(_ipCamera.VideoChannel, _provider);

            _ipCamera.Start();
            _videoViewerWpf.Start();
        }
        private void DisconnectIPCamera_Click(object sender, RoutedEventArgs e)
        {
            _videoViewerWpf.Stop();

            _ipCamera.Disconnect();
            _ipCamera.Dispose();

            _connector.Disconnect(_ipCamera.VideoChannel, _provider);
        }
        #endregion
    }
}
