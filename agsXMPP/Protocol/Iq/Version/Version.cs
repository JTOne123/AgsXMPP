/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2019 by AG-Software, FRNathan13								 *
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

namespace AgsXMPP.Protocol.Iq.version
{
	// Send:<iq type='get' id='MX_6' to='jfrankel@coversant.net/SoapBox'>
	//			<query xmlns='jabber:iq:version'></query>
	//		</iq>
	//
	// Recv:<iq from="jfrankel@coversant.net/SoapBox" id="MX_6" to="gnauck@myjabber.net/Office" type="result">
	//			<query xmlns="jabber:iq:version">
	//				<name>SoapBox</name>
	//				<version>2.1.2 beta</version>
	//				<os>Windows NT 5.1 (en-us)</os>
	//			</query>
	//		</iq> 


	/// <summary>
	/// Zusammenfassung f�r Version.
	/// </summary>
	public class Version : Element
	{
		public Version()
		{
			this.TagName = "query";
			this.Namespace = URI.IQ_VERSION;
		}

		public string Name
		{
			set { this.SetTag("name", value); }
			get { return this.GetTag("name"); }
		}

		public string Ver
		{
			set { this.SetTag("version", value); }
			get { return this.GetTag("version"); }
		}

		public string Os
		{
			set { this.SetTag("os", value); }
			get { return this.GetTag("os"); }
		}

	}
}
