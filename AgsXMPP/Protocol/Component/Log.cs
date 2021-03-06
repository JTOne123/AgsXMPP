/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2020 by AG-Software, FRNathan13								 *
 * All Rights Reserved.																 *
 * Contact information for AG-Software is available at http://www.ag-software.de	 *
 *																					 *
 * Licence:																			 *
 * The agsXMPP SDK is released under a dual licence									 *
 * agsXMPP can be used under either of two licences									 *
 * 																					 *
 * A commercial licence which is probably the most appropriate for commercial 		 *
 * corporate use and closed source projects. 										 *
 *																					 *
 * The GNU Public License (GPL) is probably most appropriate for inclusion in		 *
 * other open source projects.														 *
 *																					 *
 * See README.html for details.														 *
 *																					 *
 * For general enquiries visit our website at:										 *
 * http://www.ag-software.de														 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using AgsXMPP.Protocol.Base;

namespace AgsXMPP.Protocol.Component
{
	public enum LogType
	{
		None = -1,

		[XmppEnumMember("warn")]
		Warn,

		[XmppEnumMember("info")]
		Info,

		[XmppEnumMember("verbose")]
		Verbose,

		[XmppEnumMember("debug")]
		Debug,

		[XmppEnumMember("notice")]
		Notice
	}

	/// <summary>
	/// Zusammenfassung f�r Log.
	/// </summary>
	public class Log : Stanza
	{
		public Log()
		{
			this.TagName = "log";
			this.Namespace = URI.ACCEPT;
		}

		/// <summary>
		/// creates a new Log Packet with the given message
		/// </summary>
		/// <param name="message"></param>
		public Log(string message) : this()
		{
			this.Value = message;
		}


		/// <summary>
		/// Gets or Sets the logtype
		/// </summary>
		public LogType Type
		{
			get => this.GetAttributeEnum<LogType>("type");
			set
			{
				if (value == LogType.None)
					this.RemoveAttribute("type");
				else
					this.SetAttribute("type", value.ToString());
			}
		}

		/// <summary>
		/// The namespace for logging
		/// </summary>
		public string LogNamespace
		{
			get { return this.GetAttribute("ns"); }
			set { this.SetAttribute("ns", value); }
		}
	}
}
