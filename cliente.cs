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
	private Carta dlBrk = new Carta("POS AHORA ES MIO", 5, tipo.ACCION); // DEAL BREAKER
	private Carta slyDl = new Carta("VEN PA CA", 3, tipo.ACCION);        // SLY DEAL
	private Carta frcDl = new Carta("SWITCH", 3, tipo.ACCION);           // FORCED DEAL
	private Carta brthd = new Carta("REGALO CUMPLE", 2, tipo.ACCION);    // BIRTHDAY
	private Carta dbtCl = new Carta("PAGAME BITCH", 3, tipo.ACCION);	 // PAY DEBT
	private Carta jstNo = new Carta("NO ES NO", 4, tipo.ACCION);         // JUST SAY NO
	private Carta pssGo = new Carta("COGE 2 CARTAS", 1, tipo.ACCION);    // PASS GO
	private Carta dblRn = new Carta("PAGAS DOBLE", 1, tipo.ACCION);      // DOUBLE RENT
	
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
	
	// WILDS MEDIOS
	
	private Carta wldTt = new Carta("COMODIN CARD", 9, tipo.PROPIEDAD);
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
		
			bool     letra   = bool.Parse(node.SelectSingleNode("letra").InnerText);
		
			for (int i = 0; i < cantidad; i++) {
				precios[i] = int.Parse(_precis[i]);
				
				props[j] = new Propiedad(nombres[i], precio, color, cantidad, _precis, false, false, letra);
				j++;
				
				ps[i]    = new Propiedad(nombres[i], precio, color, cantidad, _precis, false, false, letra);
			}
			
			sets[k] = new Set(ps, precios);
			sets[k].actualizarColor(ps[0].color);
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
	
		public Carta(string _texto = "none", int _precio = -1, tipo _clase = tipo.ACCION, bool _letra = false) {
			texto = _texto;
			precio = _precio;
			clase = _clase;
			letraNegra = _letra;
		}
		
		public override string ToString() { return texto; }
	}
	
	// ----------------------------- CLASES PROPIEDADED Y SET -----------------------------

	public enum construcciones { NADA, CASA, HOTEL }
	
	public class Propiedad {
		
		public string nombre;
		public readonly int    precio;
		
		public readonly bool wildTot;
		public readonly bool wildMed;
		
		public readonly string color;
		
		public readonly int setNum; // numero de cartas en el set
		
		public readonly bool letra;
		
		public readonly int[] precios;
		
		public Propiedad(string _nombre, int _precio, string _color, int _setN, string[] _precios, bool _tot = false, bool _med = false, bool _letra = false) {
			nombre = _nombre;
			precio = _precio;
			precios = stringIntArray(_precios);
			
			color = _color;
			
			setNum = _setN;
			
			wildTot = _tot;
			wildMed = _med;
			letra = _letra;
		}
		
		public bool isNormalCart() { if (wildMed || wildTot) {return false; } return true; }
		public bool isTotWild()    { return wildTot; }
		
		int[] stringIntArray(string[] p) { 
			int[] a = new int[p.Length];
			for (int i = 0; i < p.Length; i++) { a[i] = int.Parse(p[i]); }
			return a;
		}
		
		public override string ToString() {
			return nombre + " " + precio.ToString();
		}
	}
	
	Propiedad getPropiedad(string nombre) {
		foreach (Propiedad p in props) { if (p.nombre == nombre) { return p; } }
		return null;
	}
	
	public class Set {
		public readonly int max;
		public int currentProps = 0;
		public string color;
		public bool letra;
		
		public bool completo;
		
		public construcciones cons = construcciones.NADA;
		
		public readonly Propiedad[] prop;
	
		public readonly int[] precios;
	
		public Set(Propiedad p) {   // Utilizado para guardar los sets que tiene el jugador
			max = p.setNum;
			prop = new Propiedad[max];
			prop[0] = p;
			
			// Anadir instancias vacias de propiedades para que no de null error
			for (int i = 1; i < max; i++) { prop[i] = new Propiedad("none", 0, "AMARILLO", 0, new string[1] { "0" }); }
			
			color = p.color;
			letra = p.letra;
			
			currentProps = 1;
			
			precios = p.precios;
		}
	
		public Set() { color = "none"; letra = false; }
	
		public Set(Propiedad[] p, int[] _precios) { // Utilizado sobretodo para guardar todos los sets completos y utilizarlos como tabla de busqueda
			max = p[0].setNum;
			prop = p;
			
			color = p[0].color;
			letra = p[0].letra;
			
			currentProps = p.Length;
			
			precios = _precios;
		}
		
		public void addProp(Propiedad p) { // Añade una propiedad al set
			prop[currentProps] = p;
			currentProps++;
			
			if (currentProps == max) completo = true;
		}
	
		//	public Propiedad(string _nombre, int _precio, string _color, int _setN, string[] _precios, bool _tot = false, bool _med = false, bool _letra = false) {
		public void quitarProp(int pos) {
			for (int i = pos; i < prop.Length - 1; i++) {
				prop[i] = prop[i + 1];
			}
			prop[max - 1] = new Propiedad("none", 0, "none", 0, new string[1] { "0" });
			
			if (completo) completo = false;
			currentProps--;
		}
	
		public string construir(construcciones _cons) { // Solo toma como parametro una casa o un hotel
			switch (cons) {
				case construcciones.NADA:
					cons = _cons;
					return "Exito, " + _cons.ToString() + " construido";
				default:
					return "Imposible construir";
			}
		}
		
		public void actualizarColor(string c) { color = c; }
		public void actualizarLetra(bool l)   { letra = l; }
		
		public override string ToString() {
			string tmp = string.Empty;
			
			foreach (Propiedad p in prop) {
				tmp += " - " + p.nombre;
			}
			
			return "{" + tmp + " }";
		}
	}
	
	public int[] colorPrecios(string color) {
		foreach (Set s in sets) { if (s.color == color) return s.precios; }
		return new int[4] {0, 0, 0, 0};
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
				if (txtHost != mensaje[2] && mensaje[1] != "ESPERANDO")				   { txtHost      = mensaje[2];           dibujarPantalla(); }
				if (cartaMedio.texto != mensaje[3] && mensaje[1] != "ESPERANDO")        { cartaMedio   = getCarta(mensaje[3]); dibujarPantalla(); }
			
				if (estadoActual.ToString() != mensaje[1]) { estadoActual = getEstado(mensaje[1]); accionEstado(); } // Ultimo porque accionEstado podria bloquear
			break;
			case "PEDIR":
				for (int i = 0; i < int.Parse(mensaje[1]); i++) { addCarta(mensaje[2 + i]); }
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
	
	Carta getCarta(string _c) {
		foreach (Carta c in todas)  { if (c.texto == _c) return c; } // Buscar la Carta
		return new Carta();
	}
	
	void addCarta(string _c) {
		Carta ca = getCarta(_c);
		
		if (ca.texto == "none") { foreach (Propiedad p in props) { if (p.nombre == _c) ca = new Carta(_c, p.precio, tipo.PROPIEDAD, p.letra); } }
		
		for (int i = 0; i < cartas.Length; i++) { if (cartas[i].texto == "none") { cartas[i] = ca; break; } } // Añadir a la baraja
	}
	
	void quitarCarta(int pos) {
		cartasUtiles--;
		maxDesplazab--;
		for (int i = pos; i < cartas.Length - 1; i++) {	cartas[i] = cartas[i + 1]; }
		cartas[cartas.Length - 1] = new Carta();
		if (maxDesplazab != 0) cartaSelect %= maxDesplazab;
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
	
	bool turno;
	
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
				
				// Primero pedir cartas
				if (cartasUtiles == 0) { enviarMensaje(prepararMensaje(new string[2] { "PEDIR", "5" })); cartasUtiles += 5; }
				else 				   { enviarMensaje(prepararMensaje(new string[2] { "PEDIR", "2" })); cartasUtiles += 2; }
				
				while (turno) {
					var tecla = Console.ReadKey().Key;
					
					if      (tecla == ConsoleKey.RightArrow && maxDesplazab != 0) { cartaSelect++; cartaSelect %= maxDesplazab; dibujarPantalla(); }
					else if (tecla == ConsoleKey.LeftArrow  && maxDesplazab != 0)  { cartaSelect--; cartaSelect %= maxDesplazab; dibujarPantalla(); }
					else if (tecla == ConsoleKey.UpArrow)    { selectPs--; selectPs %= 3; cartaSelect = 0; dibujarPantalla(); }
					else if (tecla == ConsoleKey.DownArrow)  { selectPs++; selectPs %= 3; cartaSelect = 0; dibujarPantalla(); }
					else if (tecla == ConsoleKey.Enter)      { if (selectPs == 1) { action(cartas[cartaSelect]); } else if (selectPs == 2) { boton(cartaSelect); } }
				}
				
				finTurno();
			break;
		}
	}
	
	void finTurno() {
		cartas_jugadas = 0;
		enviarMensaje(prepararMensaje(new string[2] { "FINTURNO", id.ToString() }));
	}
	
	void boton(int pos) { // 0 = ver dinero, 1 = ver enemigo, 2 = pasar turno
		switch (pos) {
			case 0:
				Console.Clear();
				
				Console.WriteLine();
				Console.WriteLine();
				
				for (int i = 0; i < billetes.Length; i++) { Console.WriteLine(" · BILLETES DE " + nombreBilletes[i] + ": " + billetes[i].ToString()); }
				
				Console.WriteLine();
				
				int tot = 0;
				for (int i = 0; i < billetes.Length; i++) { tot += billetes[i] * int.Parse(nombreBilletes[i]); }
				Console.WriteLine(" · TOTAL: " + tot.ToString());
				
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine("PULSA ENTER PARA VOLVER");
			
				bool temp = false;
				while (!temp) { var tecla = Console.ReadKey().Key; if (tecla == ConsoleKey.Enter) { temp = true; } }
				dibujarPantalla();
			break;
			case 1:
			break;
			case 2:
				if (turno) finTurno();
			break;
		}
	}
	
	void action(Carta c) { // !!!!!!!!!!!!!!!!! POR HACER, QUE OCURRE CUANDO EL JUGADOR JUEGA UNA CARTA
		Console.WriteLine("Programar este tipo de acción !");
		switch (c.clase) {
			case tipo.PROPIEDAD:
				if (c.texto.Contains("COMODIN")) {
					colocarComodin(c);
				} else { // Propiedad normal
					Propiedad p = getPropiedad(c.texto);
					
					int temp = -1;
					for (int i = 0; i < propiedades.Length; i++) { if (propiedades[i].color == p.color) { temp = i; break; } } // Se queda aqui si ya existe un set del mismo color
					if (temp != -1) { anadirASet(temp, p); } else { crearNuevoSet(p); } // Se llega aqui si no hay un set del mismo color
					
					quitarCarta(cartaSelect);
					dibujarPantalla();
				}
			break;
			case tipo.DINERO:
				addBillete(c.precio);
				
				quitarCarta(cartaSelect);
				cartas_jugadas++;
			
				dibujarPantalla();
			break;
			case tipo.ACCION:
				Console.WriteLine(c.texto);
			break;
			case tipo.CONSTRUCCION:
				Console.WriteLine("CONSTRUCCION");
			break;
			case tipo.RENTA:
				Console.WriteLine("RENTA");
			break;
		}
		
		if (cartas_jugadas == 3) { turno = false; Console.WriteLine("Fin"); }
	}
	
	void colocarComodin(Carta c, bool quitar = true) {
		bool elegido = false;
		if (contarEspacios(c.texto) == 1) { // Significa COMODIN CARD que puede ser cualquiera
			string[] colors = new string[sets.Length];
			for (int i = 0; i < colors.Length; i++) { colors[i] = sets[i].color; }
			
			int pos = 0;
			int dispos = colors.Length;
			
			for (int i = 0; i < propiedades.Length; i++) { if (propiedades[i].color != "none" && propiedades[i].completo) { for (int j = 0; j < colors.Length; j++) { if (colors[j] == propiedades[i].color) { colors[j] = "none"; dispos--; break; } } } }
			
			
			string[] temp = new string[5] { " #", " #", " #", string.Empty, " ╚=====╝" };
			foreach (string s in colors) { if (s != "none") { temp[1] += "$" + s + "#     $NEGRO##";  temp[2] += "######"; temp[0] += "######"; } }
			
			while (!elegido) {
				// Dibujar parntalla de eleccion
				Console.Clear();
				
				temp[4] = string.Empty;
				for (int i = 0; i < pos; i++) { temp[4] += centrarTexto(string.Empty, 6); }
				temp[4] += " ╚=====╝";
				
				dibujarString(temp);
				
				// Teclas
				var tecla = Console.ReadKey().Key;
				if 		(tecla == ConsoleKey.RightArrow) { pos++; pos %= dispos; }
				else if (tecla == ConsoleKey.LeftArrow)  { pos--; pos %= dispos; }
				else if (tecla == ConsoleKey.Enter)      { 
					foreach (Set s in propiedades) { if (s.color == colors[pos]) { s.addProp(new Propiedad(c.texto, c.precio, s.color, getTamanio(s.color), getPreciosString(s.color), true, false, c.letraNegra)); elegido = true; } } // aqui ya hay un set que existe y por lo tanto añadimos la carta
					if (!elegido) crearNuevoSet(new Propiedad(c.texto, c.precio, colors[pos], getTamanio(colors[pos]), getPreciosString(colors[pos]), true, false, c.letraNegra));
					elegido = true;
				}
			}
			if (quitar) quitarCarta(cartaSelect);
			dibujarPantalla();
		} else { // Comodin a elegir entre dos colores
			// Buscar los sets de los colores
			int[] disponibilidad = new int[2] { 0, 0 }; // 0 = crear, 1 = no completo, 2 = completo
			string[] colors = new string[2] { separarPalabras(c.texto)[1], separarPalabras(c.texto)[2] };
			Set[] ss = new Set[2] { new Set(new Propiedad("CREAR SET", 0, colors[0], 1, getPreciosString(colors[0]))), new Set(new Propiedad("CREAR SET", 0, colors[1], 1, getPreciosString(colors[1]))) };
			
			for (int i = 0; i < colors.Length; i++) { foreach (Set a in propiedades) { if (colors[i] == a.color) { ss[i] = a; disponibilidad[i] = 1; if (a.completo) { disponibilidad[i] = 2; }; break; } } }
			
			int pos = 0;
			string[] dibSets = new string[16];
			string mens = string.Empty;
			foreach (Set s in ss) { string[] proxSet = dibujarSet(s, false); for (int i = 0; i < proxSet.Length; i++) { dibSets[i] += proxSet[i]; } }
			
			while (!elegido) {
				// Dibujar la pantalla de eleccion
				Console.Clear();
				Console.Write(mens);
				dibujarString(dibSets);
				
				Console.WriteLine();
				for (int i = 0; i < pos; i++) { Console.Write(centrarTexto(string.Empty, 18)); }
				Console.Write(" ╚===============╝");
				
				// Teclas
				var tecla = Console.ReadKey().Key;
				if      (tecla == ConsoleKey.RightArrow) { pos++; pos %= disponibilidad.Length; }
				else if (tecla == ConsoleKey.LeftArrow)  { pos--; pos %= disponibilidad.Length; }
				else if (tecla == ConsoleKey.Enter)      { 
					switch (disponibilidad[pos]) {
						case 0:
							crearNuevoSet(new Propiedad("COMODIN", c.precio, colors[pos], getTamanio(colors[pos]), getPreciosString(colors[pos]), false, true, c.letraNegra));
							elegido = true;
						break;
						case 1:
							foreach (Set s in propiedades) { if (s.color == ss[pos].color) { s.addProp(new Propiedad("COMODIN", c.precio, colors[pos], getTamanio(colors[pos]), getPreciosString(colors[pos]), false, true, c.letraNegra)); } }
							elegido = true;
						break;
						case 2:
							mens = " IMPOSIBLE, EL SET SELECCIONADO YA ESTA COMPLETO";
						break;
					}
				}
			}
			if (quitar) quitarCarta(cartaSelect);
			dibujarPantalla();
		}
	}
	
	void crearNuevoSet(Propiedad p) {
		for (int i = 0; i < propiedades.Length; i++) { if (propiedades[i].color == "none") { propiedades[i] = new Set(p); break; } } setsDisponibls++;
	}
	
	void anadirASet(int pos, Propiedad p) { // pos es la posicion del set dentro de la variable propiedades; esta funcion se encarga de añadir una propiedad a un set cuando se selecciona la propiedad, no utilizar para añadir una carta a un set, para eso utilizar la funcion del set
		Console.Clear();
		
		if (propiedades[pos].completo) { // significa que el set contiene wildcards, ya que un set completo con cartas "normales" no puede recibir otra carta normal
			Propiedad[] posiblesComodines = new Propiedad[propiedades[pos].max]; // esta variable guardara el comodin
			for (int i = 0; i < propiedades[pos].max; i++) {  // Hay que separar ya que a la hora de dibujarlo la funcion lee el nombre de la carta
				if (propiedades[pos].prop[i].wildTot) {
					posiblesComodines[i] = propiedades[pos].prop[i];
					posiblesComodines[i].nombre = "COMODIN CARD";
					continue;
				} else if (propiedades[pos].prop[i].wildMed) {
					posiblesComodines[i] = propiedades[pos].prop[i];
					posiblesComodines[i].nombre = getColoresWild(posiblesComodines[i].color);				
					continue;
				}
				posiblesComodines[i] = new Propiedad("none", 0, "none", 0, new string[1] { "0" });
			}
			bool elegido = false;
			bool[] posicionesSelec = new bool[propiedades[pos].max];
			int posi = 0;
			int a = 0; // Cuenta el numero de comodines
			string[] dibCartas = new string[9];
			foreach (Propiedad po in posiblesComodines) { if (po.nombre != "none") { posicionesSelec[a] = true; a++; string[] proxCarta = dibujarCarta(new Carta(po.nombre, po.precio, tipo.PROPIEDAD, po.letra)); for (int i = 0; i < proxCarta.Length; i++) { dibCartas[i] += proxCarta[i]; } } else { for (int i = 0; i < dibCartas.Length; i++) { dibCartas[i] += string.Empty; } } };
			
			if (a == 1) { for (int i = 0; i < posiblesComodines.Length; i++) { if (posiblesComodines[i].nombre != "none") { propiedades[pos].quitarProp(i); propiedades[pos].addProp(p); colocarComodin(new Carta(posiblesComodines[i].nombre, posiblesComodines[i].precio, tipo.PROPIEDAD, posiblesComodines[i].letra), false); return; } } }
			// POR AQUI HAY UN ERROR QUE HACE QUE SE QUITE LA PRIMERA CARTA
			// Dibujar una pantalla para seleccionar el comodin que se tiene que ir
			dibujarString(dibCartas);
			while (!elegido) {
				
			}
		} else { propiedades[pos].addProp(p); }
	}
	
	int[] billetes = new int[] { 0, 0, 0, 0, 0, 0 }; // En este orden: 1, 2, 3, 4, 5, 10
	string[] nombreBilletes = new string[6] { "1", "2", "3", "4", "5", "10" };
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
	
	string[] dibProps = new string[16];
	string[] dibBarja = new string[9];
	string   dibSelec = string.Empty;
	
	Carta    cartaMedio = new Carta("QUE EMPIECE LA PARTIDA", 10, tipo.ACCION);
	string[] cartaMd = new string[9];
	string   txtHost = " Testing 101";
	
	string[] botones     = new string[3] { " ################# ################# #################", " #   VER DINERO  # #  VER ENEMIGOS # #  PASAR TURNO  #", " ################# ################# #################" };
	
	void resetearProps() { for (int i = 0; i < dibProps.Length; i++) { dibProps[i] = string.Empty; } }
	void resetearBarja() { dibSelec = string.Empty; for (int i = 0; i < dibBarja.Length; i++) { dibBarja[i] = string.Empty; } }
	void resetearCrtMd() { for (int i = 0; i < cartaMd.Length ; i++) { cartaMd[i]  = string.Empty; } }

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
		
		dibujarProps();
		dibujarCartaSelec();
		dibujarBarja();
		
		Console.WriteLine();
		Console.WriteLine();
		
		dibujarString(dibProps);
		
		Console.WriteLine();
		
		if (selectPs != 2) Console.WriteLine(dibSelec);
		
		Console.WriteLine();
		
		dibujarString(dibBarja);
		
		Console.WriteLine();
		
		Console.WriteLine();
		Console.WriteLine(txtHost);
		Console.WriteLine(" CARTA EN EL MEDIO");
		
		
		Console.WriteLine();
		
		dibujarString(dibujarCarta(cartaMedio));
		
		Console.WriteLine();
		if (selectPs == 2) Console.WriteLine(dibSelec);
		Console.WriteLine();
		
		dibujarString(botones);
	}
	
	void dibujarProps() { foreach (Set s in propiedades) { if (s.color == "none") { continue; } string[] proxSet = dibujarSet(s); for (int i = 0; i < dibProps.Length; i++) { dibProps[i] += proxSet[i]; } } }
	
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
		colores.Add("TRENES", ConsoleColor.Black);
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
	
	int getTamanio(string color) {
		foreach (Set s in sets) { if (s.prop[0].color == color) { return s.max; } }
		return 0;
	}
	
	int[] getPrecios(string color) {
		foreach (Set s in sets) { if (s.prop[0].color == color) { return s.precios; } }
		
		return null;
	}
	
	string[] getPreciosString(string color) {
		int[] prex = getPrecios(color);
		string [] p = new string[prex.Length];
		
		for (int i = 0; i < prex.Length; i++) { p[i] = prex[i].ToString(); }
		return p;
	}
	
	string[] wilds = new string[7] { "COMODIN MARRON AZULCLARO", "COMODIN NARANJA MORADO", "COMODIN AMARILLO ROJO", "COMODIN VERDE AZULOSCURO", "COMODIN VERDE TREN", "COMODIN TREN AZULCLARO", "COMODIN TREN UTILIDAD" };
	string getColoresWild(string color) { // Devuelve el nombre de una carta comodin de entre dos colores a partir de uno de los dos colores
		foreach (string s in wilds) { if (s.Contains(color)) { return s; } }
		return "none";
	}
	
	void wildCart(string[] palabras) {  // Se encarga de dibujar los comodines en las cartas en la mano
		if (palabras[1] == "CARD") { // Cualquier color
			dibCarta[2] += "  ";
			foreach (Set s in sets) {
				dibCarta[2] += "$" + s.color + "# ";
			}
			dibCarta[2] += "$NEGRO#   ";
			
			dibCarta[4] += centrarTexto("COMODIN", 15);
			
			for (int i = 1; i < 8; i++) {
				if (i == 2 || i == 4) continue;
				dibCarta[i] += centrarTexto(string.Empty, 15);
			}
			
			for (int i = 1; i < 8; i++) { dibCarta[i] += "#"; }
			return;
		}
		
		// Entre dos colores
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
	
	string[] dibSet;
	string[] dibujarSet(Set s, bool pp = true) {
		dibSet = new string[16]; // el set mas grande posible
		
		int construc = (s.cons == construcciones.NADA ? 0 : 1); // Devuelve 0 o 1 en funcion de si no hay o si (en ese orden) un edificio (construccion)
		
		if (s.cons != construcciones.NADA) { dibSet[0] += " " + getCarta(s.cons.ToString()).precio.ToString(); if (s.cons == construcciones.CASA) { dibSet[1] += " #$VERDE]" + centrarTexto("CASA", 14) + "$BLANCO]"; } else { dibSet[1] += " #$ROJO]" + centrarTexto("HOTEL", 14) + "$BLANCO]"; } } // Dibujar precio casa u hotel
	
		for (int i = construc; i < s.currentProps + construc; i++) { dibSet[i*2] += " " + s.prop[0].precio.ToString(); } // Dibujar los precios en los sets
		
		for (int i = 0; i < s.currentProps + construc; i++) { dibSet[i*2] += "-#############-"; dibSet[i*2 + 1] += " #"; } // Dibujar los bordes de las cartas de los sets
		
		for (int i = construc; i < s.currentProps + construc; i++) { dibSet[i*2 + 1] += "$" + s.color + "#"; if (s.letra) { dibSet[i*2 + 1] += "$NEGRO]"; } dibSet[i*2 + 1] += centrarTexto(s.prop[i - construc].nombre, 15); dibSet[i*2 + 1] += "$BLANCO]$NEGRO##"; }
		
		dibSet[(s.currentProps + construc) * 2] += " ·-#############-·"; // Ultima carta, linea divisoria
		
		for (int i = 0; i < 7; i++) { dibSet[(s.currentProps + construc) * 2 + 1 + i] += " #"; } // Borde izq
		
		int min = (s.currentProps + construc) * 2 + 1;
		
		for (int i = 0; i < s.precios.Length; i++) { dibSet[min + i] += centrarTexto(string.Empty, 7) + (i + 1).ToString() + " --- " + s.precios[i].ToString() + " "; } // Dibujar los precios
		
		for (int i = construc; i < s.currentProps + construc; i++) { dibSet[i*2] += s.prop[0].precio.ToString(); } // Dibujar los precios en los sets
		
		dibSet[min + 7] += " " + s.prop[0].precio.ToString() + "-#############-" + s.prop[0].precio.ToString(); // linea con precio al final de la carta
		
		// Calcular el precio de lo que costaria la renta en este set + dibujar precio del edificio
		int total = s.precios[s.currentProps - 1];
		if (s.cons != construcciones.NADA) { dibSet[0] += getCarta(s.cons.ToString()).precio.ToString(); total += getCarta(s.cons.ToString()).precio; }
		
		if (pp) { dibSet[15] += " " + centrarTexto(total.ToString(), 17); } else { dibSet[15] += " " + centrarTexto(string.Empty, 17); }
		
		for (int i = min + s.precios.Length; i < min + 7; i++) { dibSet[i] += centrarTexto(string.Empty, 15); } // Rellenar el resto de la carta
		for (int i = min; i < min + 7; i++) 				   { dibSet[i] += "#"; } // Borde exterior
		for (int i = min + 8; i < 15; i++)					   { dibSet[i] += centrarTexto(string.Empty, 18); }
		
		return dibSet;
	}
	
	string[] dibCarta;
	string[] dibujarCarta(Carta c) {
		if (c.precio == -1) return new string[9];
		dibCarta = new string[9];
		
		if (c.precio < 10) { dibCarta[0] += " " + c.precio.ToString() + "-#############-" + c.precio.ToString(); dibCarta[8] += " " + c.precio.ToString() + "-#############-" + c.precio.ToString(); }
		else               { dibCarta[0] += " " + c.precio.ToString() + "-###########-" + c.precio.ToString();   dibCarta[8] += " " + c.precio.ToString() + "-###########-" + c.precio.ToString(); }
		
		for (int i = 1; i < 8; i++) { dibCarta[i] += " #"; }
		
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
