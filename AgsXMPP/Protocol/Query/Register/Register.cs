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

using AgsXMPP.Protocol.X.Data;
using AgsXMPP.Xml.Dom;

// Sample 1
// <SENT> <iq id="2" type="set"><query xmlns="jabber:iq:register"><username>gnauck2</username><password>secret</password></query></iq>
// <RECV> <iq id='2' type='result'/>

// Sample 2
// <SEND> <iq xmlns="jabber:client" id="agsXMPP_1" type="get" to="127.0.0.1"><query xmlns="jabber:iq:register"><username>test1</username><password>secret</password></query></iq>
// <RECV> <iq xmlns="jabber:client" id="agsXMPP_1" type="result"><query xmlns="jabber:iq:register"><username>test1</username><password>mysecret</password><password /><instructions>Choose a username and password to register with this server.</instructions><name /><email /><username /></query></iq>


namespace AgsXMPP.Protocol.Query.Register
{
	/// <summary>
	/// Used for registering new usernames on Jabber/XMPP Servers
	/// </summary>
	public class Register : Element
	{
		#region << Constructors >>
		public Register()
		{
			this.TagName = "query";
			this.Namespace = URI.IQ_REGISTER;
		}

		public Register(string username, string password) : this()
		{
			this.Username = username;
			this.Password = password;
		}
		#endregion

		#region << Properties >>
		public string Username
		{
			get { return this.GetTag("username"); }
			set { this.SetTag("username", value); }
		}

		public string Password
		{
			get { return this.GetTag("password"); }
			set { this.SetTag("password", value); }
		}

		public string Instructions
		{
			get { return this.GetTag("instructions"); }
			set { this.SetTag("instructions", value); }
		}

		public string Name
		{
			get { return this.GetTag("name"); }
			set { this.SetTag("name", value); }
		}

		public string Email
		{
			get { return this.GetTag("email"); }
			set { this.SetTag("email", value); }
		}

		/// <summary>
		/// Remove registration from the server
		/// </summary>
		public bool RemoveAccount
		{
			get { return this.HasTag("remove"); }
			set
			{
				if (value == true)
					this.SetTag("remove");
				else
					this.RemoveTag("remove");
			}
		}

		/// <summary>
		/// The X-Data Element
		/// </summary>
		public Data Data
		{
			get
			{
				return this.SelectSingleElement(typeof(Data)) as Data;

			}
			set
			{
				if (this.HasTag(typeof(Data)))
					this.RemoveTag(typeof(Data));

				if (value != null)
					this.AddChild(value);
			}
		}
		#endregion
	}
}
