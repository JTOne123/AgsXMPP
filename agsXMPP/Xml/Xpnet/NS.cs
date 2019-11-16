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

/* --------------------------------------------------------------------------
 * Copyrights
 * 
 * Portions created by or assigned to Cursive Systems, Inc. are 
 * Copyright (c) 2002-2005 Cursive Systems, Inc.  All Rights Reserved.  Contact
 * information for Cursive Systems, Inc. is available at
 * http://www.cursive.net/.
 *
 * License
 * 
 * Jabber-Net can be used under either JOSL or the GPL.  
 * See LICENSE.txt for details.
 * --------------------------------------------------------------------------*/
using System.Collections;

namespace agsXMPP.Xml.Xpnet
{
	/// <summary>
	/// Namespace stack.
	/// </summary>
	public class NS
	{
		private Stack m_stack = new Stack();

		/// <summary>
		/// Create a new stack, primed with xmlns and xml as prefixes.
		/// </summary>
		public NS()
		{
			this.PushScope();
			this.AddNamespace("xmlns", "http://www.w3.org/2000/xmlns/");
			this.AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");
		}

		/// <summary>
		/// Declare a new scope, typically at the start of each element
		/// </summary>
		public void PushScope()
		{
			this.m_stack.Push(new Hashtable());
		}

		/// <summary>
		/// Pop the current scope off the stack.  Typically at the end of each element.
		/// </summary>
		public void PopScope()
		{
			this.m_stack.Pop();
		}

		/// <summary>
		/// Add a namespace to the current scope.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="uri"></param>
		public void AddNamespace(string prefix, string uri)
		{
			((Hashtable)this.m_stack.Peek()).Add(prefix, uri);
		}

		/// <summary>
		/// Lookup a prefix to find a namespace.  Searches down the stack, starting at the current scope.
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public string LookupNamespace(string prefix)
		{
			foreach (Hashtable ht in this.m_stack)
			{
				if ((ht.Count > 0) && (ht.ContainsKey(prefix)))
					return (string)ht[prefix];
			}
			return "";
		}

		/// <summary>
		/// The current default namespace.
		/// </summary>
		public string DefaultNamespace
		{
			get { return this.LookupNamespace(string.Empty); }
		}

		/// <summary>
		/// Debug output only.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new System.Text.StringBuilder();

			foreach (Hashtable ht in this.m_stack)
			{
				sb.Append("---\n");
				foreach (string k in ht.Keys)
				{
					sb.Append(string.Format("{0}={1}\n", k, ht[k]));
				}
			}
			return sb.ToString();
		}

		public void Clear()
		{
#if !CF
			this.m_stack.Clear();
#else
			while (m_stack.Count > 0)
			    m_stack.Pop();
#endif
		}
	}
}