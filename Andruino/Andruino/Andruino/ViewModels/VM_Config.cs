using System;
using System.Collections.Generic;
using System.Text;

namespace Andruino.ViewModels
{
    public class VM_Config : BaseViewModel
    {
        string localIP = string.Empty;
        string serverIP = string.Empty;
        int serverPort = -1;

        public string LocalIP { get => localIP; set => SetProperty(ref localIP, value); }
        public string ServerIP { get => serverIP; set => SetProperty(ref serverIP, value); }
        public int ServerPort { get => serverPort; set => SetProperty(ref serverPort, value); }
    }
}
