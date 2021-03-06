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

namespace AgsXMPP.Protocol.Query.Browse
{
	/// <summary>
	/// Summary description for BrowseItem.
	/// </summary>
	public class BrowseItem : Base.Item
	{
		/*
		<item version="0.6.0" name="Public Conferencing" jid="conference.myjabber.net" type="public" category="conference"> 
			<ns>http://jabber.org/protocol/muc</ns> 
		</item>
		*/
		public BrowseItem() : base()
		{
			this.Namespace = URI.IQ_BROWSE;
		}

		public string Category
		{
			get { return this.GetAttribute("category"); }
			set { this.SetAttribute("category", value); }
		}

		public string Version
		{
			get { return this.GetAttribute("version"); }
			set { this.SetAttribute("version", value); }
		}

		public string Type
		{
			get { return this.GetAttribute("type"); }
			set { this.SetAttribute("type", value); }
		}

		/// <summary>
		/// Gets all advertised namespaces of this item
		/// </summary>
		/// <returns>string array that contains the advertised namespaces</returns>
		public string[] GetNamespaces()
		{
			var elements = this.SelectElements("ns");
			var nss = new string[elements.Count];

			var i = 0;
			foreach (Element ns in elements)
			{
				nss[i] = ns.Value;
				i++;
			}

			return nss;
		}

	}
}
