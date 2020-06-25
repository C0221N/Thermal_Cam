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
using Ozeki.Media.MJPEGStreaming;
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
        
        private MJPEGStreamer _streamer;

        private _IVideoSender _videoSender;
        
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
            _videoSender = _webCamera;
            
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
            _videoSender = _ipCamera.VideoChannel;
            
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
        
         private void Start_Streaming_Click(object sender, RoutedEventArgs e)
            {
                var ip = ipAddressText.Text;
                var port = PortText.Text;

                _streamer = new MJPEGStreamer(ip, int.Parse(port));

                _connector.Connect(_videoSender, _streamer.VideoChannel);

                _streamer.ClientConnected += streamer.ClientConnected;
                _streamer.ClientDisconnected += streamer.ClientDisconnected;

                _streamer.Start;
            
            }

            void streamer_ClientConnected(object sender, Ozeki.VOIPEventArgs<IMPJEGStreamClient> e)
            {
                e.Item.StartStreaming();
            }

            void streamer_ClientDisconnected(object sender, Ozeki.VOIP.VOIPEventArgs<IMJPEGStreamClient> e)
            {
                e.Item.StopStreaming();
            }

            private void Stop_Streaming_Click(object sender, RoutedEventsArgs e)
            {
                _streamer.Stop();
                _connectror.Disconnect(_videoSender, _streamer.VideoChannel);
            }
    }
}
