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

namespace AgsXMPP.Protocol.X.Muc
{
	/// <summary>
	/// There are five defined affiliations that a user may have in relation to a room
	/// </summary>
	public enum Affiliation
	{
		/// <summary>
		/// the absence of an affiliation
		/// </summary>
		[XmppEnumMember("none")]
		None,

		[XmppEnumMember("owner")]
		Owner,

		[XmppEnumMember("admin")]
		Admin,

		[XmppEnumMember("member")]
		Member,

		[XmppEnumMember("outcast")]
		Outcast
	}
}
