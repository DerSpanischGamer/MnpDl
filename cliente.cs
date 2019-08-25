using System;
using System.Text;

using System.Threading;

using System.Collections.Generic;

using System.Xml;

using System.Net;
using System.Net.Sockets;

class cliente {
	
	// ----------------------------- TODAS LAS CARTAS -----------------------------
	
	// ACCION
	private Carta dlBrk = new Carta("POS AHORA ES MIO", 5, tipo.ACCION);
	private Carta slyDl = new Carta("VEN PA CA", 3, tipo.ACCION);
	private Carta frcDl = new Carta("SWITCH", 3, tipo.ACCION);
	private Carta brthd = new Carta("REGALO CUMPLE", 2, tipo.ACCION);
	private Carta dbtCl = new Carta("PAGAME BITCH", 3, tipo.ACCION);
	private Carta jstNo = new Carta("NO ES NO", 4, tipo.ACCION);
	private Carta pssGo = new Carta("COGE 2 CARTAS", 1, tipo.ACCION);
	private Carta dblRn = new Carta("PAGAS DOBLE", 1, tipo.ACCION);
	
	// RENTAS
	
	private Carta rnWld = new Carta("CUALQUIER COLOR", 3, tipo.RENTA);
	private Carta rnMAC = new Carta("RENTA MARRON AZULCLARO", 1, tipo.RENTA);
	private Carta rnNaM = new Carta("RENTA NARANJA MORADO", 1, tipo.RENTA);
	private Carta rnAmR = new Carta("RENTA AMARILLO ROJO", 1, tipo.RENTA);
	private Carta rnVAO = new Carta("RENTA VERDE AZULOSCURO", 1, tipo.RENTA);
	private Carta rnTrU = new Carta("RENTA TREN UTILIDAD", 1, tipo.RENTA);
	
	// CASAS Y HOTELES
	
	private Carta hotel = new Carta("HOTEL", 4, tipo.CONSTRUCCION);
	private Carta casas = new Carta("CASA",  3, tipo.CONSTRUCCION);
	
	// REVISAR PRECIOS
	// WILDS MEDIOS
	
	private Carta wldTt = new Carta("COMODIN CARD", 10, tipo.PROPIEDAD);
	private Carta mdMAC = new Carta("COMODIN MARRON AZULCLARO", 2, tipo.PROPIEDAD);
	private Carta mdNam = new Carta("COMODIN NARANJA MORADO", 2, tipo.PROPIEDAD);
	private Carta mdAmR = new Carta("COMODIN AMARILLO ROJO", 2, tipo.PROPIEDAD);
	private Carta mdVAO = new Carta("COMODIN VERDE AZULOSCURO", 2, tipo.PROPIEDAD);
	private Carta mdVTr = new Carta("COMODIN VERDE TREN", 2, tipo.PROPIEDAD);
	private Carta mdTAZ = new Carta("COMODIN TREN AZULCLARO", 2, tipo.PROPIEDAD);
	private Carta mdTrU = new Carta("COMODIN TREN UTILIDAD", 2, tipo.PROPIEDAD);
	
	// BILLETES DE DINERO
	
	private Carta bllt1 = new Carta("1", 1, tipo.DINERO);
	private Carta bllt2 = new Carta("2", 2, tipo.DINERO);
	private Carta bllt3 = new Carta("3", 3, tipo.DINERO);
	private Carta bllt4 = new Carta("4", 4, tipo.DINERO);
	private Carta bllt5 = new Carta("5", 5, tipo.DINERO);
	private Carta bll10 = new Carta("10", 10, tipo.DINERO);
	
	private Carta[] todas;
	
	// ----------------------------- XML -----------------------------
	
	Set[] sets = new Set[10];
	Propiedad[] props = new Propiedad[28]; // Todas las propiedades
	
	void cargarXML() {
		XmlDocument doc = new XmlDocument();
		doc.Load(".\\propiedades.xml");
		
		int j = 0;
		int k = 0;
		foreach (XmlNode node in doc.SelectNodes("sets/full")) { // Coger todas las propiedades de la propiedad
			int cantidad = int.Parse(node.SelectSingleNode("setNm").InnerText);
			int precio   = int.Parse(node.SelectSingleNode("precio").InnerText);
			
			string color = node.SelectSingleNode("color").InnerText;
			
			Propiedad[] ps = new Propiedad[cantidad];
			
			string[] nombres = extraerInfoXML(cantidad, node.SelectSingleNode("nombres").InnerText);
			string[] _precis = extraerInfoXML(cantidad, node.SelectSingleNode("precios").InnerText);
			int[]    precios = new int[cantidad];
		
			bool   letra   = bool.Parse(node.SelectSingleNode("letra").InnerText);
		
			for (int i = 0; i < cantidad; i++) {
				precios[i] = int.Parse(_precis[i]);
				
				Propiedad d = new Propiedad(nombres[i], precio, color, cantidad, false, false, letra);
				ps[i] = new Propiedad(nombres[i], precio, color, cantidad, false, false, letra);
				
				props[j] = ps[i];
				j++;
			}
			
			sets[k] = new Set(ps, precios);
			k++;
		}
	}
	
	private string[] extraerInfoXML(int _nm, string _info) {
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
	
	// ----------------------------- CLASE CARTA -----------------------------
	
	public enum tipo { ACCION, DINERO, PROPIEDAD, CONSTRUCCION, RENTA };
	
	public class Carta {
		
		public readonly string texto;
		public readonly int precio;
		public readonly tipo clase;
	
		public readonly bool letraNegra;
	
		public Carta(string _texto = "NONE", int _precio = -1, tipo _clase = tipo.ACCION, bool _letra = false) {
			texto = _texto;
			precio = _precio;
			clase = _clase;
			letraNegra = _letra;
		}
		
		public override string ToString() { return texto; }
	}
	
	// ----------------------------- CLASES PROPIEDADED Y SET -----------------------------

	enum construcciones { NADA, CASA, HOTEL }
	
	public class Propiedad {
		
		public readonly string nombre;
		public readonly int    precio;
		
		bool wildTot;
		bool wildMed;
		
		public readonly string color;
		
		public readonly int setNum; // numero de cartas en el set
		
		public readonly bool letra;
		
		public Propiedad(string _nombre, int _precio, string _color, int _setN, bool _tot = false, bool _med = false, bool _letra = false) {
			nombre = _nombre;
			precio = _precio;
			
			color = _color;
			
			setNum = _setN;
			
			wildTot = _tot;
			wildMed = _med;
			letra = _letra;
		}
		
		public bool isNormalCart() { if (wildMed || wildTot) {return false; } return true; }
		public bool isTotWild()    { return wildTot; }
		
		public override string ToString() {
			return nombre + " " + precio.ToString();
		}
	}
	
	Propiedad getPropiedad(string nombre) {
		foreach (Propiedad p in props) { if (p.nombre == nombre) { return p; } }
		return null;
	}
	
	public class Set {
		int max;
		int currentProps = 0;
		
		bool completo = false;
		
		construcciones cons = construcciones.NADA;
		
		public readonly Propiedad[] prop;
	
		public readonly int[] precios;
	
		public Set(Propiedad p) {   // Utilizado para guardar los sets que tiene el jugador
			max = p.setNum;
			prop = new Propiedad[max];
			
			precios = new int[max]; // ¡¡¡ hay que importar los precios
		}
		
		public Set(Propiedad[] p, int[] _precios) { // Utilizado sobretodo para guardar todos los sets completos y utilizarlos como tabla de busqueda
			max = p[0].setNum;
			prop = p;
			
			precios = _precios;
		}
		
		public void addProp(Propiedad p) { // Añade una propiedad al set
			prop[currentProps] = p;
			currentProps++;
		}
		
		public bool canAdd() { if (max == currentProps) { return false; } return true; } // Dice si se pueden añadir propiedades a este set
	
		string construir(construcciones _cons) { // Solo toma como parametro una casa o un hotel
			switch (cons) {
				case construcciones.CASA:   // En este set hay construida una casa
					if (_cons == cons) { return "Ya hay construida una casa"; } // Y la persona quiere construir otra casa -.-"
					
					// Si estamos aqui quiere decir que ya hay una casa construida y que quiere construir un hotel
					cons = construcciones.HOTEL;
					return "Exito, hotel construido";
				case construcciones.HOTEL:
					return "Imposible construir";
				default:
					cons = _cons;
					return "Exito, " + _cons.ToString() + " construido";
			}
		}
		
		public override string ToString() {
			string tmp = string.Empty;
			
			foreach (Propiedad p in prop) {
				tmp += " - " + p.nombre;
			}
			
			return "{" + tmp + " }";
		}
	}
	
	// ----------------------------- COMUNICACION CON EL SERVIDOR -----------------------------
	
	Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	byte[] _buffer = new byte[1024];
	
	const int puerto = 28999;
	int id = -1;
	
	enum estado { ESPERANDO, STANDBY, TURNO };
	estado estadoActual = estado.ESPERANDO;
	
	int numeroPuntosComas(string txt) {
		int i = 0;
		foreach (char s in txt) { if (s == ';') i++; }
		return i + 1;
	}
	
	string prepararMensaje(string[] msg) {
		string temp = string.Empty;
		foreach (string s in msg) { temp += s + ";"; }
		return temp.Substring(0, temp.Length - 1);
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
	
	void enviarMensaje(string msg) {
		byte[] buffer = Encoding.ASCII.GetBytes(msg);
		_clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
		
		buffer = new byte[2048];
		int received = _clientSocket.Receive(buffer, SocketFlags.None);
		byte[] data = new byte[received];
		Array.Copy(buffer, data, received);
		
		sgnlDcder(Encoding.ASCII.GetString(data));
	}
	
	estado getEstado(string _es) {
		foreach (estado e in (estado[]) Enum.GetValues(typeof(estado))) { if (e.ToString() == _es) return e; }
		return estado.STANDBY;
	}
	
	void sgnlDcder(string txt) {
		string[] mensaje = new string[numeroPuntosComas(txt)];
		mensaje = tratarMensaje(txt, mensaje.Length);
		
		switch (mensaje[0]) {
			case "UNIDO":
				id = int.Parse(mensaje[1]);
				
				Console.WriteLine("Unido a la partida con la id " + id.ToString() + ". Esperando que empiece...");
			break;
			case "ESTADO":
				if (estadoActual.ToString() != mensaje[1]) { estadoActual = getEstado(mensaje[1]); accionEstado(); }
			break;
			case "PEDIR":
				for (int i = 0; i < int.Parse(mensaje[1]); i++) { addCarta(mensaje[2 + i]); Console.WriteLine(mensaje[2 + i]); }
				dibujarPantalla();
			break;
			case "NOEXITO":
				Console.WriteLine("Ha habido un error :/ (NOEXITO)");
			break;
			default:
				Console.WriteLine("WTF? How.......");
			break;
		}
	}
	
	void iniciarCliente() {
		Console.Clear();
		
		while (!_clientSocket.Connected) { // Conecta al cliente con el servidor
			Console.WriteLine("Introduce la IP del servidor: ");
			string ip = Console.ReadLine();
			
			try {
				_clientSocket.Connect(ip, puerto);	
			} catch (SocketException) {
				Console.WriteLine("Error en la conexion con el servidor, comprueba la IP");
			}
		}
	
		Console.WriteLine("Conectado con el servidor");
	}
	
	// ----------------------------- INICIO CODIGO JUEGO -----------------------------
	
	// Cartas del jugador (sean propiedades puestas en la mesa o cartas por jugar)
	Set[] propiedades     = new Set[10]   { new Set(), new Set(), new Set(), new Set(), new Set(), new Set(), new Set(), new Set(), new Set(), new Set() };
	public Carta[] cartas = new Carta[12] { new Carta(), new Carta(), new Carta(), new Carta(), new Carta(), new Carta(), new Carta(), new Carta(), new Carta(), new Carta(), new Carta(), new Carta() };
	
	int setsDisponibls = 0;
	int cartaSelect    = 0;
	int cartas_jugadas = 0;
	int cartasUtiles   = 0;
	
	void addCarta(string _c) {
		Carta ca = new Carta();
		foreach (Carta c in todas)  { if (c.texto == _c) ca = c; } // Buscar la Carta
		
		if (ca.texto == "NONE") { foreach (Propiedad p in props) { if (p.nombre == _c) ca = new Carta(_c, p.precio, tipo.PROPIEDAD, p.letra); } }
		
		for (int i = 0; i < cartas.Length; i++) { if (cartas[i].texto == "NONE") { cartas[i] = ca; break; } } // Añadir a la baraja
	}
	
	void quitarCarta(int pos) {
		cartasUtiles--;
		for (int i = pos; i < cartas.Length - 1; i++) {	cartas[i] = cartas[i + 1]; }
		cartas[cartas.Length - 1] = new Carta();
	}
	
	public cliente() {
		todas = new Carta[30] { dlBrk, slyDl, frcDl, brthd, dbtCl, jstNo, pssGo, dblRn, rnWld, rnMAC, rnNaM, rnAmR, rnVAO, rnTrU, hotel, casas, wldTt, mdMAC, mdNam, mdAmR, mdVAO, mdVTr, mdTAZ, mdTrU, bllt1, bllt2, bllt3, bllt4, bllt5, bll10 };
		
		initColors();
		cargarXML();
		iniciarCliente();
		
		string nametag = "ro"; // QUITAR
		
		Console.Clear();
		while (nametag.Length == 0 || nametag.Length >= 10 || nametag.Contains(" ") || numeroPuntosComas(nametag) > 1) {
			Console.WriteLine("Introduce tu nombre de usuario: ");
			nametag = Console.ReadLine();
			Console.WriteLine("Nombre invalido, tiene que tener maximo 10 letras, no contener ni espacios ni ;");
			Console.Clear();
		}
		enviarMensaje(prepararMensaje(new string[2] { "UNIRSE", nametag }));
		

		while (true) {
			if (id != -1) enviarMensaje(prepararMensaje(new string[2] { "ESTADO", id.ToString() }));
			Thread.Sleep(500);
		}
	}
	
	public static void Main() {
		Console.WriteLine("Bienvenido a Alcor poly Real");
		
		cliente c = new cliente();
	}
	
	bool turno = false;
	
	void accionEstado() {
		switch (estadoActual) {
			case estado.ESPERANDO:
				Console.Clear();
				Console.WriteLine("Esperando al resto de jugadores");
			break;
			case estado.STANDBY:
				dibujarPantalla();
			break;
			case estado.TURNO:
				turno = true;
				
				Console.WriteLine("Programar los turnos :P");
				
				// Primero pedir cartas
				if (cartasUtiles == 0) { enviarMensaje(prepararMensaje(new string[2] { "PEDIR", "5" })); cartasUtiles += 5; }
				else 				   { enviarMensaje(prepararMensaje(new string[2] { "PEDIR", "2" })); cartasUtiles += 2; }
				
				while (turno) {
					var tecla = Console.ReadKey().Key;
					
					if      (tecla == ConsoleKey.RightArrow && maxDesplazab != 0) { cartaSelect++; cartaSelect %= maxDesplazab; dibujarPantalla(); }
					else if (tecla == ConsoleKey.LeftArrow  && maxDesplazab != 0)  { cartaSelect--; cartaSelect %= maxDesplazab; dibujarPantalla(); }
					else if (tecla == ConsoleKey.UpArrow)    { selectPs--; selectPs %= 3; cartaSelect = 0; dibujarPantalla(); }
					else if (tecla == ConsoleKey.DownArrow)  { selectPs++; selectPs %= 3; cartaSelect = 0; dibujarPantalla(); }
					else if (tecla == ConsoleKey.Enter)      { action(cartas[cartaSelect]); }
				}
				
				// Enviar que se ha acabado el turno
			break;
		}
	}
	
	void action(Carta c) { // !!!!!!!!!!!!!!!!! POR HACER, QUE OCURRE CUANDO EL JUGADOR JUEGA UNA CARTA
		Console.WriteLine("Programar este tipo de acción !");
		switch (c.clase) {
			case tipo.PROPIEDAD:
				if (c.texto.Contains("COMODIN")) {
					if (contarEspacios(c.texto) == 1) { // Significa COMODIN CARD que puede ser cualquiera
						
					} else { // Comodin a elegir entre dos colores
						
					}
				} else {
					Propiedad p = getPropiedad(c.texto);
				}
			break;
			case tipo.DINERO:
				addBillete(c.precio);
				
				quitarCarta(cartaSelect);
				cartas_jugadas++;
			
				dibujarPantalla();
			break;
			case tipo.ACCION:
				
			break;
			case tipo.CONSTRUCCION:
			break;
			case tipo.RENTA:
			break;
		}
		
		if (cartas_jugadas == 3) { turno = false; Console.WriteLine("Fin"); }
	}
	
	int[] billetes = new int[] { 0, 0, 0, 0, 0, 0 }; // En este orden: 1, 2, 3, 4, 5, 10
	void addBillete(int precio) {
		if (precio < 10) { billetes[precio - 1]++; } else { billetes[5]++; }
	}
	
	int contarEspacios(string s) {
		int a = 0;
		for (int i = 0; i < s.Length; i++) {
			if (s[i] == ' ') { a++; }
		}
		return a;
	}
	
	// ----------------------------- PARTE GRÁFICA -----------------------------
	int     selectPs = 0; // Posicion del select: 0 -> propiedades, 1 -> cartas, 2 -> botones
	int maxDesplazab = 0; // Maximo que se puede desplazar;
	
	string[] dibProps = new string[9]; // Recalcular el numero 9
	string[] dibBarja = new string[9];
	string   dibSelec = string.Empty;
	
	Carta    cartaMedio = new Carta("QUE EMPIECE LA PARTIDA", 10, tipo.ACCION);
	string[] cartaMd = new string[9];
	string   txtHost = string.Empty;
	
	string[] botones     = new string[3] { " ################# ################# #################", " #               # #               # #               #", " ################# ################# #################" };
	
	void resetearProps() { for (int i = 0; i < dibProps.Length; i++) { dibProps[i] = string.Empty; } }
	void resetearBarja() { dibSelec = string.Empty; for (int i = 0; i < dibBarja.Length; i++) { dibBarja[i] = string.Empty; } }
	void resetearCrtMd() { txtHost = string.Empty;  for (int i = 0; i < cartaMd.Length ; i++) { cartaMd[i]  = string.Empty; } }

	void dibujarPantalla() {
		switch (selectPs) {
			case 0:
				maxDesplazab = setsDisponibls;
			break;
			case 1:
				maxDesplazab = cartasUtiles;
			break;
			case 2:
				maxDesplazab = 3;
			break;
		}
		
		resetearProps();
		resetearBarja();
		resetearCrtMd();
		
		Console.Clear();
		
		dibujarCartaSelec();
		dibujarBarja();
		
		dibujarString(dibProps);
		Console.WriteLine();
		if (selectPs != 2) Console.WriteLine(dibSelec);
		Console.WriteLine();
		dibujarString(dibBarja);
		Console.WriteLine();
		Console.WriteLine("CARTA EN EL MEDIO");
		Console.WriteLine();
		dibujarMedio();
		Console.WriteLine();
		if (selectPs == 2) Console.WriteLine(dibSelec);
		Console.WriteLine();
		dibujarBotones();
	}
	
	void dibujarBotones() {
		foreach (string s in botones) { Console.WriteLine(s); }
	}
	
	void dibujarMedio() {
		string[] d = dibujarCarta(cartaMedio);
		foreach (string s in d) { Console.WriteLine(s); }
	}
	
	void dibujarString(string[] _s) {
		foreach (string s in _s) {
			Console.WriteLine();
			bool color = false;
			string clr = string.Empty;
			
			foreach (char letra in s) {
				if (letra == '$') {
					color = true;
					continue;
				}
				
				if (color && letra == '#') {
					color = false;
					Console.BackgroundColor = colores[clr];
					clr = string.Empty;
					
					continue;
				} else if (color && letra == ']') {
					color = false;
					Console.ForegroundColor = colores[clr];
					clr = string.Empty;
					
					continue;
				}
				
				if (!color) {
					Console.Write(letra);
					continue;
				}
				
				clr += letra;
			}

		}
	}
	
	string centrarTexto(string txt, int tot = 15) {
		int izq =(tot - txt.Length) / 2;
		int der = (tot - txt.Length) / 2 + (tot - txt.Length) % 2;;
		string tmp = string.Empty;
		
		while (izq != 0) { tmp += " "; izq--; }
		tmp += txt;
		while (der != 0) { tmp += " "; der--; }
		
		return tmp;
	}
	
	string[] separarPalabras(string txt) {
		int cnt = 0;
		foreach (char a in txt) { if (a == ' ') cnt++; }
		
		string[] tmp = new string[cnt + 1];
		
		cnt = 0;
		string temp = string.Empty;
		foreach (char letra in txt) {
			if (letra == ' ') {
				tmp[cnt] = temp;
				temp = string.Empty;
				cnt++;
				
				continue;
			}
			temp += letra;
		}
		tmp[cnt] = temp;
		
		return tmp;
	}
	
	IDictionary<string, ConsoleColor> colores = new Dictionary<string, ConsoleColor>();
	
	void initColors() {
		colores.Add("MARRON", ConsoleColor.DarkYellow);
		colores.Add("AZULCLARO", ConsoleColor.DarkCyan);
		colores.Add("MORADO", ConsoleColor.DarkMagenta);
		colores.Add("NARANJA", ConsoleColor.DarkRed);
		colores.Add("ROJO", ConsoleColor.Red);
		colores.Add("AMARILLO", ConsoleColor.Yellow);
		colores.Add("VERDE", ConsoleColor.DarkGreen);
		colores.Add("AZULOSCURO", ConsoleColor.Blue);
		colores.Add("TREN", ConsoleColor.Black);
		colores.Add("UTILIDAD", ConsoleColor.Cyan);
		colores.Add("NEGRO", ConsoleColor.Black);
		colores.Add("BLANCO", ConsoleColor.White);
	}
	
	string getColor(string nombre) {
		foreach (Set s in sets) {
			foreach(Propiedad p in s.prop) {
				if (p.nombre == nombre) return p.color;
			}
		}
		return null;
	}
	
	
	int[] getPrecios(string color) {
		foreach (Set s in sets) { if (s.prop[0].color == color) { return s.precios; } }
		
		return null;
	}
	
	void wildCart(string[] palabras) {  // Se encarga de dibujar los comodines en las cartas en la mano
		if (palabras[1] == "CARD") {
			dibCarta[2] += "  ";
			foreach (var c in colores) {
				dibCarta[2] += "$" + c.Key + "# ";
			}
			dibCarta[2] += "$NEGRO# ";
			
			dibCarta[4] += centrarTexto("COMODIN", 15);
			
			for (int i = 0; i < 8; i++) {
				if (i == 2 || i == 4) continue;
				dibCarta[i] += centrarTexto(string.Empty, 15);
			}
			
			for (int i = 1; i < 8; i++) { dibCarta[i] += "#"; }
			return;
		}
		
		dibCarta[1] += "   $" + palabras[1] + "#" + centrarTexto("COMODIN", 9) + "$" + "NEGRO" + "#   ";
		for (int i = 2; i < 7; i++) { if (i == 4) { continue; } dibCarta[i] += centrarTexto(string.Empty, 15); }
		dibCarta[4] += centrarTexto("ELIGE COLOR", 15);
		dibCarta[7] += "   $" + palabras[2] + "#" + centrarTexto("COMODIN", 9) + "$" + "NEGRO" + "#   ";
		
		for (int i = 1; i < 8; i++) { dibCarta[i] += "#"; }
	}
	
	void cualquier() { // Se encarga de dibujar la renta multicolor
		dibCarta[2] += "  ";
			foreach (var c in colores) {
				dibCarta[2] += "$" + c.Key + "# ";
			}
			dibCarta[2] += "$NEGRO# ";
			
			dibCarta[4] += centrarTexto("RENTA", 15);
			
			for (int i = 0; i < 8; i++) {
				if (i == 2 || i == 4) continue;
				dibCarta[i] += centrarTexto(string.Empty, 15);
			}
			
			for (int i = 1; i < 8; i++) { dibCarta[i] += "#"; }
			return;
	}
	
	void dibujarBarja() {
		foreach (Carta c in cartas) {
			int i = 0;
			foreach (string s in dibujarCarta(c)) {	dibBarja[i] += s; i++;}
		}
	}
	
	string[] dibCarta;
	string[] dibujarCarta(Carta c) {
		if (c.precio == -1) return new string[9];
		dibCarta = new string[9];
		
		if (c.precio < 10) {  dibCarta[0] += " " + c.precio.ToString() + "-#############-" + c.precio.ToString(); dibCarta[8] += " " + c.precio.ToString() + "-#############-" + c.precio.ToString(); }
		else               { dibCarta[0] += " " + c.precio.ToString() + "-###########-" + c.precio.ToString();   dibCarta[8] += " " + c.precio.ToString() + "-###########-" + c.precio.ToString(); }
		
		for (int i = 1; i < 8; i++) {		
			dibCarta[i] += " #";
		}
		
		string[] palabras = separarPalabras(c.texto);
		
		switch (c.clase) {
			case tipo.ACCION:
				for (int i = 1; i < 3; i++) {dibCarta[i] += centrarTexto(string.Empty, 15); }
				for (int i = 0; i < palabras.Length; i++) { dibCarta[i + 3] += centrarTexto(palabras[i], 15); }
				for (int i = palabras.Length; i < 5; i++) { dibCarta[i + 3] += centrarTexto(string.Empty, 15); }
				for (int i = 1; i < 8; i++) { dibCarta[i] += "#"; }
				
			break;
			case tipo.DINERO:
				goto case tipo.ACCION;
			case tipo.PROPIEDAD:
				if (c.texto.Contains("COMODIN")) { wildCart(palabras); break; }
			
				dibCarta[2] = dibCarta[2].Substring(0, dibCarta[2].Length - 2);
				dibCarta[2] += " ·-#############-·";
				
				string clr = getColor(c.texto);
				
				dibCarta[1] += "$" + clr + "#"; // Aplicar el color de fondo
				if (c.letraNegra) dibCarta[1] += "$NEGRO]"; // Aplicar el color a las letras (si es necesario)
				
				dibCarta[1] += centrarTexto(c.texto, 15);
				
				dibCarta[1] += "$NEGRO#";
				dibCarta[1] += "$BLANCO]"; // Quitar el color a las letras (si es necesario)
				
				int[] prix = getPrecios(clr);
				
				for (int i = 0; i < prix.Length; i++) {
					dibCarta[i + 3] += centrarTexto(string.Empty, 7) + (i + 1).ToString() + " --- " + prix[i].ToString() + " ";
				}
				
				for (int i = prix.Length; i < 5; i++) {
					dibCarta[i + 3] += centrarTexto(string.Empty, 15);
				}
				for (int i = 1; i < 8; i++) { if (i == 2) { continue; } dibCarta[i] += "#"; }
				
			break;
			case tipo.RENTA:
				if (c.texto.Contains("CUALQUIER")) { cualquier(); break; } // TODO: HACER LA RENTA MULTICOLOR
				
				dibCarta[1] += centrarTexto(string.Empty, 15);
				dibCarta[2] += centrarTexto("RENTA", 15);
				dibCarta[3] += centrarTexto(string.Empty, 15);
				dibCarta[4] += "   $" + palabras[1] + "#" + centrarTexto(string.Empty, 9) + "$" + "NEGRO" + "#   ";
				dibCarta[5] += centrarTexto(string.Empty, 15);
				dibCarta[6] += "   $" + palabras[2] + "#" + centrarTexto(string.Empty, 9) + "$" + "NEGRO" + "#   ";
				dibCarta[7] += centrarTexto(string.Empty, 15);
				for (int i = 1; i < 8; i++) { dibCarta[i] += "#"; }
			
			break;
			case tipo.CONSTRUCCION:
				dibCarta[1] += centrarTexto(string.Empty, 15);
				dibCarta[2] += centrarTexto(c.texto, 15);
				dibCarta[3] += centrarTexto(string.Empty, 15);
			
				switch (c.texto) {
					case "CASA":
						dibCarta[4] += "$VERDE]" + centrarTexto("▓", 15) + "$BLANCO]";
						dibCarta[5] += "$VERDE]" + centrarTexto("▓▓▓", 15) + "$BLANCO]";
						dibCarta[6] += "$VERDE]" + centrarTexto("▓▓▓▓▓", 15) + "$BLANCO]";
						dibCarta[7] += "$VERDE]" + centrarTexto("▓ ▓", 15) + "$BLANCO]";
					break;
					default:
						dibCarta[4] += "$ROJO]" + centrarTexto("▓▓▓▓▓", 15) + "$BLANCO]";
						dibCarta[5] += "$ROJO]" + centrarTexto("▓ ▓ ▓", 15) + "$BLANCO]";
						dibCarta[6] += "$ROJO]" + centrarTexto("▓▓▓▓▓", 15) + "$BLANCO]";
						dibCarta[7] += "$ROJO]" + centrarTexto("▓▓ ▓▓", 15) + "$BLANCO]";
					
					break;
				}
				
				for (int i = 1; i < 8; i++) { dibCarta[i] += "#"; }
			break;
		}
		
		return dibCarta;
	}

	void dibujarCartaSelec() {
		for (int i = 0; i < cartaSelect; i++) { dibSelec += centrarTexto(string.Empty, 18); }
		if   (selectPs != 0) dibSelec += " ╔===============╗";
		else dibSelec += " ╚===============╝";
	}
}
