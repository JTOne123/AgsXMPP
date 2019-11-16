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

using agsXMPP.Xml.Dom;

namespace agsXMPP.Protocol.iq.privacy
{
	public class List : Element
	{
		public List()
		{
			this.TagName = "list";
			this.Namespace = Namespaces.IQ_PRIVACY;
		}

		public List(string name) : this()
		{
			this.Name = name;
		}

		public string Name
		{
			get { return this.GetAttribute("name"); }
			set { this.SetAttribute("name", value); }
		}

		/// <summary>
		/// Gets all Rules (Items) when available
		/// </summary>
		/// <returns></returns>
		public Item[] GetItems()
		{
			var el = this.SelectElements(typeof(Item));
			var i = 0;
			var result = new Item[el.Count];
			foreach (Item itm in el)
			{
				result[i] = itm;
				i++;
			}
			return result;
		}

		/// <summary>
		/// Adds a rule (item) to the list
		/// </summary>
		/// <param name="itm"></param>
		public void AddItem(Item item)
		{
			this.AddChild(item);
		}

		public void AddItems(Item[] items)
		{
			foreach (var item in items)
			{
				this.AddChild(item);
			}
		}

		/// <summary>
		/// Remove all items/rules of this list
		/// </summary>
		public void RemoveAllItems()
		{
			this.RemoveTags(typeof(Item));
		}
	}
}