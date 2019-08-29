using System;
using System.Text;

using System.Threading;

using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;

using System.Xml;

class server {
	
	// ACCIONES
	
	private string dlBrk =  "POS AHORA ES MIO";
	private string slyDl =  "VEN PA CA";
	private string frcDl =  "SWITCH";
	private string brthd =  "REGALO CUMPLE";
	private string dbtCl =  "PAGAME BITCH";
	private string jstNo =  "NO ES NO";
	private string pssGo =  "COGE 2 CARTAS";
	private string dblRn =  "PAGAS DOBLE";
	
	// RENTAS
	
	private string rnWld =  "CUALQUIER COLOR";   // 3
	private string rnMAC =  "RENTA MARRON AZULCLARO"; // 2
	private string rnNaM =  "RENTA NARANJA MORADO";   // 2
	private string rnAmR =  "RENTA AMARILLO ROJO";    // 2
	private string rnVAO =  "RENTA VERDE AZULOSCURO"; // 2
	private string rnTrU =  "RENTA TREN UTILIDAD";    // 2
	
	// CASAS Y HOTELES
	
	private string hotel =  "HOTEL"; // 2
	private string casas =  "CASA";  // 3
	
	// REVISAR PRECIOS
	// COMODINES MEDIOS
	
	private string wldTt =  "COMODIN CARD";             // 2
	private string mdMAC =  "COMODIN MARRON AZULCLARO"; // 1 
	private string mdNam =  "COMODIN NARANJA MORADO";   // 2
	private string mdAmR =  "COMODIN AMARILLO ROJO";    // 2
	private string mdVAO =  "COMODIN VERDE AZULOSCURO"; // 1
	private string mdVTr =  "COMODIN VERDE TREN";       // 1
	private string mdTAZ =  "COMODIN TREN AZULCLARO";   // 1
	private string mdTrU =  "COMODIN TREN UTILIDAD";    // 1
	
	// BILLETES DE DINERO
	
	private string bllt1 =  "1";   // 6
	private string bllt2 =  "2";   // 5
	private string bllt3 =  "3";   // 3
	private string bllt4 =  "4";   // 3
	private string bllt5 =  "5";   // 2
	private string bll10 =  "10";  // 1
	
	private string[] todas  = new string[105];
	private string[] cartas = new string[105];
	private string[] mazo   = new string[77];
	
	private int cartasDisp  = 105;
	
	public enum tipo { ACCION, DINERO, PROPIEDAD, CONSTRUCCION, RENTA };
	
	void addCartas() { // ¡¡¡¡¡¡¡¡¡¡¡¡¡¡ ATENCION, LAS CARTAS QUE SE AÑADEN SON DEPENDIENTES LAS UNAS DE LAS OTRAS, ES DECIR, CAMBIAR ALGO EN UNA CARTA LO CAMBIA EN TODAS LAS SIMILIARES, ASI QUE POR DIOS NO LA CAGUES !!
		// Iniciar el mazo
		for (int i = 0; i < mazo.Length; i++) { mazo[i] = "NONE"; }
	
		// Añadir las cartas de accion
		addMultTimes(dlBrk, 2); //
		addMultTimes(slyDl, 3); //
		addMultTimes(frcDl, 3); //
		addMultTimes(brthd, 3); //
		addMultTimes(dbtCl, 3); //
		addMultTimes(jstNo, 3); //
		addMultTimes(pssGo, 10);//
		addMultTimes(dblRn, 2); //
		
		//Añadir las rentas
		addMultTimes(rnWld,3);  //
		addMultTimes(rnMAC, 2); //
		addMultTimes(rnNaM, 2); //
		addMultTimes(rnAmR, 2); //
		addMultTimes(rnVAO, 2); //
		addMultTimes(rnTrU, 2); //
		
		//Añadir casas y hoteles
		addMultTimes(hotel, 2); //
		addMultTimes(casas, 3); //
		
		// Añadir las cartas wild
		addMultTimes(wldTt, 2); //
		addMultTimes(mdMAC, 1); //
		addMultTimes(mdNam, 2); //
		addMultTimes(mdAmR, 2); //
		addMultTimes(mdVAO, 1); //
		addMultTimes(mdVTr, 1); //
		addMultTimes(mdTAZ, 1); //
		addMultTimes(mdTrU, 1); //
		
		//Añadir los billetes de dinero
		addMultTimes(bllt1, 6); //
		addMultTimes(bllt2, 5); //
		addMultTimes(bllt3, 3); //
		addMultTimes(bllt4, 3); //
		addMultTimes(bllt5, 2); //
		addMultTimes(bll10, 1); //
		
		// Añadir las propiedades
		XmlDocument doc = new XmlDocument();
		doc.Load(".\\propiedades.xml");
		
		foreach (XmlNode node in doc.SelectNodes("sets/full")) { // Coger todas las propiedades de la propiedad
			int cantidad = int.Parse(node.SelectSingleNode("setNm").InnerText);
			int precio   = int.Parse(node.SelectSingleNode("precio").InnerText);
			
			string[] nombres = extraerInfoXML(cantidad, node.SelectSingleNode("nombres").InnerText);
		
			for (int i = 0; i < cantidad; i++) {
				addMultTimes(nombres[i]);
			}
		}
	}
	
	string[] extraerInfoXML(int _nm, string _info) {
		string[] tmp = new string[_nm];
		int cnt = 0;
		string temp = string.Empty;
		
		for (int i = 0; i < _info.Length; i++) {
			if (_info[i] == ';') {
				tmp[cnt] = temp;
				cnt++;
				temp = string.Empty;
			
				continue;
			}
			
			temp += _info[i];
		}
		tmp[cnt] = temp;
		
		return tmp;
	}
	
	void addMultTimes(string c, int veces = 1) {
		while (veces != 0) {
			for (int i = 0; i < todas.Length; i++) {
				if (todas[i] == null) {
					todas[i] = c;
					Console.WriteLine(i);
					break;
				}
			}
			
			veces--;
		}
	}
	
	void randomizarCartas(string[] _cartas) {
		Random rng = new Random();
		for (int t = 0; t < _cartas.Length; t++) {
			string temp = _cartas[t];
			
			int r = rng.Next(t, _cartas.Length);
			
			_cartas[t] = _cartas[r];
			_cartas[r] = temp;
		}
	}
	
	void comprobarCartas() {
		if (cartasDisp > 0) return;
		
		for (int i = 0; i < mazo.Length; i++) {
			if (mazo[i] != "NONE") {
				cartasDisp++;
				cartas[i] = mazo[i];
				mazo[i] = "NONE";
			}
		}
		randomizarCartas(cartas);
	}
	
	string getCarta() {
		string c = string.Empty; // Esto no funciona
		
		for (int i = 0; i < cartas.Length; i++) {
			if (cartas[i] != "NONE") { c = cartas[i]; cartas[i] = "NONE"; break; }
		}
		cartasDisp--;
		comprobarCartas();
		
		return c;
	}
	
	Jugador[] jugadores = new Jugador[5] { new Jugador(), new Jugador(), new Jugador(), new Jugador(), new Jugador() };
	Jugador[] trueJugad;
	int jugadorNum = 0;
	int idCnt = 0;
	
	public server() {
		// Preparar la partida
		addCartas();
		Array.Copy(todas, cartas, todas.Length);
		randomizarCartas(cartas);
		
		// Prepapar el servidor
		empezarTCPServer();
	}
	
	// ----------------------------- SERVER -----------------------------
	
	byte[] _buffer = new byte[1024];
	Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	List<Socket> _clienteSockets = new List<Socket>();
	
	void AcceptCallback(IAsyncResult AR) {
		Socket socket = _serverSocket.EndAccept(AR);
		_clienteSockets.Add(socket);
		socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
		_serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
	}
	
	void SendCallback(IAsyncResult AR) {
		Socket socket = (Socket) AR.AsyncState;
		socket.EndSend(AR);
	}
	
	void ReceiveCallback(IAsyncResult AR) {
		Socket socket = (Socket) AR.AsyncState;
		int received  = socket.EndReceive(AR);
		byte[] dataBuf = new byte[received];
		Array.Copy(_buffer, dataBuf, received);
		
		SendMessage(signalDcder(Encoding.ASCII.GetString(_buffer, 0, _buffer.Length)), socket);
	}
	
	void SendMessage(string txt, Socket socket) {
		byte[] data = Encoding.ASCII.GetBytes(txt);
		socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
		socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
	}
	
	bool activo = true;
	bool jugando = false;
	
	public enum estado { ESPERANDO, STANDBY, TURNO };
	
	string medio 	   = "QUE EMPIECE LA PARTIDA";
	volatile string mensajeHost = "idk";
	int turnoJug       = 0;
	
	void comando(string _cmd) {
		switch (_cmd) {
			case "EMPEZAR":
				if (jugadorNum > 0 && !jugando) { // CAMBIAR PARA CUANDO EL JUEGO ESTE """LISTO"""
					mensajeHost = "[Server] Que gane el mejor :)";
					
					Console.WriteLine("Empenzado la partida");
					
					jugando = true;
	
					trueJugad = new Jugador[jugadorNum];
					for (int i = 0; i < jugadorNum; i++) { trueJugad[i] = jugadores[i]; }
					
					foreach (Jugador j in trueJugad) { j.estadoJugador = estado.STANDBY; }
					
					Random rn = new Random();
					trueJugad[rn.Next(0, jugadorNum)].estadoJugador = estado.TURNO;
				} else Console.WriteLine("Jugadores insuficientes");
			break;
			case "SALIR":
				activo = false;
			break;
			default:
				Console.WriteLine("Comando no reconocido");
			break;
		}
	}
	
	void avanzarTurno() {
		trueJugad[turnoJug].estadoJugador = estado.STANDBY;
		
		turnoJug++;
		turnoJug %= jugadorNum;
	
		trueJugad[turnoJug].estadoJugador = estado.TURNO;
	}
	
	void empezarTCPServer() {
		_serverSocket.Bind(new IPEndPoint(IPAddress.Any, 28999));
		_serverSocket.Listen(7);
		_serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
		
		Console.WriteLine("Esperando jugadores....");
		
		while (activo) {
			string cmd = Console.ReadLine();
			comando(cmd);
		}
		System.Environment.Exit(1);
	}
	
	// ----------------------------- JUGADORES -----------------------------
	
	Jugador getJugadorById(int id) {
		foreach (Jugador j in jugadores) { if (j.id == id) return j; }
		return null;
	}
	
	bool addJugador(string nametag) {
		if (jugadorNum == 5 || jugando) return false;
		
		mensajeHost = "[Server] Se ha unido " + nametag;
		
		Jugador nuevo = new Jugador(nametag, idCnt);
		for (int i = 0; i < jugadores.Length; i++) { if (jugadores[i].id == -1) { jugadores[i] = nuevo; break;} }
		
		idCnt++;
		jugadorNum++;
		
		Console.WriteLine("Bienvenido " + nametag);
		return true;
	}
	
	public class Jugador {
		public readonly string nombre;
		public readonly int    id;
		
		public estado estadoJugador;
		
		public Jugador(string _nombre = "", int _id = -1, estado _es = estado.ESPERANDO) {
			nombre = _nombre;
			id = _id;
			
			estadoJugador = _es;
		}
	}
	
	// -------------- TRATAR MENSAJES (COMS) --------------
	
	int numeroPuntosComas(string txt) {
		int i = 0;
		foreach (char s in txt) { if (s == ';') i++; }
		return i + 1;
	}
	
	string[] tratarMensaje(string _msg, int l) {
		string[] mensaje = new string[l];
		string tmp = string.Empty;
		int cnt = 0;
		
		for (int i = 0; i < _msg.Length; i++) {
			if (_msg[i] == ';') {
				mensaje[cnt] = tmp;
				tmp = string.Empty;
				cnt++;
				
				continue;
			}
			
			tmp += _msg[i];
		}
		mensaje[cnt] = tmp;
		
		return mensaje;
	}
	
	string signalDcder(string _msg) {
		string[] mensaje = new string[numeroPuntosComas(_msg)];
		mensaje = tratarMensaje(_msg, mensaje.Length);
		
		_buffer = new byte[1024];
		
		switch (mensaje[0]) {
			case "UNIRSE":
				if (addJugador(mensaje[1])) return  "UNIDO;" + (idCnt - 1).ToString();
				else return "NOEXITO";
			case "ESTADO":
				return "ESTADO;" + getJugadorById(int.Parse(mensaje[1])).estadoJugador.ToString() + ";" + mensajeHost + ";" + medio;
			case "FINTURNO":
				mensajeHost = "[" + getJugadorById(int.Parse(mensaje[1])).nombre + "]" + " Ha acabado su turno";
				return "ESTADO;" + getJugadorById(int.Parse(mensaje[1])).estadoJugador.ToString() + ";" + mensajeHost + ";" + medio;
			case "PEDIR":
				string temp = string.Empty;
				
				for (int i = 0; i < int.Parse(mensaje[1]); i++) {
					string c = getCarta();
					Console.WriteLine(c);
					temp += ";" + c;
				}
				//return "PEDIR;" + mensaje[1] + temp;
				return "PEDIR;5;CASA RO;COMODIN CARD;COMODIN VERDE AZULOSCURO;KEBAB;MESA CUADRADA"; // QUITAR, SOLO DEBUG
			default:
				Console.WriteLine("ERROR: El mensaje: " + _msg + " no puede ser reconocido");
				return "NOEXITO";
		}
	}
	
	public static void Main(string[] args) { server s = new server(); }
}
