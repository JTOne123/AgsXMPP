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

namespace AgsXMPP.Protocol.Extensions.Bookmarks
{
	/// <summary>
	/// URLs are fairly simple, as they only need to store a URL and a title, 
	/// and the client then can simply launch the appropriate browser.
	/// </summary>
	public class Url : Element
	{
		/*
            <url name='Complete Works of Shakespeare'
         url='http://the-tech.mit.edu/Shakespeare/'/>
        */
		public Url()
		{
			this.TagName = "url";
			this.Namespace = Namespaces.STORAGE_BOOKMARKS;
		}

		public Url(string address, string name) : this()
		{
			this.Address = address;
			this.Name = name;
		}

		/// <summary>
		/// A description/name for this bookmark
		/// </summary>
		public string Name
		{
			get { return this.GetAttribute("name"); }
			set { this.SetAttribute("name", value); }
		}

		/// <summary>
		/// The url address to store e.g. http://www.ag-software,de/
		/// </summary>
		public string Address
		{
			get { return this.GetAttribute("url"); }
			set { this.SetAttribute("url", value); }
		}
	}
}
