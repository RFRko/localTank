using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tanki;

namespace TankiViewOpenPage
{
    public class MainWindowViewModel: BaseViewModel
    {
        static IPAddress ipaddress = System.Net.IPAddress.Parse("127.0.0.1");
        static int port = 11001;
        static IPEndPoint loginEndPoint = new IPEndPoint(ipaddress, port);

        public IPAddress IpAddress
        {
            get
            {
                return ipaddress;
            }
            set
            {
                ipaddress = value;
                RaisePropertyChanged();
            }
        }

        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
                RaisePropertyChanged();
            }
        }

        GameClient startClient = new GameClient(loginEndPoint);

        private ICommand _goCommand;

        public ICommand GoCommand
        {
            get
            {
                return this._goCommand ?? new DelegateCommand(goCmdEcexute, null);
            }
        }

        private void goCmdEcexute(object p)
        {
            startClient.RUN(new IPEndPoint(IpAddress, Port));

            MainPage form2 = new MainPage();

            form2.Show();
        }

    }
}
