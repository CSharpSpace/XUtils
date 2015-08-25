using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
namespace XUtils.Net.FTP
{
	public class FTP
	{
		private enum ProtectionLevel
		{
			Clear,
			Safe,
			Confidential,
			Private
		}
		private enum NetworkProtocol
		{
			IPv4 = 1,
			IPv6,
			ALL = 0
		}
		private enum System
		{
			UNIX,
			Windows
		}
		private enum _TYPE
		{
			ASCII,
			EBCDIC,
			IMAGE,
			none
		}
		private enum _MODE
		{
			STREAM,
			BLOCK,
			COMPRESSED,
			ZLIB
		}
		private enum STRUCTURE
		{
			FILE,
			RECORD,
			PAGE,
			TIFF
		}
		private TcpClient ftp;
		private TcpClient dtpPassive;
		private TcpListener dtpActive;
		private NetworkStream iostream;
		private NetworkStream DATA_iostream;
		private ConnectionMode mode;
		private ConnectionType conType;
		private Encoding encoding;
		private int port;
		private string host;
		private string username;
		private string password;
		private string account_information;
		private FTP._TYPE currentType = FTP._TYPE.none;
		private int speed;
		private int totalDownloadedBytes;
		private Dictionary<string, int> monthName;
		private FTP.System system;
		private FTP.NetworkProtocol passiveNetPrt = FTP.NetworkProtocol.IPv4;
		private FTP.NetworkProtocol activeNetPrt = FTP.NetworkProtocol.IPv4;
		private List<int> notSupportedProtocols = new List<int>();
		private Timer noopTimer;
		private Timer speedTimer;
		private bool _UTF8;
		private bool _MDTM;
		private bool _CLNT;
		private bool _SIZE;
		private bool _REST;
		private bool _TVFS;
		private bool _MLST;
		private string mlst_string;
		private bool _EPRT;
		private bool _EPSV;
		private bool _TLS;
		public event ServerResponseEventHendler OnServerResponse;
		public event ClientCommandEventHendler OnClientCommand;
		public event ReConnectEventHendler OnReConnect;
		public event SpeedEventHendler OnSpeed;
		public event FileTransferEventHendler OnFileTransfer;
		public ConnectionMode ConnectionMode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}
		public ConnectionType ConnectionType
		{
			get
			{
				return this.conType;
			}
			set
			{
				this.conType = value;
			}
		}
		public string AccountInformation
		{
			set
			{
				this.account_information = value;
			}
		}
		public int Port
		{
			set
			{
				this.port = value;
			}
		}
		public string Host
		{
			set
			{
				this.host = value;
			}
		}
		public string Username
		{
			set
			{
				this.username = value;
			}
		}
		public string Password
		{
			set
			{
				this.password = value;
			}
		}
		public NetworkStream Control
		{
			get
			{
				return this.iostream;
			}
		}
		public string SpeedFloating
		{
			get
			{
				string[] array = new string[]
				{
					"B",
					"KiB",
					"MiB",
					"GiB",
					"TiB",
					"PiB",
					"EiB",
					"ZiB",
					"YiB"
				};
				if (this.speed == 0)
				{
					return "";
				}
				int num;
				return new StringBuilder().AppendFormat("{0} {1}/sec", Math.Round((double)this.speed / Math.Pow(1024.0, (double)(num = (int)Math.Floor(Math.Log((double)this.speed, 1024.0)))), 2), array[num]).ToString();
			}
		}
		private string USER(string username, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("USER {0}\r\n", username);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("230"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("331"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.ServerResponseCode(data).Trim().Equals("336"))
						{
							this.OnServerResponse(this.GetString(data));
						}
						else
						{
							if (this.ServerResponseCode(data).Trim().Equals("530"))
							{
								throw new _530_not_logged_exception(this.GetString(data));
							}
							if (this.ServerResponseCode(data).Trim().Equals("500"))
							{
								throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
							}
							if (this.ServerResponseCode(data).Trim().Equals("501"))
							{
								throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
							}
							if (this.ServerResponseCode(data).Trim().Equals("421"))
							{
								throw new _421_service_not_available_exception(this.GetString(data));
							}
							if (this.ServerResponseCode(data).Trim().Equals("332"))
							{
								throw new _332_need_account_for_login_exception(this.GetString(data));
							}
						}
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string PASS(string password, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("PASS {0}\r\n", password);
				this.OnClientCommand("PASS (hidden)\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("230"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("503"))
					{
						throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("332"))
					{
						throw new _332_need_account_for_login_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string NOOP(ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("NOOP\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("NOOP\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string SITE(string command, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("SITE {0}\r\n", command);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("202"))
					{
						throw new _202_command_not_implemented_superfluous_at_this_site(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string QUIT(ref NetworkStream iostream, ref TcpClient workTcpCl)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("QUIT\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("QUIT\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("221"))
				{
					this.OnServerResponse(this.GetString(data));
					iostream.Close();
					workTcpCl.Close();
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string PWD(ref NetworkStream iostream)
		{
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("PWD\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("PWD\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("257"))
				{
					this.OnServerResponse(this.GetString(data));
					Regex regex = new Regex("[ a-zA-Z_0-9]*\"(?<dir>[ a-zA-Z_0-9/]*)\"[ a-zA-Z_0-9.]*");
					Match match = regex.Match(this.GetString(data));
					return match.Groups["dir"].Value;
				}
				if (this.ServerResponseCode(data).Trim().Equals("500"))
				{
					throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("501"))
				{
					throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("502"))
				{
					throw new _502_command_not_implemented_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("421"))
				{
					throw new _421_service_not_available_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("550"))
				{
					throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return "";
		}
		private string CWD(string directory, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("CWD {0}\r\n", directory);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("550"))
					{
						throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string MKD(string directory, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("MKD {0}\r\n", directory);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("257"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("550"))
					{
						throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string RMD(string directory, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("RMD {0}\r\n", directory);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("550"))
					{
						throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string CDUP(ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("CDUP\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("CDUP\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("550"))
					{
						throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string DELE(string pathname, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("DELE {0}\r\n", pathname);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("550"))
					{
						throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("450"))
					{
						throw new _450_file_unavailable_busy_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string TYPE(FTP._TYPE type, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				string text = "";
				switch (type)
				{
				case FTP._TYPE.ASCII:
					text = "TYPE A\r\n";
					break;
				case FTP._TYPE.EBCDIC:
					text = "TYPE E\r\n";
					break;
				case FTP._TYPE.IMAGE:
					text = "TYPE I\r\n";
					break;
				}
				this.OnClientCommand(text);
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("504"))
					{
						throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string MODE(FTP._MODE mode, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				string text = "";
				switch (mode)
				{
				case FTP._MODE.STREAM:
					text = "MODE S\r\n";
					break;
				case FTP._MODE.BLOCK:
					text = "MODE B\r\n";
					break;
				case FTP._MODE.COMPRESSED:
					text = "MODE C\r\n";
					break;
				case FTP._MODE.ZLIB:
					text = "MODE Z\r\n";
					break;
				}
				this.OnClientCommand(text);
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("504"))
					{
						throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string ABOR(ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("ABOR\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("ABOR\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("225"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("226"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("502"))
						{
							throw new _502_command_not_implemented_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string PORT(IPAddress ip, int port, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				string text = ip.ToString();
				text = text.Replace('.', ',');
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("PORT {0},{1},{2}\r\n", text, (port & 65280) >> 8, port & 255);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string SYST(ref NetworkStream iostream)
		{
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("SYST\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("SYST\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("215"))
				{
					this.OnServerResponse(this.GetString(data));
					Regex regex = new Regex("[ 0-9]*(?<type>[a-zA-Z_0-9]*)[ a-zA-Z_0-9]*");
					Match match = regex.Match(this.GetString(data));
					return match.Groups["type"].Value;
				}
				if (this.ServerResponseCode(data).Trim().Equals("500"))
				{
					throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("501"))
				{
					throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("502"))
				{
					throw new _502_command_not_implemented_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("421"))
				{
					throw new _421_service_not_available_exception(this.GetString(data));
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return "";
		}
		private string STRU(FTP.STRUCTURE structure, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				string text = "";
				switch (structure)
				{
				case FTP.STRUCTURE.FILE:
					text = "STRU F\r\n";
					break;
				case FTP.STRUCTURE.RECORD:
					text = "STRU R\r\n";
					break;
				case FTP.STRUCTURE.PAGE:
					text = "STRU P\r\n";
					break;
				case FTP.STRUCTURE.TIFF:
					text = "STRU T\r\n";
					break;
				}
				this.OnClientCommand(text);
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("504"))
					{
						throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string[] LIST(ref NetworkStream iostream, ref NetworkStream DATA_iostream)
		{
			return this.LIST(null, ref iostream, ref DATA_iostream);
		}
		private string[] LIST(string pathname, ref NetworkStream iostream, ref NetworkStream DATA_iostream)
		{
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (pathname == null)
				{
					stringBuilder.Append("LIST -aL\r\n");
				}
				else
				{
					stringBuilder.AppendFormat("LIST -aL {0}\r\n", pathname);
				}
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("125"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("150"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.mode == ConnectionMode.Passive)
						{
							this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
						}
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("502"))
						{
							throw new _502_command_not_implemented_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("530"))
						{
							throw new _530_not_logged_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("450"))
						{
							throw new _450_file_unavailable_busy_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("504"))
						{
							throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("553"))
						{
							throw new _553_file_name_not_allowed_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("425"))
						{
							throw new _425_can_not_open_data_connection_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("426"))
						{
							throw new _426_connection_vlosed_transfer_aborted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("451"))
						{
							throw new _451_local_error_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("550"))
						{
							throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("534"))
						{
							throw new _534_request_denied_for_policy_reasons(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("535"))
						{
							throw new _535_failed_security_check(this.GetString(data));
						}
					}
				}
				byte[] data2 = this.ReadServerDATA(ref DATA_iostream);
				if (this.mode == ConnectionMode.Passive)
				{
					this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
				}
				data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("226"))
				{
					this.OnServerResponse(this.GetString(data));
					string[] result = this.GetString(data2).Split(new string[]
					{
						"\r\n"
					}, StringSplitOptions.RemoveEmptyEntries);
					return result;
				}
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
					string[] result = this.GetString(data2).Split(new string[]
					{
						"\r\n"
					}, StringSplitOptions.RemoveEmptyEntries);
					return result;
				}
				if (this.ServerResponseCode(data).Trim().Equals("425"))
				{
					throw new _425_can_not_open_data_connection_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("426"))
				{
					throw new _426_connection_vlosed_transfer_aborted_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("451"))
				{
					throw new _451_local_error_exception(this.GetString(data));
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return new string[0];
		}
		private string[] NLIST(ref NetworkStream iostream, ref NetworkStream DATA_iostream)
		{
			return this.NLIST(null, ref iostream, ref DATA_iostream);
		}
		private string[] NLIST(string pathname, ref NetworkStream iostream, ref NetworkStream DATA_iostream)
		{
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (pathname == null)
				{
					stringBuilder.Append("NLIST\r\n");
				}
				else
				{
					stringBuilder.AppendFormat("NLIST {0}\r\n", pathname);
				}
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("125"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("150"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.mode == ConnectionMode.Passive)
						{
							this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
						}
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("502"))
						{
							throw new _502_command_not_implemented_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("530"))
						{
							throw new _530_not_logged_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("450"))
						{
							throw new _450_file_unavailable_busy_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("504"))
						{
							throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("553"))
						{
							throw new _553_file_name_not_allowed_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("425"))
						{
							throw new _425_can_not_open_data_connection_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("426"))
						{
							throw new _426_connection_vlosed_transfer_aborted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("451"))
						{
							throw new _451_local_error_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("550"))
						{
							throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("534"))
						{
							throw new _534_request_denied_for_policy_reasons(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("535"))
						{
							throw new _535_failed_security_check(this.GetString(data));
						}
					}
				}
				byte[] data2 = this.ReadServerDATA(ref DATA_iostream);
				if (this.mode == ConnectionMode.Passive)
				{
					this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
				}
				data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("226"))
				{
					this.OnServerResponse(this.GetString(data));
					string[] result = this.GetString(data2).Split(new string[]
					{
						"\r\n"
					}, StringSplitOptions.RemoveEmptyEntries);
					return result;
				}
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
					string[] result = this.GetString(data2).Split(new string[]
					{
						"\r\n"
					}, StringSplitOptions.RemoveEmptyEntries);
					return result;
				}
				if (this.ServerResponseCode(data).Trim().Equals("425"))
				{
					throw new _425_can_not_open_data_connection_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("426"))
				{
					throw new _426_connection_vlosed_transfer_aborted_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("451"))
				{
					throw new _451_local_error_exception(this.GetString(data));
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return new string[0];
		}
		private long SIZE(string pathname, ref NetworkStream iostream)
		{
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("SIZE {0}\r\n", pathname);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("213"))
				{
					this.OnServerResponse(this.GetString(data));
					return Convert.ToInt64(this.GetString(data).Substring(4).Trim());
				}
				if (this.ServerResponseCode(data).Trim().Equals("550"))
				{
					throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return 0L;
		}
		private string RNFR(string filename, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("RNFR {0}\r\n", filename);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("350"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("550"))
					{
						throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("450"))
					{
						throw new _450_file_unavailable_busy_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string RNTO(string filename, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("RNTO {0}\r\n", filename);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("503"))
					{
						throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("553"))
					{
						throw new _553_file_name_not_allowed_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("532"))
					{
						throw new _532_need_account_for_storing_files_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private PassiveConnectionInfo PASV(ref NetworkStream iostream)
		{
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("PASV\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("PASV\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("227"))
				{
					this.OnServerResponse(this.GetString(data));
					PassiveConnectionInfo result = default(PassiveConnectionInfo);
					Regex regex = new Regex("(?<ip1>[ a-zA-Z_0-9]*),(?<ip2>[ a-zA-Z_0-9]*),(?<ip3>[ a-zA-Z_0-9]*),(?<ip4>[ a-zA-Z_0-9]*),(?<p1>[ a-zA-Z_0-9]*),(?<p2>[ a-zA-Z_0-9]*)");
					Match match = regex.Match(this.GetString(data));
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("{0}.{1}.{2}.{3}", new object[]
					{
						match.Groups["ip1"].Value,
						match.Groups["ip2"].Value,
						match.Groups["ip3"].Value,
						match.Groups["ip4"].Value
					});
					result.ip = stringBuilder.ToString();
					result.port = Convert.ToInt32(match.Groups["p1"].Value) * 256 + Convert.ToInt32(match.Groups["p2"].Value);
					return result;
				}
				if (this.ServerResponseCode(data).Trim().Equals("500"))
				{
					throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("501"))
				{
					throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("502"))
				{
					throw new _502_command_not_implemented_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("421"))
				{
					throw new _421_service_not_available_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("530"))
				{
					throw new _530_not_logged_exception(this.GetString(data));
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return default(PassiveConnectionInfo);
		}
		private string RETR(string filename, ref NetworkStream iostream, ref NetworkStream DATA_iostream, ref Stream stream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("RETR {0}\r\n", filename);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("120"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("150"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.mode == ConnectionMode.Passive)
						{
							this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
						}
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("530"))
						{
							throw new _530_not_logged_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("450"))
						{
							throw new _450_file_unavailable_busy_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("550"))
						{
							throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("534"))
						{
							throw new _534_request_denied_for_policy_reasons(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("535"))
						{
							throw new _535_failed_security_check(this.GetString(data));
						}
					}
				}
				this.ReadWriteServerDATA(ref DATA_iostream, ref stream);
				if (this.mode == ConnectionMode.Passive)
				{
					this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
				}
				data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("226"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("250"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.ServerResponseCode(data).Trim().Equals("425"))
						{
							throw new _425_can_not_open_data_connection_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("426"))
						{
							throw new _426_connection_vlosed_transfer_aborted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("451"))
						{
							throw new _451_local_error_exception(this.GetString(data));
						}
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string STOR(string filename, ref NetworkStream iostream, ref NetworkStream DATA_iostream, ref Stream stream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("STOR {0}\r\n", filename);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("125"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("150"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.mode == ConnectionMode.Passive)
						{
							this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
						}
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("530"))
						{
							throw new _530_not_logged_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("450"))
						{
							throw new _450_file_unavailable_busy_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("553"))
						{
							throw new _553_file_name_not_allowed_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("532"))
						{
							throw new _532_need_account_for_storing_files_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("452"))
						{
							throw new _452_insufficient_storage_space_in_system_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("550"))
						{
							throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("534"))
						{
							throw new _534_request_denied_for_policy_reasons(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("535"))
						{
							throw new _535_failed_security_check(this.GetString(data));
						}
					}
				}
				this.ReadWriteServerDATA(ref stream, ref DATA_iostream);
				if (this.mode == ConnectionMode.Passive)
				{
					this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
				}
				data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("226"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("250"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.ServerResponseCode(data).Trim().Equals("425"))
						{
							throw new _425_can_not_open_data_connection_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("426"))
						{
							throw new _426_connection_vlosed_transfer_aborted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("451"))
						{
							throw new _451_local_error_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("551"))
						{
							throw new _551_page_type_unknown_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("552"))
						{
							throw new _552_exceeded_storage_allocation_exception(this.GetString(data));
						}
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string FEAT(ref NetworkStream iostream)
		{
			string @string;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("FEAT\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("FEAT\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("211"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
				}
				@string = this.GetString(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return @string;
		}
		private byte[] OPTS(string command, ref NetworkStream iostream)
		{
			byte[] result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("OPTS {0}\r\n", command);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] array = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(array).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(array));
				}
				else
				{
					if (this.ServerResponseCode(array).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(array));
					}
					if (this.ServerResponseCode(array).Trim().Equals("451"))
					{
						throw new _451_local_error_exception(this.GetString(array));
					}
				}
				result = array;
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string CLNT(string name, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("CLNT {0}\r\n", name);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("451"))
					{
						throw new _451_local_error_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string REST(long marker, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("REST {0}\r\n", marker);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("350"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string MLST(ref NetworkStream iostream)
		{
			return this.MLST(null, ref iostream);
		}
		private string MLST(string pathname, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (pathname == null)
				{
					stringBuilder.AppendFormat("MLST\r\n", new object[0]);
				}
				else
				{
					stringBuilder.AppendFormat("MLST {0}\r\n", pathname);
				}
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("504"))
					{
						throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("553"))
					{
						throw new _553_file_name_not_allowed_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("550"))
					{
						throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string[] MLSD(ref NetworkStream iostream, ref NetworkStream DATA_iostream)
		{
			return this.MLSD(null, ref iostream, ref DATA_iostream);
		}
		private string[] MLSD(string pathname, ref NetworkStream iostream, ref NetworkStream DATA_iostream)
		{
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (pathname == null)
				{
					stringBuilder.Append("MLSD\r\n");
				}
				else
				{
					stringBuilder.AppendFormat("MLSD {0}\r\n", pathname);
				}
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("125"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("150"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.mode == ConnectionMode.Passive)
						{
							this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
						}
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("502"))
						{
							throw new _502_command_not_implemented_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("530"))
						{
							throw new _530_not_logged_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("450"))
						{
							throw new _450_file_unavailable_busy_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("504"))
						{
							throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("553"))
						{
							throw new _553_file_name_not_allowed_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("425"))
						{
							throw new _425_can_not_open_data_connection_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("426"))
						{
							throw new _426_connection_vlosed_transfer_aborted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("451"))
						{
							throw new _451_local_error_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("550"))
						{
							throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
						}
					}
				}
				byte[] data2 = this.ReadServerDATA(ref DATA_iostream);
				if (this.mode == ConnectionMode.Passive)
				{
					this.DicsonnectFrom(ref this.dtpPassive, ref DATA_iostream);
				}
				data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("226"))
				{
					this.OnServerResponse(this.GetString(data));
					string[] result = this.GetString(data2).Split(new string[]
					{
						"\r\n"
					}, StringSplitOptions.RemoveEmptyEntries);
					return result;
				}
				if (this.ServerResponseCode(data).Trim().Equals("250"))
				{
					this.OnServerResponse(this.GetString(data));
					string[] result = this.GetString(data2).Split(new string[]
					{
						"\r\n"
					}, StringSplitOptions.RemoveEmptyEntries);
					return result;
				}
				if (this.ServerResponseCode(data).Trim().Equals("425"))
				{
					throw new _425_can_not_open_data_connection_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("426"))
				{
					throw new _426_connection_vlosed_transfer_aborted_exception(this.GetString(data));
				}
				if (this.ServerResponseCode(data).Trim().Equals("451"))
				{
					throw new _451_local_error_exception(this.GetString(data));
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return new string[0];
		}
		private DateTime MDTM(string filename, ref NetworkStream iostream)
		{
			DateTime result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("MDTM {0}\r\n", filename);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("213"))
				{
					this.OnServerResponse(this.GetString(data));
					Regex regex = new Regex("([0-9]{3}) (?<time>[0-9/.]*)");
					Match match = regex.Match(this.GetString(data));
					stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("{0}", match.Groups["time"].Value);
					int year = Convert.ToInt32(stringBuilder.ToString().Substring(0, 4));
					int month = Convert.ToInt32(stringBuilder.ToString().Substring(4, 2));
					int day = Convert.ToInt32(stringBuilder.ToString().Substring(6, 2));
					Convert.ToDouble(stringBuilder.ToString().Substring(8));
					DateTime dateTime = new DateTime(year, month, day);
					result = dateTime;
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("550"))
					{
						throw new _550_file_unavailable_not_found_no_access_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("450"))
					{
						throw new _450_file_unavailable_busy_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("451"))
					{
						throw new _451_local_error_exception(this.GetString(data));
					}
					result = default(DateTime);
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string EPRT(string NetPrtAddrPort, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("EPRT {0}\r\n", NetPrtAddrPort);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("522"))
					{
						throw new _522_protocol_not_supported(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private PassiveConnectionInfo EPSV(FTP.NetworkProtocol netPrt, ref NetworkStream iostream)
		{
			PassiveConnectionInfo result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				byte[] bytes;
				switch (netPrt)
				{
				case FTP.NetworkProtocol.ALL:
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("EPSV {0}\r\n", "ALL");
					this.OnClientCommand(stringBuilder.ToString());
					bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
					break;
				}
				case FTP.NetworkProtocol.IPv4:
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("EPSV {0}\r\n", 1);
					this.OnClientCommand(stringBuilder.ToString());
					bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
					break;
				}
				case FTP.NetworkProtocol.IPv6:
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("EPSV {0}\r\n", 2);
					this.OnClientCommand(stringBuilder.ToString());
					bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
					break;
				}
				default:
					this.OnClientCommand("EPSV\r\n");
					bytes = Encoding.ASCII.GetBytes("EPSV\r\n");
					break;
				}
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("229"))
				{
					this.OnServerResponse(this.GetString(data));
					PassiveConnectionInfo passiveConnectionInfo = default(PassiveConnectionInfo);
					Regex regex = new Regex("\\|(?<prt>.*)\\|(?<addr>.*)\\|(?<port>.*)\\|");
					Match match = regex.Match(this.GetString(data));
					passiveConnectionInfo.ip = this.host;
					passiveConnectionInfo.port = Convert.ToInt32(match.Groups["port"].Value);
					result = passiveConnectionInfo;
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("522"))
					{
						throw new _522_protocol_not_supported(this.GetString(data));
					}
					result = default(PassiveConnectionInfo);
				}
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string AUTH(string mechanism_name, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (mechanism_name == null)
				{
					stringBuilder.Append("AUTH\r\n");
				}
				else
				{
					stringBuilder.AppendFormat("AUTH {0}\r\n", mechanism_name);
				}
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("234"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("334"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("502"))
						{
							throw new _502_command_not_implemented_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("504"))
						{
							throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("534"))
						{
							throw new _534_request_denied_for_policy_reasons(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("431"))
						{
							throw new _431_need_some_unavailable_resource_to_process_security(this.GetString(data));
						}
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string ADAT(string base64data, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("ADAT {0}\r\n", Convert.ToBase64String(Encoding.UTF8.GetBytes(base64data)));
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("235"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("335"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("503"))
						{
							throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("502"))
						{
							throw new _502_command_not_implemented_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("504"))
						{
							throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("534"))
						{
							throw new _534_request_denied_for_policy_reasons(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("431"))
						{
							throw new _431_need_some_unavailable_resource_to_process_security(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("535"))
						{
							throw new _535_failed_security_check(this.GetString(data));
						}
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string PBSZ(int buffer_size, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("PBSZ {0}\r\n", buffer_size);
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("503"))
					{
						throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("530"))
					{
						throw new _530_not_logged_exception(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string PROT(FTP.ProtectionLevel prot_code, ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				switch (prot_code)
				{
				case FTP.ProtectionLevel.Clear:
					stringBuilder.AppendFormat("PROT C\r\n", new object[0]);
					this.OnClientCommand(stringBuilder.ToString());
					break;
				case FTP.ProtectionLevel.Safe:
					stringBuilder.AppendFormat("PROT S\r\n", new object[0]);
					this.OnClientCommand(stringBuilder.ToString());
					break;
				case FTP.ProtectionLevel.Confidential:
					stringBuilder.AppendFormat("PROT E\r\n", new object[0]);
					this.OnClientCommand(stringBuilder.ToString());
					break;
				case FTP.ProtectionLevel.Private:
					stringBuilder.AppendFormat("PROT P\r\n", new object[0]);
					this.OnClientCommand(stringBuilder.ToString());
					break;
				}
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("150"))
					{
						this.OnServerResponse(this.GetString(data));
					}
					else
					{
						if (this.ServerResponseCode(data).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("503"))
						{
							throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("502"))
						{
							throw new _502_command_not_implemented_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("530"))
						{
							throw new _530_not_logged_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("504"))
						{
							throw new _504_command_not_implemented_for_that_parameter_exception(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("536"))
						{
							throw new _536_requested_PROT_level_not_supported_by_mechanism(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("534"))
						{
							throw new _534_request_denied_for_policy_reasons(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("431"))
						{
							throw new _431_need_some_unavailable_resource_to_process_security(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("521"))
						{
							throw new _521_data_connection_cannot_be_opened_with_this_PROT_setting(this.GetString(data));
						}
						if (this.ServerResponseCode(data).Trim().Equals("522"))
						{
							throw new _522_TLS_negotiation_failed_or_was_unacceptable(this.GetString(data));
						}
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string CCC(ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				this.OnClientCommand("CCC\r\n");
				byte[] bytes = Encoding.ASCII.GetBytes("CCC\r\n");
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("200"))
				{
					this.OnServerResponse(this.GetString(data));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("503"))
					{
						throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("533"))
					{
						throw new _533_command_protection_level_denied_for_policy_reasons(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("534"))
					{
						throw new _534_request_denied_for_policy_reasons(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string MIC(ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("MIC {0}\r\n", Convert.ToBase64String(Encoding.UTF8.GetBytes("safe")));
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("631"))
				{
					this.OnServerResponse(this.GetString(Convert.FromBase64String(this.GetString(data))));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("503"))
					{
						throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("535"))
					{
						throw new _535_failed_security_check(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("533"))
					{
						throw new _533_command_protection_level_denied_for_policy_reasons(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("537"))
					{
						throw new _537_command_protection_level_not_supported_by_security_mechanism(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string CONF(ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("CONF {0}\r\n", Convert.ToBase64String(Encoding.UTF8.GetBytes("confidential")));
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("633"))
				{
					this.OnServerResponse(this.GetString(Convert.FromBase64String(this.GetString(data))));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("503"))
					{
						throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("535"))
					{
						throw new _535_failed_security_check(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("533"))
					{
						throw new _533_command_protection_level_denied_for_policy_reasons(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("537"))
					{
						throw new _537_command_protection_level_not_supported_by_security_mechanism(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		private string ENC(ref NetworkStream iostream)
		{
			string result;
			try
			{
				if (!iostream.CanWrite)
				{
					throw new Exception("Client closed the connection.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("ENC {0}\r\n", Convert.ToBase64String(Encoding.UTF8.GetBytes("confidential")));
				this.OnClientCommand(stringBuilder.ToString());
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				iostream.Write(bytes, 0, bytes.Length);
				iostream.Flush();
				byte[] data = this.ReadServerResponseMultiline(ref iostream);
				if (this.ServerResponseCode(data).Trim().Equals("632"))
				{
					this.OnServerResponse(this.GetString(Convert.FromBase64String(this.GetString(data))));
				}
				else
				{
					if (this.ServerResponseCode(data).Trim().Equals("501"))
					{
						throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("502"))
					{
						throw new _502_command_not_implemented_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("503"))
					{
						throw new _503_bad_sequence_of_commands_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("500"))
					{
						throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("421"))
					{
						throw new _421_service_not_available_exception(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("535"))
					{
						throw new _535_failed_security_check(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("533"))
					{
						throw new _533_command_protection_level_denied_for_policy_reasons(this.GetString(data));
					}
					if (this.ServerResponseCode(data).Trim().Equals("537"))
					{
						throw new _537_command_protection_level_not_supported_by_security_mechanism(this.GetString(data));
					}
				}
				result = this.ServerResponseCode(data);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				this.DicsonnectFrom(ref this.ftp, ref iostream);
				throw new IOException(ex.Message);
			}
			return result;
		}
		public FTP()
		{
			this.ftp = new TcpClient();
			this.dtpPassive = new TcpClient();
			this.OnServerResponse += new ServerResponseEventHendler(this.FTP_OnServerResponse);
			this.OnClientCommand += new ClientCommandEventHendler(this.FTP_OnClientCommand);
			this.OnReConnect += new ReConnectEventHendler(this.FTP_OnReConnect);
			this.OnSpeed += new SpeedEventHendler(this.FTP_OnSpeed);
			this.OnFileTransfer += new FileTransferEventHendler(this.FTP_OnFileTransfer);
			this.encoding = Encoding.ASCII;
			this.mode = ConnectionMode.Passive;
			this.conType = ConnectionType.DefaultConnection;
			this.monthName = new Dictionary<string, int>();
			this.monthName.Add("JAN", 1);
			this.monthName.Add("FEB", 2);
			this.monthName.Add("MAR", 3);
			this.monthName.Add("APR", 4);
			this.monthName.Add("MAY", 5);
			this.monthName.Add("JUN", 6);
			this.monthName.Add("JUL", 7);
			this.monthName.Add("AUG", 8);
			this.monthName.Add("SEP", 9);
			this.monthName.Add("OCT", 10);
			this.monthName.Add("NOV", 11);
			this.monthName.Add("DEC", 12);
		}
		private void FTP_OnFileTransfer(int totalTransferedBytes)
		{
		}
		private void FTP_OnSpeed(FTP ftp)
		{
		}
		private void FTP_OnReConnect(FTP ftp)
		{
		}
		private void FTP_OnClientCommand(string command)
		{
		}
		private void FTP_OnServerResponse(string response)
		{
		}
		public double Speed(SpeedPerSecond speedpersec)
		{
			return this.SpeedConvert(speedpersec, this.speed);
		}
		public void Open(string hostname, int port)
		{
			this.noopTimer = new Timer(new TimerCallback(this.DoNOOP));
			this.noopTimer.Change(60000, 30000);
			this.speedTimer = new Timer(new TimerCallback(this.DoTimer));
			this.speedTimer.Change(0, 1000);
			if (string.IsNullOrEmpty(hostname))
			{
				throw new ArgumentNullException("hostname", "Cannot be empty or null");
			}
			this.host = hostname;
			this.port = port;
			TcpClient tcpClient = new TcpClient();
			tcpClient.Connect(hostname, port);
			AddressFamily addressFamily = tcpClient.Client.AddressFamily;
			if (addressFamily != AddressFamily.InterNetwork)
			{
				if (addressFamily == AddressFamily.InterNetworkV6)
				{
					this.passiveNetPrt = FTP.NetworkProtocol.IPv6;
				}
			}
			else
			{
				this.passiveNetPrt = FTP.NetworkProtocol.IPv4;
			}
			tcpClient.Close();
			AddressFamily addressFamily2 = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].AddressFamily;
			if (addressFamily2 != AddressFamily.InterNetwork)
			{
				if (addressFamily2 == AddressFamily.InterNetworkV6)
				{
					this.activeNetPrt = FTP.NetworkProtocol.IPv6;
				}
			}
			else
			{
				this.activeNetPrt = FTP.NetworkProtocol.IPv4;
			}
			this.ConnectTo(hostname, port, ref this.ftp, ref this.iostream);
			byte[] array = this.ReadServerResponseMultiline(ref this.iostream);
			if (this.ServerResponseCode(array).Trim().Equals("220"))
			{
				this.OnServerResponse(Encoding.ASCII.GetString(array));
				return;
			}
			throw new Exception(Encoding.ASCII.GetString(array));
		}
		public bool Login(string username, string password)
		{
			this.username = username;
			this.password = password;
			ConnectionType connectionType = this.conType;
			if (connectionType == ConnectionType.SecureConnection)
			{
				try
				{
					if (this.AUTH("TLS", ref this.iostream).Equals("234"))
					{
						this._TLS = true;
					}
				}
				catch (ServerResponseException ex)
				{
					this.OnServerResponse(ex.Message);
				}
			}
			try
			{
				this.USER(username, ref this.iostream);
				this.PASS(password, ref this.iostream);
			}
			catch (_530_not_logged_exception 530_not_logged_exception)
			{
				this.OnServerResponse(530_not_logged_exception.Message);
				this.OnReConnect(this);
				bool result = false;
				return result;
			}
			catch (ServerResponseException ex2)
			{
				this.OnServerResponse(ex2.Message);
				bool result = false;
				return result;
			}
			try
			{
				string text = this.SYST(ref this.iostream);
				if (text.IndexOf("Windows") > -1)
				{
					this.system = FTP.System.Windows;
				}
				if (text.IndexOf("UNIX") > -1)
				{
					this.system = FTP.System.UNIX;
				}
			}
			catch (ServerResponseException ex3)
			{
				this.OnServerResponse(ex3.Message);
			}
			this.SetType(FTP._TYPE.ASCII, ref this.iostream);
			try
			{
				this.CheckFEAT(this.FEAT(ref this.iostream));
			}
			catch (ServerResponseException ex4)
			{
				this.OnServerResponse(ex4.Message);
			}
			if (this._CLNT)
			{
				try
				{
					this.CLNT("FTPLibrary", ref this.iostream);
				}
				catch (ServerResponseException ex5)
				{
					this.OnServerResponse(ex5.Message);
				}
			}
			if (this._UTF8)
			{
				try
				{
					if (this.ServerResponseCode(this.OPTS("UTF8 ON", ref this.iostream)).Trim().Equals("200"))
					{
						this.encoding = Encoding.UTF8;
					}
				}
				catch (ServerResponseException ex6)
				{
					this.OnServerResponse(ex6.Message);
				}
			}
			if (this._MLST)
			{
				try
				{
					this.mlst_string = this.GetString(this.OPTS(this.mlst_string, ref this.iostream));
				}
				catch (ServerResponseException ex7)
				{
					this.OnServerResponse(ex7.Message);
				}
			}
			return true;
		}
		public void LogOut()
		{
			try
			{
				this.QUIT(ref this.iostream, ref this.ftp);
				this.currentType = FTP._TYPE.none;
				this.noopTimer.Dispose();
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
			}
		}
		public void ReConnect()
		{
			this.DicsonnectFrom(ref this.ftp, ref this.iostream);
			this.currentType = FTP._TYPE.none;
			this.Open(this.host, this.port);
			this.Login(this.username, this.password);
		}
		public string GetCurrentWorkDirectory()
		{
			string result;
			try
			{
				result = this.PWD(ref this.iostream);
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				result = "";
			}
			return result;
		}
		public bool ChangeCurrentWorkDirectory(string directory)
		{
			bool result;
			try
			{
				if (this.CWD(directory, ref this.iostream).Equals("250"))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				result = false;
			}
			return result;
		}
		public bool GotoParentOfCurrentDirectory()
		{
			bool result;
			try
			{
				if (this.CDUP(ref this.iostream).Equals("250"))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				result = false;
			}
			return result;
		}
		public bool CreateDirectory(string directory)
		{
			bool result;
			try
			{
				if (this.MKD(directory, ref this.iostream).Equals("257"))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				result = false;
			}
			return result;
		}
		public bool RemoveDirectory(string directory)
		{
			bool result;
			try
			{
				if (this.RMD(directory, ref this.iostream).Equals("250"))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				result = false;
			}
			return result;
		}
		public void GetDirectoryContent(string path, ref List<ContentItemInformation> itemCollection)
		{
			try
			{
				itemCollection.Clear();
				this.SetType(FTP._TYPE.ASCII, ref this.iostream);
				if (this.mode == ConnectionMode.Passive)
				{
					PassiveConnectionInfo passiveConnectionInfo = default(PassiveConnectionInfo);
					if (this._EPSV)
					{
						try
						{
							passiveConnectionInfo = this.EPSV(this.passiveNetPrt, ref this.iostream);
						}
						catch (ServerResponseException ex)
						{
							this.OnServerResponse(ex.Message);
							this._EPSV = false;
						}
					}
					if (!this._EPSV)
					{
						passiveConnectionInfo = this.PASV(ref this.iostream);
					}
					this.ConnectTo(this.host, passiveConnectionInfo.port, ref this.dtpPassive, ref this.DATA_iostream);
				}
				else
				{
					if (this.mode == ConnectionMode.Active)
					{
						if (this._EPRT)
						{
							this.dtpActive = new TcpListener(((IPEndPoint)this.ftp.Client.LocalEndPoint).Address, ((IPEndPoint)this.ftp.Client.LocalEndPoint).Port + 1);
							this.dtpActive.Start();
							this.PORT(((IPEndPoint)this.ftp.Client.LocalEndPoint).Address, ((IPEndPoint)this.ftp.Client.LocalEndPoint).Port + 1, ref this.iostream);
							Console.WriteLine("Waiting for a connection on {0}:{1} ... ", ((IPEndPoint)this.ftp.Client.LocalEndPoint).Address, ((IPEndPoint)this.ftp.Client.LocalEndPoint).Port + 1);
							TcpClient tcpClient = this.dtpActive.AcceptTcpClient();
							Console.WriteLine("Connected!");
							this.DATA_iostream = tcpClient.GetStream();
						}
						if (!this._EPRT)
						{
							this.PORT(((IPEndPoint)this.ftp.Client.LocalEndPoint).Address, ((IPEndPoint)this.ftp.Client.LocalEndPoint).Port + 1, ref this.iostream);
						}
					}
				}
				if (this._MLST)
				{
					string[] array = this.MLSD(path, ref this.iostream, ref this.DATA_iostream);
					string text = "";
					if (this.mlst_string.IndexOf("size") > -1)
					{
						text += "(size=(?<size>[0-9]*);)|";
					}
					if (this.mlst_string.IndexOf("modify") > -1)
					{
						text += "(modify=(?<modify>[0-9/.]*);)|";
					}
					if (this.mlst_string.IndexOf("create") > -1)
					{
						text += "((created|create)=(?<create>[0-9/.]*);)|";
					}
					if (this.mlst_string.IndexOf("type") > -1)
					{
						text += "(type=(?<type>[a-z_A-Z]*);)|";
					}
					if (this.mlst_string.IndexOf("unique") > -1)
					{
						text += "(unique=(?<unique>.*);)|";
					}
					if (this.mlst_string.IndexOf("perm") > -1)
					{
						text += "(perm=(?<perm>[a-z_A-Z]*);)|";
					}
					if (this.mlst_string.IndexOf("lang") > -1)
					{
						text += "(lang=(?<lang>.*);)|";
					}
					if (this.mlst_string.IndexOf("media-type") > -1)
					{
						text += "(media-type=(?<media_type>.*);)|";
					}
					if (this.mlst_string.IndexOf("charset") > -1)
					{
						text += "(charset=(?<charset>.*);)|";
					}
					text += "\\s(?<name>.*)$";
					Regex regex = new Regex(text, RegexOptions.IgnoreCase | RegexOptions.Compiled);
					for (int i = 0; i < array.Length; i++)
					{
						MatchCollection matchCollection = regex.Matches(array[i]);
						ContentItemInformation contentItemInformation = new ContentItemInformation();
						for (int j = 0; j < matchCollection.Count; j++)
						{
							if (matchCollection[j].Groups[0].Value.IndexOf("type") > -1)
							{
								if (!(matchCollection[j].Groups["type"].Value.Equals("file", StringComparison.OrdinalIgnoreCase) | matchCollection[j].Groups["type"].Value.Equals("dir", StringComparison.OrdinalIgnoreCase)))
								{
									break;
								}
								string value;
								if ((value = matchCollection[j].Groups["type"].Value) != null)
								{
									if (value == "dir")
									{
										contentItemInformation.ItemType = ContentItemType.Directory;
										goto IL_454;
									}
									if (value == "file")
									{
										contentItemInformation.ItemType = ContentItemType.File;
										goto IL_454;
									}
								}
								contentItemInformation.ItemType = ContentItemType.Unknown;
							}
							IL_454:
							if (!string.IsNullOrEmpty(matchCollection[j].Groups["size"].Value))
							{
								contentItemInformation.Size = Convert.ToInt64(matchCollection[j].Groups["size"].Value);
							}
							if (!string.IsNullOrEmpty(matchCollection[j].Groups["create"].Value))
							{
								if (matchCollection[j].Groups["create"].Value.IndexOf(".") > -1)
								{
									contentItemInformation.Created = new DateTime(int.Parse(matchCollection[j].Groups["create"].Value.Substring(0, 4)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(4, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(6, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(8, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(10, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(12, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(15)));
								}
								else
								{
									contentItemInformation.Created = new DateTime(int.Parse(matchCollection[j].Groups["create"].Value.Substring(0, 4)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(4, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(6, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(8, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(10, 2)), int.Parse(matchCollection[j].Groups["create"].Value.Substring(12, 2)));
								}
							}
							if (!string.IsNullOrEmpty(matchCollection[j].Groups["modify"].Value))
							{
								if (matchCollection[j].Groups["modify"].Value.IndexOf(".") > -1)
								{
									contentItemInformation.LastChange = new DateTime(int.Parse(matchCollection[j].Groups["modify"].Value.Substring(0, 4)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(4, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(6, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(8, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(10, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(12, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(15)));
								}
								else
								{
									contentItemInformation.LastChange = new DateTime(int.Parse(matchCollection[j].Groups["modify"].Value.Substring(0, 4)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(4, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(6, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(8, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(10, 2)), int.Parse(matchCollection[j].Groups["modify"].Value.Substring(12, 2)));
								}
							}
							if (!string.IsNullOrEmpty(matchCollection[j].Groups["name"].Value))
							{
								contentItemInformation.Name = matchCollection[j].Groups["name"].Value;
							}
							if (j == matchCollection.Count - 1)
							{
								itemCollection.Add(contentItemInformation);
							}
						}
					}
				}
				else
				{
					if (this.system == FTP.System.UNIX)
					{
						string[] array2 = this.LIST(path, ref this.iostream, ref this.DATA_iostream);
						Regex regex2 = new Regex("^(?<fileType>\\w|-)(?<fileRights>[r-][w-][x-][r-][w-][x-][r-][w-][x-])\\s+(?<linksCount>\\d+)\\s+(?<owner>\\S+)\\s+(?<group>\\S+)\\s+(?<fileSize>\\d+)\\s+(?<month>\\w+)\\s+(?<day>\\d+)\\s+(?<yearDayTime>\\d\\d\\d\\d|\\d\\d:\\d\\d)\\s+(?<fileName>.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
						for (int k = 0; k < array2.Length; k++)
						{
							Match match = regex2.Match(array2[k]);
							if (match.Success && match.Groups["fileName"].Value != "." && match.Groups["fileName"].Value != "..")
							{
								ContentItemInformation contentItemInformation2 = new ContentItemInformation();
								string value2;
								if ((value2 = match.Groups["fileType"].Value) == null)
								{
									goto IL_B42;
								}
								if (!(value2 == "-"))
								{
									if (!(value2 == "d"))
									{
										if (!(value2 == "l"))
										{
											goto IL_B42;
										}
										contentItemInformation2.ItemType = ContentItemType.SoftLink;
									}
									else
									{
										contentItemInformation2.ItemType = ContentItemType.Directory;
									}
								}
								else
								{
									contentItemInformation2.ItemType = ContentItemType.File;
								}
								IL_B4A:
								int num = 0;
								int num2 = 0;
								int num3 = 0;
								string value3 = match.Groups["fileRights"].Value;
								for (int l = 0; l < 3; l++)
								{
									if (value3.Substring(0, 3).Substring(l, 1) != "-")
									{
										switch (l)
										{
										case 0:
											num += 4;
											break;
										case 1:
											num += 2;
											break;
										case 2:
											num++;
											break;
										}
									}
								}
								for (int m = 0; m < 3; m++)
								{
									if (value3.Substring(3, 3).Substring(m, 1) != "-")
									{
										switch (m)
										{
										case 0:
											num2 += 4;
											break;
										case 1:
											num2 += 2;
											break;
										case 2:
											num2++;
											break;
										}
									}
								}
								for (int n = 0; n < 3; n++)
								{
									if (value3.Substring(6, 3).Substring(n, 1) != "-")
									{
										switch (n)
										{
										case 0:
											num3 += 4;
											break;
										case 1:
											num3 += 2;
											break;
										case 2:
											num3++;
											break;
										}
									}
								}
								contentItemInformation2.Rights = int.Parse(new StringBuilder().AppendFormat("{0}{1}{2}", num, num2, num3).ToString());
								contentItemInformation2.LinksCount = int.Parse(match.Groups["linksCount"].Value);
								contentItemInformation2.Owner = match.Groups["owner"].Value;
								contentItemInformation2.Group = match.Groups["group"].Value;
								contentItemInformation2.Size = long.Parse(match.Groups["fileSize"].Value);
								DateTime now = DateTime.Now;
								int day = int.Parse(match.Groups["day"].Value);
								int month;
								this.monthName.TryGetValue(match.Groups["month"].Value.ToUpper(), out month);
								string value4 = match.Groups["yearDayTime"].Value;
								int num4 = value4.IndexOf(':');
								int year;
								int hour;
								int minute;
								if (num4 >= 0)
								{
									year = now.Year;
									hour = int.Parse(value4.Substring(0, num4));
									minute = int.Parse(value4.Substring(num4 + 1));
								}
								else
								{
									year = int.Parse(value4);
									hour = 0;
									minute = 0;
								}
								contentItemInformation2.LastChange = new DateTime(year, month, day, hour, minute, 0);
								contentItemInformation2.Name = match.Groups["fileName"].Value;
								itemCollection.Add(contentItemInformation2);
								goto IL_E21;
								IL_B42:
								contentItemInformation2.ItemType = ContentItemType.Unknown;
								goto IL_B4A;
							}
							IL_E21:;
						}
					}
					else
					{
						if (this.system == FTP.System.Windows)
						{
							string[] array3 = this.LIST(path, ref this.iostream, ref this.DATA_iostream);
							for (int num5 = 0; num5 < array3.Length; num5++)
							{
								Console.WriteLine(array3[num5]);
							}
						}
					}
				}
			}
			catch (ServerResponseException ex2)
			{
				this.OnServerResponse(ex2.Message);
			}
		}
		public void SetPermition(string name, UserPermitionOptions userPermition, GroupPermitionOptions grouPermition, WorldPermitionOptions worldPermition)
		{
			try
			{
				this.SITE(new StringBuilder().AppendFormat("CHMOD {0}{1}{2} {3}", new object[]
				{
					(int)userPermition,
					(int)grouPermition,
					(int)worldPermition,
					name
				}).ToString(), ref this.iostream);
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
			}
		}
		public long GetFileSize(string filename)
		{
			long result;
			try
			{
				if (this._SIZE)
				{
					this.SetType(FTP._TYPE.IMAGE, ref this.iostream);
					result = this.SIZE(filename, ref this.iostream);
				}
				else
				{
					result = -1L;
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				result = -1L;
			}
			return result;
		}
		public bool RemoveFile(string filename)
		{
			bool result;
			try
			{
				if (this.DELE(filename, ref this.iostream).Equals("250"))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				result = false;
			}
			return result;
		}
		public void Download(string filename, ref FileStream stream)
		{
			try
			{
				this.SetType(FTP._TYPE.IMAGE, ref this.iostream);
				if (this.mode == ConnectionMode.Passive)
				{
					PassiveConnectionInfo passiveConnectionInfo = default(PassiveConnectionInfo);
					if (this._EPSV)
					{
						try
						{
							passiveConnectionInfo = this.EPSV(this.passiveNetPrt, ref this.iostream);
						}
						catch (ServerResponseException ex)
						{
							this.OnServerResponse(ex.Message);
							this._EPSV = false;
						}
					}
					if (!this._EPSV)
					{
						passiveConnectionInfo = this.PASV(ref this.iostream);
					}
					this.ConnectTo(passiveConnectionInfo.ip, passiveConnectionInfo.port, ref this.dtpPassive, ref this.DATA_iostream);
					Stream stream2 = stream;
					this.RETR(filename, ref this.iostream, ref this.DATA_iostream, ref stream2);
				}
				else
				{
					ConnectionMode arg_AF_0 = this.mode;
				}
			}
			catch (ServerResponseException ex2)
			{
				this.OnServerResponse(ex2.Message);
			}
		}
		public void Download_RS_to_LS(string filename, NetworkStream rcstream)
		{
			try
			{
				this.TYPE(FTP._TYPE.IMAGE, ref rcstream);
				PassiveConnectionInfo passiveConnectionInfo = this.PASV(ref rcstream);
				TcpClient tcpClient = new TcpClient();
				NetworkStream networkStream = null;
				this.ConnectTo(passiveConnectionInfo.ip, passiveConnectionInfo.port, ref tcpClient, ref networkStream);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("RETR {0}\r\n", filename);
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				rcstream.Write(bytes, 0, bytes.Length);
				rcstream.Flush();
				byte[] array = this.ReadServerResponseMultiline(ref rcstream);
				if (this.ServerResponseCode(array).Trim().Equals("120"))
				{
					this.OnServerResponse(Encoding.ASCII.GetString(array));
				}
				else
				{
					if (this.ServerResponseCode(array).Trim().Equals("150"))
					{
						this.OnServerResponse(Encoding.ASCII.GetString(array));
					}
					else
					{
						if (this.ServerResponseCode(array).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("530"))
						{
							throw new _530_not_logged_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("450"))
						{
							throw new _450_file_unavailable_busy_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("550"))
						{
							throw new _550_file_unavailable_not_found_no_access_exception(Encoding.ASCII.GetString(array));
						}
					}
				}
				this.SetType(FTP._TYPE.IMAGE, ref this.iostream);
				passiveConnectionInfo = this.PASV(ref this.iostream);
				this.ConnectTo(passiveConnectionInfo.ip, passiveConnectionInfo.port, ref this.dtpPassive, ref this.DATA_iostream);
				Stream stream = networkStream;
				this.STOR(filename, ref this.iostream, ref this.DATA_iostream, ref stream);
				this.DicsonnectFrom(ref tcpClient, ref networkStream);
				array = this.ReadServerResponseMultiline(ref rcstream);
				if (this.ServerResponseCode(array).Trim().Equals("226"))
				{
					this.OnServerResponse(Encoding.ASCII.GetString(array));
				}
				else
				{
					if (this.ServerResponseCode(array).Trim().Equals("250"))
					{
						this.OnServerResponse(Encoding.ASCII.GetString(array));
					}
					else
					{
						if (this.ServerResponseCode(array).Trim().Equals("425"))
						{
							throw new _425_can_not_open_data_connection_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("426"))
						{
							throw new _426_connection_vlosed_transfer_aborted_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("451"))
						{
							throw new _451_local_error_exception(Encoding.ASCII.GetString(array));
						}
					}
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
			}
		}
		public void Upload(string filename, ref FileStream stream)
		{
			try
			{
				this.SetType(FTP._TYPE.IMAGE, ref this.iostream);
				if (this.mode == ConnectionMode.Passive)
				{
					PassiveConnectionInfo passiveConnectionInfo = default(PassiveConnectionInfo);
					if (this._EPSV)
					{
						try
						{
							passiveConnectionInfo = this.EPSV(this.passiveNetPrt, ref this.iostream);
						}
						catch (ServerResponseException ex)
						{
							this.OnServerResponse(ex.Message);
							this._EPSV = false;
						}
					}
					if (!this._EPSV)
					{
						passiveConnectionInfo = this.PASV(ref this.iostream);
					}
					this.ConnectTo(passiveConnectionInfo.ip, passiveConnectionInfo.port, ref this.dtpPassive, ref this.DATA_iostream);
					Stream stream2 = stream;
					try
					{
						this.STOR(filename, ref this.iostream, ref this.DATA_iostream, ref stream2);
						goto IL_C4;
					}
					catch (ServerResponseException ex2)
					{
						this.OnServerResponse(ex2.Message);
						goto IL_C4;
					}
				}
				ConnectionMode arg_C3_0 = this.mode;
				IL_C4:;
			}
			catch (ServerResponseException ex3)
			{
				this.OnServerResponse(ex3.Message);
			}
		}
		public void Upload_LS_to_RS(string filename, NetworkStream rcstream)
		{
			try
			{
				this.SetType(FTP._TYPE.IMAGE, ref this.iostream);
				PassiveConnectionInfo passiveConnectionInfo = this.PASV(ref this.iostream);
				this.ConnectTo(passiveConnectionInfo.ip, passiveConnectionInfo.port, ref this.dtpPassive, ref this.DATA_iostream);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("RETR {0}\r\n", filename);
				byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
				this.iostream.Write(bytes, 0, bytes.Length);
				this.iostream.Flush();
				byte[] array = this.ReadServerResponseMultiline(ref this.iostream);
				if (this.ServerResponseCode(array).Trim().Equals("120"))
				{
					this.OnServerResponse(Encoding.ASCII.GetString(array));
				}
				else
				{
					if (this.ServerResponseCode(array).Trim().Equals("150"))
					{
						this.OnServerResponse(Encoding.ASCII.GetString(array));
					}
					else
					{
						if (this.ServerResponseCode(array).Trim().Equals("500"))
						{
							throw new _500_駉mmand_syntax_error_could_not_interpreted_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("501"))
						{
							throw new _501_command_syntax_error_invalid_parameter_or_argument_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("421"))
						{
							throw new _421_service_not_available_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("530"))
						{
							throw new _530_not_logged_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("450"))
						{
							throw new _450_file_unavailable_busy_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("550"))
						{
							throw new _550_file_unavailable_not_found_no_access_exception(Encoding.ASCII.GetString(array));
						}
					}
				}
				this.TYPE(FTP._TYPE.IMAGE, ref rcstream);
				passiveConnectionInfo = this.PASV(ref rcstream);
				TcpClient tcpClient = new TcpClient();
				NetworkStream networkStream = null;
				this.ConnectTo(passiveConnectionInfo.ip, passiveConnectionInfo.port, ref tcpClient, ref networkStream);
				Stream dATA_iostream = this.DATA_iostream;
				this.STOR(filename, ref rcstream, ref networkStream, ref dATA_iostream);
				this.DicsonnectFrom(ref tcpClient, ref networkStream);
				array = this.ReadServerResponseMultiline(ref rcstream);
				if (this.ServerResponseCode(array).Trim().Equals("226"))
				{
					this.OnServerResponse(Encoding.ASCII.GetString(array));
				}
				else
				{
					if (this.ServerResponseCode(array).Trim().Equals("250"))
					{
						this.OnServerResponse(Encoding.ASCII.GetString(array));
					}
					else
					{
						if (this.ServerResponseCode(array).Trim().Equals("425"))
						{
							throw new _425_can_not_open_data_connection_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("426"))
						{
							throw new _426_connection_vlosed_transfer_aborted_exception(Encoding.ASCII.GetString(array));
						}
						if (this.ServerResponseCode(array).Trim().Equals("451"))
						{
							throw new _451_local_error_exception(Encoding.ASCII.GetString(array));
						}
					}
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
			}
		}
		public bool Rename(string old_fd_name, string new_fd_name)
		{
			bool result;
			try
			{
				if (this.RNFR(old_fd_name, ref this.iostream).Equals("350"))
				{
					if (this.RNTO(new_fd_name, ref this.iostream).Equals("250"))
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				result = false;
			}
			return result;
		}
		private void ConnectTo(string hostname, int port, ref TcpClient workTcpCl, ref NetworkStream iostream)
		{
			try
			{
				workTcpCl = new TcpClient();
				workTcpCl.Connect(hostname, port);
				iostream = workTcpCl.GetStream();
			}
			catch (SocketException ex)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Cannot connect to {0} on port: {1}\nSystem message: {2}", hostname, port, ex.Message);
				throw new Exception(stringBuilder.ToString());
			}
		}
		private void AcceptFrom(IPAddress ip, int port, ref TcpListener workTcpLis, ref NetworkStream iosream)
		{
			workTcpLis = new TcpListener(ip, port);
			workTcpLis.Start();
			Console.WriteLine("Waiting for a connection on {0}:{1} ... ", ip, port);
			TcpClient tcpClient = workTcpLis.AcceptTcpClient();
			Console.WriteLine("Connected!");
			iosream = tcpClient.GetStream();
		}
		private void DicsonnectFrom(ref TcpClient workTcpCl, ref NetworkStream iostream)
		{
			iostream.Close();
			workTcpCl.Close();
		}
		private byte[] ReadServerResponseMultiline(ref NetworkStream iostream)
		{
			byte[] result;
			try
			{
				StreamReader streamReader = new StreamReader(iostream);
				string text = "";
				string text2 = "";
				bool flag = false;
				bool flag2 = true;
				while (!flag)
				{
					string text3 = streamReader.ReadLine();
					text2 = text2 + text3 + '\n';
					if (text3.Length >= 4)
					{
						this.speed += text3.Length;
						if (flag2)
						{
							text = text2.Substring(0, 3);
							flag2 = false;
						}
						if (text.Equals(text3.Substring(0, 3)) && text3[3] == ' ')
						{
							flag = true;
						}
						this.noopTimer.Change(60000, 30000);
					}
				}
				result = Encoding.ASCII.GetBytes(text2);
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				result = new byte[1];
			}
			return result;
		}
		private byte[] ReadServerDATA(ref NetworkStream iostream)
		{
			byte[] result;
			try
			{
				if (!iostream.CanRead)
				{
					throw new Exception("Cannot read from NetworkStream.");
				}
				byte[] array = new byte[1024];
				List<byte> list = new List<byte>();
				while (true)
				{
					int num = iostream.Read(array, 0, array.Length);
					if (num <= 0)
					{
						break;
					}
					this.speed += num;
					for (int i = 0; i < num; i++)
					{
						list.Add(array[i]);
					}
					this.noopTimer.Change(60000, 30000);
				}
				result = list.ToArray();
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
				result = new byte[1];
			}
			return result;
		}
		private void ReadWriteServerDATA(ref Stream stream_r, ref Stream stream_w)
		{
			try
			{
				this.totalDownloadedBytes = 0;
				if (!stream_r.CanRead)
				{
					throw new Exception("Cannot read from NetworkStream.");
				}
				byte[] array = new byte[1024];
				while (true)
				{
					int num = stream_r.Read(array, 0, array.Length);
					if (num <= 0)
					{
						break;
					}
					this.speed += num;
					this.totalDownloadedBytes += num;
					this.OnFileTransfer(this.totalDownloadedBytes);
					stream_w.Write(array, 0, num);
					stream_w.Flush();
					this.noopTimer.Change(60000, 30000);
				}
				this.totalDownloadedBytes = 0;
			}
			catch (IOException ex)
			{
				this.OnServerResponse(ex.Message);
			}
		}
		private void ReadWriteServerDATA(ref NetworkStream stream_r, ref Stream stream_w)
		{
			Stream stream = stream_r;
			this.ReadWriteServerDATA(ref stream, ref stream_w);
		}
		private void ReadWriteServerDATA(ref Stream stream_r, ref NetworkStream stream_w)
		{
			Stream stream = stream_w;
			this.ReadWriteServerDATA(ref stream_r, ref stream);
		}
		private string ServerResponseCode(byte[] data)
		{
			return this.GetString(data).Substring(0, 3);
		}
		private string GetString(byte[] data)
		{
			if (this.encoding == Encoding.UTF8)
			{
				return Encoding.UTF8.GetString(data);
			}
			return Encoding.ASCII.GetString(data);
		}
		private void SetType(FTP._TYPE type, ref NetworkStream iostream)
		{
			try
			{
				if (this.currentType != type && this.TYPE(type, ref iostream).Equals("200"))
				{
					this.currentType = type;
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
			}
		}
		private void DoNOOP(object state)
		{
			Timer timer = (Timer)state;
			try
			{
				this.NOOP(ref this.iostream);
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
			}
			catch (IOException)
			{
				timer.Dispose();
			}
		}
		private void DoTimer(object state)
		{
			this.OnSpeed(this);
			this.speed = 0;
		}
		private void DoEPRT(FTP.NetworkProtocol prt, ref List<int> notSupportedProtocols)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("|{2}|{0}|{1}|", Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), ((IPEndPoint)this.ftp.Client.LocalEndPoint).Port + 1, (int)prt);
				this.EPRT(stringBuilder.ToString(), ref this.iostream);
			}
			catch (_522_protocol_not_supported 522_protocol_not_supported)
			{
				notSupportedProtocols.Add((int)prt);
				this.OnServerResponse(522_protocol_not_supported.Message);
				Regex regex = new Regex("\\((?<prt>.*)\\)");
				Match match = regex.Match(522_protocol_not_supported.Message);
				if (match.Success)
				{
					string[] array = match.Groups["prt"].Value.Replace("(", "").Replace(")", "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						int item = Convert.ToInt32(array[i]);
						if (!notSupportedProtocols.Contains(item))
						{
							switch (item)
							{
							case 1:
								this.DoEPRT(FTP.NetworkProtocol.IPv4, ref notSupportedProtocols);
								i = array.Length;
								break;
							case 2:
								this.DoEPRT(FTP.NetworkProtocol.IPv6, ref notSupportedProtocols);
								i = array.Length;
								break;
							}
						}
					}
				}
			}
			catch (ServerResponseException ex)
			{
				this.OnServerResponse(ex.Message);
				this._EPRT = false;
			}
		}
		private double SpeedConvert(SpeedPerSecond type, int size)
		{
			return Math.Round((double)size / Math.Pow(2.0, (double)type), 1);
		}
		private void CheckFEAT(string feat)
		{
			if (feat.IndexOf("UTF8", StringComparison.OrdinalIgnoreCase) > -1)
			{
				this._UTF8 = true;
			}
			if (feat.IndexOf("MDTM", StringComparison.OrdinalIgnoreCase) > -1)
			{
				this._MDTM = true;
			}
			if (feat.IndexOf("CLNT", StringComparison.OrdinalIgnoreCase) > -1)
			{
				this._CLNT = true;
			}
			if (feat.IndexOf("SIZE", StringComparison.OrdinalIgnoreCase) > -1)
			{
				this._SIZE = true;
			}
			if (feat.IndexOf("REST STREAM", StringComparison.OrdinalIgnoreCase) > -1)
			{
				this._REST = true;
			}
			if (feat.IndexOf("TVFS", StringComparison.OrdinalIgnoreCase) > -1)
			{
				this._TVFS = true;
			}
			Regex regex = new Regex("(?<mlst>MLST .*)");
			Match match = regex.Match(feat);
			if (match.Success)
			{
				this._MLST = true;
				this.mlst_string = match.Groups["mlst"].Value.Replace("*", "");
			}
			if (feat.IndexOf("EPRT", StringComparison.OrdinalIgnoreCase) > -1)
			{
				this._EPRT = true;
			}
			if (feat.IndexOf("EPSV", StringComparison.OrdinalIgnoreCase) > -1)
			{
				this._EPSV = true;
			}
		}
	}
}
