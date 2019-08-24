using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class cliente_test {
	private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	private byte[] _buffer = new byte[1024];
	
	string mensaje = "2UNIRSE;RO";
	int id = 0;

	public void conectarse() {
		try {
			_clientSocket.Connect("localhost", 28999);
			Console.WriteLine("Conectado");
			enviarMensaje(mensaje);
		} catch (SocketException e) {
			Console.WriteLine("Error" + e.ToString());
		}
	}
	
	private void enviarMensaje(string txt) {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
			
			
            buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            byte[] data = new byte[received];
            Array.Copy(buffer, data, received);
            Console.WriteLine(Encoding.ASCII.GetString(data));
	}
	
	public static void Main(string[] args) {
		cliente_test c = new cliente_test();
		
		c.conectarse();
		Console.ReadLine();
	}
}