using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Andruino.ViewModels
{
    /// <summary>
    /// ViewModel d'émission des messages UDP
    /// </summary>
    public class VM_UDPSender : BaseViewModel
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static VM_UDPSender Instance { get; set; } = new VM_UDPSender();

        /// <summary>
        /// Contructeur privé
        /// </summary>
        private VM_UDPSender() { }

        /// <summary>
        /// Envoi l'IP/Port du serveur le message
        /// </summary>
        /// <param name="config"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> SendAsync(VM_Config config, string message)
        {
            if(((App)App.Current).CurrentConfig.ModeDemo)
            {
                if(message.StartsWith("[CMD]"))
                {
                    var cmd = message.Replace("[CMD]", "");
                    cmd = cmd.Replace("[/CMD]", "");

                    if (cmd == "RESET")
                        await System.Threading.Tasks.Task.Delay(new Random().Next(0, 10) * 100).ContinueWith(a =>
                        {
                            VM_UDPReceiver.Instance.AddLog(cmd + "_" + "acknowledged pushReset()");
                        });
                    if (cmd == "SETDATE")
                        await System.Threading.Tasks.Task.Delay(new Random().Next(0, 10) * 100).ContinueWith(a =>
                        {
                            VM_UDPReceiver.Instance.AddLog(cmd + "_" + "acknowledged SETDATE");
                        });
                    if (cmd == "DATE")
                        await System.Threading.Tasks.Task.Delay(new Random().Next(0, 10) * 100).ContinueWith(a =>
                        {
                            VM_UDPReceiver.Instance.AddLog(cmd + "_" + DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy"));
                        });
                    if (cmd == "LIGHT")
                        await System.Threading.Tasks.Task.Delay(new Random().Next(0, 10) * 100).ContinueWith(a =>
                        {
                            VM_UDPReceiver.Instance.AddLog(cmd + "_" + "Light Analog reading = " + new Random().Next(0, 1024));
                        });
                    if (cmd == "TEMPC")
                        await System.Threading.Tasks.Task.Delay(new Random().Next(0, 10) * 100).ContinueWith(a =>
                        {
                            VM_UDPReceiver.Instance.AddLog(cmd + "_" + new Random().Next(15, 30));
                        });
                    if (cmd == "HR")
                        await System.Threading.Tasks.Task.Delay(new Random().Next(0, 10) * 100).ContinueWith(a =>
                        {
                            VM_UDPReceiver.Instance.AddLog(cmd + "_" + new Random().Next(10, 90));
                        });
                    if (cmd == "TEMP")
                        await System.Threading.Tasks.Task.Delay(new Random().Next(0, 10) * 100).ContinueWith(a =>
                        {
                            VM_UDPReceiver.Instance.AddLog(cmd + "_" + "Humidity: 50.05 %	Temperature: 20.02 *C 75.57 *F	Heat index: 71.17*F");
                        });
                }
                return true;
            }

            var client = new UdpClient();
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(config.ServerIP), config.ServerPort);
                client.Connect(ep);

                var messageArray = Encoding.ASCII.GetBytes(message);
                await client.SendAsync(messageArray, messageArray.Length);

                var port = ((IPEndPoint)client.Client.LocalEndPoint).Port;
                VM_UDPReceiver.Instance.StartListener(port);
                return true;
            }
            catch (Exception ex)
            {
                ((Views.MainPage)App.Current.MainPage).Manage_Error(ex);
                return false;
            }
            finally
            {
                client.Close();
            }
        }
    }
}
