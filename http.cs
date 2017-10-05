using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;

using System.Data.SqlClient;

namespace HTTPServer
{
	
	class Index
	{   
	
		public class Leaf
		{
			public int num {get; set;}
			public bool libruseq {get; set;}
			public Leaf next {get; set;}
		}
		
		public class Node
		{
			public char c {get; set;}
			public Node next {get; set;}
			public Node child {get; set;}
			public Leaf nums {get; set;}
		}
		
		public Node root = new Node();
		public Dictionary<string,string> ips;
		public Index()
        {
			root.next = null;
			root.child = null;
			root.nums = null;
			root.c = ' ';
			
			Node a;
			string example = "abcd";
			int num = 30456;
			
			a = GetNode(example,root);
			if (a == null) {a = CreateChain(example,root);}
			if (!AddLeaf(a,num))
			{
				// error occurs
			}
			example = DumpAllTree(root);
			
			ips =  new Dictionary<string,string>();
			ips.Add("Key","Value");
			
		}
		
		public Node GetNode(string chain, Node pwd) // pwd should exists and be child node or root
		{
			// get first char, ?? if empty string, return pwd ?? return null
			if (chain.Length==0) {return null;}
			char c = chain[0];
			// find in "next" branch-chain proper node, return null if we fail
			Node tmp = pwd;
			while ((tmp.next!=null)&&(tmp.c!=c))
			{
				tmp = tmp.next;
			} 
			if (tmp.c!=c) {return null;}
			// if cutted string have chars, call GetNode with cutted string and child of found node
			if (chain.Length>1)
			{
				if (tmp.child == null)
				{
					return null;
				}
				string cutted_chain = chain.Substring(1,chain.Length-1);
				Node result = GetNode(cutted_chain,tmp.child);
				return result;
			} else
			// else return found node
			{
				return tmp;
			}	
		}
		
		public Node CreateChain(string chain, Node pwd) // pwd should exists and be child node or root
		{
			// get first char, ?? if empty string, return pwd ?? return null
			if (chain.Length==0) {return null;}
			char c = chain[0];
			// find in "next" branch-chain proper node, and latch last
			Node tmp = pwd;
			Node last;
			while ((tmp.next!=null)&&(tmp.c!=c))
			{
				tmp = tmp.next;
			} 
			// if we fail with find, create new Node and bind it to last next Node
			if ((tmp.c!=c)&&(tmp.next==null)) 
			{
				tmp.next = new Node();
				tmp = tmp.next;
				tmp.next = null;
				tmp.nums = null;
				tmp.c = c;
				tmp.child = null;
			}
			// if child == null create child Node and fill all fields
			if (tmp.child == null)
			{
				tmp.child = new Node();
				tmp.child.next = null;
				tmp.child.child = null;
				tmp.child.nums = null;
				tmp.child.c = ' ';
			}
			// if cutted string have chars, call CreateChain with cutted string and child of found node or with created child of created node
			if (chain.Length>1)
			{
				string cutted_chain = chain.Substring(1,chain.Length-1);
				Node result = CreateChain(cutted_chain,tmp.child);
				return result;
			} else
			// else return found (created) node
			{
				return tmp;
			}	
			
			
		}
		
		public bool AddLeaf(Node node, int num, bool librueq)
		{
			// check if node exists
			if (node==null) {return false;}
			// check if num exists and latch last leaf
			Leaf tmp = node.nums;
			if (node.nums==null)
			{
				tmp = new Leaf();
				node.nums = tmp;
			}
			else
			{
				tmp = node.nums;
				while ((tmp.next!=null)&&(tmp.num!=num))
				{
					tmp = tmp.next;
				}
				if (tmp.num!=num)
				{
					return true;
				} else
				{
					tmp.next = new Leaf();
					tmp = tmp.next;
				}
			}
			
			// add leaf if it doesn't exists
			tmp.num = num;
			tmp.libruseq = libruseq;
			tmp.next = null;
			// return false if error and true if ok
			return true;
		}
		
		public string GetLeavesAsString(Leaf num)
		{
			// recursive collect all numbers into comma separated string
			
		}
		
		public string DumpAllTree(Node node)
		{
			// recursive walk through all tree and collect all leaves name and number list
			
		}
		
		public string GetBooks(string Author)
		{
			
			return ips[Author];
		}
	}
	
    // Класс-обработчик клиента
    class Client
    {
        // Отправка страницы с ошибкой
        private void SendError(TcpClient Client, int Code)
        {
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1
            string CodeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            // Код простой HTML-странички
            string Html = "<html><body><h1>" + CodeStr + "</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 " + CodeStr + "\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            Client.Close();
        }

       
		public void LogToDB(string thisip,string RequestUri)
		{
			string cmdString="INSERT INTO log ([IP_address],[request],[time]) VALUES (@val1, @val2, @val3)";
			string connString = "Server=tcp:uali.database.windows.net,1433;Initial Catalog=books-index;User ID=USERNAME;Password=PASSWORD;Encrypt=True;Connection Timeout=30;";
			using (SqlConnection conn = new SqlConnection(connString))
			{
				using (SqlCommand comm = new SqlCommand())
				{
					comm.Connection = conn;
					comm.CommandText = cmdString;
					comm.Parameters.AddWithValue("@val1", thisip);
					comm.Parameters.AddWithValue("@val2", RequestUri);
					comm.Parameters.AddWithValue("@val3", DateTime.Now);
					try
					{
						conn.Open();
						comm.ExecuteNonQuery();
					}
					catch (SqlException e)
					{
						Console.WriteLine(e.ToString());
					}
				}
			}
		}
		
		 // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client, Index Lib)
        {
			try
			{
            // Объявим строку, в которой будет хранится запрос клиента
            string Request = "";
			string webroot = @"C:\dl_tl\TL";
            // Буфер для хранения принятых от клиента данных
            byte[] Buffer = new byte[1024];
            // Переменная для хранения количества байт, принятых от клиента
            int Count;
            // Читаем из потока клиента до тех пор, пока от него поступают данные
            while ((Count = Client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                // Преобразуем эти данные в строку и добавим ее к переменной Request
                Request += Encoding.GetEncoding("windows-1251").GetString(Buffer, 0, Count);
                // Запрос должен обрываться последовательностью \r\n\r\n
                // Либо обрываем прием данных сами, если длина строки Request превышает 4 килобайта
                // Нам не нужно получать данные из POST-запроса (и т. п.), а обычный запрос
                // по идее не должен быть больше 4 килобайт
                if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
                {
                    break;
                }
            }

            // Парсим строку запроса с использованием регулярных выражений
            // При этом отсекаем все переменные GET-запроса
			string zip_file = "";
			int zip_ind = Request.IndexOf("{ZIP}");
			if (zip_ind>0)
				{
					zip_file = Request.Substring(zip_ind+5);
					zip_ind = zip_file.IndexOf("HTTP/");
					zip_file = zip_file.Substring(0,zip_ind);
				}
			
            Match ReqMatch = Regex.Match(Request, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");

            // Если запрос не удался
            if (ReqMatch == Match.Empty)
            {
                // Передаем клиенту ошибку 400 - неверный запрос
                SendError(Client, 400);
                return;
            }

            // Получаем строку запроса
            string RequestUri = ReqMatch.Groups[1].Value;

            // Приводим ее к изначальному виду, преобразуя экранированные символы
            // Например, "%20" -> " "
            RequestUri = Uri.UnescapeDataString(RequestUri);

            // Если в строке содержится двоеточие, передадим ошибку 400
            // Это нужно для защиты от URL типа http://example.com/../../file.txt
            if (RequestUri.IndexOf("..") >= 0)
            {
                SendError(Client, 400);
                return;
            }

			string thisip = Client.Client.RemoteEndPoint.ToString();
			thisip = thisip.Substring(0,thisip.IndexOf(':'));
				
			Console.WriteLine(DateTime.Now.ToString()+" : "+thisip+" : "+RequestUri);	
			LogToDB(thisip,Uri.UnescapeDataString(Request));
				
			if (zip_ind>0)							
            {	//##############################################################  begin of send requested file from zip_file to client							
				string folder = RequestUri;
				
				long asize = 0;
				long fsize = 0;
				System.DateTimeOffset lw = new DateTimeOffset();
				while (folder.IndexOf('/')>=0)
					{
						folder = folder.Substring(folder.IndexOf('/')+1);
					}
				if (!File.Exists(zip_file))
					{
						SendError(Client, 404);
						return;
					}	
				using (ZipArchive archive = ZipFile.OpenRead(zip_file))
				{
					foreach (ZipArchiveEntry entry in archive.Entries)
					{
						if (entry.FullName.EndsWith(folder, StringComparison.OrdinalIgnoreCase))
						{
							ZipArchiveEntry myEntry = entry;
							asize = myEntry.CompressedLength;
							fsize = myEntry.Length;
							lw = myEntry.LastWriteTime;
							if ((asize==0)&&(fsize==0))
							{
								SendError(Client, 404);
								return;
							}
							string Headers = "HTTP/1.1 200 OK\nContent-Type: " + "document/fb2" + "\nContent-Length: " + fsize + "\n\n";
							byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
							Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);
							
							long ftc = 0;
							try
							{
								using (var FS = myEntry.Open())
								{
									//Console.Write("#2 Entry open");
										while ((Count = FS.Read(Buffer, 0, Buffer.Length))>0)
										{
											// Читаем данные из файла

											// И передаем их клиенту
											ftc = ftc + Count;
											Client.GetStream().Write(Buffer, 0, Count);
										}
								}
							}
							catch (Exception ex)
							{
								// Если случилась ошибка, не посылаем клиенту ошибку
								Console.Write("#Exception"+ex.ToString());
								return;
							}
						}
					}
				}	
				
            }  //##############################################################  end of send requested file from zip_file to client
			else
			{	
				if (RequestUri.EndsWith("/~"))
				{ //
					string mydir = RequestUri.Replace("/~","/").Replace("/",@"\");
					string folder = RequestUri.Replace("/~","");
					while (folder.IndexOf('/')>=0)
					{
						folder = folder.Substring(folder.IndexOf('/')+1);
					}
					int dir_l = 0;
					int file_l = 0;
					string my_dir = webroot+mydir;
					mydir = "<table border=1>";
					try
					{
					string [] subdirectoryEntries = Directory.GetDirectories(my_dir);	
					string [] fileEntries = Directory.GetFiles(my_dir);
					dir_l = subdirectoryEntries.Length;
					file_l = fileEntries.Length;
					foreach(string DirName in subdirectoryEntries)
						{
							
							mydir = mydir + "<tr><td><a href='"+DirName.Replace(webroot,"")+"/~'>"+DirName.Replace(webroot,"~")+"</a></td><td>{DIR}</td><td>"+Directory.GetCreationTime(DirName).ToString("yyyy.MMM.dd")+"</td></tr>";
							
						}
					foreach(string fileName in fileEntries)
						{
							FileInfo fi = new FileInfo(fileName); //.zip
							mydir = mydir + "<tr><td><a href='"+fileName.Replace(webroot,"")+"'>"+fi.Name.Replace(webroot,"").Replace(".fb2","").Replace(".zip","")+"</a></td><td>"+(fi.Length/1024).ToString()+" KB</td><td>"+fi.LastWriteTime.ToString("yyyy.MMM.dd")+"</td></tr>";
							
						}
					}
					catch (Exception)
					{
						// Если случилась ошибка, посылаем клиенту ошибку 500
						// SendError(Client, 500);
						// return;
					}
					//------------ Get autors from SQL ---------------------------
					int sql_co = 0;
					if (((dir_l>file_l)||((dir_l==file_l)&&(dir_l==0))) && (folder.Length>0))
					{
						string connString = "Server=tcp:uali.database.windows.net,1433;Initial Catalog=books-index;User ID=USERNAME;Password=USERNAME;Encrypt=True;Connection Timeout=30;";
						using(SqlConnection con = new SqlConnection(connString))
						{
							con.Open();
							using(SqlCommand cmd = new SqlCommand("select top 10000 autor,count(*) as num from autors where autor like '"+folder+"%' group by autor order by 1", con))
							{
								using (SqlDataReader reader = cmd.ExecuteReader())
								{
									if (reader != null)
									{
										while (reader.Read())
										{
											string autor = reader.GetString(0);	
											mydir = mydir + "<tr><td><a href='"+autor+"/~'>"+autor+"</a></td><td>{SQL}</td><td>"+reader.GetValue(1).ToString()+"</td></tr>";
											sql_co++;
											//do something
										}
									}
								} // reader closed and disposed up here
							} // command disposed here
						} //connection closed and disposed here
					}
					
					if ((((dir_l<file_l)||((dir_l==file_l)&&(dir_l==0))) && (folder.Length>5))||(((dir_l==file_l)&&(dir_l==0))&&(RequestUri.IndexOf("{SQL}")>=0)))
					{
						string connString = "Server=tcp:uali.database.windows.net,1433;Initial Catalog=books-index;User ID=USERNAME;Password=USERNAME;Encrypt=True;Connection Timeout=30;";
						using(SqlConnection con = new SqlConnection(connString))
						{
							con.Open(); // TO DO add join on 
							using(SqlCommand cmd = new SqlCommand("select distinct [book-title],[file-name],main.number,size,[date] from main left join autors on main.number = autors.number and main.lib = autors.lib where autor like '"+folder+"%' order by main.number", con))
							{
								using (SqlDataReader reader = cmd.ExecuteReader())
								{
									if (reader != null)
									{
										while (reader.Read())
										{
											string book = reader.GetString(0);	
											
											mydir = mydir + "<tr><td><a href='"+reader.GetValue(2).ToString()+".fb2"+"?{ZIP}"+reader.GetString(1)+"'>"+book+"</a></td><td>"+reader.GetValue(3).ToString()+"</td><td>"+reader.GetValue(4).ToString()+"</td></tr>";
	
											//do something
										}
									}
								} // reader closed and disposed up here
							} // command disposed here
						} //connection closed and disposed here
					}
					// --------------------------------------------------------------------	
					mydir = mydir + "</table>"; 
					mydir = mydir + "<br>"+Lib.GetBooks("Key");
					string DirIndex = "<html><head><title>description</title><meta http-equiv='Content-Type' content='text/html; charset=windows-1251'></head> <body><h2>index of current dir :"+folder+"<br>You have connected from "+thisip+"<br>"+mydir+"</body></html>";
					string Headers = "HTTP/1.1 200 OK\nContent-Type: " + "text/html" + "\nContent-Length: " + DirIndex.Length + "\n\n";
					byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
					Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);
					
					byte[] DirIndexBuffer = Encoding.GetEncoding("windows-1251").GetBytes(DirIndex);
					Client.GetStream().Write(DirIndexBuffer, 0, DirIndexBuffer.Length);
				}
				else
				{   // ##################################### begin of dummy web server ##############################################
					// Если строка запроса оканчивается на "/", то добавим к ней index.html
					if (RequestUri.EndsWith("/"))
					{
						RequestUri += "index.html";
					}
					
					string FilePath = webroot+"/" + RequestUri;
		
					// Если в папке www не существует данного файла, посылаем ошибку 404
					if (!File.Exists(FilePath))
					{
						SendError(Client, 404);
						return;
					}
		
					// Получаем расширение файла из строки запроса
					string Extension = RequestUri.Substring(RequestUri.LastIndexOf('.'));
		
					// Тип содержимого
					string ContentType = "";
		
					// Пытаемся определить тип содержимого по расширению файла
					switch (Extension)
					{
						case ".htm":
						case ".html":
							ContentType = "text/html";
							break;
						case ".css":
							ContentType = "text/stylesheet";
							break;
						case ".js":
							ContentType = "text/javascript";
							break;
						case ".jpg":
							ContentType = "image/jpeg";
							break;
						case ".jpeg":
						case ".png":
						case ".gif":
							ContentType = "image/" + Extension.Substring(1);
							break;
						case ".svg": 
							ContentType = "image/svg+xml";
							break;
						default:
							if (Extension.Length > 1)
							{
								ContentType = "application/" + Extension.Substring(1);
							}
							else
							{
								ContentType = "application/unknown";
							}
							break;
					}
		
					// Открываем файл, страхуясь на случай ошибки
					FileStream FS;
					try
					{
						FS = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
					}
					catch (Exception)
					{
						// Если случилась ошибка, посылаем клиенту ошибку 500
						SendError(Client, 500);
						return;
					}
		
					// Посылаем заголовки
					string Headers = "HTTP/1.1 200 OK\nContent-Type: " + ContentType + "\nContent-Length: " + FS.Length + "\n\n";
					byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
					Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);
					
					try
					{
						
						// Пока не достигнут конец файла
						while (FS.Position < FS.Length)
						{
							// Читаем данные из файла
							Count = FS.Read(Buffer, 0, Buffer.Length);
							// И передаем их клиенту
							Client.GetStream().Write(Buffer, 0, Count);
						}
					}
					catch (Exception)
					{
						// Если случилась ошибка, не посылаем клиенту ошибку
						return;
					}
					
					// Закроем файл и соединение
					FS.Close();
				}   // ##################################### end of dummy web server ##############################################
			}
            Client.Close();
			}
		catch (Exception)
		{}
        }
		
    }

    class Server
    {
        TcpListener Listener; // Объект, принимающий TCP-клиентов
		public static Index Lib;
		
        // Запуск сервера
        public Server(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port); // Создаем "слушателя" для указанного порта
            Listener.Start(); // Запускаем его

            // В бесконечном цикле
            while (true)
            {
                // Принимаем новых клиентов. После того, как клиент был принят, он передается в новый поток (ClientThread)
                // с использованием пула потоков.
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());

                /*
                // Принимаем нового клиента
                TcpClient Client = Listener.AcceptTcpClient();
                // Создаем поток
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(Client);
                */
            }
        }

        static void ClientThread(Object StateInfo)
        {
            // Просто создаем новый экземпляр класса Client и передаем ему приведенный к классу TcpClient объект StateInfo
            new Client((TcpClient)StateInfo,(Index)Lib);
        }

        // Остановка сервера
        ~Server()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }

        static void Main(string[] args)
        {
			Lib = new Index();
            // Определим нужное максимальное количество потоков
            // Пусть будет по 4 на каждый процессор
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            // Установим максимальное количество рабочих потоков
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            // Установим минимальное количество рабочих потоков
            ThreadPool.SetMinThreads(2, 2);
            // Создадим новый сервер на порту 80
            new Server(80);
        }
    }
}
