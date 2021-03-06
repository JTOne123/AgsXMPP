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

using AgsXMPP.Xml.Dom;

namespace AgsXMPP.Protocol.Query.Agent
{
	//	<agent jid="conference.myjabber.net"><name>Public Conferencing</name><service>public</service></agent>
	//	<agent jid="aim.myjabber.net"><name>AIM Transport</name><service>aim</service><transport>Enter ID</transport><register/></agent>
	//	<agent jid="yahoo.myjabber.net"><name>Yahoo! Transport</name><service>yahoo</service><transport>Enter ID</transport><register/></agent>
	//	<agent jid="icq.myjabber.net"><name>ICQ Transport</name><service>icq</service><transport>Enter ID</transport><register/></agent>
	//	<agent jid="msn.myjabber.net"><name>MSN Transport</name><service>msn</service><transport>Enter ID</transport><register/></agent>

	/// <summary>
	/// Zusammenfassung f�r Agent.
	/// </summary>
	public class Agent : Element
	{
		public Agent()
		{
			this.TagName = "agent";
			this.Namespace = URI.IQ_AGENTS;
		}

		public Jid Jid
		{
			get { return new Jid(this.GetAttribute("jid")); }
			set { this.SetAttribute("jid", value.ToString()); }
		}

		public string Name
		{
			get { return this.GetTag("name"); }
			set { this.SetTag("name", value); }
		}

		public string Service
		{
			get { return this.GetTag("service"); }
			set { this.SetTag("service", value); }
		}

		public string Description
		{
			get { return this.GetTag("description"); }
			set { this.SetTag("description", value); }

		}

		/// <summary>
		/// Can we register this agent/transport
		/// </summary>
		public bool CanRegister
		{
			get { return this.HasTag("register"); }
			set
			{
				if (value == true)
					this.SetTag("register");
				else
					this.RemoveTag("register");
			}
		}

		/// <summary>
		/// Can we search thru this agent/transport
		/// </summary>
		public bool CanSearch
		{
			get { return this.HasTag("search"); }
			set
			{
				if (value == true)
					this.SetTag("search");
				else
					this.RemoveTag("search");
			}
		}

		/// <summary>
		/// Is this agent a transport?
		/// </summary>
		public bool IsTransport
		{
			get { return this.HasTag("transport"); }
			set
			{
				if (value == true)
					this.SetTag("transport");
				else
					this.RemoveTag("transport");
			}
		}

		/// <summary>
		/// Is this agent for groupchat
		/// </summary>
		public bool IsGroupchat
		{
			get { return this.HasTag("groupchat"); }
			set
			{
				if (value == true)
					this.SetTag("groupchat");
				else
					this.RemoveTag("groupchat");
			}
		}

	}
}
